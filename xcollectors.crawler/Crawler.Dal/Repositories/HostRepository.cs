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
	
	public class HostRepository : RepositoryBase, IHostRepository
	{
		public HostRepository()
				: base(RepositoryConsts.Crawler)
		{
		}

		public Host Get(string url)
		{
			using (var connection = GetConnection())
			{
				var p = new DynamicParameters();

				p.Add("@url", url);
				return connection.Query<Host>($"SELECT [Id],[Name],[Url]  FROM [dbo].[Hosts] with(nolock) where [Url]=@url", param:p).FirstOrDefault();
				
			}
		}

		public Host Save(Host item)
		{
			using (var connection = GetConnection())
			{
				var p = new DynamicParameters();

				p.Add("@name", item.Name);
				p.Add("@url", item.Url);
				if (item.Id == 0)
				{
					item.Id = connection.Query<int>(
						$"INSERT INTO [dbo].[Hosts]([Name],[Url])VALUES(@name,@url) SELECT @@IDENTITY", param: p,
						commandType: CommandType.Text,
						commandTimeout: 180).FirstOrDefault();

				}
				else
				{
					p.Add("@id", item.Id);
					connection.Execute(
						$"UPDATE [dbo].[HtmlParserPatterns] SET [Url] = @url, Name=@name where Id=@id", param: p,
						commandType: CommandType.Text,
						commandTimeout: 180);
				}
			}
			return item;
		}
	}
}
