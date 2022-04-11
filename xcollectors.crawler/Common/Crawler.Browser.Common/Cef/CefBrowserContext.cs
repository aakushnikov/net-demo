using System;
using CefSharp;
using Crawler.Browser.Common.AccountManagment;

namespace Crawler.Browser.Common.Cef
{
	public class CefBrowserContext : IDisposable
	{
		public IWebBrowser WebBrowser { get; }
		public DateTime CreationTime { get; }


		private readonly IRequestContext _requestContext;

		public CefBrowserContext(IRequestContext requestContext, IWebBrowser webBrowser)
		{
			_requestContext = requestContext;
			WebBrowser = webBrowser;
			CreationTime = DateTime.Now;
		}

		~CefBrowserContext()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public bool IsDisposed { get; private set; }

		private void Dispose(bool disposing)
		{
			if (!IsDisposed)
			{
				try
				{
					WebBrowser.Dispose();
					_requestContext.Dispose();
				}
				catch 
				{
					if (disposing)
					{
						throw;
					}
				}

				IsDisposed = true;
			}
		}
	}
}