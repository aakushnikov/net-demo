using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Castle.Core.Logging;
using CefSharp;
using CefSharp.Internals;
using CefSharp.OffScreen;
using Crawler.Browser.Common.AccountManagment.Entities;
using Crawler.Browser.Common.Browser;
using Crawler.Browser.Common.Exceptions;

namespace Crawler.Browser.Common.Cef
{
	public static class Commands
	{
		public const string True = "true;";
		public const string IsDocumentLoaded = "Boolean(document.body);";
		public const string ScrollEnd = "if (document.body) window.scrollTo(0, document.body.scrollHeight || 0);";
		public const string IsDomContentLoaded = "document.addEventListener('DOMContentLoaded', function(){ return Boolean(document.body); });";
	}

	public class CefBrowserManager : IBrowserManager
	{
		private readonly ICefInstanceSettings _cefInstanceSettings;
		private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);
		private readonly IWebBrowser _browser;
		public ILogger Logger { get; set; } = NullLogger.Instance;

		public CefBrowserManager(ICefInstanceSettings cefInstanceSettings)
		{
			_cefInstanceSettings = cefInstanceSettings;
			_browser = new ChromiumWebBrowser("", new BrowserSettings()
			{
				WebGl = _cefInstanceSettings.WebGl ? CefState.Enabled : CefState.Default,
				ImageLoading = _cefInstanceSettings.ImageLoading ? CefState.Enabled : CefState.Default,
				Javascript = _cefInstanceSettings.Javascript ? CefState.Enabled : CefState.Default,
				JavascriptDomPaste = _cefInstanceSettings.JavascriptDomPaste ? CefState.Enabled : CefState.Default,
				JavascriptCloseWindows =
					_cefInstanceSettings.JavascriptCloseWindows ? CefState.Enabled : CefState.Default,
				
			});
		}

		public string GetHtml(string address)
		{
			return LoadScrollablePage(address, Commands.IsDomContentLoaded, null).Result;
		}

		public string GetHtml(string address, Proxy proxy)
		{
			// официальный метод смены проксей в рантайме
			// https://stackoverflow.com/questions/36095566/cefsharp-3-set-proxy-at-runtime
			CefSharp.Cef.UIThreadTaskFactory.StartNew(delegate
			{
				_browser.RequestHandler = new CefRequestHandler(proxy);

				var error = String.Empty;
				var requestContext = _browser.GetBrowser().GetHost().RequestContext;
				var proxyDict = new Dictionary<string, object>
				{
					{ "mode", "fixed_servers" },
					{ "server", $"{proxy.Address}:{proxy.Port}" }
				};

				requestContext.SetPreference("proxy", proxyDict, out error);

				if(CefSharp.Cef.IsInitialized)
					return;

				var globalSettings = new CefSettings();
				//globalSettings.WindowlessRenderingEnabled = true;

				//Chromium Command Line args
				//http://peter.sh/experiments/chromium-command-line-switches/

				globalSettings.CefCommandLineArgs.Add("disable-extensions", "1"); //Extension support can be disabled

				if (_cefInstanceSettings.DisableGpu > 0)
				{
					//NOTE: For OSR best performance you should run with GPU disabled: (you'll loose WebGL support but gain increased FPS and reduced CPU usage).
					//The following function will set all three params (disable-gpu, disable-gpu-compositing, enable-begin-frame-scheduling)
					globalSettings.SetOffScreenRenderingBestPerformanceArgs();

					globalSettings.CefCommandLineArgs.Add("disable-gpu-vsync", "1"); //Disable Vsync
				}

				if (!_cefInstanceSettings.WebGl)
				{
					globalSettings.CefCommandLineArgs.Add("disable-webgl", "1");
				}

				//globalSettings.CefCommandLineArgs.Add("enable-media-stream", "1"); //Enable WebRTC

				//Disable discovering third-party plugins. Effectively loading only ones shipped with the browser plus third-party ones as specified by --extra-plugin-dir and --load-plugin switches
				globalSettings.CefCommandLineArgs.Add("disable-plugins-discovery", "1");

				//Disables the DirectWrite font rendering system on windows.
				//Possibly useful when experiencing blury fonts.
				globalSettings.CefCommandLineArgs.Add("disable-direct-write", "1");
				CefSharp.Cef.Initialize(globalSettings, performDependencyCheck: true, browserProcessHandler: null);


			}).Wait();

			return LoadScrollablePage(address, Commands.IsDomContentLoaded, null).Result;
		}

