namespace Crawler.Browser.Common.AccountManagment.Entities
{
	public class Proxy
	{
		public int Id { get; set; }

		public string Address { get; set; }

		public int Port { get; set; }

		public string User { get; set; }
		public string Password { get; set; }
		public bool IsEnabled { get; set; }
		public string Reason { get; set; }

		public override string ToString()
		{
			return $"{Address}:{Port}";
		}
	}
}