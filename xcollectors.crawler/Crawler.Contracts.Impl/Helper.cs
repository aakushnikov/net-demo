using Crawler.Contracts.Entities;
using Newtonsoft.Json;

namespace Crawler.Contracts.Impl
{
	public static class Helper
	{

		public static MappingScheme MappingScheme(this PatternHostRelations relation)
		{
			return relation.Pattern.MappingScheme();
		}

		public static MappingScheme MappingScheme(this Pattern pattern)
		{
			MappingScheme map = JsonConvert.DeserializeObject<MappingScheme>(pattern.Value);
			return map;
		}


	}
}
