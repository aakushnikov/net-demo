using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Crawler.Contracts;
using Crawler.Contracts.Services;

namespace Crawler.Contracts.Services
{

	

	public interface IHtmlParserService
	{
		ParseResult GetContent(string url);
		ParseResult GetContent(string url, Encoding encoding);

		ParseResult GetContent(string url, Encoding encoding, string postData);

		ParseResult GetContent(string url, Encoding encoding, string postData, List<Cookie> cookies,
			ProxyItem proxy);

		Stream GetStream(string url);
	}

	public interface IWebHtmlParserService
	{
		ParseResult GetContent(string url);
		ParseResult GetContent(string url, Encoding encoding);

		ParseResult GetContent(string url, Encoding encoding, string postData, List<Cookie> cookies, ProxyItem proxy);
	}

}
