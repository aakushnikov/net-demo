using Castle.Windsor;

namespace Crawler.Browser.Api.Host.WindsorIntegration
{
	internal static class WindsorExtensions
	{
		internal static IWindsorContainer Container { get; private set; }

		public static void UseAsApplicationWideContainer(this IWindsorContainer container)
		{
			Container = container;


		}
	}
}