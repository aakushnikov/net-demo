using System.Collections.Generic;

namespace Crawler.Contracts.Services
{
	public interface IProxyContainerService
	{
		ProxyItem Get();

		ProxyItem GetValidProxy();

		List<ProxyItem> GetProxies(bool isValid);

		bool IsValid(ProxyItem proxy);
	}


}
