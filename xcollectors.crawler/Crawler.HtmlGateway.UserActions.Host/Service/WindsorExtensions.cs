using Castle.MicroKernel.Registration;
using Castle.Windsor;
using CefSharp;
using Crawler.Browser.Common.Browser;
using Crawler.Browser.Common.Cef;

namespace Crawler.HtmlGateway.UserActions.Host.Service
{
	internal static class WindsorExtensions
	{
		internal static IWindsorContainer Container { get; private set; }

		public static void UseAsApplicationWideContainer(this IWindsorContainer container)
		{
			
			Container = container;
			container
				.Register(
					Component.For<ICefInstanceSettings>()
						.ImplementedBy<CefInstanceSettings>()
						.LifestyleSingleton(),


					Component
						.For<IBrowserManager>()
						.ImplementedBy<CefBrowserManager>()
						.LifeStyle.Transient
				);
		}
	}
}
