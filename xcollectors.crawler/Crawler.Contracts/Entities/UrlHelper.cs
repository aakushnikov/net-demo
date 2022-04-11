using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler.Contracts.Entities
{
	public static class UrlHelper
	{
		public static string GetNormalizedUrl(string url)
		{
			var tolowerUrl = url;
			if (tolowerUrl.StartsWith("http://"))
			{
				tolowerUrl = tolowerUrl.Remove(0, 7);
			}
			if (tolowerUrl.StartsWith("https://"))
			{
				tolowerUrl = tolowerUrl.Remove(0, 8);
			}
			if (tolowerUrl.StartsWith("www."))
			{
				tolowerUrl = tolowerUrl.Remove(0, 4);
			}
			if (tolowerUrl.EndsWith("/"))
			{
				tolowerUrl = tolowerUrl.Remove(tolowerUrl.Length - 1, 1);
			}
			if (tolowerUrl.EndsWith("?"))
			{
				tolowerUrl = tolowerUrl.Remove(tolowerUrl.Length - 1, 1);
			}
			if (tolowerUrl.EndsWith("?rss"))
			{
				tolowerUrl = tolowerUrl.Remove(tolowerUrl.Length - 4, 4);
			}
			if (tolowerUrl.EndsWith("#"))
			{
				tolowerUrl = tolowerUrl.Remove(tolowerUrl.Length - 1, 1);
			}
			return $"http://{tolowerUrl}";
		}

		public static string GetHostUrl(string uri)
		{
			return GetHostUrl(uri, true);
		}

		public static string GetHostUrl(string uri, bool splitByDot)
		{
			var host = GetNormalizedUrl(uri);
			Uri url = null;
			if (Uri.TryCreate(uri, UriKind.Absolute, out url))
			{
				var domains = url.Host.Split('.');
				if (splitByDot && domains.Length > 2)
				{
					host = GetNormalizedUrl($"{domains[domains.Length - 2]}.{domains[domains.Length - 1]}");
				}
				else
				{
					host = GetNormalizedUrl(url.Host);
				}
			}
			return host;
		}

	}
}
