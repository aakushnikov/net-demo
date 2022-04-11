using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Crawler.Browser.Api.Host.Extensions;
using Crawler.Browser.Api.Host.Proxies;
using Crawler.Contracts.Entities;
using Crawler.Contracts.Services;
using Nancy;
using Newtonsoft.Json;
using Crawler.Contracts.Impl;
using Crawler.Contracts.Services;
using HttpStatusCode = Nancy.HttpStatusCode;

namespace Crawler.Browser.Api.Host.Modules.HtmlParser
{

	public sealed class HtmlParserModel : Api.v1
	{
		private readonly IHtmlParserModuleService _htmlParserModuleService;
		private readonly IHtmlParserService _service;


		public HtmlParserModel(IHtmlParserModuleService htmlParserModuleService,
			IModuleConfigurator moduleConfigurator,
			IHtmlParserService service)

			: base("/htmlparser")
		{
			_htmlParserModuleService = htmlParserModuleService;
			_service = service;

			moduleConfigurator.Configure(Post, "/", Parse(), Response, () => Request);
			moduleConfigurator.Configure(Post, "/parse", ParseContent(), Response, () => Request);
			moduleConfigurator.Configure(Post, "/post", ParsePost(), Response, () => Request);
			moduleConfigurator.Configure(Get, "/html", GetContent(), Response, () => Request);
			moduleConfigurator.Configure(Post, "/pattern", GetPattern(), Response, () => Request);
			moduleConfigurator.Configure(Put, "/pattern", PutPattern(), Response, () => Request);
			moduleConfigurator.Configure(Post, "/pattern/change", ChangePatternMatch(), Response, () => Request);

			moduleConfigurator.Configure(Get, "{url}", ParseGet(), Response, () => Request);
			moduleConfigurator.Configure(Get, "Init", Init(), Response, () => Request);

			moduleConfigurator.Configure(Post, "/validate", ValidatePattern(), Response, () => Request);
			moduleConfigurator.Configure(Post, "/tryfind", TryFindPattern(), Response, () => Request);
			moduleConfigurator.Configure(Post, "/validate/save", ValidateAndSavePattern(), Response, () => Request);
			moduleConfigurator.Configure(Post, "/validate/add", ValidateAndAddPattern(), Response, () => Request);


			moduleConfigurator.Configure(Post, "/validate/pattern", GetValidatePattern(), Response, () => Request);

		}

		private Func<dynamic, dynamic> GetContent()
		{
			return __ =>
			{
				string url = WebUtility.HtmlDecode(Request.Query["url"]);
				if (url == null)
					return HttpStatusCode.NoContent;


				var str = _htmlParserModuleService.GetContent(new ValidationRequest()
				{
					Url = url,
					Encoding = Encoding.UTF8,
					UseBrowser = false
				});

				return Response.AsText(str.Content, ContentTypeHtml);

			};
		}

		private Func<dynamic, dynamic> Parse()
		{
			return __ =>
			{

				var model = Request.Body.DeserializeObject<string>();
				if (model == null)
					return HttpStatusCode.NoContent;

				var str = _htmlParserModuleService.Parse(model);
				return Response.AsText(str, ContentType);

			};
		}

		private Func<dynamic, dynamic> ParseContent()
		{
			return __ =>
			{

				var model = Request.Body.DeserializeObject<ParseModelData>();
				if (model == null)
					return HttpStatusCode.NoContent;

				var str = _htmlParserModuleService.ParseContent(model.Url, model.PostData);
				return Response.AsText(str, ContentType);

			};
		}

		private Func<dynamic, dynamic> ParsePost()
		{
			return __ =>
			{
				var model = Request.Body.DeserializeObject<ParseModelData>();
				if (model == null)
					return HttpStatusCode.NoContent;

				var str2 = _htmlParserModuleService.Parse(model.Url, model.PostData);
				return Response.AsText(str2, ContentType);

			};
		}
		private Func<dynamic, dynamic> ParseGet()
		{
			return __ =>
			{
				string model = __.url;
				if (model == null)
					return HttpStatusCode.NoContent;


				var str = _htmlParserModuleService.Parse(model);
				return Response.AsText(str, ContentType);

			};
		}

		private Func<dynamic, dynamic> GetPattern()
		{
			return __ =>
			{
				var model = Request.Body.DeserializeObject<string>();
				if (model == null)
					return HttpStatusCode.NoContent;

				var result = _htmlParserModuleService.GetPatternHostRelations(model);
				return Response.AsText(JsonConvert.SerializeObject(result.MappingScheme(), Formatting.Indented), ContentType);

			};
		}

