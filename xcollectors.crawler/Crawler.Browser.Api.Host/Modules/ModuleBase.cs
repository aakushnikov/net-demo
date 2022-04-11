using System;
using System.Configuration;
using Nancy;

namespace Crawler.Browser.Api.Host.Modules
{
	public abstract class ModuleBase : NancyModule
	{
		private readonly string _tokenKey;
		private readonly bool _enableToken;
		

		protected ModuleBase(string modulePath)
			: base(modulePath)
		{
			_tokenKey = ConfigurationManager.AppSettings["accesstoken"];
			_enableToken = Convert.ToBoolean(ConfigurationManager.AppSettings["enabletoken"]);

			Before += ctx =>
			{
				if (!_enableToken)
					return null;

				var token = Request.Query["accesstoken"];
				if (token != null && token == _tokenKey)
					return null;
				return HttpStatusCode.Forbidden;
			};
		}
	}
}
