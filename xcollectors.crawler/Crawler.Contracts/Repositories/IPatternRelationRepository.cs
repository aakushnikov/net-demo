using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crawler.Contracts.Entities;

namespace Crawler.Contracts.Repositories
{
	public interface IPatternRelationRepository
	{
		List<PatternHostRelations> Get(int id);

		List<PatternHostRelations> Get(string url);

		PatternHostRelations Save(PatternHostRelations item);

		List<PatternHostRelations> Get(int hostId, int patternId);

		void Delete(int hostId);
	}
}
