using System;

namespace Crawler.Browser.Common.Exceptions
{
	public class ProxyException : Exception
	{
		public string Address { get; }
		public ProxyException(string address, string messge) : base(messge)
		{
			Address = address;
		}

		public override string ToString()
		{
			return $"{base.ToString()}, Address: {Address}";
		}
	}
}