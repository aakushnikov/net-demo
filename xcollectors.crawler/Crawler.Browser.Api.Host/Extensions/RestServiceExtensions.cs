using System;
using System.IO;
using System.Text;
using Nancy.IO;
using Newtonsoft.Json;

namespace Crawler.Browser.Api.Host.Extensions
{
	public sealed class IncorrectRequestException : Exception
	{
		private const string RestCode = "IncorrectRequest";

		public IncorrectRequestException(string message)
			: base(message)
		{
		}
	}

	public static class RestServiceExtensions
	{
		public static string GetAssemblyVersion()
		{
			return typeof(RestServiceExtensions).Assembly.GetName().Version.ToString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="leaveOpen">leaveOpen = false => close stream</param>
		/// <returns></returns>
		public static string AsStringWithLeaveOpen(this RequestStream stream, bool leaveOpen = false)
		{
			using (var streamReader = new StreamReader(stream, Encoding.UTF8, false, 1024, leaveOpen))
				return streamReader.ReadToEnd();

		}


		private static readonly JsonSerializer JsonSerializer = new JsonSerializer();

		/// <summary>
		/// Deserialize json and close stream
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="stream"></param>
		/// <param name="leaveOpen">leaveOpen = false => close stream</param>
		/// <returns></returns>
		public static T DeserializeObject<T>(this RequestStream stream, bool leaveOpen = false) where T : class
		{
			using (var streamReader = new StreamReader(stream, Encoding.UTF8, false))
			{
				using (var jsonTextReader = new JsonTextReader(streamReader))
				{
					var model = JsonSerializer.Deserialize<T>(jsonTextReader);

					if (model == default(T))
						throw new IncorrectRequestException("Unknown input request format");

					return model;
				}
			}
		}

		public static T DeserializeObject<T>(this string body) where T : class
		{
			if (string.IsNullOrEmpty(body))
				throw new IncorrectRequestException("Unknown input request format");

			var model = JsonConvert.DeserializeObject<T>(body);

			if (model == default(T))
				throw new IncorrectRequestException("Unknown input request format");

			return model;
		}
	}
}