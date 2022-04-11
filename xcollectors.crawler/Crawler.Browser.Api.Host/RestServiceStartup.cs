using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Crawler.Browser.Api.Host.LogicalServices;
using Crawler.Browser.Api.Host.WindsorIntegration;
using Service.Common.Interfaces;
using Topshelf;

namespace Crawler.Browser.Api.Host
{

	class RestServiceStartup
	{
		private static void Main(string[] args)
		{

			var container = new WindsorContainer();
			container.UseAsApplicationWideContainer();
			container.Install(FromAssembly.This());


			container.Register(Component
				.For<IWindowsService>()
				.ImplementedBy<RestService>()
				.LifestyleSingleton());



			HostFactory.Run(x =>
			{

				x.SetServiceName("Crawler.Browser.Api.Host");

				x.Service<IWindowsService>(
					s => s.ConstructUsing(
						() =>
						container.Resolve<IWindowsService>())
						.WhenStarted(tc => tc.Start())
						.WhenStopped(tc => tc.Stop())
						.WhenPaused(tc => tc.Paused())
						.WhenContinued(tc => tc.Continued()));

				x.UseNLog();
				x.RunAsLocalSystem();
			});
		}
		
	}
}
