using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crawler.Contracts.Entities;

namespace Crawler.Contracts.Repositories
{
	public interface IHostRepository
	{
		Host Get(string url);
		Host Save(Host host);
	}


}
