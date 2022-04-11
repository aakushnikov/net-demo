using System;

namespace Crawler.Browser.Common.Exceptions
{
	public class NoUsersException : Exception
	{
		public NoUsersException(string messge) : base(messge)
		{

		}
	}
}