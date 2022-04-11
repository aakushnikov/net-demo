using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Crawler.Contracts;
using Crawler.Contracts.Entities;
using Crawler.Contracts.Repositories;
using Crawler.Contracts.Services;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Crawler.Contracts.Impl.Entities;
using Crawler.Contracts.Impl.Common.Helpers;
using Crawler.Contracts.Services;

namespace Crawler.Contracts.Impl
{
	public class HtmlParserModuleService : IHtmlParserModuleService
	{
		private readonly IHtmlParserService _htmlParserService;
		private readonly IPatternRelationRepository _patternRelationRepository;
		private readonly IHostRepository _hostRepository;
		private readonly IPatternRepository _patternRepository;
		private readonly IAuthRepository _authRepository;
		private readonly IAuthService _authService;
		private readonly IProxyContainerService _containerService;
		private readonly IWebHtmlParserService _webHtmlParserService;
		private readonly IDefaultHtmlParserSettings _defaultHtmlParserSettings;
	    private readonly ICache<PatternHostRelations> _relationsCache;

		public HtmlParserModuleService(IHtmlParserService htmlParserService,
			IPatternRelationRepository patternRelationRepository,
			IHostRepository hostRepository,
			IPatternRepository patternRepository,
			IAuthRepository authRepository,
			IAuthService authService,
			IProxyContainerService containerService,

			IWebHtmlParserService webHtmlParserService,
			IDefaultHtmlParserSettings defaultHtmlParserSettings,
		    ICache<PatternHostRelations> relationsCache)
		{
			_htmlParserService = htmlParserService;
			_patternRelationRepository = patternRelationRepository;
			_hostRepository = hostRepository;
			_patternRepository = patternRepository;

			_authRepository = authRepository;
			_authService = authService;
			_containerService = containerService;
			_webHtmlParserService = webHtmlParserService;
			_defaultHtmlParserSettings = defaultHtmlParserSettings;
		    _relationsCache = relationsCache;
		}

        private static readonly object _cacheSync = new object();

		public string Parse(string url, bool? useBrowser = false)
		{
			return Parse(url, string.Empty, null,useBrowser);
		}

		public string Parse(string url, string postData, List<Cookie> cookies = null, bool? useBrowser = false)
		{

			var relation = GetPatternHostRelations(url);
			if (relation == null)
				throw new InvalidOperationException($"Not found pattern for url:{url}");

			var map = relation.MappingScheme();

			var encoding = Encoding.UTF8;
			if (relation.EncodingCode.HasValue)
			{
				encoding = Encoding.GetEncoding(relation.EncodingCode.Value);
			}
			var validationRequest = new ValidationRequest()
			{
				Url = url,
				Encoding = encoding,
				Map = map,
				UseBrowser = useBrowser ?? relation.UseBrowser.GetValueOrDefault(),
				PostData = postData,
				Host = relation.Host.Url,
				Cookies = cookies
			};
			if (relation.Host != null)
			{
				validationRequest.Auth = _authRepository.Get(relation.Host.Id);
			}
			return Validate(validationRequest);

		}

		public string ParseContent(string url, string content)
		{
			var relation = GetPatternHostRelations(url);
			if (relation == null)
				throw new InvalidOperationException($"Not found pattern for url:{url}");

			var result = PageDataHelper.Parse(relation.MappingScheme(), content);
			var keyValue = new KeyValue(ActionType.Xpath)
			{
				Name = "Url",
				Value = UrlHelper.GetNormalizedUrl(url)
			};
			result.Values.Add(keyValue);

			StringBuilder sb = new StringBuilder();
			StringWriter sw = new StringWriter(sb);

			using (JsonWriter writer = new JsonTextWriter(sw))
			{
				Write(writer, result.Values, result.Name);
			}

			return sb.ToString();
		}


		public string Validate(ValidationRequest request)
		{
			var key = request.Host ?? "default";
			try
			{
				ThreadKeyLock.Instance.Register(key, 200);
				return ExecuteInternal(request);
			}
			finally
			{
				ThreadKeyLock.Instance.Release(key);
			}
		}

		private string ExecuteInternal(ValidationRequest request)
		{
			var content = GetContent(request);

			var result = PageDataHelper.Parse(request.Map, content.Content);
			var keyValue = new KeyValue(ActionType.Xpath)
			{
				Name = "Url",
				Value = UrlHelper.GetNormalizedUrl(content.Url ?? request.Url),
			};
			result.Values.Add(keyValue);
			result.Values.Add(new KeyValue(ActionType.Xpath)
			{
				Name = "Proxy",
				Value = content.Proxy,
			});
			StringBuilder sb = new StringBuilder();
			StringWriter sw = new StringWriter(sb);

			using (JsonWriter writer = new JsonTextWriter(sw))
			{
				Write(writer, result.Values, result.Name);
			}

			return sb.ToString();
		}

