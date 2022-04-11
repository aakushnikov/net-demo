using System;
using System.Collections.Generic;
using System.Text;

namespace Crawler.Contracts.Impl.Proxies
{
	internal class ProxyItemDataResult
	{
		public ProxyItemData[] Items { get; set; }
	}

	internal class ProxyItemData
	{

		public int Id { get; set; }

		public string Address { get; set; }

		public int Port { get; set; }

		public string User { get; set; }

		public string Password { get; set; }
	}
}
