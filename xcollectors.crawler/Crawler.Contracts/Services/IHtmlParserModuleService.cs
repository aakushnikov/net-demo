using System.Collections.Generic;
using System.Net;
using System.Text;
using Crawler.Contracts;
using Crawler.Contracts.Entities;
using Crawler.Contracts.Repositories;
using Crawler.Contracts.Services;

namespace Crawler.Contracts.Services
{

	public interface IHtmlParserModuleService
	{
		string Parse(string url, bool? useBrowser = false);

		string Parse(string url, string postData, List<Cookie> cookies = null, bool? useBrowser = false);

		string ParseContent(string url, string content);

		PatternHostRelations GetPatternHostRelations(string url);

		void Update(PatternHostRelations pattern);

		string Validate(ValidationRequest request);

		MappingScheme TryFindPattern(string url, Encoding encoding);

		ParseResult GetContent(ValidationRequest request);

		void Init();

		//void Save(string url, MappingScheme mappingScheme, bool delete, int? encoding, bool? useBrowser);
		void Save(PatternValidateRequest reques, MappingScheme mappingScheme, bool delete);

	}

	public sealed class ValidationRequest
	{
		public string Url { get; set; }
		public Encoding Encoding { get; set; }
		public MappingScheme Map { get; set; }
		public bool UseBrowser { get; set; }
		public string PostData { get; set; }
		public List<AuthData> Auth { get; set; }
		public AuthResult AuthResult { get; set; }
		public string[] Headers { get; set; }
		public List<Cookie> Cookies { get; set; }
		public string Host { get; set; }

	}

	
}
