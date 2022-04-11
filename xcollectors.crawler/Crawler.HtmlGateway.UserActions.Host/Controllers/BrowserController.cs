using Crawler.Browser.Common.Browser;
using System.Web.Http;
using Crawler.Browser.Common.AccountManagment.Entities;

namespace Crawler.HtmlGateway.UserActions.Host.Controllers
{
	public class BrowserController : ApiController
	{
		private readonly IBrowserManager _browserManager;

		public BrowserController(IBrowserManager browserManager)
		{
			_browserManager = browserManager;
		}

		[HttpGet]
		public IHttpActionResult Get([FromUri] string url)
		{
			var html = _browserManager.GetHtml(url);

			return Ok(html);
		}

		[HttpGet]
		public IHttpActionResult Get([FromUri] string url, string address, int port, string user, string password)
		{
			var proxy = new Proxy { Address = address, Port = port, User = user, Password = password };
			var html = _browserManager.GetHtml(url, proxy);

			return Ok(html);
		}
	}
}
