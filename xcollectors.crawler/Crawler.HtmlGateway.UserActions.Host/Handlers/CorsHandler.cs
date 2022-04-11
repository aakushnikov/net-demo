using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Crawler.HtmlGateway.UserActions.Host.Handlers
{
	public class CorsHandler : DelegatingHandler
	{
		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			var response = await base.SendAsync(request, cancellationToken);
			response.Headers.Add("Access-Control-Allow-Origin", "*");
			return response;
		}
	}
}
