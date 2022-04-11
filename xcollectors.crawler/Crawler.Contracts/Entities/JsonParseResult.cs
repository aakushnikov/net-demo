using System.Net;

namespace Crawler.Contracts.Entities
{
	public class JsonParseResult <T>
	{
		public T Object { get; set; }

		public HttpStatusCode StatusCode { get; set; }
	}
}
