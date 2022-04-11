using Nancy;

namespace Crawler.Browser.Api.Host.Logging
{
	public interface INancyInterceptor
	{
		Response RequestIntercept(NancyContext context);

	}
}