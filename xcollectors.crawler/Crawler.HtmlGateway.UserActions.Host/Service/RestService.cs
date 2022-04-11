using System;
using Services.Core.Services.Interfaces;

namespace Crawler.HtmlGateway.UserActions.Host.Service
{
	/// <summary>
	/// Логический сервис для RestApi.
	/// </summary>
	public class RestService : IWindowsService
	{

		public RestService()
		{
		}

		/// <summary>
		/// Starts execution of this service.
		/// </summary>
		public void Start()
		{
		}

		public void Stop()
		{


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
