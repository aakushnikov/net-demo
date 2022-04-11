using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using CefSharp;
using Crawler.Browser.Common.Exceptions;

namespace Crawler.Browser.Common.Cef
{
	internal class CefPageLoader
	{
		private int _retryCount = 10;
		private readonly IWebBrowser _browser;
		private readonly string _address;
		private readonly TaskCompletionSource<bool> _tcs;
		private readonly Timer _timeoutTimer;
		private string _redirectUrl;
		private readonly CefRequestHandler _requestHandler;

		public CefPageLoader(IWebBrowser browser, string address, long timeOutMs = 30000)
		{
			if (string.IsNullOrEmpty(address)) throw new ArgumentNullException(nameof(address));

			_browser = browser;
			_address = address;

			_tcs = new TaskCompletionSource<bool>();

			_timeoutTimer = new Timer(timeOutMs) { AutoReset = false };
			_timeoutTimer.Elapsed += TimeoutTimerOnElapsed;
			_timeoutTimer.Start();

			_browser.LoadError += BrowserOnLoadError;
			_browser.LoadingStateChanged += BrowserOnLoadingStateChanged;

			_requestHandler = _browser.RequestHandler as CefRequestHandler;
			if (_requestHandler != null)
				_requestHandler.Redirect += HandlerOnRedirect;

			
		}

		private void HandlerOnRedirect(object sender, BrowserRedirectEventArgs browserRedirectEventArgs)
		{
			_redirectUrl = browserRedirectEventArgs.NewUrl;
		}

		public async void Load(string address)
		{
			try
			{
				for (var i = 0; i < _retryCount; i++)
				{
					if (_browser.IsBrowserInitialized)
					{
						break;
					}

					await Task.Delay(200).ConfigureAwait(false);
				}

				_browser.Load(address);

			}
			catch (Exception e)
			{
				Describe();
				_tcs.TrySetException(e);
			}
		}


		public Task Task => _tcs.Task;

		private async void BrowserOnLoadError(object sender, LoadErrorEventArgs args)
		{
			if (args.ErrorCode == CefErrorCode.Aborted && _retryCount > 0)
			{
				if (_browser.IsBrowserInitialized)
				{
					_retryCount--;
					await Task.Delay(200).ConfigureAwait(false);
					if (_browser.IsBrowserInitialized)
					{
						if (!_browser.IsLoading)
						{
							_browser.Load(_address);
						}
						return;
					}
				}
			}

			Exception result;
			if (args.ErrorCode == CefErrorCode.ProxyConnectionFailed || args.ErrorCode == CefErrorCode.TunnelConnectionFailed)
			{
				var rc = args.Browser.GetHost().RequestContext;
				var pref = rc.GetPreference("server") ?? _requestHandler?.ProxyItems?.FirstOrDefault()?.ToString();

				TryDisposeBrowser(args.Browser);
				result = new ProxyException(pref?.ToString() ?? "?", $"Text: {args.ErrorText}, code: {args.ErrorCode}, url = {args.FailedUrl}");
			}
			else
			{
				result = new LoadPageException($"Text: {args.ErrorText}, code: {args.ErrorCode}");
			}

			Describe();
			_tcs.TrySetException(result);
		}



		private void TryDisposeBrowser(IBrowser browser)
		{
			try
			{
				browser.Dispose();
			}
			catch
			{
				// ignored
			}
		}

		private void BrowserOnLoadingStateChanged(object sender, LoadingStateChangedEventArgs args)
		{
			Debug.WriteLine($"BrowserOnLoadingStateChanged args.IsLoading={args.IsLoading} Address={_browser.Address}");
			//Wait for while page to finish loading not just the first frame
			if (!args.IsLoading)
			{
				if (_redirectUrl == null || _redirectUrl == _browser.Address) //Redirect detection
				{
					EndLoading();
				}
			}
		}

		private void TimeoutTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
		{
			EndLoading();
		}


		private void EndLoading()
		{
			Describe();

			_tcs.TrySetResult(true);
		}

		private void Describe()
		{
			_timeoutTimer.Elapsed -= TimeoutTimerOnElapsed;
			_timeoutTimer.Dispose();

			if (_requestHandler != null)
				_requestHandler.Redirect -= HandlerOnRedirect;

			_browser.LoadingStateChanged -= BrowserOnLoadingStateChanged;
			_browser.LoadError -= BrowserOnLoadError;
		}
	}
}