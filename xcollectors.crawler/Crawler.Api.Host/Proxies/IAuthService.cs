using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Crawler.Contracts;
using Crawler.Contracts.Impl.Entities;

namespace Crawler.Api.Host.Proxies
{
	
	public class AuthService : IAuthService
	{
		private readonly ICache<AuthResult> _cache;

		private const string DefaultUserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; WOW64; Trident/7.0)";
		private readonly Regex _regexInputTag = new Regex(@"<input([^>]+)/>");

		public AuthService(ICache<AuthResult> cache)
		{
			_cache = cache;
		}

		private string GetPostDataFromContent(string content, AuthInfo info)
		{
			var dictOfPostData = new Dictionary<string, string>();
			dictOfPostData.Add(info.LoginFieldName, info.Login);
			dictOfPostData.Add(info.PasswordFieldName, info.Password);


			if (!string.IsNullOrEmpty(content))
			{
				var mathes2 = _regexInputTag.Matches(content);
				foreach (var match in mathes2)
				{
					var item = XElement.Parse(((Match)match).Value);

					var attrbuteName = item.Attributes(XName.Get("name")).FirstOrDefault();
					//var attrbuteId = item.Attributes(XName.Get("id")).FirstOrDefault();
					var attrbuteValue = item.Attributes(XName.Get("value")).FirstOrDefault();
					if (attrbuteName != null && !dictOfPostData.ContainsKey(attrbuteName.Value))
					{
						if (info.LoginFieldName == attrbuteName?.Value)
						{
							dictOfPostData.Add(attrbuteName.Value, info.Login);
						}
						else if (info.PasswordFieldName == attrbuteName.Value)
						{
							dictOfPostData.Add(attrbuteName.Value, info.Password);
						}
						else
						{
							if (attrbuteValue?.Value == null)
								continue;

							switch (attrbuteName.Value)
							{
								default:
									if (String.Equals(info.LoginFieldName, attrbuteName.Value, StringComparison.CurrentCultureIgnoreCase))
									{
										dictOfPostData.Add(attrbuteName.Value, info.Login);
									}
									if (String.Equals(info.PasswordFieldName, attrbuteName.Value, StringComparison.CurrentCultureIgnoreCase))
									{
										dictOfPostData.Add(attrbuteName.Value, info.Password);
									}
									dictOfPostData.Add(attrbuteName.Value,
										attrbuteValue?.Value);

									break;

							}
						}
					}

				}
			}


			return string.Join("&", dictOfPostData.Select(pair => $"{pair.Key}={pair.Value}"));
			;
		}

