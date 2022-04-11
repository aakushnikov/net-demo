using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler.Contracts.Repositories
{
	public sealed class AuthData
	{
		public int Id { get; set; }
		public string UserName { get; set; }
		public string Password { get; set; }
		public int HostId { get; set; }
		public string LoginUrl { get; set; }
		public string FirstPageUrl { get; set; }
		public string LoginField { get; set; }
		public string PasswordField { get; set; }
		public int RefreshTime { get; set; }
		public int IsEnabled { get; set; }
		public string SuccessIfContainValue { get; set; }
		
	}
	
	public interface IAuthRepository
	{
		List<AuthData> Get(int hostId);
	}
}
