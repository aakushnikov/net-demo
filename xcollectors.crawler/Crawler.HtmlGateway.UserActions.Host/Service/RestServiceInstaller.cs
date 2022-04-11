using Castle.Facilities.Logging;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Crawler.HtmlGateway.UserActions.Host.Handlers;

namespace Crawler.HtmlGateway.UserActions.Host.Service
{
	public class RestServiceInstaller : IWindsorInstaller
	{
		public void Install(IWindsorContainer container, IConfigurationStore store)
		{

			container.AddFacility<LoggingFacility>(__c => __c.UseNLog());

			container.Register(
				
				Component
					.For<ILocalLogger>()
					.ImplementedBy<LocalLogger>()
					.LifestyleSingleton()
				);
			
		}
	}
}
