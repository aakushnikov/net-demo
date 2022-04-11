using System;
using Castle.Core.Logging;
using Crawler.HtmlGateway.UserActions.Host.Handlers;

namespace Crawler.HtmlGateway.UserActions.Host.Service
{
	public class LocalLogger : ILocalLogger
	{
		private readonly ILogger _logger;

		public LocalLogger(ILogger logger)
		{
			_logger = logger;
		}

		public void Debug(string message)
		{
			_logger.Debug(message);
		}

		public void Error(string message)
		{
			_logger.Error(message);
		}

		public void Error(string message, Exception ex)
		{
			_logger.Error(message, ex);

		}

		public void Info(string message)
		{
			_logger.Info(message);
		}

		public bool IsErrorEnabled => _logger.IsErrorEnabled;
		public bool IsDebugEnabled => _logger.IsDebugEnabled;
		public bool IsFatalEnabled => _logger.IsFatalEnabled;
		public bool IsInfoEnabled => _logger.IsInfoEnabled;
		public bool IsWarnEnabled => _logger.IsWarnEnabled;

		public void Warn(string message)
		{
			_logger.Warn(message);
		}
	}
}
