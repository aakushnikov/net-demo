using System;
using System.Net.Http;

namespace Crawler.Browser.Common.AccountManagment
{
	internal class HttpClientFactory
	{
		private readonly Uri _baseApiAddres;

		public HttpClientFactory(string baseApiAddres)
		{
			_baseApiAddres = new Uri(baseApiAddres);
		}


		public HttpClient Get()
		{
			return new HttpClient
			{
				BaseAddress = _baseApiAddres
			};
		}

	}
}