		private Func<dynamic, dynamic> ValidatePattern()
		{
			return __ =>
			{
				var model = Request.Body.DeserializeObject<PatternValidateRequest>();
				if (model == null)
					return HttpStatusCode.NoContent;


				var mappingScheme = new MappingScheme()
				{
					Matches = new List<KeyValue>()
				};
				var result = MappingScheme(model, mappingScheme);

				return Response.AsText(result, ContentType);

			};
		}

		private Func<dynamic, dynamic> TryFindPattern()
		{
			return __ =>
			{
				var model = Request.Body.DeserializeObject<PatternValidateRequest>();
				if (model == null)
					return HttpStatusCode.NoContent;
				var encoding = Encoding.UTF8;
				if (model.CodePage.HasValue)
					encoding = Encoding.GetEncoding(model.CodePage.Value);

				var result = _htmlParserModuleService.TryFindPattern(model.Url, encoding);

				return Response.AsText(JsonConvert.SerializeObject(result), ContentType);

			};
		}

		private Func<dynamic, dynamic> ValidateAndSavePattern()
		{
			return __ =>
			{
				var model = Request.Body.DeserializeObject<PatternValidateRequest>();
				if (model == null)
					return HttpStatusCode.NoContent;


				var mappingScheme = new MappingScheme()
				{
					Matches = new List<KeyValue>()
				};
				var result = MappingScheme(model, mappingScheme);

				_htmlParserModuleService.Save(model, mappingScheme, true);

				return Response.AsText(result, ContentType);

			};
		}

		private Func<dynamic, dynamic> ValidateAndAddPattern()
		{
			return __ =>
			{
				var model = Request.Body.DeserializeObject<PatternValidateRequest>();
				if (model == null)
					return HttpStatusCode.NoContent;

				var relations = _htmlParserModuleService.GetPatternHostRelations(model.Url);

				var mappingScheme = new MappingScheme()
				{
					Matches = new List<KeyValue>()
				};
				relations.EncodingCode = model.CodePage;
				var result = MappingScheme(model, mappingScheme);

				mappingScheme = relations.MappingScheme();
				MappingScheme(model, mappingScheme);
				_htmlParserModuleService.Save(model, mappingScheme, true);

				return Response.AsText(result, ContentType);

			};
		}

		private string MappingScheme(PatternValidateRequest model, MappingScheme mappingScheme)
		{

			if (model.Patterns.Any())
				mappingScheme.Matches.AddRange(model.Patterns);
			if (model.CleanMatches != null && model.CleanMatches.Any())
				mappingScheme.CleanMatches = model.CleanMatches;


			var encoding = Encoding.UTF8;
			if (model.CodePage.HasValue)
				encoding = Encoding.GetEncoding(model.CodePage.Value);
			var useBrowser = model.UseBrowser.HasValue && model.UseBrowser.Value;

			return _htmlParserModuleService.Validate(new ValidationRequest()
			{
				Url = model.Url,
				Encoding = encoding,
				Map = mappingScheme,
				UseBrowser = useBrowser,
				PostData = model.PostData,
				Headers = model.Headers
			});

		}

		private Func<dynamic, dynamic> GetValidatePattern()
		{
			return __ =>
			{
				var model = Request.Body.DeserializeObject<PatternValidateRequest>();
				if (model == null)
					return HttpStatusCode.NoContent;


				var mappingScheme = new MappingScheme()
				{
					Matches = model.Patterns
				};

				var result = mappingScheme.Matches.FirstOrDefault();
				return Response.AsText(JsonConvert.SerializeObject(result, Formatting.Indented), ContentType);

			};
		}

		private Func<dynamic, dynamic> ChangePatternMatch()
		{
			return __ =>
			{
				var model = Request.Body.DeserializeObject<UpdatePatternRequest>();
				if (model == null)
					return HttpStatusCode.NoContent;


				var relation = _htmlParserModuleService.GetPatternHostRelations(model.Url);
				var map = relation.MappingScheme();
				map.Update(model.Value);
				_htmlParserModuleService.Update(relation);

				return Response.AsText(JsonConvert.SerializeObject(relation, Formatting.Indented), ContentType);


			};
		}



		private Func<dynamic, dynamic> PutPattern()
		{
			return __ =>
			{
				var model = Request.Body.DeserializeObject<PatternHostRelations>();
				if (model == null)
					return HttpStatusCode.NoContent;


				_htmlParserModuleService.Update(model);
				return HttpStatusCode.OK;

			};
		}

		private Func<dynamic, dynamic> Init()
		{
			return __ =>
			{
				_htmlParserModuleService.Init();
				return string.Empty;
			};
		}

	}

	internal class ParseModelData
	{
		public string Url { get; set; }

		public string PostData { get; set; }
	}
}
