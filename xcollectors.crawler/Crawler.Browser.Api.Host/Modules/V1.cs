namespace Crawler.Browser.Api.Host.Modules
{
	public sealed partial class Api
	{
		public abstract class v1 : ModuleBase
		{
			public const string ContentType = "application/json; charset=utf-8";
			public const string ContentTypeHtml = "text/html; charset=UTF-8";

			protected v1(string modulePath)
				: base("/api/v1" + modulePath)
			{
			}
		}
	}
}