		public ParseResult GetContent(ValidationRequest request)
		{
			if (request.Auth != null && request.Auth.Any())
			{
				var auth = request.Auth.FirstOrDefault();
				var dataAuth = new AuthInfo()
				{
					LoginPageUrl = auth.LoginUrl,
					FirstPageUrl = auth.FirstPageUrl ?? auth.LoginUrl,
					LoginFieldName = auth.LoginField,
					PasswordFieldName = auth.PasswordField,
					Password = auth.Password,
					Login = auth.UserName,
					SuccessIfContainValue = auth.SuccessIfContainValue,
					Proxy = _containerService.GetValidProxy()
				};
				request.AuthResult = _authService.Auth(dataAuth);
				if (request.AuthResult != null)
				{
					return _htmlParserService.GetContent(request.Url,
						request.Encoding,
						request.PostData,
						request.AuthResult.Cookies,
						request.AuthResult.Proxy);
				}
			}
			if (request.UseBrowser)
			{
				return _webHtmlParserService.GetContent(request.Url, request.Encoding, null, null,
					_containerService.GetValidProxy());
			}
			else
			{
				var content = _htmlParserService.GetContent(request.Url, request.Encoding, request.PostData, request.Cookies, null);
				return content;
			}
		}



		public MappingScheme TryFindPattern(string url, Encoding encoding)
		{
			var map = new MappingScheme()
			{
				Matches = new List<KeyValue>()
			};
			var patterns = _patternRepository.GetAll().Select(x => x.MappingScheme());
			var content = _htmlParserService.GetContent(url, encoding);
			
			var keys = new List<KeyValue>();
			foreach (var m in patterns)
			{
				keys.AddRange(m.Matches.Children());
			}
			var html = WebUtility.HtmlDecode(content.Content);
			var document = new HtmlDocument();
			document.LoadHtml(html);
			var context = new PageDataContext()
			{
				Html = html,
				RootNode = document.DocumentNode
			};
			var groupOfKey = keys.GroupBy(x => x.Name);
			foreach (var g in groupOfKey)
			{
				foreach (var p in g)
				{
					if (p.Type == ActionType.Regex)
						continue;
					try
					{
						var itemResult = new KeyValue(ActionType.Xpath)
						{
							Name = p.Name,
							Values = new List<KeyValue>(),
							Value = p.Value,
						};
						var result = PageDataHelper.CkeckResult(context, document, itemResult);

						var list = result[g.Key].Where(x => x.Type != ActionType.Regex && !string.IsNullOrEmpty(x.Value)).ToList();
						if (list.Any())
						{
							map.Matches.Add(p);
							break;
						}
					}
					catch (Exception ex)
					{
					}

				}
			}
			return map;
		}

		public void Update(PatternHostRelations pattern)
		{
			_patternRelationRepository.Save(pattern);
		}

		public PatternHostRelations GetPatternHostRelations(string url)
		{
		    var hostUrl = UrlHelper.GetHostUrl(url);

            var cachedRelation = _relationsCache[hostUrl];

            if (cachedRelation != null)
		    {
		        return _relationsCache[hostUrl];
		    }
			var relations = GetRelations(url);
			PatternHostRelations relation = null;
			foreach (var r in relations)
			{
				if (!string.IsNullOrEmpty(r.Url))
				{
					var urlLike = new Regex(r.Url);
					if (urlLike.IsMatch(url))
						continue;
				}

				if (!string.IsNullOrEmpty(r.Pattern.Value))
				{
					relation = r;
				}
			}

		    if (_relationsCache[hostUrl] == null)
		        lock (_cacheSync)
		            if (_relationsCache[hostUrl] == null)
		                _relationsCache.Set(hostUrl, relation, 1);

		    return relation;
		}

		public List<PatternHostRelations> GetRelations(string url)
		{
			var urlNorm = UrlHelper.GetHostUrl(url);
			var relations = _patternRelationRepository.Get(urlNorm);
			return relations;
		}


