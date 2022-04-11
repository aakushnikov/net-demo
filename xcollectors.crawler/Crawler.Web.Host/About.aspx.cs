using System;
using System.Web.UI;

namespace Crawler.Web.Host
{
	public partial class About : Page
	{
		public string Title { get; set; }

		public About() : base()
		{
			Title = "HoHoHo";
		}

		protected void Page_Load(object sender, EventArgs e)
		{
		}
	}
}