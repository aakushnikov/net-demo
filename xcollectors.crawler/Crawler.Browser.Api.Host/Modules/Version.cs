using Crawler.Browser.Api.Host.Extensions;
using Nancy;

namespace Crawler.Browser.Api.Host.Modules
{
	partial class Api
	{
		public sealed class Version : NancyModule
		{
			public Version()
				: base("/api/version")
			{
				Get["/"] = __ => Response.AsJson(RestServiceExtensions.GetAssemblyVersion());
			}
		}
	}
}
