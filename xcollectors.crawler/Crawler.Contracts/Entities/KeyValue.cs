using System.Collections.Generic;
using System.Linq;

namespace Crawler.Contracts.Entities
{
	public class KeyValue
	{
		public KeyValue(ActionType type)
		{
			Type = type;
		}

		public string Name { get; set; }
		public ActionType Type { get; }
		public string Value { get; set; }
		public List<KeyValue> Values { get; set; }

		private KeyValue _parent;

		internal void SetParent(KeyValue parent)
		{
			_parent = parent;
		}
		internal KeyValue GetParent()
		{
			return _parent;
		}

		public void AddValue(KeyValue kv, bool addNew)
		{
			var data = Values?.FirstOrDefault(x => x.Name == kv.Name);
			if (!addNew && data != null)
			{
				data.Values = data.Values ?? new List<KeyValue>();
				if (!data.IsValue)
				{
					data.Values.Add(kv);
				}
				else
				{
					Values = Values ?? new List<KeyValue>();
					Values.Add(kv);
				}

			}
			else
			{
				Values = Values ?? new List<KeyValue>();
				Values.Add(kv);
			}
			kv.SetParent(this);
		}

		public bool UseParentNode { get; set; }

		public string AdditionalRegex { get; set; }

		public bool IsOuterHtml { get; set; }

		public bool IsJson { get; set; }

		public bool NeedConcat { get; set; }

		public bool FirstValue { get; set; }

		public bool IsBranch { get; set; }

		public bool IsStringValue { get; set; }

		public string ConcatValue { get; set; }
		public string PrevValue { get; set; }

		public string AfterValue { get; set; }

		public bool? IsNewRow { get; set; }

		public bool? IsArray { get; set; }

		public bool? IsObject { get; set; }

		public IEnumerable<KeyValue> this[string name]
		{
			get { return Values.Children().Where(x => x.Name == name).ToList(); }
		}

		public bool IsValue => !string.IsNullOrEmpty(Value) && (Values == null || !Values.Any());

		public override string ToString()
		{
			return $"{Name}:{Value}";
		}
	}
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