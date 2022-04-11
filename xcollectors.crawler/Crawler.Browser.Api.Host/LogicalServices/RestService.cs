using System;
using System.Configuration;
using Crawler.Browser.Api.Host.NancyIntegration;
using Microsoft.Owin.Hosting;
using Owin;
using Service.Common.Interfaces;

namespace Crawler.Browser.Api.Host.LogicalServices
{
	/// <summary>
	/// Логический сервис для RestApi.
	/// </summary>
	public class RestService : IWindowsService
	{
		private IDisposable _owinApp;

		/// <summary>
		/// Starts execution of this service.
		/// </summary>
		public void Start()
		{
			var startOptions =
				new StartOptions(ConfigurationManager.AppSettings["listenUrl"])
				{
					ServerFactory = "Microsoft.Owin.Host.HttpListener"
				};

			_owinApp = WebApp.Start(
				startOptions,
				appBuilder =>
				{
					appBuilder
						.UseNancy(
							nancyOptions =>
							{
								nancyOptions.Bootstrapper = new RestServiceBootstrapper();
							});
				});
		}

		public void Stop()
		{
			_owinApp.Dispose();
			_owinApp = null;
		}

		public void Continued()
		{
			throw new NotImplementedException();
		}

		public void Paused()
		{
			throw new NotImplementedException();
		}
		
	}
}