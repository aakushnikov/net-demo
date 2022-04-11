using System;
using System.Threading.Tasks;

namespace Crawler.Browser.Common.Browser
{
	public interface IActionHtmlLoader
	{
		Task ContextAction(Func<IBrowserManager, Task> action, int accountId);
	}
}