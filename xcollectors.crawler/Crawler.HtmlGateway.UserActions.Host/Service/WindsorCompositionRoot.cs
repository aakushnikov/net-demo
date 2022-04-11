using System;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace Crawler.HtmlGateway.UserActions.Host.Service
{
	public class WindsorCompositionRoot : IHttpControllerActivator
	{
		private readonly IWindsorContainer _container;

		public WindsorCompositionRoot(IWindsorContainer container)
		{
			_container = container;

			_container.Register(
				Classes
					.FromThisAssembly()
					.BasedOn<IHttpController>()
					.LifestyleTransient()
			);
		}

		public IHttpController Create(
			HttpRequestMessage request,
			HttpControllerDescriptor controllerDescriptor,
			Type controllerType)
		{
			var controller = (IHttpController)_container.Resolve(controllerType);

			request.RegisterForDispose(
				new Release(() => _container.Release(controller)));

			return controller;
		}

		private class Release : IDisposable
		{
			private readonly Action _release;

			public Release(Action release)
			{
				_release = release;
			}

			public void Dispose()
			{
				_release();
			}
		}
	}
}
