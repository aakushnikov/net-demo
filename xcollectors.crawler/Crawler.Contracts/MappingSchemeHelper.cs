using System.Collections.Generic;
using Crawler.Contracts.Entities;

namespace Crawler.Contracts
{
	public static class MappingSchemeHelper
	{
		public static MappingScheme GetPostParserMatches(string path)
		{
			return GetPostParserMatches(new List<string>() { path });
		}

		public static MappingScheme GetPostParserMatches(List<string> paths)
		{
			return GetPostParserMatches(paths.ToArray());
		}

		public static MappingScheme GetPostParserMatches(string[] paths)
		{
			return GetPostParserMatches(paths, true);
		}

		public static MappingScheme GetPostParserMatches(string[] paths, bool appendImagePathern)
		{
			var match = new MappingScheme();
			var result = new List<KeyValue>();
			foreach (var path in paths)
			{
				var imagesXPathKeyValue = GetImagesXPathKey(path);

				result.Add(
					new KeyValue(ActionType.Xpath)
					{
						Name = KeyNames.PostContent.ToString(),
						Value = path,
					});


				result.Add(imagesXPathKeyValue);
			}
			match.Matches = result;
			return match;
		}

		public static List<KeyValue> Children(this List<KeyValue> matches)
		{
			var matcheChildren = new List<KeyValue>();
			if (matches == null)
			{
				return matcheChildren;
			}
			foreach (var m in matches)
			{
				matcheChildren.Add(m);
				if (m?.Values != null)
					matcheChildren.AddRange(m.Values.Children());
			}
			return matcheChildren;
		}

		public static KeyValue GetImagesXPathKey(string path)
		{
			var imagePath = path;
			var index = path.LastIndexOf("/");
			if (index > 2)
			{
				imagePath = path.Substring(0, index);
				if (imagePath[imagePath.Length - 1] == '/')
					imagePath = imagePath.Substring(0, imagePath.Length - 1);
			}
			var imagesXPathKeyValue = GetImagesXPathKeyValue(imagePath);
			return imagesXPathKeyValue;
		}



		public static KeyValue GetImagesXPathKeyValue(string imagePath)
		{
			return new KeyValue(ActionType.Xpath)
			{
				Name = KeyNames.ImageTagUrl.ToString(),

				IsOuterHtml = true,
				Value = imagePath + "//img",
				Values = new List<KeyValue>()
				{
					GetImageRegex()
				}
			};
		}

		public static KeyValue GetImageRegex()
		{
			return new KeyValue(ActionType.Regex)
			{
				Value = "(?<=src=\")[^\"]+",
				Name = KeyNames.ImageUrl.ToString()
			};
		}

		public static KeyValue GetAuthorUrlXPathKeyValue(string authorUrl)
		{
			return new KeyValue(ActionType.Xpath)
			{
				Name = KeyNames.AuthorUrl.ToString(),

				IsOuterHtml = true,
				Value = authorUrl,
				Values = new List<KeyValue>()
				{
					GetAuhtorUrl()
				}
			};
		}

		public static KeyValue GetAuhtorUrl()
		{
			return new KeyValue(ActionType.Regex)
			{
				Value = "(?<=(href|src)=\")[^\"]+",
				Name = KeyNames.AuthorUrl.ToString()
			};
		}
	}
}
