using System;
using System.Text;
using Castle.Core.Logging;
using Crawler.Browser.Api.Host.Modules;
using Nancy;

namespace Crawler.Browser.Api.Host.Logging
{
	public class NancyInterceptor : INancyInterceptor
	{
		private readonly ILogger _logger;

		public NancyInterceptor(ILoggerFactory loggerFactory)
		{
			_logger = loggerFactory.Create(typeof(NancyInterceptor));
		}

		public Response RequestIntercept(NancyContext context)
		{
			try
			{
				var sb = new StringBuilder();
				var request = context.Request;
				sb.AppendFormat("Input Method: {0} {1} [IP:{2}, userName {3}]{4}", request.Method, request.Url, request.UserHostAddress, context.CurrentUser != null ? context.CurrentUser.UserName : "Unknown", Environment.NewLine);
				var method = request.Method.ToLower();
				if ((_logger.IsDebugEnabled || _logger.IsInfoEnabled) && (method == "post" || method == "put"))
				{
					sb.AppendFormat("Body: {0}{1}", ModuleHelper.TryGetBodyAsLocalString(request), Environment.NewLine);
				}
				_logger.Info(sb.ToString());
			}
			catch (Exception ex)
			{
				_logger.Error(ex.Message);
			}
			return null;
		}
	}
}