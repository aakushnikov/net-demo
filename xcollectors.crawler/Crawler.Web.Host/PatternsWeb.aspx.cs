using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Crawler.Web.Host.Entities;

namespace Crawler.Web.Host
{

	public partial class PatternsWeb : Page
	{
		public string ParseResult { get; set; }

		private static string _apiUrl = "http://localhost:5195/api/v1";

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				var list = GetPatternLocalsFromPost();
				if (list != null)
				{
					BindPatterns(list);
				}
				else
				{
					list = GetPatternLocals();
					BindPatterns(list);
				}
			}

			_apiUrl = ConfigurationManager.AppSettings["apiUrl"] ?? _apiUrl;
		}

		private MappingScheme GetPatternLocalsFromPost()
		{
			if (txtPatterns == null || string.IsNullOrEmpty(txtPatterns.Value))
				return null;

			return JsonConvert.DeserializeObject<MappingScheme>(txtPatterns.Value);
		}

		private static MappingScheme GetPatternLocals()
		{
			var names = Enum.GetNames(typeof(KeyNames));
			var list = names.Select(name => new KeyValue(ActionType.Xpath)
				{
					Name = name,
					Value = string.Empty
				})
				.ToList();
			return new MappingScheme
			{
				Matches = list,
				CleanMatches = new List<KeyValue>()
			};
		}

		protected void btnValidate_Click(object sender, EventArgs e)
		{
			var request = GetPatternValidateRequest();
			var serializeObject = JsonConvert.SerializeObject(request);
			var result = Helper.GetContent($"{_apiUrl}/htmlparser/Validate", Encoding.UTF8, serializeObject);

			BindPatterns(GetPatternLocalsFromPost());
			LoadTree(result);
		}

		private void LoadTree(string result)
		{
			var data = JsonConvert.DeserializeObject<dynamic>(result);
			ParseResult = JsonConvert.SerializeObject(data, Formatting.Indented);
//			var dataJson = new HierarchyDataJson(data, null);
//			treeResult.DataSource = dataJson;
//			treeResult.DataBind();
		}

		protected void btnParse_OnClick(object sender, EventArgs e)
		{

			var url = txtUrl.Text;
			if (string.IsNullOrEmpty(url))
				return;

			LoadGrid(url);

			var postRequest = string.IsNullOrEmpty(txtPostData?.Text) ? string.Empty : "/post";
			string serializeObject;
			if (postRequest == string.Empty)
				serializeObject = JsonConvert.SerializeObject(url);
			else
				serializeObject = JsonConvert.SerializeObject(new {Url = url, PostData = txtPostData.Text});

			var result = Helper.GetContent($"{_apiUrl}/htmlparser{postRequest}", Encoding.UTF8, serializeObject);

			LoadTree(result);
		}

		private void LoadGrid(string url)
		{
			var map = GetListOfPatterns(url);

			BindPatterns(map);
		}

		private MappingScheme GetListOfPatterns(string url)
		{
			if (!string.IsNullOrEmpty(url))
			{
				var serializeObject = JsonConvert.SerializeObject(url);
				var result = Helper.GetContent($"{_apiUrl}/htmlparser/pattern", Encoding.UTF8, serializeObject);
				LoadTree(result);
				var data = JsonConvert.DeserializeObject<MappingScheme>(result);
				if (data.Matches != null)
					return data;
			}
			return GetPatternLocals();
		}

		private void BindPatterns(MappingScheme map)
		{
			var list = map.Matches.Children().ToList();
			var additional = GetPatternLocals();
			if (additional.Matches != null)
				list.AddRange(additional.Matches.Where(x => x != null && list.All(s => s.Name != x.Name)));

			GridView1.DataSource = list;
//			Session["DataGrid"] = map;
			if (txtPatterns != null)
			{
				var ms = new MappingScheme
				{
					Matches = new List<KeyValue>(),
					CleanMatches = map.CleanMatches?.Count > 0 ? map.CleanMatches : null,
				};
				ms.Matches.AddRange(map.Matches.Where(_ => _.Values != null || !string.IsNullOrEmpty(_.Value)));
				txtPatterns.Value = JsonConvert.SerializeObject(ms);
			}

			GridView1.DataBind();
		}

		protected void btnValidateAndSave_OnClick(object sender, EventArgs e)
		{
			var request = GetPatternValidateRequest();
			if (request.Patterns.Any())
			{
				var serializeObject = JsonConvert.SerializeObject(request);
				var result = Helper.GetContent($"{_apiUrl}/htmlparser/validate/save", Encoding.UTF8, serializeObject);
				LoadTree(result);
			}
		}

		private PatternValidateRequest GetPatternValidateRequest()
		{
			var list = GetPatternLocalsFromPost();
			if (list == null)
				return new PatternValidateRequest();
			var request = new PatternValidateRequest()
			{
				Patterns = list.Matches?.Where(x => !string.IsNullOrEmpty(x.Value)).ToList(),
				CleanMatches = list.CleanMatches?.Where( x => !string.IsNullOrEmpty(x.Value)).ToList(),
				Url = this.txtUrl.Text,
				UseBrowser = this.chUseBrowser.Checked,
				//AppendImage = true,
				CodePage = string.IsNullOrEmpty(this.txtCodePage.Text) ? null : (int?)int.Parse(this.txtCodePage.Text),
				PostData = string.IsNullOrEmpty(txtPostData?.Text) ? null : txtPostData.Text,
			};
			if (!string.IsNullOrEmpty(txtCleareContent.Text))
			{
				request.CleanMatches = new List<KeyValue>
				{
					new KeyValue(ActionType.Regex)
					{
						Name = "Clear",
						Value = txtCleareContent.Text
					}
				};
			}
			return request;
		}

		protected void GridView1_OnRowEditing(object sender, GridViewEditEventArgs e)
		{
			Console.WriteLine("GridView1_OnRowEditing");
			GridView1.EditIndex = e.NewEditIndex;
			BindPatterns(GetPatternLocalsFromPost());
		}

		protected void GridView1_OnRowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
		{
			GridView1.EditIndex = -1;
			LoadGrid(txtUrl.Text);
		}

		protected void GridView1_OnRowUpdated(object sender, GridViewUpdatedEventArgs e)
		{
			GridView1.EditIndex = -1;
		}

		protected void GridView1_OnRowUpdating(object sender, GridViewUpdateEventArgs e)
		{
			var name = GridView1.DataKeys[e.RowIndex].Values["Name"].ToString();

			var typeTextBox = (TextBox) GridView1.Rows[e.RowIndex].FindControl("Type");

			var storValue = (TextBox)GridView1.Rows[e.RowIndex].FindControl("Value");
			var regexValue = (TextBox)GridView1.Rows[e.RowIndex].FindControl("Regex");
			if (storValue == null || e.RowIndex < 0 || string.IsNullOrEmpty(typeTextBox?.Text))
				return;
			var type = (ActionType)Enum.Parse(typeof(ActionType), typeTextBox.Text);

			var mappingScheme = GetPatternLocalsFromPost();
			if (mappingScheme == null)
				mappingScheme = GetListOfPatterns(txtUrl.Text);

			if (mappingScheme.Matches != null)
			{
				var keyValues = mappingScheme.Matches.Children();

				bool isNew = true;
				if (keyValues.Count >= e.RowIndex + 1)
				{
					for (int i = 0; i < e.RowIndex + 1; i++)
					{
						var item = keyValues[i];
						if (item != null && item.Name == name)
						{
							item.Value = storValue.Text;
							isNew = false;
							AddChildRegex(item, name, regexValue);

						}
					}
				}
				if (isNew)
				{
					var item = new KeyValue(type)
					{
						Name = name,
						Value = storValue.Text
					};

					mappingScheme.Matches.Add(item);
					AddChildRegex(item, name, regexValue);
				}
			}

			GridView1.EditIndex = -1;
			BindPatterns(mappingScheme);
		}

		private static void AddChildRegex(KeyValue item, string name, TextBox regexValue)
		{
			if (item.Values != null && item.Values.Any())
			{
				AddRegex(regexValue, item, name);
			}
			else
			{
				switch (name)
				{
					//case "AuthorUrl":
					//	if (item.Values == null)
					//		item.Values = new List<KeyValue>();
					//item.Values.Add(MappingSchemeHelper.GetAuhtorUrl());
					//break;
					
					default:
						AddRegex(regexValue, item, name);
						break;
				}
			}
		}

		private static void AddRegex(TextBox regexValue, KeyValue item, string name)
		{
			if (!string.IsNullOrEmpty(regexValue?.Text))
			{
				if (item.Values == null)
					item.Values = new List<KeyValue>();
				var va = item.Values.FirstOrDefault();
				if (va == null)
				{
					va = new KeyValue(ActionType.Regex)
					{
						Name = name,
						Value = regexValue.Text,
					};
					item.Values.Add(va);
				}
				else
				{
					va.Value = regexValue.Text;
				}
			}
		}
	}
}