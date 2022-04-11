using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crawler.Contracts.Entities;
using Crawler.Contracts.Repositories;
using Dapper;

namespace Crawler.Dal.Repositories
{





	public sealed class PatternRepository : RepositoryBase, IPatternRepository
	{
		public PatternRepository()
				: base(RepositoryConsts.Crawler)
		{
		}

		public Pattern Get(string hash)
		{
			using (var connection = GetConnection())
			{
				var p = new DynamicParameters();

				p.Add("@hash", hash);
				return connection.Query<Pattern>($"SELECT [Id],[Value],[Hash]  FROM [dbo].[Patterns] with(nolock) where [Hash]=@hash", param: p).FirstOrDefault();
			}
		}

		public List<Pattern> GetAll()
		{
			using (var connection = GetConnection())
			{
				var p = new DynamicParameters();

				return connection.Query<Pattern>($"SELECT [Id],[Value],[Hash]  FROM [dbo].[Patterns] with(nolock) ").ToList();
			}
		}

		public Pattern Save(Pattern item)
		{
			using (var connection = GetConnection())
			{
				var p = new DynamicParameters();

				p.Add("@pattern", item.Value);
				p.Add("@hash", item.Hash);
				if (item.Id == 0)
				{
					item.Id = connection.Query<int>(
						$"INSERT INTO [dbo].[Patterns]([Value],[Hash])VALUES(@pattern,@hash) SELECT @@IDENTITY", param: p,
						commandType: CommandType.Text,
						commandTimeout: 180).FirstOrDefault();

				}
				else
				{
					p.Add("@id", item.Id);
					connection.Execute(
						$"UPDATE [dbo].[Patterns] SET [Value] = @pattern,[Hash]=@hash where Id=@id", param: p,
						commandType: CommandType.Text,
						commandTimeout: 180);
				}
			}
			return item;
		}
	}
}
