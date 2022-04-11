using System;

namespace Crawler.Browser.Common.Cef
{
	public interface IMonitoringService: IDisposable
	{
		void StartMonitoring();
	}
}