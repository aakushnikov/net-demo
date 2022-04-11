using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler.HtmlGateway.UserActions.Host.Handlers
{
	public interface ILocalLogger
	{
		bool IsDebugEnabled { get; }
		bool IsErrorEnabled { get; }
		bool IsFatalEnabled { get; }
		bool IsInfoEnabled { get; }
		bool IsWarnEnabled { get; }

		void Debug(string message);
		void Error(string message);
		void Error(string text, Exception ex);
		void Info(string message);
		void Warn(string message);
	}
}
