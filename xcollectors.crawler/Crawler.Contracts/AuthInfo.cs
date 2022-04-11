using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Crawler.Contracts
{
	public class AuthInfo
	{
		public string Login { get; set; }

		public string Password { get; set; }

		public string LoginPageUrl { get; set; }

		public string FirstPageUrl { get; set; }

		public string LoginFieldName { get; set; }

		public string PasswordFieldName { get; set; }

		public string UserAgent { get; set; }

		public string HttpsRefUrl { get; set; }

		public string SuccessIfContainValue { get; set; }

		public ProxyItem Proxy { get; set; }
	}

	public class AuthResult
	{
		public string Content { get; set; }
		public List<Cookie> Cookies { get; set; }

		public bool Success { get; set; }

		public ProxyItem Proxy { get; set; }

		public WebHeaderCollection Headers { get; set; }

	}

	public interface IAuthService
	{
		AuthResult Auth(AuthInfo info);
	}

}
