using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace Crawler.Browser.Api.Host.Logging
{
	public static class WindsorIntegrationExtensions
	{
		public static IWindsorContainer InstallNancyInterceptor(this IWindsorContainer container)
		{
			container.Register(
				Component
					.For<INancyInterceptor>()
					.ImplementedBy<NancyInterceptor>()
					.LifestyleSingleton());

			return container;
		}
	}
}