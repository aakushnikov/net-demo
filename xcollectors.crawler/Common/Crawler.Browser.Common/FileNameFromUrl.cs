using System.Text;
using System.Text.RegularExpressions;

namespace Crawler.Browser.Common
{
	internal static class FileNameFromUrl
	{
		private static readonly Regex R = new Regex(@"[a-z0-9]+", RegexOptions.IgnoreCase);
		public static string ConvertToWindowsFileName(string urlText)
		{
			var sb = new StringBuilder("source");

			foreach (Match urlPart in R.Matches(urlText))
			{
				sb.Append($"_{urlPart.Value}");
			}
			sb.Append(".html");
			return sb.ToString();
		}
	}
}
