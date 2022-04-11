using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler.Contracts.Entities
{
	public sealed class PatternHostRelations
	{
		public int Id { get; set; }

		public Pattern Pattern { get; set; }
		public Host Host { get; set; }
		public string Url { get; set; }
		public int? EncodingCode { get; set; }

		public bool? UseBrowser { get; set; }
	}

	
}
