using System;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using Crawler.Browser.Api.Host.Logging;
using Crawler.Browser.Api.Host.WindsorIntegration;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Bootstrappers.Windsor;
using Nancy.Diagnostics;
using Nancy.Json;

namespace Crawler.Browser.Api.Host.NancyIntegration
{
	public class RestServiceBootstrapper : WindsorNancyBootstrapper
	{
		protected override DiagnosticsConfiguration DiagnosticsConfiguration
		{
			get { return new DiagnosticsConfiguration { Password = @"qwerty" }; }
		}

		protected override void ConfigureApplicationContainer(IWindsorContainer existingContainer)
		{
			base.ConfigureApplicationContainer(existingContainer);
			existingContainer.Kernel.ProxyFactory.AddInterceptorSelector(new NancyRequestScopeInterceptorSelector());
		}

		protected override IWindsorContainer GetApplicationContainer()
		{
			if (ApplicationContainer != null)
			{
				return ApplicationContainer;
			}

			var container = WindsorExtensions.Container;
			container.Kernel.Resolver.AddSubResolver(new CollectionResolver(container.Kernel, true));
			return container;
		}

		protected override void ApplicationStartup(IWindsorContainer container, IPipelines pipelines)
		{
			StaticConfiguration.DisableErrorTraces = false;

			var interceptor = container.Resolve<INancyInterceptor>();

			pipelines.BeforeRequest.AddItemToStartOfPipeline(ctx => interceptor.RequestIntercept(ctx));

			base.ApplicationStartup(container, pipelines);
			JsonSettings.MaxJsonLength = Int32.MaxValue;

			//UpperCamelCase (PascalCase) - при вызове Response.AsJson
			JsonSettings.RetainCasing = true; 

			//JsonConvert.DefaultSettings = () => new JsonSerializerSettings
			//{
			//    DateFormatString = "yyyy-MM-ddThh:mm:ssZ"
			//};
		}

		protected override void RequestStartup(IWindsorContainer container, IPipelines pipelines, NancyContext context)
		{
		
		}
	}
}