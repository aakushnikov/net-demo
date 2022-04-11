using System.Collections.Generic;
using Crawler.Contracts.Entities;

namespace Crawler.Contracts.Services
{
	public interface IDefaultHtmlParserSettings
	{
		MappingScheme Get(string host);

		Dictionary<string, MappingScheme> GetAll();
		void Add(string host, MappingScheme pam);
		void Add(string host, string path);

	}

	public class DefaultHtmlParserSettings : IDefaultHtmlParserSettings
	{
		private readonly Dictionary<string, MappingScheme> _maps = new Dictionary<string, MappingScheme>();

		public DefaultHtmlParserSettings()
		{
			var articlebodyP = @"//div[@itemprop='articleBody']/*/text()";
			var divClassFieldItemsDivP = @"//div[@class='field-items']/div/text()";
			var divClassEntryContentP = @"//div[@class='entry-content']/*/text()";
			var divClassItemfulltextP = @"//div[@class='itemFullText']/*/text()";
			var divClassArticleDivP = @"//div[@class='article']/div/text()";

			var matches = MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='article_text_wrapper']/*/text()",
				@"//article/div/text()"
			});

			matches.Matches.Add(MappingSchemeHelper.GetImagesXPathKeyValue(@"//div[@class='photo']/*/text()"));

