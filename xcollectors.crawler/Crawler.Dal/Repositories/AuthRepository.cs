using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crawler.Contracts.Entities;
using Crawler.Contracts.Repositories;
using Dapper;

namespace Crawler.Dal.Repositories
{
	public sealed class AuthRepository : RepositoryBase, IAuthRepository
	{
		public AuthRepository()
				: base(RepositoryConsts.Crawler)
		{
		}
		private Dictionary<int, List<AuthData>> _dictionary = new Dictionary<int, List<AuthData>>();

		public List<AuthData> Get(int hostId)
		{
			if (_dictionary.ContainsKey(hostId))
				return _dictionary[hostId];
			using (var connection = GetConnection())
			{
				var p = new DynamicParameters();

				p.Add("@hostId", hostId);
				var authDatas = connection.Query<AuthData>($"SELECT [Id],[UserName],[Password],[HostId],[LoginUrl],[FirstPageUrl],[LoginField],[PasswordField],[RefreshTime],[IsEnabled],[SuccessIfContainValue] " +
				                                            $" FROM [dbo].[Auth] with(nolock) where [HostId]=@hostId and [IsEnabled]=1", param: p)
					.ToList();
				_dictionary[hostId] = authDatas;
				return authDatas;

			}
		}
	}
}
