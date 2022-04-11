using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Crawler.Browser.Common.AccountManagment.Entities;

namespace Crawler.Browser.Common.Browser
{
	public interface IBrowserManager : IDisposable
	{
		string Address { get; }
		Task LoadPage(string address);
		Task LoadPage2(string address);

		Task<string> EvaluateScript(string script);
		Task<bool> EvaluateScriptBool(string script);
		Task SetInputFieldValue(string inputId, string value);

		Task<bool> WaitElement(string elementSelector, string action, TimeSpan maxWaitTime);

		Task<string> LoadScrollablePage(string url, string stopCondition, string nextStepScript, int maxTryCount);

		Task<string> LoadScrollablePage(string url, string targetItemSelector, int targetItemsCount, string nextStepScript, int maxTryCount);
		Task ExecuteLoop(string targetItemSelector, int targetItemsCount, string nextStepScript, int maxTryCount);
		Task ExecuteLoop(ICollection<string> targetItemSelectors, int targetItemsCount, string nextStepScript, int maxTryCount);
		string GetHtml(string address);
		string GetHtml(string address, Proxy proxy);
		Task<object> GetDocument();
		Task<string> GetHtml();
	}
}