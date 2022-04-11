using Crawler.Api.Host.Services;
using Crawler.Contracts.Entities;
using Crawler.Contracts.Impl;
using Crawler.Contracts.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Crawler.Api.Host.Controllers
{
	[ApiController]
	[Route("api/v1/[controller]")]
	public class HtmlParserController : ControllerBase
	{
		private readonly IHtmlParserModuleService _htmlParserModuleService;
		private readonly IHtmlParserService _service;
		public HtmlParserController(IHtmlParserModuleService htmlParserModuleService,
			IHtmlParserService service)
		{

			_htmlParserModuleService = htmlParserModuleService;
			_service = service;
		}

		[HttpPost]
		[Route("")]
		public IActionResult Parse([FromBody] string url, bool? bybrowser = null)
		{
			if (url == null)
				return BadRequest();

			var result = _htmlParserModuleService.Parse(url, bybrowser);
			return Ok(result);
		}

		[HttpPost]
		[Route("parse")]
		public IActionResult Parse([FromBody] ParseModelData data)
		{
			if (data == null)
				return BadRequest();

			var result = _htmlParserModuleService.ParseContent(data.Url, data.PostData);
			return Ok(result);
		}

		[HttpPost]
		[Route("post")]
		public IActionResult ParsePost([FromBody] ParseModelData data)
		{
			if (data == null)
				return BadRequest();

			var cookies = new List<Cookie>();
			foreach (var nancyCookie in Request.Cookies)
			{
				string targetDomain = UrlHelper.GetHostUrl(data.Url).Replace("http://", string.Empty);
				cookies.Add(new Cookie(nancyCookie.Key, nancyCookie.Value, string.Empty, targetDomain));
			}
			var result = _htmlParserModuleService.Parse(data.Url, data.PostData, cookies);
			return Ok(result);
		}

		[HttpPost]
		[Route("pattern")]
		public IActionResult GetPattern([FromBody] string data)
		{
			if (data == null)
				return BadRequest();

			var result = _htmlParserModuleService.GetPatternHostRelations(data);
			return Ok(result);
		}

		[HttpPut]
		[Route("pattern")]
		public IActionResult PutPattern([FromBody] PatternHostRelations data)
		{
			if (data == null)
				return BadRequest();

			_htmlParserModuleService.Update(data);
			return Ok();
		}
		[HttpPost]
		[Route("pattern/change")]
		public IActionResult ChangePatternMatch([FromBody] UpdatePatternRequest data)
		{
			if (data == null)
				return BadRequest();

			var relation = _htmlParserModuleService.GetPatternHostRelations(data.Url);
			var map = relation.MappingScheme();
			map.Update(data.Value);
			_htmlParserModuleService.Update(relation);
			return Ok(relation);
		}

		[HttpPost]
		[Route("validate")]
		public IActionResult ValidatePattern([FromBody] PatternValidateRequest data)
		{
			if (data == null)
				return BadRequest();

			var mappingScheme = new MappingScheme()
			{
				Matches = new List<KeyValue>()
			};
			var result = MappingScheme(data, mappingScheme);
			return Ok(result);
		}

		[HttpPost]
		[Route("tryfind")]
		public IActionResult TryFindPattern([FromBody] PatternValidateRequest data)
		{
			if (data == null)
				return BadRequest();
			var encoding = Encoding.UTF8;
			if (data.CodePage.HasValue)
				encoding = Encoding.GetEncoding(data.CodePage.Value);

			var result = _htmlParserModuleService.TryFindPattern(data.Url, encoding);

			return Ok(result);
		}

		[HttpPost]
		[Route("validate/save")]
		public IActionResult ValidateAndSavePattern([FromBody] PatternValidateRequest data)
		{
			if (data == null)
				return BadRequest();
			var mappingScheme = new MappingScheme()
			{
				Matches = new List<KeyValue>()
			};
			var result = MappingScheme(data, mappingScheme);

			_htmlParserModuleService.Save(data, mappingScheme, true);

			return Ok(result);
		}

		[HttpPost]
		[Route("validate/add")]
		public IActionResult ValidateAndAddPattern([FromBody] PatternValidateRequest data)
		{
			if (data == null)
				return BadRequest();
			var relations = _htmlParserModuleService.GetPatternHostRelations(data.Url);

			var mappingScheme = new MappingScheme()
			{
				Matches = new List<KeyValue>()
			};
			relations.EncodingCode = data.CodePage;
			var result = MappingScheme(data, mappingScheme);

			mappingScheme = relations.MappingScheme();
			MappingScheme(data, mappingScheme);
			_htmlParserModuleService.Save(data, mappingScheme, true);

			return Ok(result);
		}

		[HttpPost]
		[Route("stream")]
		public IActionResult GetStream([FromBody] string url)
		{
			if (url == null)
				return BadRequest();
			var stream = _service.GetStream(url);

			return Ok(stream);
		}

		[HttpPost]
		[Route("validate/pattern")]
		public IActionResult GetValidatePattern([FromBody] PatternValidateRequest data)
		{
			if (data == null)
				return BadRequest();
			var mappingScheme = new MappingScheme()
			{
				Matches = data.Patterns
			};

			var result = mappingScheme.Matches.FirstOrDefault();

			return Ok(result);
		}

		[HttpGet]
		[Route("html")]
		public IActionResult ParsePost(string url)
		{
			if (url == null)
				return BadRequest();

			var str = _htmlParserModuleService.GetContent(new ValidationRequest()
			{
				Url = url,
				Encoding = Encoding.UTF8,
				UseBrowser = false,
				Host = url,
			});

			return Ok(str);
		}

		private string MappingScheme(PatternValidateRequest model, MappingScheme mappingScheme)
		{

			if (model.Patterns.Any())
				mappingScheme.Matches.AddRange(model.Patterns);
			if (model.CleanMatches != null && model.CleanMatches.Any())
				mappingScheme.CleanMatches = model.CleanMatches;


			var encoding = Encoding.UTF8;
			if (model.CodePage.HasValue)
				encoding = Encoding.GetEncoding(model.CodePage.Value);
			var useBrowser = model.UseBrowser.HasValue && model.UseBrowser.Value;

			return _htmlParserModuleService.Validate(new ValidationRequest()
			{
				Url = model.Url,
				Encoding = encoding,
				Map = mappingScheme,
				UseBrowser = useBrowser,
				PostData = model.PostData,
				Headers = model.Headers,
				Host = model.Url
			});

		}



	}
	internal class GetBytesResult
	{
		public int? Offset { get; set; }
		public int? Size { get; set; }
		public byte[] Bytes { get; set; }

	}

}
