using CefSharp;

namespace Crawler.Browser.Common.Cef
{
	public class CefTgSchemeHandlerFactory : ISchemeHandlerFactory
	{
		public IResourceHandler Create(IBrowser browser, IFrame frame, string schemeName, IRequest request)
		{
			return new CefTgSchemeHandler();
		}
	}
}