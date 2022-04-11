using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using Newtonsoft.Json.Linq;

namespace Crawler.Web.Host
{

	public abstract class HierarchyData<T> : IHierarchyData
	{
		public abstract HierarchyData<T> GetParent();
		public abstract IHierarchicalEnumerable GetChildren();
		public abstract T Item { get; }

		#region IHierarchyData Members

		IHierarchyData IHierarchyData.GetParent()
		{
			return GetParent();
		}

		public abstract bool HasChildren { get; }

		IHierarchicalEnumerable IHierarchyData.GetChildren()
		{
			return GetChildren();
		}

		object IHierarchyData.Item
		{
			get { return Item; }
		}

		public abstract string Path { get; }
		public abstract string Type { get; }

		#endregion
	}

	public class HierarchyDataJson : HierarchyData<JToken>, IHierarchicalEnumerable
	{
		private readonly HierarchyDataJson _parent;
		private readonly List<HierarchyDataJson> _child = new List<HierarchyDataJson>();

		public HierarchyDataJson(JToken token, HierarchyDataJson parent)
		{
			Item = token;

			_parent = parent;
			foreach (var child in Item.Children())
			{
				_child.Add(new HierarchyDataJson(child, this));
			}

		}

		public override HierarchyData<JToken> GetParent()
		{
			return _parent;
		}

		public override bool HasChildren => _child.Any();

		public override string Path => Item.Path;

		public override string Type => Item.Type.ToString();

		public override IHierarchicalEnumerable GetChildren()
		{
			return this;
		}

		public override JToken Item { get; }

		public IEnumerator GetEnumerator()
		{
			return _child.GetEnumerator();
		}

		public IHierarchyData GetHierarchyData(object enumeratedItem)
		{
			return _child.Find(x => x == enumeratedItem);
		}

		public override string ToString()
		{
			return Parse(Item);
		}

		private string Parse(JToken item)
		{
			var property = item as JProperty;
			var value = item as JValue;
			var array = item as JArray;
			var obj = item as JObject;
			if (property != null)
			{
				var array2 = property.Value as JArray;
				if (array != null)
				{
					return $"{property.Name}";
				}
				else
				{
					return $"{property.Name}";
				}
			}
			else if (value != null)
			{
				return $"{value.Value}";
			}
			else if (array != null)
			{
				return $"{array.Type}";
			}
			else if (obj != null)
			{
				return $"{obj.Type}";
			}
			return item.ToString();
		}
	}
}