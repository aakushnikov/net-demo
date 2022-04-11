using System;

namespace Crawler.Browser.Common.Exceptions
{
	public class LoadPageException : Exception
	{
		public LoadPageException(string messge) : base(messge)
		{

		}
	}
}