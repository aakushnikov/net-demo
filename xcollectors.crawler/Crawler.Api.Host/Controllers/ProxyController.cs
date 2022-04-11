using Crawler.Api.Host.Services;
using Crawler.Contracts.Entities;
using Crawler.Contracts.Impl;
using Crawler.Contracts.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Crawler.Contracts;
using Newtonsoft.Json;

namespace Crawler.Api.Host.Controllers
{
	[ApiController]
	[Route("api/v1/[controller]")]
	public class ProxyController : ControllerBase
	{
		private readonly IProxyContainerService _service;

		public ProxyController(IProxyContainerService service)
		{
			_service = service;
		}

	

		[HttpGet]
		[Route("valid")]
		public IActionResult Valid()
		{
			var proxies = GetProxyItems();
			return Ok(proxies);
		}

		private List<ProxyItem> GetProxyItems(bool isValid = true)
		{
			var proxies = _service.GetProxies(isValid);

			var str = JsonConvert.SerializeObject(proxies);
			return proxies;
		}

		[HttpGet]
		[Route("invalid")]
		public IActionResult Invalid()
		{
			var proxies = GetProxyItems(false);
			return Ok(proxies);
		}


	}
	

}
