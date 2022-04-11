//using System.Collections.Generic;
//using System.Net;
//using System.Text;
//using Crawler.Contracts;
//using Crawler.Contracts.Services;
//using Service.Registry.Common;
//namespace Crawler.Contracts.Impl.Proxies
//{
//	public class WebHtmlParserService : IWebHtmlParserService
//	{
//		private readonly IServiceRegistryFactory _serviceRegistryFactory;


//		public WebHtmlParserService(IServiceRegistryFactory serviceRegistryFactory)
//		{
//			_serviceRegistryFactory = serviceRegistryFactory;
//		}

//		public ParseResult GetContent(string url)
//		{

//			var data = _serviceRegistryFactory.CreateProxy<IWebService>().Browse(url);
//			var result = new ParseResult()
//			{
//				Content = data.Content,
//				Url = data.Url
//			};
//			return result;

//		}

//		public ParseResult GetContent(string url, Encoding encoding)
//		{
//			return GetContent(url, encoding, string.Empty, new List<Cookie>(), null);
//		}

//		public ParseResult GetContent(string url, Encoding encoding, string postData, List<Cookie> cookies, ProxyItem proxy)
//		{
//			var data = _serviceRegistryFactory.CreateProxy<IWebService>().BrowseCodePage(url, encoding.CodePage);
//			var result = new ParseResult()
//			{
//				Content = data.Content,
//				Url = data.Url
//			};
//			return result;
//		}

//		public ParseResult GetContent(string url, Encoding encoding, string postData)
//		{
//			return GetContent(url, encoding, postData, new List<Cookie>(), null);

//		}
//	}
//}
