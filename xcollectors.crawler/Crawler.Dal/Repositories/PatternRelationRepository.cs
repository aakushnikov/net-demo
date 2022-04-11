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
	internal sealed class PatternHostRelationsData
	{
		public int Id { get; set; }

		public int PatternId { get; set; }

		public int HostId { get; set; }

		public string Url { get; set; }

		public int? EncodingCode { get; set; }

		public string HostUrl { get; set; }

		public string HostName { get; set; }


		public string Value { get; set; }

		public int? UseBrowser { get; set; }

	}
	public sealed class PatternRelationRepository : RepositoryBase, IPatternRelationRepository
	{
		public PatternRelationRepository()
				: base(RepositoryConsts.Crawler)
		{
		}

		public List<PatternHostRelations> Get(int id)
		{
			using (var connection = GetConnection())
			{
				return connection.Query<PatternHostRelationsData>($"SELECT rh.[Id],rh.[PatternId], rh.[HostId], rh.[Url],h.[Name] as HostName,h.[Url] as HostUrl,rh.[EncodingCode]," +
															  $" p.[Value],rh.[UseBrowser]" +
															  $"  FROM [dbo].[PatternHostRelations] rh with(nolock) " +
															  $" JOIN [dbo].[Hosts] h  with(nolock) on h.Id=rh.HostId" +
															  $" JOIN [dbo].[Patterns] p  with(nolock) on p.Id=rh.PatternId")
															  .Select(MapPatternHostRelations).ToList();
			}
		}

		public List<PatternHostRelations> Get(string url)
		{
			using (var connection = GetConnection())
			{
				var p = new DynamicParameters();
				p.Add("@url", url);
				var select =
					$"SELECT rh.[Id],rh.[PatternId], rh.[HostId], rh.[Url],h.[Name] as HostName,h.[Url] as HostUrl,rh.[EncodingCode]," +
					$" p.[Value],rh.[UseBrowser]" +
					$"  FROM [dbo].[PatternHostRelations] rh with(nolock) " +
					$" JOIN [dbo].[Hosts] h  with(nolock) on h.Id=rh.HostId" +
					$" JOIN [dbo].[Patterns] p  with(nolock) on p.Id=rh.PatternId" +
					$" where h.Url=@url ";
				return connection.Query<PatternHostRelationsData>(select, param: p).Select(MapPatternHostRelations).ToList();
			}
		}

		public List<PatternHostRelations> Get(int hostId, int patternId)
		{
			using (var connection = GetConnection())
			{
				var p = new DynamicParameters();
				p.Add("@hostId", hostId);
				p.Add("@patternId", patternId);
				return connection.Query<PatternHostRelationsData>($"SELECT rh.[Id],rh.[PatternId], rh.[HostId], rh.[Url],h.[Name] as HostName,h.[Url] as HostUrl,rh.[EncodingCode]," +
															  $" p.[Value],rh.[UseBrowser]" +
															  $"  FROM [dbo].[PatternHostRelations] rh with(nolock) " +
															  $" JOIN [dbo].[Hosts] h  with(nolock) on h.Id=rh.HostId" +
															  $" JOIN [dbo].[Patterns] p  with(nolock) on p.Id=rh.PatternId" +
															  $" where HostId=@hostId  and PatternId=@patternId ", param: p).Select(MapPatternHostRelations).ToList();
			}
		}

		public void Delete(int hostId)
		{
			using (var connection = GetConnection())
			{
				var p = new DynamicParameters();
				p.Add("@hostId", hostId);
				connection.Execute($"DELETE FROM [dbo].[PatternHostRelations] where HostId=@hostId ", param: p, commandType: CommandType.Text);
			}
		}

		private static PatternHostRelations MapPatternHostRelations(PatternHostRelationsData x)
		{
			return new PatternHostRelations()
			{
				Id = x.Id,
				Url = x.Url,
				EncodingCode = x.EncodingCode,
				Host = new Host()
				{
					Id = x.HostId,
					Url = x.HostUrl,
					Name = x.HostName
				},
				Pattern = new Pattern()
				{
					Id = x.PatternId,
					Value = x.Value
				},
				UseBrowser = x.UseBrowser.HasValue && x.UseBrowser.Value > 0 ? true : false

			};
		}

		public PatternHostRelations Save(PatternHostRelations item)
		{
			using (var connection = GetConnection())
			{
				var p = new DynamicParameters();


				p.Add("@patternId", item.Pattern.Id);
				p.Add("@hostId", item.Host.Id);
				p.Add("@encodingCode", item.EncodingCode);
				p.Add("@useBrowser", item.UseBrowser.HasValue && item.UseBrowser.Value ? 1 : 0);



				p.Add("@url", item.Url ?? string.Empty);
				if (item.Id == 0)
				{
					item.Id = connection.Query<int>(
						$"INSERT INTO [dbo].[PatternHostRelations]([PatternId], [HostId], [Url],[EncodingCode],[UseBrowser])VALUES(@patternId,@hostId,@url,@encodingCode,@useBrowser) SELECT @@IDENTITY", param: p,
						commandType: CommandType.Text,
						commandTimeout: 180).FirstOrDefault();

				}
				else
				{
					p.Add("@id", item.Id);
					connection.Execute(
						$"UPDATE [dbo].[PatternHostRelations] SET [PatternId] = @patternId,[HostId] = @hostId,[Url] = @url,[EncodingCode]=@encodingCode,[UseBrowser]=useBrowser where Id=@id", param: p,
						commandType: CommandType.Text,
						commandTimeout: 180);
				}
			}
			return item;
		}
	}
}
