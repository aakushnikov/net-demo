using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Crawler.HtmlGateway.UserActions.Host.Handlers
{
	public class RequestLoggingHandler : DelegatingHandler
	{
		private readonly ILocalLogger _logger;

		public RequestLoggingHandler(ILocalLogger logger)
		{
			_logger = logger;
		}

		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			var message = $"{request.Method} {request.RequestUri}";
			if (!_logger.IsErrorEnabled || !_logger.IsWarnEnabled)
			{
				var contect = request.Content.ReadAsStringAsync().Result;
				message = $"{request.Method} {request.RequestUri}, Headers:{request.Headers}, Content:{contect}";
			}
			_logger.Debug(message);

			return await base.SendAsync(request, cancellationToken);
		}
	}
}
