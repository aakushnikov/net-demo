using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Crawler.Web.Host.Entities;

namespace Crawler.Web.Host
{

	public partial class CheckPatternsWeb : Page
	{
		private static string _apiUrl = "http://localhost:5195/api/v1";
		protected void Page_Load(object sender, EventArgs e)
		{
			_apiUrl = System.Configuration.ConfigurationManager.AppSettings["apiUrl"] ?? _apiUrl;
			//if (!this.Page.IsPostBack)
			//{
			//	this.txtName.Text = Session["txtName"].ToString();
			//	this.txtRegex.Text = Session["txtRegex"].ToString();
			//	this.txtXpath.Text = Session["txtXpath"].ToString();
			//	this.txtUrl.Text = Session["txtUrl"].ToString();

			//}
		}

		protected void Button1_Click(object sender, EventArgs e)
		{
			var request = GetPatternValidateRequest();
			var serializeObject = JsonConvert.SerializeObject(request);
			var result = Helper.GetContent($"{_apiUrl}/htmlparser/Validate", Encoding.UTF8, serializeObject);

			LoadTree(result);
		}

		private void LoadTree(string result)
		{
			var data = JsonConvert.DeserializeObject<dynamic>(result);
			txtResult.Value = JsonConvert.SerializeObject(data, Formatting.Indented);
			var dataJson = new HierarchyDataJson(data, null);
			treeResult.DataSource = dataJson;
			treeResult.DataBind();
		}

		protected void btnParse_OnClick(object sender, EventArgs e)
		{

			var url = txtUrl.Text;
			if (!string.IsNullOrEmpty(url))
			{
				var serializeObject = JsonConvert.SerializeObject(url);
				var result = Helper.GetContent($"{_apiUrl}/htmlparser", Encoding.UTF8, serializeObject);

				LoadTree(result);
			}
		}

		protected void btnLoad_OnClick(object sender, EventArgs e)
		{
			var url = txtUrl.Text;
			if (!string.IsNullOrEmpty(url))
			{
				var serializeObject = JsonConvert.SerializeObject(url);
				var result = Helper.GetContent($"{_apiUrl}/htmlparser/pattern", Encoding.UTF8, serializeObject);

				LoadTree(result);
			}
		}

		protected void btnValidateAndSave_OnClick(object sender, EventArgs e)
		{

			var request = GetPatternValidateRequest();
			var serializeObject = JsonConvert.SerializeObject(request);
			var result = Helper.GetContent($"{_apiUrl}/htmlparser/validate/save", Encoding.UTF8, serializeObject);

			LoadTree(result);

		}

		private PatternValidateRequest GetPatternValidateRequest()
		{
			var patterns = new List<KeyValue>()
			{
			};


			var request = new PatternValidateRequest()
			{
				Url = this.txtUrl.Text,
				UseBrowser = this.chUseBrowser.Checked,
				//AppendImage = true,
				CodePage = string.IsNullOrEmpty(this.txtCodePage.Text) ? null : (int?)int.Parse(this.txtCodePage.Text)
			};
			return request;
		}

		protected void btnValidateAndAdd_OnClick(object sender, EventArgs e)
		{

			var request = GetPatternValidateRequest();
			var serializeObject = JsonConvert.SerializeObject(request);
			var result = Helper.GetContent($"{_apiUrl}/htmlparser/validate/add", Encoding.UTF8, serializeObject);

			LoadTree(result);

		}
	}

}