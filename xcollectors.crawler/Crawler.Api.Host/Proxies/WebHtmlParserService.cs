using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Crawler.Contracts;
using Crawler.Contracts.Services;
using Service.Registry.Common;

namespace Crawler.Api.Host.Proxies
{
	public class WebHtmlParserService : IWebHtmlParserService
	{
		private readonly IServiceRegistryFactory _serviceRegistryFactory;


		public WebHtmlParserService(IServiceRegistryFactory serviceRegistryFactory)
		{
			_serviceRegistryFactory = serviceRegistryFactory;
		}

		public ParseResult GetContent(string url)
		{
			return GetContent(url, Encoding.Default);
		}

		public ParseResult GetContent(string url, Encoding encoding)
		{
			return GetContent(url, encoding, string.Empty, new List<Cookie>(), null);
		}

		public ParseResult GetContent(string url, Encoding encoding, string postData, List<Cookie> cookies, ProxyItem proxy)
		{
			return null;
			//var browser = _serviceRegistryFactory.CreateRest<ICefBrowserService, CefBrowserService>();
			//var content = proxy != null
			//	? browser.GetContent(url, encoding, proxy)
			//	: browser.GetContent(url, encoding);

			//if (!content.IsNullOrEmpty())
			//{
			//	content = Regex.Unescape(content);
			//}

			//return new ParseResult
			//{
			//	Content = content,
			//	Url = url
			//};
		}

		public ParseResult GetContent(string url, Encoding encoding, string postData)
		{
			return GetContent(url, encoding, postData, new List<Cookie>(), null);
		}
	}
}
