using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Crawler.Contracts;
using Crawler.Contracts.Impl.Proxies;
using Crawler.Contracts.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Service.Registry.Common;

namespace Crawler.Contracts.Impl
{

	public class ProxyContainerService : IProxyContainerService
	{
		private readonly IServiceRegistryFactory _serviceRegistryFactory;
		private readonly ILogger _logger;
		private ConcurrentQueue<ProxyItem> _queue;
		private readonly object _sync = new object();
		private readonly MemoryCache _cache;
		private readonly MemoryCache _cacheInvalid;
		private readonly MemoryCacheEntryOptions _policy;
		private string _host;
		public ProxyContainerService(IServiceRegistryFactory serviceRegistryFactory, ILoggerFactory loggerFactory)
		{
			_logger = loggerFactory.CreateLogger("default");
			_serviceRegistryFactory = serviceRegistryFactory;
			_cache = new MemoryCache(new MemoryCacheOptions() { });
			_cacheInvalid = new MemoryCache(new MemoryCacheOptions() { });
			_policy = new MemoryCacheEntryOptions();
			
		}

		public string Host
		{
			get
			{
				if (!string.IsNullOrEmpty(_host))
					return _host;
				_host = _serviceRegistryFactory.GetCommonClient("accountManagement");
				if (!_host.EndsWith("/"))
					_host += "/";
				Console.WriteLine(_host);
				return _host;
			}
		}
		public ProxyItem Get()
		{
			try
			{
				if (_queue == null)
				{
					Init();
				}
				ProxyItem address;
				if (_queue.TryDequeue(out address))
					_queue.Enqueue(address);
				return address;

			}
			catch (Exception ex)
			{
				//_logger.Error(ex.Message, ex);
				//TODO: место для вашего лога
			}
			return null;
		}

		public ProxyItem GetValidProxy()
		{

			var proxy = Get();
			if (proxy == null)
				return null;
			if (_cache.TryGetValue(proxy.Address, out proxy))
			{
				return proxy;
			}
			bool isValid = false;
			while (!isValid && proxy != null)
			{
				isValid = IsValidLocal(proxy);
				if (isValid)
					break;
				proxy = Get();

			}


			return proxy;

		}

		private void AddToValid(ProxyItem proxy)
		{
			_policy.AbsoluteExpiration = new DateTimeOffset(DateTime.Now.AddMinutes(30));
			_cache.Set(proxy.Address, proxy, _policy);
		}
		private void AddToInvalid(ProxyItem proxy)
		{
			_policy.AbsoluteExpiration = new DateTimeOffset(DateTime.Now.AddMinutes(30));
			_cacheInvalid.Set(proxy.Address, proxy, _policy);
		}

		private bool _isProcessStart;
		public List<ProxyItem> GetProxies(bool isValid)
		{
			var result = new ConcurrentBag<ProxyItem>();
			Init();
			var proxies = _queue.ToArray();
			Parallel.ForEach(proxies, proxy =>
			{
				var valid = IsLocal(proxy);
				if (valid == isValid)
				{
					result.Add(proxy);

				}
			});
			var resultCheck = new ConcurrentBag<ProxyItem>();
			var task = Task.Factory.StartNew(() =>
			{
				CheckProcess(isValid, proxies, resultCheck);
			});
			if (result.IsEmpty)
			{
				task.Wait(2000);
				return resultCheck.ToList();
			}

			return result.ToList();
		}

		private void CheckProcess(bool isValid, ProxyItem[] proxies, ConcurrentBag<ProxyItem> result)
		{
			if (!_isProcessStart)
			{
				_isProcessStart = true;
				try
				{
					Parallel.ForEach(proxies, proxy =>
					{
						var valid = IsValidLocal(proxy);
						if (valid == isValid)
						{
							result.Add(proxy);
						}
					});
				}
				finally
				{
					_isProcessStart = false;
				}
			}
		}

		private bool IsLocal(ProxyItem proxy)
		{
			if (_cache.Get(proxy.Address) != null)
			{
				return true;
			}
			if (_cacheInvalid.Get(proxy.Address) != null)
			{
				return false;
			}
			return false;
		}

		private bool IsValidLocal(ProxyItem proxy)
		{
			if (_cache.Get(proxy.Address) != null)
			{
				return true;
			}
			if (_cacheInvalid.Get(proxy.Address) != null)
			{
				return false;
			}
			var valid = IsValid(proxy);
			if (valid)
			{
				AddToValid(proxy);
			}
			else
			{
				AddToInvalid(proxy);
			}
			return valid;
		}

		public bool IsValid(ProxyItem proxy)
		{

			try
			{


				var webRequest = (HttpWebRequest)WebRequest.Create(GetUrl());
				webRequest.Accept = "text/html, application/xhtml+xml, */*";
				webRequest.Headers.Add("Accept-Language", "ru");
				webRequest.ContentType = "application/x-www-form-urlencoded";
				webRequest.KeepAlive = true;
				webRequest.AllowAutoRedirect = false;
				webRequest.Timeout = 3000;

				var myProxy = new WebProxy(proxy.Address, false);
				webRequest.Proxy = myProxy;
				if (!string.IsNullOrEmpty(proxy.User))
				{
					webRequest.Proxy.Credentials = new NetworkCredential(proxy.User, proxy.Password);
				}
				using (var webResponse = (HttpWebResponse)webRequest.GetResponse())
				{
					using (var stream = webResponse.GetResponseStream())
					{
						using (var streamReader = new StreamReader(stream, Encoding.UTF8))
						{
							var content = streamReader.ReadToEnd();
						}
					}
				}
			}
			catch (Exception)
			{
				return false;
			}

			return true;
		}
		private static string[] _urls = new string[] { "http://ya.ru", "https://mail.ru", "https://www.google.ru", "http://tv.mindscan.ru", "http://mindscan.ru" };
		private static string GetUrl()
		{
			var random = new Random();
			var index = random.Next(0, 4);
			return _urls[index];
		}

		public List<ProxyItem> GetProxies()
		{
			var result = new List<ProxyItem>();
			try
			{
			
				var url = $"{Host}api/v1/Proxy/all";
				using (var client = new WebClient())
				{
					
					if (string.IsNullOrEmpty(Host))
						throw new InvalidOperationException("ServiceRegistry client.config is Empty. Please set  <client id=\"accountManagement\" host=\"http://localhost:8081/api/\"/>");

					
					var data = client.DownloadString(url);
					if (string.IsNullOrEmpty(data))
							throw new InvalidOperationException($"Method return empty data {data}");

					_logger.LogInformation(data);
					var res = JsonConvert.DeserializeObject<ProxyItemDataResult>(data);
					foreach (ProxyItemData item in res.Items)
					{
						result.Add(new ProxyItem()
						{
							Address = $"{item.Address}:{item.Port}",
							User = item.User,
							Password = item.Password

						});
					}
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, ex.Message);
				throw;
				return null;
			}
			return result;
		}
		private void Init()
		{
			lock (_sync)
			{
				if (_queue == null)
				{
					var proxies = GetProxies();
					var queue = new ConcurrentQueue<ProxyItem>();
					if (proxies == null)
						throw new InvalidOperationException("Proxy is empty");
					Parallel.ForEach(proxies, p => { IsValidLocal(p); });
					foreach (var p in proxies)
					{
						queue.Enqueue(p);
					}
					_queue = queue;
				}
			}
		}
	}
}
