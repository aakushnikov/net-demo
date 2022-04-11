﻿using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Crawler.Api.Host
{
	public sealed class ServiceHost : IHostedService, IDisposable
	{
		private readonly IServiceProvider serviceProvider;

		public ServiceHost(IServiceProvider serviceProvider)
		{
			this.serviceProvider = serviceProvider;
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{

			return Task.CompletedTask;
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}

		public void Dispose()
		{
		}

	}
}
