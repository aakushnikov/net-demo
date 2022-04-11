using System;
using System.Collections.Generic;
using System.Diagnostics;
using Castle.Core.Logging;
using Crawler.Browser.Api.Host.Extensions;
using Nancy;
using Newtonsoft.Json;

namespace Crawler.Browser.Api.Host.Modules
{

	public class ErrorContainer
	{
		public string Request;

		public ErrorContainer(string request)
		{
			Request = request;
			Errors = new List<ErrorMessage>();
		}

		public List<ErrorMessage> Errors;
	}
	public class ErrorMessage
	{
		public ErrorMessage(string code, string message)
		{

			Code = code;
			Message = message;
		}



		public string Code;

		public string Message;
	}
	public interface IModuleConfigurator
	{
		void Configure(NancyModule.RouteBuilder routeBuilder,
			string name,
			Func<dynamic, dynamic> func,
			IResponseFormatter response, Func<Request> getRequest);
	
	}

	public class ModuleConfigurator : IModuleConfigurator
	{
		private readonly ILogger _logger;
		public ModuleConfigurator(ILogger logger)
		{
			_logger = logger;
		}
		
		public void Configure(NancyModule.RouteBuilder routeBuilder, string name, Func<dynamic, dynamic> func, IResponseFormatter response, Func<Request> getRequest)
		{
			ModuleHelper.Configure(routeBuilder, name, func, response, getRequest, _logger, null);
		}
	}

	public static class ModuleHelper
	{
		public static void Configure(NancyModule.RouteBuilder routeBuilder,
			string name,
			Func<dynamic, dynamic> func,
			IResponseFormatter response,
			Func<Request> getRequest,
			ILogger logger)
		{
			Configure(routeBuilder, name, func, response, getRequest, logger, null);
		}


		public static void Configure(NancyModule.RouteBuilder routeBuilder,
			string name,
			Func<dynamic, dynamic> func,
			IResponseFormatter response,
			Func<Request> getRequest,
			ILogger logger,
			Action<dynamic> additionalAction)
		{
			routeBuilder[name] = variable =>
			{
				var watch = Stopwatch.StartNew();

				try
				{
					try
					{
						dynamic result = func.Invoke(variable);
						return result;
					}
					catch (Exception ex)
					{
						throw;
					}
					finally
					{
						var timeSpan = watch.Elapsed;
						if (timeSpan.Milliseconds > 800)
						{
							var request = getRequest.Invoke();
							logger.Info($"Elapsed:{timeSpan} Method:{request.Method}.{request.Url}, {request.UserHostAddress}.");
						}
					}
				}
				catch (AggregateException ae)
				{
					logger.Error(ae.Message, ae);
					return GetErrorResponse(response, getRequest, ae);
				}
				catch (Exception ex)
				{
					logger.Error(ex.Message, ex);
					return GetErrorResponse(response, getRequest, ex);
				}
			};
		}



		private static dynamic GetErrorResponse(IResponseFormatter response, Func<Request> getRequest, Exception ex)
		{
			var request = getRequest.Invoke();
			var requestBody = TryGetBodyAsLocalString(request);

			var container = new ErrorContainer(requestBody);
			var error = new ErrorMessage("Error_" + request.Path, ex.Message);
			container.Errors.Add(error);
			var result = JsonConvert.SerializeObject(container);
			return response.AsText(result, "application/json");
		}

		public static string TryGetBodyAsLocalString(Request request)
		{
			try
			{
				request.Body.Position = 0;
				var str = request.Body.AsStringWithLeaveOpen(true);
				request.Body.Position = 0;
				return str;
			}
			catch (ObjectDisposedException)
			{
			}
			return string.Empty;
		}

		private static dynamic GetErrorResponse(IResponseFormatter response, Func<Request> getRequest, AggregateException aex)
		{
			var request = getRequest.Invoke();
			var requestBody = TryGetBodyAsLocalString(request);

			var container = new ErrorContainer(requestBody);
			if (aex.InnerExceptions != null)
			{
				foreach (var ex in aex.InnerExceptions)
				{
					var error = new ErrorMessage("Error_" + request.Path, ex.Message);
					container.Errors.Add(error);
				}
			}

			var result = JsonConvert.SerializeObject(container);
			return response.AsText(result, "application/json");
		}

		
	}
};