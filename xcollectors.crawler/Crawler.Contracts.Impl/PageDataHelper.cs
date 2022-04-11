using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Crawler.Contracts.Entities;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Crawler.Contracts.Impl
{
	public class PageDataContext
	{
		public string Html;
		public HtmlNode RootNode;
	}

	public static class PageDataHelper
	{
		private const string CrawlerCleanMatchesAll = "CrawlerCleanMatchesAll";
		private static Regex _emptySpace = new Regex("\\s+");
		//private static Regex _anyWords = new Regex("\\w+");
		public static KeyValue Parse(MappingScheme request, string html)
		{
			//html = WebUtility.HtmlDecode(html);
			var document = new HtmlDocument();
			document.LoadHtml(html);
			var rootNode = document.DocumentNode;
			var pageDataContext = new PageDataContext() { Html = html, RootNode = rootNode };
			KeyValue result = new KeyValue(ActionType.Xpath)
			{
				Name = "Root",
				Values = new List<KeyValue>()
			};

			if (request.Matches != null)
			{
				foreach (var matchKeyValue in request.Matches)
				{
					//var itemResult = GetSingleItemResult(html, result, matchKeyValue, rootNode);

					GetSingleItemResult(pageDataContext, result, matchKeyValue);
					//if (itemResult.Values.Any())
					//	result.AddValue(itemResult);
				}
			}
			if (request.CleanMatches != null)
			{
				foreach (var clean in request.CleanMatches)
				{
					Clear(result.Values, clean);

				}
			}
			return result;
		}

		private static KeyValue GetSingleItemResult(PageDataContext context, KeyValue root, KeyValue matchKeyValue)
		{
			//var itemResult = new KeyValue(ActionType.Xpath)
			//{
			//	Name = matchKeyValue.Name,
			//	Values = new List<KeyValue>(),
			//	Value = matchKeyValue.Value,
			//};

			FillResult(context, root, matchKeyValue);
			return root;
		}

		private static void Clear(List<KeyValue> result, KeyValue clean)
		{
			if (string.IsNullOrEmpty(clean.Name))
				throw new ArgumentNullException("CleanMatch has no name");

			foreach (var item in result.Where(x => clean.Name == CrawlerCleanMatchesAll || x.Name == clean.Name))
			{
				Clear(clean, item);
			}
		}

		private static void Clear(KeyValue clean, KeyValue item)
		{
			if (!string.IsNullOrEmpty(item.Value) && clean.Type == ActionType.Regex && !string.IsNullOrEmpty(clean.Value))
			{
				var regex = new Regex((clean.Value));
				var replacement = clean.AdditionalRegex ?? "";
				item.Value = regex.Replace(item.Value, replacement);
			}

			if (item.Values != null)
			{
				Clear(item.Values, clean);
			}
		}

		//private static void Clear(List<KeyValue> result, KeyValue clean)
		//{
		//	foreach (var value in result)
		//	{
		//		if (!string.IsNullOrEmpty(value?.Value))
		//		{
		//			if (clean.Type == ActionType.Regex && !string.IsNullOrEmpty(clean.Value))
		//			{
		//				var regex = new Regex(clean.Value);
		//				value.Value = regex.Replace(value.Value, "");
		//			}
		//		}
		//		if (value.Values != null)
		//		{
		//			Clear(value.Values, clean);
		//		}
		//	}
		//}

		public static KeyValue CkeckResult(PageDataContext context, HtmlDocument document, KeyValue result)
		{
			;
			return GetSingleItemResult(context, result, result);
		}

		private static void FillResult(PageDataContext context, KeyValue result, KeyValue parent)
		{
			FillResult(context, result, parent, false);
		}

		private static void FillResult(PageDataContext context, KeyValue result, KeyValue parent, bool addNew)
		{

			if (string.IsNullOrEmpty(context.Html) || parent == null)
				return;
			var pattern = parent.Value;
			switch (parent.Type)
			{
				case ActionType.Json:
					ParseJson(context, result, parent);
					break;
				case ActionType.Regex:
					ParseRegex(context, result, parent, pattern, addNew);
					break;
				case ActionType.Xpath:
					//var parentNode = context.RootNode.ParentNode;
					//var tmp = HtmlNode.CreateNode($"<div></div>");
					//var clonedNode = context.RootNode;
					//tmp.AppendChild(clonedNode);
					//parentNode?.RemoveChild(context.RootNode);
					try
					{
						ParseXPath(context, result, parent, pattern, addNew);
					}
					finally
					{
						//parentNode?.AppendChild(context.RootNode);
						//tmp.RemoveChild(clonedNode);
						
					}

					break;
			}

		}

		private static void ParseXPath(PageDataContext context, KeyValue result, KeyValue parent, string pattern, bool addNew)
		{
			var rootNode = context.RootNode;
			var clonedNode = context.RootNode;
			if (parent.Name == "Content")
				;
			if (context.RootNode.Name != "#document")
			{
				clonedNode = clonedNode.CloneNode(true);
				var tmp = HtmlNode.CreateNode($"<div></div>");
				clonedNode.ParentNode?.RemoveChild(clonedNode);
				tmp.AppendChild(clonedNode);
				clonedNode = tmp;
				//clonedNode = HtmlNode.CreateNode($"<div>{rootNode.InnerHtml}</div>");

			}


			Regex _att = new Regex(@"/@((?'name')[\d\w\-]+)\b");
			var attName = string.Empty;
			var matchAtt = _att.Match(pattern);
			if (matchAtt.Success)
			{
				attName = matchAtt.Value.Replace("/@", "");
				pattern = pattern.Substring(0, matchAtt.Index);
			}
			var nodes = clonedNode.SelectNodes(pattern);
			
			if (pattern.StartsWith("@"))
			{
				attName = pattern.Replace("@", "");
				nodes = new HtmlNodeCollection(rootNode.ParentNode);
				nodes.Add(rootNode);
			}


			if (nodes != null)
			{
				var groups = nodes.GroupBy(x => x.ParentNode).ToList();
				if (!addNew)
					addNew = groups.Count > 1;
				HtmlNode prevNode = null;
				//foreach (var group in nodes)
				{
					KeyValue currentKey = null;
					var listPrev = new List<HtmlNode>();
					foreach (var node in nodes)
					{
						var curentNode = node.ParentNode;
						var innerHtml = parent.IsOuterHtml ? node.OuterHtml : node.InnerHtml;
						if (!string.IsNullOrEmpty(attName))
						{
							var att = node.Attributes[attName];
							if (att == null)
								continue;
							innerHtml = att.Value;
						}

						var value = !string.IsNullOrEmpty(parent.AdditionalRegex)
							? Regex.Match(innerHtml, parent.AdditionalRegex).Value
							: innerHtml;


						if (node.ParentNode != null &&
							(node.ParentNode.Name == "style"
							|| node.ParentNode.Name == "script"))
							continue;

						var match = _emptySpace.Match(innerHtml);
						if (match.Success && match.Length == innerHtml.Length)
							continue;


						currentKey = Create(parent, value);
						if (parent?.Values?.Count > 0)
						{
							currentKey = Create(parent, value);

							foreach (var match1 in parent.Values)
							{

								var currentParent = match1.UseParentNode ? node.ParentNode : node;
								if (string.IsNullOrEmpty(value))
									value = parent.IsOuterHtml ? currentParent.OuterHtml : currentParent.InnerHtml;
								FillResult(
									new PageDataContext()
									{
										RootNode = currentParent,
										Html = value,
									},
									currentKey,
									match1,
									!match1.NeedConcat);
							}
							AddResult(result, parent, currentKey);

						}
						else if (!string.IsNullOrEmpty(currentKey.Value))
						{
							result.AddValue(currentKey, addNew);
						}

						if (!string.IsNullOrEmpty(currentKey?.Value)
							&& node.Name == "#text"
							&& prevNode != null
							&& curentNode != prevNode
							&& !listPrev.Contains(curentNode))
						{

							switch (curentNode?.Name)
							{
								case "p":
								case "h1":
								case "h2":
								case "h3":
								case "div":
									currentKey.IsNewRow = true;
									//if (!keyValueNode.Value.EndsWith(System.Environment.NewLine))
									//	keyValueNode.Value = $"{keyValueNode.Value}{System.Environment.NewLine}";
									break;
							}
						}
						prevNode = curentNode;
						listPrev.Add(prevNode);
					}
				}
			}
			return;
		}

		private static void ParseRegex(PageDataContext context, KeyValue result, KeyValue parent, string pattern, bool addNew)
		{
			var regex = new Regex(pattern);
			var matches = regex.Matches(context.Html);
			if (!addNew)
				addNew = matches.Count > 1;

			foreach (Match match in matches)
			{
				var child = new PageDataContext()
				{
					RootNode = context.RootNode,
					Html = match.Value,
				};
				var value = match.Value;
				var keyValue = Create(parent, value);
				if (parent.Values != null)
				{
					foreach (var match1 in parent.Values)
					{
						if (match1.Type == ActionType.Xpath)
						{
							HtmlDocument html2 = new HtmlDocument();
							value = value.Replace("\\", "");
							html2.LoadHtml(value);
							//var cleanedContent = WebUtility.HtmlDecode(html2.DocumentNode.InnerText);
							//keyValue = Create(parent, html2.to);
							var p = new PageDataContext()
							{
								Html = value,
								RootNode = html2.DocumentNode
							};
							FillResult(p, keyValue, match1, match1.NeedConcat);
						}
						else
						{
							FillResult(child, keyValue, match1, match1.NeedConcat);
						}
					}
				}

				result.AddValue(keyValue, addNew || keyValue.NeedConcat);
			}
		}

		private static void ParseJson(PageDataContext context, KeyValue result, KeyValue parent)
		{

			dynamic data = null;
			try
			{
				if (parent.IsStringValue)
					JsonConvert.DefaultSettings = () => new JsonSerializerSettings { DateParseHandling = DateParseHandling.None };
				else
					JsonConvert.DefaultSettings = () => new JsonSerializerSettings { DateParseHandling = DateParseHandling.DateTime };

				data = JsonConvert.DeserializeObject<dynamic>(context.Html);
			}
			catch
			{
			}
			if (data == null)
				return;
			var o = data as JContainer;
			if (o == null)
				return;
			var tokens = o.SelectTokens(parent.Value).ToList();
			
			foreach (var token in tokens)
			{
				var val = token?.ToString();
				if (val != null)
				{
					var valueJ = val.ToString();
					KeyValue currentKey = Create(parent, valueJ);
					if (parent.Values != null && parent.Values.Any())
					{
						foreach (var match1 in parent.Values)
						{
							if (match1.Type == ActionType.Xpath)
							{
								HtmlDocument html2 = new HtmlDocument();
								valueJ = valueJ.Replace("\\", "");
								html2.LoadHtml(valueJ);
								//var cleanedContent = WebUtility.HtmlDecode(html2.DocumentNode.InnerText);
								//keyValue = Create(parent, html2.to);
								var @new = parent?.Values?.Count > 1;
								if (parent.Name == match1.Name)
									@new = false;
								FillResult(new PageDataContext()
								{
									RootNode = html2.DocumentNode,
									Html = valueJ,

								}, currentKey, match1, @new);
							}
							else
							{

								FillResult(new PageDataContext()
								{
									Html = valueJ,
									RootNode = context.RootNode
								}, currentKey, match1);
							}
						}
						AddResult(result, parent, currentKey);
					}
					else
					{
						result.AddValue(currentKey, true);
					}
				}
			}
		}

		private static void AddResult(KeyValue result, KeyValue parent, KeyValue currentKey)
		{
			if (currentKey?.Values?.Count > 0)
			{
				bool addAsIs = true;
				foreach (var v in currentKey.Values)
				{
					if (v.Name == currentKey.Name)
					{
						result.AddValue(v, !parent.NeedConcat);
						addAsIs = false;
					}
				}
				if (addAsIs)
					result.AddValue(currentKey, !parent.NeedConcat);
			}
		}


		private static KeyValue Create(KeyValue parent, string value)
		{
			var keyValueNode = new KeyValue(parent.Type)
			{
				Name = parent.Name,
				Values = new List<KeyValue>(),
				Value = value,
				IsStringValue = parent.IsStringValue,
				IsBranch = parent.IsBranch,
				NeedConcat = parent.NeedConcat,
				FirstValue = parent.FirstValue,
				PrevValue = parent.PrevValue,
				AfterValue = parent.AfterValue,
				ConcatValue = parent.ConcatValue,
				UseParentNode = parent.UseParentNode,
				IsOuterHtml = parent.IsOuterHtml,
				IsArray = parent.IsArray,
				IsObject = parent.IsObject,
				IsJson = parent.IsJson,
				AdditionalRegex = parent.AdditionalRegex
			};
			return keyValueNode;
		}
	}
}