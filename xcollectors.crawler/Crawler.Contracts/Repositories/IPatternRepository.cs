using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crawler.Contracts.Entities;

namespace Crawler.Contracts.Repositories
{
	public interface IPatternRepository
	{
		Pattern Get(string hash);
		Pattern Save(Pattern item);
		List<Pattern> GetAll();
	}
}
