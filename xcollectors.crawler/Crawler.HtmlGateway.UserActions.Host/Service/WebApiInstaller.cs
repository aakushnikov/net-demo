using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.SelfHost;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Crawler.HtmlGateway.UserActions.Host.Handlers;


namespace Crawler.HtmlGateway.UserActions.Host.Service
{
	public class WebApiInstaller : IWindsorInstaller
	{
		public void Install(IWindsorContainer container, IConfigurationStore store)
		{
			
			container.Register(new IRegistration[]
			{
				Component
					.For<RequestLoggingHandler>()
					.ImplementedBy<RequestLoggingHandler>()
					.LifestyleSingleton(),
			});

			var url = System.Configuration.ConfigurationManager.AppSettings["listenUrl"] ?? "http://localhost:9001";
			url = url.Replace("+", "localhost");
			var listen = url;
			var selfHostConfiguraiton = new HttpSelfHostConfiguration(listen);

			selfHostConfiguraiton.Routes
				.MapHttpRoute(
					"default", "api/{controller}/{id}", new { id = RouteParameter.Optional });
			selfHostConfiguraiton.Routes.MapHttpRoute("withAction", "api/{controller}/{action}");
			selfHostConfiguraiton.MaxBufferSize = 30 * 1024 * 1024;
			selfHostConfiguraiton.MaxReceivedMessageSize = selfHostConfiguraiton.MaxBufferSize;
			selfHostConfiguraiton.Services.Replace(typeof(IHttpControllerActivator), new WindsorCompositionRoot(container));
			selfHostConfiguraiton.MessageHandlers.Add(container.Resolve<RequestLoggingHandler>());
			selfHostConfiguraiton.MessageHandlers.Add(new CorsHandler());
			selfHostConfiguraiton.Services.Add(typeof(IExceptionLogger), new NlogExceptionLogger(container.Resolve<ILocalLogger>()));

			var server = new HttpSelfHostServer(selfHostConfiguraiton);
			server.OpenAsync().Wait();

			var logger = container.Resolve<ILocalLogger>();
			logger.Info($"Listen at <{listen}>.");
		}
	}
}
