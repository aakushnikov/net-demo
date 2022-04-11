using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler.Contracts.Entities
{
	public sealed class KeyContainer
	{
		public List<KeyValue> array = new List<KeyValue>();
		public List<KeyValue> values = new List<KeyValue>();
		public List<IGrouping<string, KeyValue>> group = new List<IGrouping<string, KeyValue>>();
		public List<KeyValue> obj = new List<KeyValue>();
		public List<KeyValue> needConcat = new List<KeyValue>();
	}
}
