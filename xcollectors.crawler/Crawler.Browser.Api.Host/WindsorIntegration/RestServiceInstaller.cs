using Castle.Facilities.Logging;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Crawler.Browser.Api.Host.Logging;
using Crawler.Browser.Api.Host.Modules;
using Crawler.Contracts;
using Crawler.Contracts.Repositories;
using Crawler.Contracts.Services;
using Crawler.Dal.Repositories;
using Service.Registry.Common;
using Crawler.Contracts.Impl;
using Crawler.Contracts.Impl.Entities;
using Crawler.Contracts.Impl.Proxies;
using Crawler.Contracts.Services;
using WebHtmlParserService = Crawler.Browser.Api.Host.Proxies.WebHtmlParserService;

namespace Crawler.Browser.Api.Host.WindsorIntegration
{
	public class RestServiceInstaller : IWindsorInstaller
	{
		public void Install(IWindsorContainer container, IConfigurationStore store)
		{
			container.AddFacility<LoggingFacility>(__c => __c.UseNLog());

			container.InstallNancyInterceptor();

			Repository(container);


			container.Register(
						Component
						.For<IAuthService>()
						.ImplementedBy<AuthService>()
						.LifestyleSingleton());
			container.Register(
							Component
							.For<ICache<string>>()
							.ImplementedBy<Cache<string>>()
							.LifestyleSingleton());
			container.Register(
							Component
							.For<ICache<AuthResult>>()
							.ImplementedBy<Cache<AuthResult>>()
							.LifestyleSingleton());

			container.Register(
				Component
				.For<IWebHtmlParserService>()
				.ImplementedBy<WebHtmlParserService>()
				.LifestyleSingleton());

			container.Register(
				Component
				.For<IContentCleaner>()
				.ImplementedBy<ContentCleaner>()
				.LifestyleSingleton());

			container.Register(
				Component
				.For<IHtmlParserModuleService>()
				.ImplementedBy<HtmlParserModuleService>()
				.LifestyleSingleton());
			container.Register(
				Component
				.For<IHtmlParserService>()
				.ImplementedBy<HtmlParserService>()
				.LifestyleSingleton());

			container.Register(
				Component
					.For<IProxyContainerService>()
					.ImplementedBy<ProxyContainerService>()
					.LifestyleSingleton());

			container.Register(
				Component
					.For<IServiceRegistryFactory>()
					.ImplementedBy<ServiceRegistryFactory>()
					.LifestyleSingleton());

			InstallRateLimit(container);

		}

		private static void Repository(IWindsorContainer container)
		{

			container.Register(Component
					.For<IDefaultHtmlParserSettings>()
					.ImplementedBy<DefaultHtmlParserSettings>()
					.LifestyleSingleton());

			container.Register(
				Component
					.For<IAuthRepository>()
					.ImplementedBy<AuthRepository>()
					.LifestyleSingleton());
			container.Register(
				Component
					.For<IHostRepository>()
					.ImplementedBy<HostRepository>()
					.LifestyleSingleton());

			container.Register(
				Component
					.For<IPatternRepository>()
					.ImplementedBy<PatternRepository>()
					.LifestyleSingleton());

			container.Register(
				Component
					.For<IPatternRelationRepository>()
					.ImplementedBy<PatternRelationRepository>()
					.LifestyleSingleton());
		}

		public void InstallRateLimit(IWindsorContainer container)
		{


			container.Register(

				Component.For<IModuleConfigurator>()
					.ImplementedBy<ModuleConfigurator>()
					.LifestyleSingleton());
		}


	}
}