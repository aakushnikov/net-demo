using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Http.ExceptionHandling;

namespace Crawler.HtmlGateway.UserActions.Host.Handlers
{
	class MessageLoggerHandler : HttpRoutingDispatcher, IExceptionLogger
	{
		private readonly ILocalLogger _loggger;
		public MessageLoggerHandler(ILocalLogger logger, HttpConfiguration configuration) : base(configuration)
		{
			_loggger = logger;
		}

		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			_loggger.Debug(request.ToString());
			var response = await base.SendAsync(request, cancellationToken);
			_loggger.Debug(response.ToString());
			return response;
		}

		public Task LogAsync(ExceptionLoggerContext context, CancellationToken cancellationToken)
		{
			_loggger.Error(context.ExceptionContext.Exception.ToString());
			return Task.FromResult(0);
		}
	}
}