		private void Write(JsonWriter writer, List<KeyValue> result, string name)
		{
			writer.WriteStartObject();

			writer.WritePropertyName(name);

			writer.WriteStartArray();

			//List<KeyValue> array = result.Where(x => x != null && !x.IsValue);
			var container = GetKeyContainer(result);

			WriteContainer(writer, container, true);
			writer.WriteEndObject();


			writer.WriteEndArray();

			writer.WriteEndObject();
		}

		private void WriteContainer(JsonWriter writer, KeyContainer container, bool isArray = false)
		{
			foreach (var item in container.array)
			{
				if (item.Values != null && item.Values.Any())
					Write(writer, item.Values, item.Name);
			}
			if (isArray)
				writer.WriteStartObject();
			foreach (var item in container.@group)
			{
				var count = item.Count();
				if (count == 1 || item.Any(x => x.NeedConcat))
				{
					WriteSingleValue(writer, item, count);
				}
			}
			//foreach (var og in groupGroup)
			{
				foreach (var item in container.obj)
				{
					var buf = item;
					while (buf.IsBranch)
					{
						if (!buf.IsValue && buf.Values != null && buf.Values.Count == 1)
						{
							buf = buf.Values[0];
						}
					}
					if (buf.IsValue)
					{
						WriteProperty(writer, buf);
					}
					else if (buf.Values != null)
					{
						WriteObject(writer, buf, false);
					}
				}
			}

			foreach (var item in container.needConcat)
			{
				writer.WritePropertyName(item.Name);
				var concatValue = GetConcatValue(item.Values, item.Values.Count, item.ConcatValue);
				JToken value = new JValue(concatValue);
				writer.WriteToken(JsonToken.String, value);
			}

			foreach (var item in container.@group.Where(x => x.Count() > 1))
			{
				if (!item.Any(x => x.NeedConcat))
				{
					var firstOrDefault = item.FirstOrDefault();
					var firstValue = firstOrDefault?.Value;
					var isEqual = true;
					foreach (var v in item)
					{
						if (firstValue != v.Value)
						{
							isEqual = false;
							break;
						}
					}
					if (isEqual)
					{
						WriteProperty(writer, firstOrDefault);
					}
					else
					{
						WriteArray(writer, item);
					}
				}
			}
		}

		private static KeyContainer GetKeyContainer(List<KeyValue> result)
		{
			var container = new KeyContainer();

			foreach (var x in result)
			{
				if (x == null)
					continue;
				if (x.NeedConcat && !x.IsValue)
				{
					container.needConcat.Add(x);
					continue;
				}

				if (x.IsObject != null && x.IsObject.Value)
				{
					container.obj.Add(x);
				}
				else if (x.IsArray != null && x.IsArray.Value)
				{
					container.array.Add(x);
				}
				else if (!x.IsValue && x.Values != null && x.Values.Count(v => !v.IsValue) > 1)
				{
					container.array.Add(x);
				}
				else if (x.IsValue)
				{
					container.values.Add(x);
				}
				else if (x.Values?.Count == 1)
				{
					container.obj.Add(x);
				}
				else
				{
					container.obj.Add(x);
				}
			}
			container.@group = container.values.GroupBy(x => x.Name).ToList();
			var groupGroup = container.obj.GroupBy(x => x.Name).ToList();
			container.obj.Clear();

			foreach (var gg in groupGroup)
			{
				if (gg.Count() > 1)
				{
					container.array.AddRange(gg);
				}
				else
				{
					container.obj.AddRange(gg);
				}
			}
			return container;
		}

		private void WriteObject(JsonWriter writer, KeyValue obj, bool isArray)
		{
			if (obj.Values == null)
				return;
			writer.WritePropertyName(obj.Name);
			writer.WriteStartObject();

			var container = GetKeyContainer(obj.Values);

			WriteContainer(writer, container);

			writer.WriteEndObject();
		}

		private static void WriteArray(JsonWriter writer, IGrouping<string, KeyValue> item)
		{
			writer.WritePropertyName(item.Key);
			writer.WriteStartArray();

			foreach (var v in item)
			{
				if (!string.IsNullOrEmpty(v.Value))
					writer.WriteValue(v.Value);
			}
			//writer.WriteEnd();
			writer.WriteEndArray();
		}

		private static void WriteProperty(JsonWriter writer, KeyValue buf)
		{
			writer.WritePropertyName(buf.Name);
			JToken value = new JValue(GetValue(buf, buf.ConcatValue));
			writer.WriteToken(JsonToken.String, value);
		}

