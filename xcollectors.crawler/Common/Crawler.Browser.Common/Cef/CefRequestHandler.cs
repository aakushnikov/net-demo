using System;
using System.Diagnostics;
using System.Linq;
using Castle.Core.Logging;
using CefSharp;
using CefSharp.Handler;
using Crawler.Browser.Common.AccountManagment.Entities;

namespace Crawler.Browser.Common.Cef
{

    internal class BrowserRedirectEventArgs : EventArgs
    {
        public string NewUrl { get; set; }
    }

    internal sealed class CefRequestHandler : RequestHandler
    {
        private readonly Proxy[] _proxyItems;
        private int _enterCredentialsTrying = 5;
        public ILogger Logger { get; set; } = NullLogger.Instance;

        public Proxy[] ProxyItems => _proxyItems;

        public event EventHandler<BrowserRedirectEventArgs> Redirect;

        public CefRequestHandler()
        {
        }

        public CefRequestHandler(Proxy proxy)
        {
	        _proxyItems = new[] { proxy };
        }

        public void OnResourceRedirect(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response, ref string newUrl)
        {
	        OnRedirect(new BrowserRedirectEventArgs { NewUrl = newUrl });
	        Debug.WriteLine("OnResourceRedirect " + newUrl);
        }

        private void OnRedirect(BrowserRedirectEventArgs e)
        {
	        Redirect?.Invoke(this, e);
        }

	    #region Override

	    protected override bool GetAuthCredentials(IWebBrowser chromiumWebBrowser, IBrowser browser, string originUrl, bool isProxy, string host,
		    int port, string realm, string scheme, IAuthCallback callback)
	    {
		    if (!isProxy)
			    return false;

		    var proxy = ProxyItems.FirstOrDefault(p => p.Address == host);
		    if (proxy == null)
			    return false;

		    callback.Continue(proxy.User, proxy.Password);

		    return true;
	    }

        #endregion override
    }
}
