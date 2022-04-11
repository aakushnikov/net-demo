using System;
using System.IO;
using CefSharp;
using CefSharp.Callback;

namespace Crawler.Browser.Common.Cef
{
	internal class CefTgSchemeHandler : IResourceHandler
	{
		public void Dispose()
		{

		}

		public bool Open(IRequest request, out bool handleRequest, ICallback callback)
		{
			handleRequest = false;

			return true;
		}

		public bool ProcessRequest(IRequest request, ICallback callback)
		{
			throw new NotImplementedException();
		}

		public void GetResponseHeaders(IResponse response, out long responseLength, out string redirectUrl)
		{
			throw new NotImplementedException();
		}

		public bool Skip(long bytesToSkip, out long bytesSkipped, IResourceSkipCallback callback)
		{
			throw new NotImplementedException();
		}

		public bool Read(Stream dataOut, out int bytesRead, IResourceReadCallback callback)
		{
			throw new NotImplementedException();
		}

		public bool ReadResponse(Stream dataOut, out int bytesRead, ICallback callback)
		{
			throw new NotImplementedException();
		}

		public void Cancel()
		{

		}
	}
}