		private static void WriteSingleValue(JsonWriter writer, IGrouping<string, KeyValue> item, int count)
		{
			writer.WritePropertyName(item.Key);
			var concatValue = GetConcatValue(item, count);
			JToken value = new JValue(concatValue);
			writer.WriteToken(JsonToken.String, value);
		}

		private static string GetConcatValue(IEnumerable<KeyValue> item, int count, string concatValue = null)
		{
			var sb = new StringBuilder(100);
			int i = 0;
			foreach (var x in item)
			{
				if (string.IsNullOrEmpty(concatValue))
				{
					concatValue = x.ConcatValue ?? string.Empty;
				}

				if (count == 1 || i == count - 1)
					concatValue = string.Empty;

				var rValue = GetValue(x, concatValue);
				sb.Append(rValue);
				i++;
			}

			var v = sb.ToString();
			return v;
		}

		private static string GetValue(KeyValue x, string concatValue)
		{
			var isNewRow = string.Empty;
			if (x.IsNewRow.HasValue && x.IsNewRow.Value)
			{
				isNewRow = System.Environment.NewLine;
			}
			var rValue = $"{isNewRow}{x.PrevValue ?? string.Empty}{x.Value}{concatValue}{x.AfterValue ?? string.Empty}";
			return rValue;
		}

		private static void SaveProperty(JsonWriter writer, KeyValue item)
		{
			var tValue = GetValue(item, item.ConcatValue ?? string.Empty);

			JToken value = new JValue(tValue);
			if (!string.IsNullOrEmpty(item.Name))
			{
				writer.WriteStartObject();
				writer.WritePropertyName(item.Name);
				writer.WriteToken(JsonToken.String, value);
				writer.WriteEndObject();
			}
			else
			{

				writer.WriteToken(JsonToken.String, value);
			}
		}

		private void WriteOld(JsonWriter writer, List<KeyValue> result, string name)
		{
			writer.WriteStartObject();

			writer.WritePropertyName(name);
			writer.WriteStartArray();

			foreach (var item in result)
			{
				if (item.Values != null && item.Values.Any())
				{

					WriteOld(writer, item.Values, item.Name);
				}
				else
				{
					SaveProperty(writer, item);
					//writer.WriteStartObject();
					//writer.WritePropertyName(item.Name);
					//writer.WriteStartArray();
					//JToken value = new JValue(item.Value);
					//writer.WriteToken(JsonToken.String, value);
					//writer.WriteEndArray();
					//writer.WriteEndObject();

				}
			}

			writer.WriteEndArray();
			writer.WriteEndObject();
		}

		private void WriteProperty(JsonWriter writer, string name, object value)
		{
			writer.WritePropertyName(name);

			if (value == null)
			{
				writer.WriteNull();
			}
			else
			{
				writer.WriteValue(value);
			}
		}

		public void Init()
		{
			var map = _defaultHtmlParserSettings.GetAll().OrderBy(x => x.Key);
			foreach (var h in map)
			{
				var url = h.Key;
				var mappingScheme = h.Value;
				Save(new PatternValidateRequest() { },
				mappingScheme, false);
			}

		}

		public void Save(PatternValidateRequest request, MappingScheme mappingScheme, bool deleteExistsRelations)
		{
			var url = request.Url;
			var useBrowser = request.UseBrowser;
			var encoding = request.CodePage;

			var urlNorm = UrlHelper.GetHostUrl(url);
			var host = _hostRepository.Get(urlNorm);
			if (host == null)
			{
				host = _hostRepository.Save(new global::Crawler.Contracts.Entities.Host()
				{
					Name = urlNorm,
					Url = urlNorm
				});
			}

			if (deleteExistsRelations)
			{
				_patternRelationRepository.Delete(host.Id);
			}

			var value = JsonConvert.SerializeObject(mappingScheme, Newtonsoft.Json.Formatting.Indented);
			var computeMd5String = value.ComputeMd5String();
			var pattern = _patternRepository.Get(computeMd5String);
			if (pattern == null)
			{
				pattern = new Pattern()
				{
					Value = value,
					Hash = computeMd5String
				};
				pattern = _patternRepository.Save(pattern);
			}
			var repations = _patternRelationRepository.Get(host.Id, pattern.Id).FirstOrDefault();
			if (repations == null)
			{
				repations = new PatternHostRelations()
				{
					Host = host,
					Pattern = pattern,
					Url = string.Empty,
					EncodingCode = encoding,
					UseBrowser = useBrowser

				};
				_patternRelationRepository.Save(repations);
			}

		}
	}
}
