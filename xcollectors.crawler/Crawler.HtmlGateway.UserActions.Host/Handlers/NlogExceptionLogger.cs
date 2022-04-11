using System.Web.Http.ExceptionHandling;

namespace Crawler.HtmlGateway.UserActions.Host.Handlers
{
	public class NlogExceptionLogger : ExceptionLogger
	{
		private readonly ILocalLogger _logger;

		public NlogExceptionLogger(ILocalLogger logger)
		{
			_logger = logger;
		}

		public override void Log(ExceptionLoggerContext context)
		{
			_logger?.Error(context.Exception.Message, context.Exception);
		}
	}
}