			Add("tvzvezda.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@id='news_text']/*/text()"
			}));
			var parserMatches = MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@id='Article']/*/text()"

			});
			parserMatches.CleanMatches =
				new List<KeyValue>()
				{
					new KeyValue(ActionType.Regex)
					{
						Value = "Распечатать"
					}
				};
			Add("vestifinance.ru", parserMatches);
			Add("24vesti.mk", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='content']/*/text()"
			}));

			Add("yandex.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//*[@class='multimedia_main_content_text clearfix' or @class='material_content increase_text']/*/text()",
				@"//article//div/text()",
			}));

			Add("vesti.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='article__text' or @class='article-text' or @class='b-doc__article' or @class='article-block' or @id='Article']/*/text()"
			}));
			Add("vedomosti.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			 {
				@"//article/div/text()",
				@"//div[@class='title']/*/text()",
			}));
			Add("uralinform.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@data-news_content_type='text']/div/text()"
			}));

			Add("tvzvezda.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@id='news_text']/*/text()"
			}));

			Add("tvrain.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='document-content__text']/*/text()"
			}));

			Add("360tv.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@id='article-text']/*/text()"
			}));

			Add("trend.az", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@itemprop='articleBody']/*/text()"
			}));

			Add("tass.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='b-material-text__l']/*/text()",
				@"//div[@class='b-material-text']/div/text()",
				@"//div[@class='padding-left news-item__content']/*/text()",
				@"//p[@class='ng-scope']/*/text()",
				@"//theme/text()"
			}));

			Add("72.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//article[@class='news-block-justify']/*/text()"
			}));
			Add("sovsport.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='article_content']//text()",
				@"//div[@class='article-textBlock js-mediator-article']/*/text()",
				@"//div[@class='message_content']/*/text()",
				@"//div[@class='article_content']/*/text()",
			}));

			Add("slon.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='post-content ' or @class='post-content with-marker' or @class='post-content']/*/text()"
			}));

			Add("rusplt.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//*[@class='body']/*/text()"
			}));

			Add("rusnovosti.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//*[@class='post-content']/*/text()"
			}));

			Add("rt.com", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='article-body' or @class='article__text ']/*/text()"
			}));

			Add("rosbalt.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='newstext']/*/text()"
			}));

			Add("rosbalt.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='newstext']/*/text()"
			}));

			Add("ridus.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='article']/*/text()",
			}));

			Add("riafan.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='entry-content']/*/text()",
			}));

			Add("rg.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//*[@class='text' or @class='main-text' or @itemprop='articleBody' or @class='b-article-main-block' or @class='article-main__main-text__text']/*/text()"
			}));

			Add("regnum.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='news_body']/*/text()"
			}));

			Add("rbc.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='article__text' or @class='newsBlock' or @class='article__full-text js-article-colorbox__full-text']/*/text()",
				@"//div[@class='block_content']/div/text()",
				@"//div[@class='js-materials-description']/*/text()",
			}));

			Add("radiovesti.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='text']/*/text()"
			}));

			Add("novayagazeta.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='b-article g-content g-clearfix']/*/text()"
			}));

			Add("mk.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='content']/*/text()"
			}));
			Add("mail.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='js-newstext text js-mediator-article' or @class='article__item article__item_alignment_left article__item_html']/*/text()",
				@"//div[@id='wpost_post']/div[@class='wall_post_text']/*/text()"
			}));
			Add("m24.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='b-material']/*/text()"
			}));
			Add("lenta.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='b-text clearfix']/*/text()",
				@"//div[@itemprop='articleBody']/*/text()",
				@"//div[@class='b-text clearfix b-topic__body']/*/text()"
			}));
			Add("lenizdat.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='textdiv']/*/text()"
			}));
			Add("kuban24.tv", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='news_content']/*/text()"
			}));
			Add("kp.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='text']/*/text()"
			}));
			Add("izvestia.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@itemprop='articleBody']/*/text()"
			}));
			Add("intermedia.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='news_content page_news_item']/*/text()"
			}));
			Add("interfax-russia.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@itemprop='articleBody' or @class='article']/*/text()",
				@"//div[@class='article']/*/text()",
				@"//div[@class='gm']/*/text()"
			}));

			Add("interfax.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@itemprop='articleBody']/*/text()",
				@"//div[@class='article']/*/text()"
			}));
			Add("govoritmoskva.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='textContent']/*/text()"
			}));
			Add("gazeta.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='news-body__text' or @class='article_text txt_1' or @class='txt_1' or @class='sport-article']/*/text()",
			}));
			Add("gazetadaily.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@id='text-news']/*/text()",
			}));
			Add("fontanka.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='article_fulltext']/*/text()",
				@"//div[@itemprop='opinion_fulltext clr-a']/*/text()",
			}));
			Add("msk.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{

				@"//section[@class='content']//div[@class='typical']//text()",
				@"//section[@class='content']//div[@class='typical include-relap-widget']/*/text()",
				@"//section[@class='content']/div//text()"

			}));
			Add("cnews.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//article[@class='article']/section/text()"
			}));

			Add("championat.com", MappingSchemeHelper.GetPostParserMatches(new[]
				{

					@"//div[@class='text-decor article__contain js-mediator-article']/*/text()" ,
					@"//div[@class='text-decor _reachbanner_ article__contain js-mediator-article']/*/text()" ,
					@"//div[@class='text-decor _reachbanner_ article__contain']/*/text()" ,
					@"//div[@class='text-decor article__contain']/*/text()" ,

				}));
			Add("b-port.com", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='itemFullText']//text()"
			}));
			Add("aviaport.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='justify']/*/text()"
			}));
			Add("arms-expo.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='news-full__text ']/*/text()"
			}));
			Add("aif.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='article-body']/*/text()"
			}));
			Add("1prime.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='article-body']/*/text()"
			}));
			Add("ria.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@id='article_full_text' or @itemprop='articleBody']/*/text()"
			}));
			Add("avito.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='item-description  clearfix' or @class='description description-text']/*/text()"
			}));
			Add("evpatoriya.today", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='td-post-content td-pb-padding-side']/*/text()"
			}));
			Add("kontur.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@id='ArticleItem']/*/text()"
			}));
			Add("buhonline.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='text-block']/*/text()"
			}));
			Add("otr-online.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='news-in__item innews']/*/text()"
			}));
			Add("lomonholding.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//content/text()"
			}));
			Add("novpressa.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='theme']/*/text()"
			}));
			Add("mr7.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//article/div/text()"
			}));
			Add("ucheba.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//article/div/*/text()"
			}));
			Add("fulledu.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='article-item']/*/text()"
			}));
			Add("argumentiru.com", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='fulltext']/*/text()"
			}));
			Add("trucksale.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='col-xs-12']/*/text()"
			}));
			Add("osetiatimes.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='textt']/*/text()"
			}));
			Add("presidentruo.org", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='thecontent']/*/text()"
			}));
			Add("resfo.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='item-page']/*/text()"
			}));
			Add("rambler.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='article__paragraph']/*/text()"
			}));
			Add("rueconomics.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@id='news_content']/*/text()"
			}));
			Add("upravavernadskogo.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='entry']/div/text()"
			}));
			Add("utro.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				divClassArticleDivP
			}));
			Add("sroportal.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='entry']/*/text()"
			}));
			Add("saroblnews.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='txt']/*/text()"
			}));
			Add("yugtimes.com", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//article/text()"
			}));
			Add("nia-rf.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='clear']/div/text()"
			}));
			Add("android-robot.com", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='entry-content']/*/text()"
			}));
			Add("ca-news.org", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='newstext']/div/text()"
			}));
			Add("kvadroom.media", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='a_text']/div/text()"
			}));
			Add("polit-gramota.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@id='post-content']/*/text()"
			}));
			Add("an-crimea.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='text']/*/text()"
			}));
			Add("kafanews.com", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='text-body']/*/text()"
			}));
			Add("stfw.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//span[@itemprop='articleBody']/*/text()"
			}));
			Add("forum-msk.org", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				articlebodyP
			}));
			Add("teleprogramma.pro", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				articlebodyP
			}));
			Add("wellnews.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//span[@itemprop='articleBody']/*/text()"
			}));
			Add("golosinfo.org", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='text']/*/text()"
			}));
			Add("nalin.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				articlebodyP
			}));
			Add("realty-agency.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@id='innerMainContent']/*/text()"
			}));
			Add("moscow-baku.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='news_detail_text']/*/text()"
			}));
			Add("theins.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='post-entry']/*/text()"
			}));
			Add("vaonews.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='entry-content']/div/text()"
			}));
			Add("reforum.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='content']/div/text()"
			}));
			Add("novostroyki.org", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='txt-box']/*/text()"
			}));
			Add("saroblnews.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='post-content']/*/text()"
			}));
			Add("mesto.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				articlebodyP
			}));
			Add("udm.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				articlebodyP
			}));
			Add("saroblnews.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@id='content']/*/div[@class='txt']/*/text()"
			}));
			Add("vistanews.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@itemprop='articleBody']/*/text()"
			}));
			Add("gazeta-moy-rayon-donskoy.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				divClassEntryContentP
			}));
			Add("censury.net", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				divClassItemfulltextP
			}));
			Add("nsp.su", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@style='float:left;clear:left;width:100%']/*/text()"
			}));
			Add("znak.com", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//article/text()"
			}));
			Add("zyorna.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='content_text_wrapper']/*/text()"
			}));
			Add("realnoevremya.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//article/text()",
			}));
			Add("ystav.com", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				articlebodyP
			}));
			Add("caoinform.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='entry-content clearfix']/*/text()"
			}));
			Add("trud.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='text']/*/text()",
			}));
			Add("mperspektiva.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='topic__content']/*/text()",
			}));
			Add("icmos.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='detail_text']/*/text()",
			}));
			Add("balloone.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@id='content']/*/text()",
			}));
			Add("irn.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//article/div/text()",
			}));
			Add("rsport.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='article_text']/*/text()",
			}));
			Add("sportsdaily.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='article_text']/*/text()",
			}));
			Add("tsargrad.tv", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//article/*/text()",
			}));
			Add("pronedra.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='class=main-text']/*/text()",
			}));
			Add("admtroitsk.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='class=PageTextPd']/*/text()",
			}));
			Add("svpressa.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				articlebodyP
			}));


			Add("cheremuha.com", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				divClassItemfulltextP,
			}));
			Add("business-gazeta.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//article/text()",
			}));
			Add("svopi.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='maincont']/*/text()",
			}));
			Add("vnukovskoe.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='content_text']/*/text()",
			}));
			Add("zbulvar.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='entry clearfix']/*/text()",
			}));
			Add("gazeta-na-varshavke-nagorny.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				divClassEntryContentP,
			}));
			Add("tvc.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='b-article__text']/*/text()",
			}));
			Add("gazeta-tsaricinsky-vestnik.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				divClassEntryContentP,
			}));
			Add("kommersant.ru", matches);
			//
			Add("mossovetinfo.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='news-detail']/*/text()",
			}));
			Add("leonidvolkov.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='b-post__content']/*/text()",
			}));
			Add("the-village.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='article-text']/*/text()",
			}));
			Add("snob.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@data-article-page]/*/text()",
			}));
			Add("mossovet.tv", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='single_post']/div/text()",
			}));
			Add("scandaly.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='entryinbody']/*/text()",
			}));
			Add("zona.media", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//section/text()",
			}));
			Add("ok-inform.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//section/text()",
			}));
			Add("sova-center.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='article']/*/text()",
			}));
			Add("avtoradio.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='news-time-text']/*/text()",
			}));
			Add("silver.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='text']/*/text()",
			}));
			Add("news.tj", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='content']/div/text()",
			}));
			Add("gov.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='news-one__content']/div/text()",
			}));
			Add("sibnovosti.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='content']/*/text()",
			}));
			Add("baikal24.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//article/text()",
			}));
			Add("bloknot.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				divClassArticleDivP,
			}));
			Add("perebezhchik.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='news-full']/div/text()",
			}));
			Add("newslab.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//article/text()",
			}));
			Add("prmira.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='openPage__content']/div/text()",
			}));

			Add("flb.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//tbody/tr/td/index/text()",
			}));

			Add("ren.tv", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				divClassFieldItemsDivP,
			}));
			Add("most.tv", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				articlebodyP,
			}));
			Add("caravan.kz", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='article_text']/*/text()",
			}));
			Add("sledcomrf.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='article-content']/*/text()",
			}));
			Add("nation-news.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='single_text']/*/text()",
			}));
			Add("dolgoprudny-news.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='major_stat_content']/*/text()",
			}));
			Add("mos.news", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='detail_text_container']/*/text()",
			}));
			Add("bloknot-voronezh.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='news-text']/*/text()",
			}));
			Add("36on.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='doc-content']/div/text()",
			}));
			Add("politsib.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='entry entry-content']/*/text()",
			}));
			Add("krasrab.net", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='news-detail']/*/text()",
			}));
			Add("hayinfo.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='td-post-content td-content']/*/text()",
			}));
			Add("glavny.tv", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				divClassFieldItemsDivP,
			}));
			Add("stolica-s.su", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='content']/*/text()",
			}));
			Add("dontr.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='full-text']/*/text()",
			}));

			Add("business.ua", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				articlebodyP,
			}));
			Add("gordonua.com", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='a_body']/*/text()",
			}));
			Add("tvr.by", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//article/text()",
			}));
			Add("rapsinews.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='body']/*/text()",
			}));
			Add("krivoe-zerkalo.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				articlebodyP,
			}));
			Add("mixnews.lv", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='news-content']/*/text()",
			}));
			Add("odnarodyna.org", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='field-items']/div/div/text()",
			}));
			Add("rybinsk-once.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='post']/*/text()",
			}));
			Add("kapitalist.tv", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				divClassItemfulltextP,
			}));
			Add("rospres.com", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='shortcode-content']/*/text()",
			}));

			Add("province.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				divClassItemfulltextP,
			}));

			Add("gursesintour.com", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='entry']/*/text()",
			}));
			Add("sovsekretno.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='text']/*/text()",
			}));
			Add("footballtop.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='content']//text()",
			}));
			Add("soccerland.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//td[@class='newsbody']/*/text()",
			}));
			Add("kbrria.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='field-content']/*/text()",
			}));
			Add("ko.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				divClassItemfulltextP,
			}));
			Add("163gorod.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='field-item even']/*/text()",
			}));
			Add("sevkavportal.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='itemFullText ']/*/text()",
			}));
			Add("vyborg.to", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='vp-article']/*/text()",
			}));
			Add("sayanogorsk.info", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='artContent']/*/text()",
			}));
			Add("rusk.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='freetext hyphens']/*/text()",
			}));
			Add("komiinform.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='row']/div[@class='daGallery']/*/text()",
			}));
			Add("v-kurse.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='news-detail-text']/*/text()",
			}));
			Add("logistic.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='newsbody']/*/text()",
			}, false));
			Add("abnews.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				divClassEntryContentP,
			}));
			Add("urfo.org", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				articlebodyP,
			}));

			Add("varlamov.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@itemprop='text']/*/text()",
			}));
			Add("terrikon.com", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='posttext _ga1_on_']/*/text()",
			}));
			Add("stadium.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@data-newsid]/*/text()",
			}));
			Add("wek.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				articlebodyP,
			}));
			Add("rusvesna.su", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='field-item even']/*/text()",
			}));
			Add("joinfo.ua", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='entry']/*/text()",
			}));
			Add("mid.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='text article-content']/*/text()",
			}));
			Add("woman.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='article-text']/*/text()",
			}));
			Add("esquire.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//article//div[@class='display']/*/text()",
				@"//article/div/text()"
			}));
			Add("loveradio.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='main_news_block']/*/text()"
			}));
			Add("internovosti.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//tbody/tr/td[@width='100%']/*/text()"
			}));
			Add("mospravda.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='post_content']/*/text()"
			}));
			Add("mospravda.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='post_content']/*/text()"
			}));
			Add("sports.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//article/div/text()",
				@"//div[@class='mainPart']//text()",
			}));
			Add("finmarket.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@clas='body']/*/text()",
			}));
			Add("pravdevglaza.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@clas='full-news-content']/div/div[@id]/*/text()",
			}));
			Add("fergananews.com", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@id='text']/*/text()",
			}));
			Add("inosmi.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='article-body']/*/text()",
				@"//article//text()",
				@"//article//div[@class='photo-media__desc']/*/text()",
			}));
			Add("24smi.org", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='article__content']/div/text()"
			}));
			Add("kapital-rus.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//article[@class='newsbody']/*/text()"
			}));
			Add("lifenews.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@itemprop='text']/*/text()"
			}));
			Add("ntv.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='newitemntext']/*/text()"
			}));

			Add("mk-kaliningrad.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='content']/*/text()",
			}));
			Add("mk-smolensk.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='content']/*/text()",
			}));
			Add("mk-kirov.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='content']/*/text()",
			}));

			Add("vnovomsvete.com", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='content']/*/text()",
			}));

			Add("meduza.io", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='Body']/*/text()",
			}));
			Add("bbc.com", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@property='articleBody']/*/text()",
			}));
			Add("alta.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//article/text()",
			}));
			Add("mil.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@id='content']/div[@id='center']/*/text()",
			}));

			Add("kerch.fm", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='article_text']/*/text()",
			}));
			Add("mynewsonline24.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='post_content cf']/*/text()",
			}));

			Add("eg.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='textcont']/*/text()",
			}));
			Add("dp.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@id='ArticleContent']/*/text()",
			}));
			Add("news2world.net", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@id='article_content']/*/text()",
			}));
			Add("kavkaz-uzel.eu", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='articles-body']/*/text()",
			}));
			Add("klerk.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@id='redactor']/*/text()",
			}));
			Add("mskagency.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='Text']/*/text()",
			}));
			Add("golos-ameriki.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='wysiwyg']/*/text()",
			}));

			Add("autond.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='full-text']/*/text()",
			}));
			Add("currenttime.tv", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='wysiwyg']/*/text()",
			}));
			//Add("ng.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			//{
			//	@"//div[@class='content']//div[@class='w700']/*/text()",
			//}));
			Add("levada.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				divClassEntryContentP,
			}));
			Add("banki.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//article/text()",
			}));
			Add("vz.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='text']/*/text()",
			}));
			Add("neelov.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//article/text()",
			}));

			Add("businesspuls.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='entry']/*/text()",
			}));
			Add("sport.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='article-text clearfix']/*/text()",
				@"//div[@class='article-content']//text()",

			}));
			Add("finanz.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='news_text']/*/text()",
			}));
			Add("na-zapade-mos.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='entry-box clearfix' or @class='ctext']/*/text()",
			}));
			Add("premium-watches.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='post-view']/div/div/text()",
			}));

			Add("rcmm.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//*[@class='full-story']/*/text()",
			}));
			Add("infokam.su", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='colCenter']/*/text()",
			}));

			Add("co.uk", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@property='articleBody']/*/text()",
				@"//div[@property='gallery-intro']/*/text()",

			}));
			Add("moslenta.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='topic__body js-list']/*/text()",
				@"//div[@class='card-in-topic__body']/*/text()",

			}));
			Add("politrussia.com", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='news-detail']/*/text()",
				articlebodyP,
			}));

			Add("fapnews.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='single_text']/*/text()",
			}));

			Add("politobzor.net", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='full-story']/div/text()",
			}));
			Add("versia.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='article-text']/*/text()",
				articlebodyP,
			}));

			Add("warfiles.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='articleBody']/*/text()",
			}));

			Add("ryb.ru", @"//div[@class='post']/*/text()");
			Add("rusfootball.info", @"//div[@class='short-story-news']/*/text()");
			Add("sportbox.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@class='node-content__body']/*/text()",
				@"//div[@class='node-header__wrapper']/*/text()",


			}));
			Add("life.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@itemprop='text']/*/text()",
				@"//div[@class='item_freetext']/*/text()"
			}));
			Add("uincar.ru", @"//article[@itemprop='articleBody']/*/text()");
			Add("siapress.ru", articlebodyP);
			Add("b-mag.ru", @"//div[@class='blogcontent']/*/text()");
			Add("gismeteo.ru", @"//div[@class='article__i ugc']/div/text()");
			Add("mir-politika.ru", @"//div[@class='full-story']/div/div/text()");


			Add("rusdialog.ru", MappingSchemeHelper.GetPostParserMatches(@"//div[@class='news-text']/*/text()"));

			Add("newsru.com", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@id='text' or @class='maintext' or @class='article-text']/*/text()",
				@"//div[@id='text' or @class='maintext' or @class='article-text']//text()",
			}));

			Add("inopressa.ru", MappingSchemeHelper.GetPostParserMatches(@"//div[@class='topic']/div[@class='body']/*/text()"));
			Add("newsmsk.com", MappingSchemeHelper.GetPostParserMatches(@"//div[@class='article_text']/div[@class='_ga1_on_']/*/text()"));
			Add("eadaily.com", MappingSchemeHelper.GetPostParserMatches(@"//article/div[@class='news-text-body']/*/text()"));
			Add("primamedia.ru", MappingSchemeHelper.GetPostParserMatches(@"//div[@class='page-content']/*/text()"));
			Add("pln-pskov.ru", MappingSchemeHelper.GetPostParserMatches(@"//div[@id='material_txt']/*/text()"));
			Add("voicesevas.ru", MappingSchemeHelper.GetPostParserMatches(@"//div[@class='maincont']/*/text()"));
			Add("47news.ru", MappingSchemeHelper.GetPostParserMatches(@"//div[@class='font-m article-text']/*/text()"));
			var postParserMatches = MappingSchemeHelper.GetPostParserMatches(@"//div[@itemprop='articleBody']/*");
			postParserMatches.CleanMatches =
				new List<KeyValue>()
				{
					new KeyValue(ActionType.Regex)
					{
						Value = "Другие новости[\\W\\S]+"

					} };
			Add("newdaynews.ru", postParserMatches);

			//	Add("radiovesti.ru",Helper.GetPostParserMatches(@"//div[@class='text']/*/text()"));
			Add("golos-ameriki.ru", MappingSchemeHelper.GetPostParserMatches(@"//div[@class='articleContent']/div/div/text()"));
			Add("newokruga.ru", MappingSchemeHelper.GetPostParserMatches(@"//div[@class='entry-content clearfix']/*/text()"));
			Add("5-tv.ru", MappingSchemeHelper.GetPostParserMatches(@"//div[@class='descr' or @class='text']/*/text()"));
			Add("mir24.tv", MappingSchemeHelper.GetPostParserMatches(@"//div[@class='part__wrap']/div/text()"));
			Add("forbes.ru", MappingSchemeHelper.GetPostParserMatches(@"//div[@class='news']/*/text()"));
			Add("dni.ru", MappingSchemeHelper.GetPostParserMatches(@"//div[@class='article__text']/*/text()"));
			Add("expert.ru", MappingSchemeHelper.GetPostParserMatches(@"//div[@id='doc_body']/*/text()"));
			Add("odnako.org",
			 MappingSchemeHelper.GetPostParserMatches(@"//article/div/text()"));
			Add("newtimes.ru",
			 MappingSchemeHelper.GetPostParserMatches(@"//div[@class='text']/div[@class='story']/*/text()"));
			Add("mirnov.ru",
			 MappingSchemeHelper.GetPostParserMatches(@"//div[@class='con_text']/*/text()"));
			Add("newizv.ru",
			 MappingSchemeHelper.GetPostParserMatches(@"//article/div[@class='text-body']/*/text()"));
			Add("pnp.ru",
			 MappingSchemeHelper.GetPostParserMatches(@"//div[@class='context clear']/*/text()"));
			//
			Add("klerk.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//article/div/*",
				@"//div[@class='content-article-text']/*/text()",
			}));

			Add("bankir.ru",
			 MappingSchemeHelper.GetPostParserMatches(@"//div[@class='article-text clearfix']/*/text()"));
			Add("finparty.ru",
			 MappingSchemeHelper.GetPostParserMatches(@"//div[@class='news-detail']/*/text()"));
			Add("newsinfo.ru",
			 MappingSchemeHelper.GetPostParserMatches(@"//div[@class='article articleLink _ga1_on_']/*/text()"));
			Add("moneytimes.ru",
			 MappingSchemeHelper.GetPostParserMatches(@"//div[@class='_ga1_on_']/*/text()"));
			Add("colta.ru",
			 MappingSchemeHelper.GetPostParserMatches(@"//div[@class='content']/*/text()"));

			Add("moscow-post.com",
				MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@id='text_content']/*/text()",
			}));

			Add("kavkaz-uzel.ru",
			 MappingSchemeHelper.GetPostParserMatches(@"//div[@class='articles-body']/*/text()"));
			Add("file-rf.ru",
			 MappingSchemeHelper.GetPostParserMatches(@"//div[@class='content']/div/text()"));
			Add("yoki.ru",
					 MappingSchemeHelper.GetPostParserMatches(@"//div[@id='article_text']/*/text()"));
			Add("fedpress.ru",
			 MappingSchemeHelper.GetPostParserMatches(@"//div[@class='content']/*/text()"));
			Add("mfd.ru",
			 MappingSchemeHelper.GetPostParserMatches(@"//div[@class='m-content']/*/text()"));
			Add("nvdaily.ru",
			 MappingSchemeHelper.GetPostParserMatches(@"//div[@class='post-inner']/div/text()"));

			Add("kaluga24.tv",
					 MappingSchemeHelper.GetPostParserMatches(divClassEntryContentP));
			Add("ura.ru",
			 MappingSchemeHelper.GetPostParserMatches(new[]
			 {
				 @"//div[@ng-bind-html='item.text']/*/text()",
				 @"//div[@ng-bind-html='item.text_ajax']/*/text()",
				 @"//div[@class='item']/*/text()"
			 }));


			Add("ferra.ru",
			 MappingSchemeHelper.GetPostParserMatches(@"//div[@class='newsbody']/*/text()"));

			Add("rzn.info",
					 MappingSchemeHelper.GetPostParserMatches(@"//div[@class='text']/*/text()"));

			//Add("interfax-russia.ru",Helper.GetPostParserMatches(@"//div[@class='gm']/*/text()"));

			Add("justmedia.ru",
			 MappingSchemeHelper.GetPostParserMatches(@"//div[@class='content content_clear']/*/text()"));
			Add("regions.ru", MappingSchemeHelper.GetPostParserMatches(@"//div[@class='content-entry']/*/text()"));
			Add("arb.ru",
			 MappingSchemeHelper.GetPostParserMatches(divClassArticleDivP));
			Add("gudok.ru", MappingSchemeHelper.GetPostParserMatches(@"//div[@class='publication__body']/*/text()"));
			Add("svoboda.org",
					 MappingSchemeHelper.GetPostParserMatches(@"//div[@class='zoomMe' or @class='wysiwyg']/*/text()"));
			Add("vm.ru",
			 MappingSchemeHelper.GetPostParserMatches(new[]
			 {
				 @"//article/div/div/text()",
				 @"//div[@class='content clearfix']/*/text()"
			 }));
			Add("infox.ru",
			 MappingSchemeHelper.GetPostParserMatches(@"//div[@class='_ga1_on_']/*/text()"));
			Add("mos.ru",
			 MappingSchemeHelper.GetPostParserMatches(@"//article/text()"));

			Add("sport-express.ru",
			 MappingSchemeHelper.GetPostParserMatches(@"//main/div/text()"));

			Add("pravda.ru",
			 MappingSchemeHelper.GetPostParserMatches(@"//div[@class='full article']/*/text()"));
			Add("kulichki.net",
					 MappingSchemeHelper.GetPostParserMatches(@"//div[@class='text_n']/*/text()"));
			Add("ru24.su",
					 MappingSchemeHelper.GetPostParserMatches(@"//article/div/text()"));

			Add("lratvakan.com", MappingSchemeHelper.GetPostParserMatches(@"//div[@class='text']/*/text()"));

			Add("ctv.by",
					 MappingSchemeHelper.GetPostParserMatches(@"//div[@class='field-body']/*/text()"));
			Add("1nnc.net",
					 MappingSchemeHelper.GetPostParserMatches(@"//div[@class='text_box']/*/text()"));

			Add("spbdnevnik.ru",
			 MappingSchemeHelper.GetPostParserMatches(@"//div[@class='news-article']/*/text()"));

			Add("3dnews.ru",
			 MappingSchemeHelper.GetPostParserMatches(@"//div[@class='article-entry']/div/text()"));

			Add("allhockey.ru", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//tbody/tr/td/div/text()",

			}));

			Add("riamo.ru",
			 MappingSchemeHelper.GetPostParserMatches(new[]
			 {
				 @"//div[@class='body wysiwyg']/*/text()",
				 @"//div[@class='richtext']/*/text()",


			 }));

			Add("apiural.ru",
			 MappingSchemeHelper.GetPostParserMatches(@"//div[@class='newsOne']/div/text()"));

			Add("filki.net",
					 MappingSchemeHelper.GetPostParserMatches(articlebodyP));

			var divClassTdPostContentP = @"//div[@class='td-post-content']/*/text()";
			Add("mos7ya.ru", MappingSchemeHelper.GetPostParserMatches(divClassTdPostContentP));

			Add("2x2.su", MappingSchemeHelper.GetPostParserMatches(new[]
			{
				@"//div[@id='article_txt']/*/text()",
				@"//div[@class='cont_block']/div/text()"
			}));


			Add("molnet.ru", MappingSchemeHelper.GetPostParserMatches(@"//article/div/text()"));

			Add("metronews.ru", MappingSchemeHelper.GetPostParserMatches(@"//div[@class='article-body']/*/text()"));

			Add("autostavka.ru", MappingSchemeHelper.GetPostParserMatches(divClassTdPostContentP));

			Add("mngz.ru", MappingSchemeHelper.GetPostParserMatches(@"//div[@id='content']/div/div/text()"));

			Add("lgz.ru", MappingSchemeHelper.GetPostParserMatches(@"//div[@id='display_text']/*/text()"));

			Add("sovross.ru", MappingSchemeHelper.GetPostParserMatches(@"//*[@class='content-text']/*/text()"));


			Add("volgadaily.ru", @"//div[@class='post']/*/text()");
			Add("politonline.ru", @"//div[@class='full article']/*/text()");



		}

		public MappingScheme Get(string host)
		{
			if (_maps.ContainsKey(host))
				return _maps[host];
			return null;
		}

		public Dictionary<string, MappingScheme> GetAll()
		{
			return _maps;
		}

		public void Add(string host, MappingScheme pam)
		{
			if (!_maps.ContainsKey(host))
				_maps.Add(host, pam);
		}
		public void Add(string host, string path)
		{

			Add(host, MappingSchemeHelper.GetPostParserMatches(path));
		}
	}
}
