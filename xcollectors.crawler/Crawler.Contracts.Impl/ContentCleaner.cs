using System.Text.RegularExpressions;
using HtmlAgilityPack;
using System.Web;

namespace Crawler.Contracts.Impl
{
	public interface IContentCleaner
	{
		string Clean(string content);
	}
	public sealed class ContentCleaner : IContentCleaner
	{
		private static readonly Regex RegextToStripExceedWhitespaces = new Regex(@"\s+", RegexOptions.Compiled | RegexOptions.Singleline);

		private static readonly Regex _regexToStripPunctuations = new Regex(@"((?'punct'[\p{P}\p{S}])\k'punct')", RegexOptions.Compiled | RegexOptions.Singleline);

		private static readonly Regex RegexToBrTag = new Regex(@"(<br>)|(<br/>)", RegexOptions.Compiled | RegexOptions.Singleline);
		private static readonly Regex RegexToQuoat = new Regex(@"(&|)(nbsp;|raquo;|mdash;|laquo;)", RegexOptions.Compiled | RegexOptions.Singleline);

		private static readonly Regex RegexToShyTag = new Regex(@"&shy;", RegexOptions.Compiled | RegexOptions.Singleline);
		private static readonly Regex RegexToXmlTag = new Regex(@"(\<\!\-\-((?!\-\-\>)[\s\S])*\-\-\>)|(<script[^<]+<\/script>) | (&lt;[^&gt;]+&gt;)|(<[^>]+>)", RegexOptions.Compiled | RegexOptions.Singleline);
		private static readonly Regex RegexToScript = new Regex(@"(<script.*script>)|(<style.*style>)", RegexOptions.Compiled | RegexOptions.Singleline);

		public string Clean(string content)
		{
			if (string.IsNullOrEmpty(content))
				return content;

			string cleanedContent = string.Empty;
			cleanedContent = RegexToScript.Replace(content, " ");
			cleanedContent = RegexToShyTag.Replace(cleanedContent, "");
			try
			{
				HtmlDocument html = new HtmlDocument();
				html.LoadHtml(cleanedContent);

				cleanedContent = HttpUtility.HtmlDecode(html.DocumentNode.InnerText);
			}
			catch { }


			cleanedContent = RegextToStripExceedWhitespaces.Replace(cleanedContent, " ");
			cleanedContent = RegexToBrTag.Replace(cleanedContent, System.Environment.NewLine);

			int length;
			do
			{
				length = cleanedContent.Length;
				//cleanedContent = _regexToStripPunctuations.Replace(cleanedContent, "${punct}");

			} while (length != cleanedContent.Length);
			cleanedContent = RegexToXmlTag.Replace(cleanedContent, " ");
			cleanedContent = RegexToQuoat.Replace(cleanedContent, "");





			return cleanedContent.Trim();
		}
	}
}
