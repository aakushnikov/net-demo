using System.Diagnostics;
using System.Threading.Tasks;
using CefSharp;
using CefSharp.OffScreen;

namespace Crawler.Browser.Common.Cef
{
	internal static class CefBrowserExtensions
	{
		private static void Invalidate(this IWebBrowser browser)
		{
			//Gets a wrapper around the underlying CefBrowser instance
			var cefBrowser = browser.GetBrowser();
			// Gets a warpper around the CefBrowserHost instance
			// You can perform a lot of low level browser operations using this interface
			var cefHost = cefBrowser.GetHost();

			//You can call Invalidate to redraw/refresh the image
			cefHost.Invalidate(PaintElementType.View);
		}

		/// <summary>
		/// Use it to debug current browser state
		/// </summary>
		/// <param name="browser"></param>
		/// <returns></returns>
		public static async Task OpenCurrentWindow(this ChromiumWebBrowser browser)
		{
			Invalidate(browser);
			var s = await browser.ScreenshotAsync().ConfigureAwait(false);
			using (s)
			{
				s.Save("CefSharp_screenshot.png");
				Process.Start("CefSharp_screenshot.png");
			}
		}
	}
}