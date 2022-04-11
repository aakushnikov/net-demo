using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Crawler.Contracts;
using Crawler.Contracts.Services;
using Crawler.Contracts.Services;

namespace Crawler.Api.Host.Proxies
{
	public class HtmlParserService : IHtmlParserService
	{
		private readonly IProxyContainerService _proxyContainerService;
		public HtmlParserService(IProxyContainerService proxyContainerService)
		{
			_proxyContainerService = proxyContainerService;
		}

		public ParseResult GetContent(string url)
		{
			return GetContent(url, Encoding.UTF8, string.Empty, new List<Cookie>(), null);
		}

		public ParseResult GetContent(string url, Encoding encoding)
		{
			return GetContent(url, encoding, string.Empty, new List<Cookie>(), null);
		}
		public ParseResult GetContent(string url, Encoding encoding, string postData)
		{
			return GetContent(url, encoding, postData, new List<Cookie>(), null);
		}

		public ParseResult GetContent(string url, Encoding encoding, string postData, List<Cookie> cookies, ProxyItem proxy)
		{
			var key = "get.content";
			try
			{
				ThreadKeyLock.Instance.Register(key, 300);
				return ParseResultInternal(url, encoding, postData, cookies, proxy);
			}
			finally
			{
				ThreadKeyLock.Instance.Release(key);
			}

		}

		public Stream GetStream(string url)
		{
			var proxy = _proxyContainerService.GetValidProxy();
			var request = GetHttpWebRequest(url, string.Empty, new List<Cookie>(), proxy);
			var memoryStream = new MemoryStream();
			using (var webResponse = (HttpWebResponse)request.GetResponse())
			{
				using (var stream = webResponse.GetResponseStream())
				{
					if (stream != null)
					{
						stream.CopyTo(memoryStream);
						memoryStream.Flush();
						memoryStream.Position = 0;

					}
					stream?.Close();
				}
			}
			return memoryStream;
		}

		private ParseResult ParseResultInternal(string url, Encoding encoding, string postData, List<Cookie> cookies, ProxyItem proxy)
		{
			encoding = encoding ?? Encoding.UTF8;
			proxy = proxy ?? _proxyContainerService.GetValidProxy();
			var request = GetHttpWebRequest(url, postData, cookies, proxy);

			if (!string.IsNullOrEmpty(postData))
			{
				// Write post data
				var dataBytes = encoding.GetBytes(postData);
				request.ContentLength = dataBytes.Length;
				using (var requestStream = request.GetRequestStream())
				{
					requestStream.Write(dataBytes, 0, dataBytes.Length);
				}
			}
			var parseResult = GetContent(encoding, request);
			parseResult.Proxy = proxy?.Address;
			return parseResult;
		}

		private HttpWebRequest GetHttpWebRequest(string url, string postData, List<Cookie> cookies, ProxyItem proxy)
		{
			var cookieContainer = new CookieContainer();
			if (cookies != null)
			{
				foreach (var c in cookies)
				{
					cookieContainer.Add(c);
				}
			}


			var request = (HttpWebRequest)WebRequest.Create(url);
			if (!string.IsNullOrEmpty(postData))
			{
				request.Method = "POST";
			}

			Uri uri;

			request.UserAgent =
				"Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/77.0.3865.120 YaBrowser/19.10.1.238 Yowser/2.5 Safari/537.36";
			//ext/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/
			request.Accept =
				"text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3";
			request.Headers.Add("Accept-Language", "ru");
			if (Uri.TryCreate(url, UriKind.Absolute, out uri))
			{
				request.Referer = uri.Host;
				request.Host = uri.Host;
			}
			else
			{
				request.Referer = url;
			}
			request.ContentType = "application/x-www-form-urlencoded";

			request.KeepAlive = true;
			request.AllowAutoRedirect = true;
			//Connection: keep-alive
			request.CookieContainer = cookieContainer;


			if (proxy != null)
			{
				var myProxy = new WebProxy(proxy.Address, false);
				request.Proxy = myProxy;
				if (!string.IsNullOrEmpty(proxy.User))
				{
					request.Proxy.Credentials = new NetworkCredential(proxy.User, proxy.Password);
				}
			}
			return request;
		}

		private static ParseResult GetContent(Encoding encoding, HttpWebRequest request)
		{
			var result = new ParseResult()
			{
				Content = string.Empty
			};
			using (var webResponse = (HttpWebResponse)request.GetResponse())
			{
				if (webResponse.Headers["Content-Type"] != null &&
					webResponse.Headers["Content-Type"].ToLower().IndexOf("windows-1251") > 0)
				{
					encoding = Encoding.GetEncoding("windows-1251");
				}
				if (webResponse.Headers["Content-Type"] != null && webResponse.Headers["Content-Type"].ToLower().Contains("utf-8"))
				{
					encoding = Encoding.UTF8;
				}


				using (var stream = webResponse.GetResponseStream())
				{
					if (stream != null)
					{
						using (var streamReader = new StreamReader(stream, encoding))
						{
							if (Encoding.UTF8 != encoding)
							{
								var content = streamReader.ReadToEnd();
								var bytes = encoding.GetBytes(content);

								result.Content = Encoding.UTF8.GetString(Encoding.Convert(encoding, Encoding.UTF8, bytes));
							}
							else
							{
								result.Content = streamReader.ReadToEnd();
							}
							result.Url = webResponse.ResponseUri.ToString();
							streamReader.Close();
						}
					}
					stream?.Close();
				}
			}
			return result;
		}
	}
}
