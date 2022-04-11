using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler.Contracts.Entities
{
	public sealed class Host
	{
		public int Id { get; set; }

		public string Name { get; set; }

		public string Url { get; set; }
	}
}
