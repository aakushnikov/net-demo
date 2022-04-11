using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Crawler.Web.Host.Entities
{
	public static class Helper
	{
		public static List<KeyValue> Children(this List<KeyValue> matches)
		{
			var matcheChildren = new List<KeyValue>();
			if (matches == null)
			{
				return matcheChildren;
			}
			foreach (var m in matches)
			{
				matcheChildren.Add(m);
				if (m?.Values != null)
					matcheChildren.AddRange(m.Values.Children());
			}
			return matcheChildren;
		}
	}
}