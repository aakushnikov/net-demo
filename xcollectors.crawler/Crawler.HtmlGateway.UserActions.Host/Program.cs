using System;
using System.IO;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using CefSharp;
using CefSharp.Enums;
using CefSharp.OffScreen;
using Crawler.Browser.Common.Cef;
using Crawler.HtmlGateway.UserActions.Host.Service;
using Topshelf;
using Services.Core.Services.Interfaces;

namespace Crawler.HtmlGateway.UserActions.Host
{
	class Program
	{
		static void Main(string[] args)
		{

			var container = new WindsorContainer();
			container.UseAsApplicationWideContainer();
			container.Install(FromAssembly.This());


			container.Register(Component
				.For<IWindowsService>()
				.ImplementedBy<RestService>()
				.LifestyleSingleton());

			CefSharpSettings.SubprocessExitIfParentProcessClosed = true;
			var settings = new CefSettings()
			{
				//By default CefSharp will use an in-memory cache, you need to specify a Cache Folder to persist data
				CachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CefSharp\\Cache")
			};

			// кастомный протокол телеграма
			settings.RegisterScheme(new CefCustomScheme("tg", SchemeOptions.Standard) { SchemeHandlerFactory = new CefTgSchemeHandlerFactory() });

			Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);

			HostFactory.Run(x =>
			{

				x.SetServiceName("Crawler.HtmlGateway.UserActions.Host");

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
			Cef.Shutdown();

		}
	}
}
