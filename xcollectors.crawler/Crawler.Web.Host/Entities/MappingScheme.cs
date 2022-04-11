using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Crawler.Web.Host.Entities
{
	public class MappingScheme
	{
		public List<KeyValue> Matches { get; set; }

		public List<KeyValue> this[string name]
		{
			get { return Matches.Children().Where(x => x.Name == name).ToList(); }
		}



		public List<KeyValue> CleanMatches { get; set; }

		public void Update(KeyValue value)
		{
			var keys = Matches.Find(m => m.Name.Equals(value.Name));
			if (keys != null)
			{
				Matches.Remove(keys);
			}
			Matches.Add(value);
		}
	}
}