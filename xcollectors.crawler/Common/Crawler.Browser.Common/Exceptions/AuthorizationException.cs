using System;

namespace Crawler.Browser.Common.Exceptions
{
	public class AuthorizationException : Exception
	{
		public string Login { get; }

		public AuthorizationException(string login, string messge) : base(messge)
		{
			Login = login;
		}


		public override string ToString()
		{
			return $"{base.ToString()}, Login: {Login}";
		}
	}
}