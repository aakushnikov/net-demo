using System;
using System.Xml;
using System.Xml.Linq;
using Castle.Core.Internal;
using SettingsBase = Service.Common.Configurations.SettingsBase;

namespace Crawler.Browser.Common.Cef
{
	public sealed class CefInstanceSettings : SettingsBase, ICefInstanceSettings
	{
		public bool JavascriptOpenWindows { get; private set; }
		public bool Javascript { get; private set; }
		public bool ImageLoading { get; private set; }
		public bool JavascriptDomPaste { get; private set; }
		public bool JavascriptCloseWindows { get; private set; }
		public int BrowserWidth { get; private set; }
		public int BrowserHeight { get; private set; }
		public int DisableGpu { get; private set; }
		public int MaxInstanceLiveTime { get; private set; }
		public bool WebGl { get; private set; }

		protected override string SectionName => "CefInstanceSettings";

		public CefInstanceSettings()
		{
			LoadSettings(GetElement());
		}

		public CefInstanceSettings(XElement raw)
		{
			LoadSettings(raw);
		}

		private void LoadSettings(XElement raw)
		{
			JavascriptOpenWindows = XmlConvert.ToBoolean(raw.Attribute("javascriptOpenWindows")?.Value ?? "false");
			Javascript = XmlConvert.ToBoolean(raw.Attribute("javascript")?.Value ?? "true");
			ImageLoading = XmlConvert.ToBoolean(raw.Attribute("imageLoading")?.Value ?? "false");
			JavascriptDomPaste = XmlConvert.ToBoolean(raw.Attribute("javascriptDomPaste")?.Value ?? "false");
			JavascriptCloseWindows = XmlConvert.ToBoolean(raw.Attribute("javascriptCloseWindows")?.Value ?? "false");
			WebGl = XmlConvert.ToBoolean(raw.Attribute("webGl")?.Value ?? "false");
			BrowserWidth = XmlConvert.ToInt32(raw.Attribute("browserWidth")?.Value ?? "1366");
			BrowserHeight = XmlConvert.ToInt32(raw.Attribute("browserHeight")?.Value ?? "3072");
			DisableGpu = XmlConvert.ToInt32(raw.Attribute("disableGpu")?.Value ?? "1");
			MaxInstanceLiveTime = XmlConvert.ToInt32(raw.Attribute("maxInstanceLiveTime")?.Value ?? "600");

			if (MaxInstanceLiveTime < 10)
			{
				throw new InvalidOperationException("MaxInstanceLiveTime value is too low.");
			}
		}
	}
}