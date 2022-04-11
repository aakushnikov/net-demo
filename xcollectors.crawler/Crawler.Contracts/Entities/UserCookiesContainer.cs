using System.Net;

namespace Crawler.Contracts.Entities
{
	public class UserCookiesContainer
	{
		public string Login { get; set; }

		public string Password { get; set; }

		public CookieCollection Cookies { get; set; }
	}
}
