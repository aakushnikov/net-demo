using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler.Contracts.Services
{
	public interface IWebService
	{
		WebServiceResult Browse(string url);

		WebServiceResult BrowseCodePage(string url, int codePage);

		WebServiceResult BrowseProxy(string url, ProxyItem proxy);

		void Register(string url, ProxyItem proxy);

		void Register(string url);

		void Release();
	}
}
