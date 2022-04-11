
using Crawler.Browser.Common.AccountManagment;
using Crawler.Browser.Common.Browser;
using Crawler.Contracts;

namespace Crawler.HtmlGateway.UserActions.Impl.Parsers
{
	public class AuthService : IAuthService
	{
		private readonly IActionHtmlLoader _htmlLoader;

		public AuthService(IActionHtmlLoader htmlLoader)
		{
			_htmlLoader = htmlLoader;
		}

		public AuthResult Auth(AuthInfo info)
		{
			//var action = new CustomUserActionLoader(new UserReactionPool(),  )
			throw new System.NotImplementedException();
		}
	}
}