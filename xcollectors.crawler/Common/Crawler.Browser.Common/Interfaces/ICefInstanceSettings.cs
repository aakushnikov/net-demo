namespace Crawler.Browser.Common.Cef
{
	public interface ICefInstanceSettings 
	{
		bool JavascriptOpenWindows { get; }

		bool Javascript { get; }

		bool ImageLoading { get; }

		bool JavascriptDomPaste { get; }

		bool JavascriptCloseWindows { get; }
		int BrowserWidth { get; }
		int BrowserHeight { get; }

		/// <summary>
		/// 1 for disable hardware acceleration
		/// </summary>
		int DisableGpu { get; }

		/// <summary>
		/// Seconds
		/// </summary>
		int MaxInstanceLiveTime { get; }

		bool WebGl { get; }
	}
}