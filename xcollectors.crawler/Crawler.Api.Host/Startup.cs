using Autofac;
using Crawler.Api.Host.Proxies;
using Crawler.Api.Host.Services;
using Crawler.Contracts;
using Crawler.Contracts.Entities;
using Crawler.Contracts.Impl;
using Crawler.Contracts.Impl.Entities;
using Crawler.Contracts.Repositories;
using Crawler.Contracts.Services;
using Crawler.Dal.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Service.Registry.Common;

namespace Crawler.Api.Host
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers();
		}

		public void ConfigureContainer(ContainerBuilder builder)
		{
			builder
				   .RegisterType<AuthService>()
				   .As<IAuthService>()
				   .SingleInstance();
			
			builder
				.RegisterType<Cache<string>>()
				.As<ICache<string>>()
				.SingleInstance();

			builder
				.RegisterType<Cache<AuthResult>>()
				.As<ICache<AuthResult>>()
				.SingleInstance();

			builder
				.RegisterType<WebHtmlParserService>()
				.As<IWebHtmlParserService>()
				.SingleInstance();
			builder
				.RegisterType<Cache<byte[]>>()
				.As<ICache<byte[]>>()
				.SingleInstance();
			builder
				.RegisterType<ContentCleaner>()
				.As<IContentCleaner>()
				.SingleInstance();
			builder
				.RegisterType<HtmlParserModuleService>()
				.As<IHtmlParserModuleService>()
				.SingleInstance();

			builder
				.RegisterType<LoggerFactory>()
				.As<ILoggerFactory>()
				.SingleInstance();
			builder
				.RegisterType<HtmlParserService>()
				.As<IHtmlParserService>()
				.SingleInstance();
			builder
				.RegisterType<ProxyContainerService>()
				.As<IProxyContainerService>()
				.SingleInstance();
			builder
				.RegisterType<ServiceRegistryFactory>()
				.As<IServiceRegistryFactory>()
				.SingleInstance();
			builder
				.RegisterType<Cache<PatternHostRelations>>()
				.As<ICache<PatternHostRelations>>()
				.SingleInstance();
			Repository(builder);
		}


		private static void Repository(ContainerBuilder builder)
		{
			builder
				   .RegisterType<DefaultHtmlParserSettings>()
				   .As<IDefaultHtmlParserSettings>()
				   .SingleInstance();

			builder
				   .RegisterType<AuthRepository>()
				   .As<IAuthRepository>()
				   .SingleInstance();

			builder
				   .RegisterType<HostRepository>()
				   .As<IHostRepository>()
				   .SingleInstance();
			builder
				   .RegisterType<PatternRepository>()
				   .As<IPatternRepository>()
				   .SingleInstance();
			builder
				   .RegisterType<PatternRelationRepository>()
				   .As<IPatternRelationRepository>()
				   .SingleInstance();


		}
		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseMiddleware(typeof(RequestLoggingMiddleware));
			app.UseRouting();
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