		/// <summary>
		/// Get content from web page or write post data
		/// If post data empty call method=GET, else POST
		/// </summary>
		/// <param name="url"></param>
		/// <param name="info"></param>
		/// <param name="postData"></param>
		/// <returns></returns>
		public void GetContent(string url, AuthInfo info, string postData, AuthResult res)
		{
			var cookieContainer = new CookieContainer();
			if (res.Cookies != null)
			{
				foreach (var c in res.Cookies)
				{
					cookieContainer.Add(c);
				}
			}
			else
			{
				res.Cookies = new List<Cookie>();
			}




			var encoding = Encoding.UTF8;

			var webRequest = (HttpWebRequest)WebRequest.Create(url);

			if (!string.IsNullOrEmpty(postData))
			{
				webRequest.Method = "POST";
			}

			webRequest.Referer = info.HttpsRefUrl ?? url;

			webRequest.UserAgent = info.UserAgent ?? DefaultUserAgent;
			webRequest.Accept = "text/html, application/xhtml+xml, */*";
			if (res.Headers != null)
			{
				foreach (var header in res.Headers.Keys)
				{
					var s = header.ToString();
					var values = res.Headers.GetValues(s);
					webRequest.Headers.Add(s, values.FirstOrDefault());
					
				}
				//webRequest.Headers = res.Headers;
			}
			webRequest.Headers.Add("Accept-Language", "ru");
			webRequest.ContentType = "application/x-www-form-urlencoded";
			webRequest.CookieContainer = cookieContainer;
			webRequest.KeepAlive = true;
			webRequest.AllowAutoRedirect = false;
			if (info.Proxy != null)
			{
				var myProxy = new WebProxy(info.Proxy.Address, false);
				webRequest.Proxy = myProxy;
				//webRequest.Timeout = 12000;
				if (!string.IsNullOrEmpty(info.Proxy.User))
				{
					webRequest.Proxy.Credentials = new NetworkCredential(info.Proxy.User, info.Proxy.Password);
				}
			}


			if (!string.IsNullOrEmpty(postData))
			{
				// Write post data
				//var htmlDecode = WebUtility.UrlEncode(postData);
				var htmlDecode = WebUtility.HtmlEncode(postData);
				var dataBytes = encoding.GetBytes(htmlDecode);
				webRequest.ContentLength = dataBytes.Length;
				using (var requestStream = webRequest.GetRequestStream())
				{
					requestStream.Write(dataBytes, 0, dataBytes.Length);
					requestStream.Close();
				}

			}

			// make request
			using (var webResponse = (HttpWebResponse)webRequest.GetResponse())
			{
				if (webResponse.Headers["Content-Type"] != null && webResponse.Headers["Content-Type"].IndexOf("windows-1251", StringComparison.Ordinal) > 0)
				{
					encoding = Encoding.GetEncoding("windows-1251");
				}
				using (var stream = webResponse.GetResponseStream())
				{
					res.Headers = webResponse.Headers;
					var uri = new Uri(url);
					var local = new Uri("http://" + uri.Host);
					ParseCookie(webResponse, cookieContainer, local);
					foreach (Cookie x in cookieContainer.GetCookies(local))
					{
						res.Cookies.Add(x);
					}
					if (webResponse.Cookies != null)
					{
						foreach (Cookie cookie in webResponse.Cookies)
						{
							if (res.Cookies == null)
								res.Cookies = new List<Cookie>();
							var find = res.Cookies.Find(x => x.Name == cookie.Name);
							if (find != null)
							{
								find.Value = cookie.Value;
							}
							else
							{
								res.Cookies.Add(cookie);
							}

						}
					}
					using (var streamReader = new StreamReader(stream, encoding))
					{
						res.Content = streamReader.ReadToEnd();
					}
				}

				if (!string.IsNullOrEmpty(info.SuccessIfContainValue))
				{
					res.Success = res.Content.Contains(info.SuccessIfContainValue);
				}
				// If we have status 302
				if (webResponse.StatusCode == HttpStatusCode.Found || webResponse.StatusCode == HttpStatusCode.MovedPermanently)
				{
					var redirectUrl = Convert.ToString(webResponse.Headers["Location"]);
					// call handly
					this.GetContent(redirectUrl, info, string.Empty, res);
				}
			}

		}

		private static void ParseCookie(HttpWebResponse response, CookieContainer cookieJar, Uri uri)
		{
			var CookieContainer = new CookieContainer(20);
			var regex = new Regex("expires=.*$");
			var cookies = response?.Headers?.GetValues("Set-Cookie");
			if (cookies == null || cookies.Length == 0)
			{
				return;
			}
			foreach (var headerCookie in cookies)
			{
				if (headerCookie.Contains("access"))
				{
					var cookie = regex.Replace(headerCookie, "");
					var c = new Cookie();
					CookieContainer.SetCookies(uri, cookie);
					cookieJar.SetCookies(uri, cookie);
				}
			}
		}

		public AuthResult Auth(AuthInfo info)
		{

			var result = _cache[info.Login];
			if (result != null)
				return result;
			result = result ?? new AuthResult();
			GetContent(info.FirstPageUrl, info, string.Empty, result);
			var postdata = GetPostDataFromContent(result.Content, info);
			GetContent(info.LoginPageUrl, info, postdata, result);
			if (result.Success)
			{
				result.Proxy = info.Proxy;
				_cache[info.Login] = result;
			}
			return result;
		}
	}
}
