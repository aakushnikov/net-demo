using System.Collections.Generic;

namespace Crawler.Contracts.Entities
{


	public sealed class PatternValidateRequest
	{
		public string Url { get; set; }

		public List<KeyValue> Patterns { get; set; }

		public List<KeyValue> CleanMatches { get; set; }

		public int? CodePage { get; set; }

		public bool? AppendImage { get; set; }

		public PatternValidateRequest[] Values { get; set; }

		public bool? UseBrowser { get; set; }

		public string PostData { get; set; }

		public string[] Headers { get; set; }
	}

	public sealed class UpdatePatternRequest
	{

		public string Url { get; set; }

		public KeyValue Value { get; set; }
	}
}