		public async Task LoadPage(string address)
		{
			Logger.Debug($"Start loading: address: {address}");

			if (string.IsNullOrEmpty(address))
				return;

			var loader = new CefPageLoader(_browser, address);
			loader.Load(address);
			await loader.Task.ConfigureAwait(false);
			Logger.Info($"Loading completed: address: {address}");
		}

		public async Task LoadPage2(string address)
		{
			Logger.Debug($"Start loading: address: {address}");

			if (string.IsNullOrEmpty(address))
				return;

			await LoadPageAsync(_browser, address);
			Logger.Info($"Loading completed: address: {address}");
		}

		public static Task LoadPageAsync(IWebBrowser browser, string address = null)
		{
			var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.LongRunning);
			EventHandler<LoadingStateChangedEventArgs> handler = null;
			handler = (sender, args) =>
			{
				//Wait for while page to finish loading not just the first frame
				if (!args.IsLoading)
				{
					browser.LoadingStateChanged -= handler;
					//Important that the continuation runs async using TaskCreationOptions.RunContinuationsAsynchronously
					tcs.TrySetResult(true);
				}
			};

			browser.LoadingStateChanged += handler;
			if (!string.IsNullOrEmpty(address))
			{
				browser.Load(address);
			}

			return tcs.Task;
		}

		public Task<string> EvaluateScript(string script)
		{
			return EvaluateScript(script, TimeSpan.FromSeconds(30));
		}

		public async Task<bool> EvaluateScriptBool(string script)
		{
			var r = await EvaluateScript(script);

			return r == "True";
		}

		private async Task<string> EvaluateScript(string script, TimeSpan timeOut)
		{
			JavascriptResponse res;
			if (_browser.CanExecuteJavascriptInMainFrame)
			{
				res = await _browser.EvaluateScriptAsync(script, timeOut).ConfigureAwait(false);
			}
			else
			{
				res = await _browser.GetMainFrame().EvaluateScriptAsync(script, timeout: timeOut).ConfigureAwait(false);
			}

			await ThrowIfError(_browser, res).ConfigureAwait(false);

			return (res.Result ?? string.Empty).ToString();
		}

		public async Task SetInputFieldValue(string inputId, string value)
		{
			await EvaluateScript($"document.getElementById('{inputId}').click();").ConfigureAwait(false);
			await EvaluateScript($"document.getElementById('{inputId}').value='{value}';").ConfigureAwait(false);
		}

		public string Address => _browser.Address;

		public async Task<string> LoadScrollablePage(string url,
			string stopCondition = null,
			string nextStepScript = Commands.ScrollEnd,
			int maxTryCount = 20
		)
		{
			if (!string.IsNullOrEmpty(url))
			{
				await LoadPage(url).ConfigureAwait(false);
			}

			if (!string.IsNullOrEmpty(stopCondition))
			{
				await ExecuteLoop(stopCondition, nextStepScript, maxTryCount).ConfigureAwait(false);
			}

			await Task.Delay(1000).ConfigureAwait(false);

			return await GetHtml().ConfigureAwait(false);
		}

		public async Task<string> LoadScrollablePage(string url, string targetItemSelector, int targetItemsCount,
			string nextStepScript = Commands.ScrollEnd, int maxTryCount = 20)
		{
			await LoadPage(url).ConfigureAwait(false);
			await ExecuteLoop(targetItemSelector, targetItemsCount, nextStepScript, maxTryCount).ConfigureAwait(false);

			return await GetHtml().ConfigureAwait(false);
		}

		public Task ExecuteLoop(string targetItemSelector, int targetItemsCount, string nextStepScript, int maxTryCount)
		{
			return ExecuteLoop(new[] { targetItemSelector }, targetItemsCount, nextStepScript, maxTryCount);
		}

		public async Task<bool> WaitElement(string elementSelector, string action, TimeSpan maxWaitTime)
		{
			const int loopMs = 500;
			var maxLoopCount = Convert.ToInt32(maxWaitTime.TotalMilliseconds) / loopMs;

			if (maxLoopCount == 0)
				return await ElementFound(elementSelector);

			for (var i = 0; i < maxLoopCount; i++)
			{
				if (await ElementFound(elementSelector))
					return true;

				if (!string.IsNullOrEmpty(action))
					await _browser.EvaluateScriptAsync(action, DefaultTimeout);

				await Task.Delay(loopMs);
			}

			return false;
		}

		private async Task<bool> ElementFound(string elementSelector)
		{
			var res = await _browser
				.EvaluateScriptAsync($"Boolean(document.querySelector(\"{elementSelector}\"))", DefaultTimeout)
				.ConfigureAwait(false);

			return res.Result?.ToString() == "True";
		}

		public async Task ExecuteLoop(ICollection<string> targetItemSelectors, int targetItemsCount,
			string nextStepScript, int maxTryCount)
		{
			if (targetItemSelectors == null || targetItemSelectors.Count == 0)
				throw new ArgumentNullException(nameof(targetItemSelectors));

			var selectors = targetItemSelectors.Select(targetItemSelector =>
			{
				var quote = targetItemSelector.Contains("'") ? "\"" : "'";

				return $"document.querySelectorAll({quote + targetItemSelector + quote}).length";
			});

			var getCountScript = targetItemSelectors.Count > 1
				? $"Math.max({string.Join(",", selectors)})"
				: selectors.First();

			var prevCount = 0;
			var maxFailCount = 5;
			for (var i = 0; i < maxTryCount && maxFailCount > 0; i++)
			{
				var jsRes = await EvaluateScript(getCountScript).ConfigureAwait(false);
				var currentCount = int.Parse(jsRes);
				if (currentCount >= targetItemsCount)
				{
					break;
				}

				if (currentCount > prevCount)
				{
					maxFailCount = 5;
					prevCount = currentCount;
				}
				else
				{
					//if we execute the steps, but the number does not grow
					maxFailCount--;
				}

				if (!string.IsNullOrEmpty(nextStepScript))
				{
					await EvaluateScript(nextStepScript).ConfigureAwait(false);
				}

				await Task.Delay(1000).ConfigureAwait(false);
			}
		}

		private async Task ExecuteLoop(string stopCondition, string nextStepScript, int maxTryCount)
		{
			if (stopCondition == null) throw new ArgumentNullException(nameof(stopCondition));
			for (var i = 0; i < maxTryCount; i++)
			{
				var stop = await EvaluateScript(stopCondition).ConfigureAwait(false);
				if (string.Compare(stop, "true", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					break;
				}

				if (!string.IsNullOrEmpty(nextStepScript))
				{
					await EvaluateScript(nextStepScript).ConfigureAwait(false);
				}

				await Task.Delay(1000).ConfigureAwait(false);
			}
		}

		public Task<string> GetHtml()
		{
			return _browser.GetSourceAsync();
		}

		public async Task<object> GetDocument()
		{
			var text = await _browser.GetSourceAsync().WithTimeout(DefaultTimeout).ConfigureAwait(false);

			return text;
		}

		private async Task ThrowIfError(IWebBrowser browser, JavascriptResponse res)
		{
			if (!res.Success)
			{
				await SaveCurrentWindow(browser).ConfigureAwait(false);

				throw new PageFormatException(res.Message);
			}
		}

		private Task SaveCurrentWindow(IWebBrowser browser)
		{
			var folderName = DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
			var path = Path.Combine("Logs", "Error_" + folderName);
			var dir = Directory.CreateDirectory(path);

			return TrySaveCurrentText(dir.FullName, browser);
		}

		private async Task TrySaveCurrentText(string folderName, IWebBrowser browser)
		{
			try
			{
				var s = await GetHtml().ConfigureAwait(false);
				var fileName = FileNameFromUrl.ConvertToWindowsFileName(browser.Address);
				File.WriteAllText(Path.Combine(folderName, fileName), s);
			}
			catch (Exception e)
			{
				Logger.Error(e.ToString);
			}
		}

		public void Dispose()
		{
			_browser?.Dispose();
		}
	}
}