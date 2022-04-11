<%@ Page Title="Patterns" Language="C#" MasterPageFile="~/Site.Master"
    ValidateRequest="false"
    AutoEventWireup="true" CodeBehind="PatternsWeb.aspx.cs"
    Inherits="Crawler.Web.Host.PatternsWeb" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Patterns</h2>
    <br>

    <div class="container-fluid">
        <div class="row">
            <div class="form-group col-xs-12">
                <label for="MainContent_txtUrl">Url for Parse</label>
                <div class="input-group">
                    <asp:TextBox runat="server" ID="txtUrl" CssClass="form-control input-lg"></asp:TextBox>
                    <span class="input-group-btn">
                        <asp:Button ID="btnParse" runat="server" Text="Parse" OnClick="btnParse_OnClick" CssClass="btn btn-success btn-lg" />
                    </span>
                </div>
            </div>
            <div class="form-group col-xs-12">
                <label for="MainContent_postData">PostData <span class="text-muted">(param1=val1&amp;param2=val2...)</span></label>
                <asp:TextBox runat="server" ID="txtPostData" CssClass="form-control input-lg"></asp:TextBox>
            </div>
        </div>
        <div class="row">
            <div class="form-group col-xs-2">
                <label for="MainContent_txtCodePage">CodePage</label>
                <asp:TextBox runat="server" ID="txtCodePage" CssClass="form-control" placeholder="Auto"></asp:TextBox>
            </div>
            <div class="form-group col-xs-2">
                <br />
                <div class="checkbox">
                    <label>
                        <asp:CheckBox runat="server" ID="chUseBrowser"></asp:CheckBox>
                        Use Browser
                    </label>
                </div>
            </div>
            <div class="form-group col-xs-8">
                <label for="MainContent_txtCleareContent">Clear Content with RegEx</label>
                <asp:TextBox runat="server" ID="txtCleareContent" CssClass="form-control"></asp:TextBox>
            </div>
        </div>
    </div>
    <div class="container">
        <div class="row">
            <ul class="nav nav-tabs" role="tablist">
                <li role="presentation" class="active"><a href="#table" aria-controls="table" role="tab" data-toggle="tab">as Table</a></li>
                <li role="presentation"><a href="#json" aria-controls="json" role="tab" data-toggle="tab">as JSON</a></li>
            </ul>
            <div class="tab-content">
                <div role="tabpanel" class="tab-pane active" id="table">
                    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="false" BorderWidth="1px" BackColor="White"
                        OnRowEditing="GridView1_OnRowEditing"  ViewStateMode="Enabled"
                        DataKeyNames="Name"
                        OnRowCancelingEdit="GridView1_OnRowCancelingEdit"
                        OnRowUpdated="GridView1_OnRowUpdated" OnRowUpdating="GridView1_OnRowUpdating"
                        CellPadding="4" BorderStyle="None" BorderColor="#3366CC"
                                  Width="100%">
                        <FooterStyle ForeColor="#003399"
                            BackColor="#99CCCC"></FooterStyle>
                        <PagerStyle ForeColor="#003399" HorizontalAlign="Left"
                            BackColor="#99CCCC"></PagerStyle>
                        <HeaderStyle ForeColor="#CCCCFF" Font-Bold="True"
                            BackColor="#003399"></HeaderStyle>

                        <Columns>
                            <asp:CommandField ShowEditButton="True"></asp:CommandField>
                            <asp:BoundField HeaderText="Name" DataField="Name" ReadOnly="True"/>
                            <asp:TemplateField HeaderText="Type">
                                <ItemTemplate>
                                    <asp:Label ID="lbltype" runat="server" Text='<%#Eval("Type") %>'/>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="Type" width="100%"  runat="server" Text='<%#Eval("Type") %>'/>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="inType" runat="server"/>
                                    <asp:RequiredFieldValidator ID="vType" runat="server"
                                         ControlToValidate="inType" Text="?" ValidationGroup="validaiton"/>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Value">
                                <ItemTemplate>
                                    <asp:Label ID="lblValue" runat="server" Text='<%#Eval("Value") %>'/>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="Value" width="100%"  runat="server" Text='<%#Eval("Value") %>'/>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="inValue" runat="server"/>
                                    <asp:RequiredFieldValidator ID="vValue" runat="server"
                                         ControlToValidate="inValue" Text="?" ValidationGroup="validaiton"/>
                                </FooterTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <SelectedRowStyle ForeColor="#CCFF99" Font-Bold="True"
                            BackColor="#009999"></SelectedRowStyle>
                        <RowStyle ForeColor="#003399" BackColor="White"></RowStyle>
                    </asp:GridView>
                </div>
                <div role="tabpanel" class="tab-pane" id="json">
                    <div class="row">
                        <div class="col-xs-12">
                            <textarea id="txtPatterns" runat="server" style="height: 800px" class="form-control" />
                            <div id="txtPatterns"></div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="row" style="margin-top: 20px">
            <asp:Button ID="btnValidate" runat="server" Text="Validate" OnClick="btnValidate_Click" CssClass="btn btn-primary pull-right btn-lg" style="width: 220px" />

            <asp:Button ID="btnValidateAndSave" runat="server" Text="Validate and Save" OnClick="btnValidateAndSave_OnClick" CssClass="btn btn-success btn-lg" style="width: 220px" />
        </div>
    </div>
    <div class="container-fluid" style="margin-top: 20px">
        <div class="row">
            <div class="col-xs-12">
                <label>Parse result</label>
                <div id="parseResult" class="form-control"><%= HttpUtility.HtmlEncode(ParseResult) %></div>
            </div>
        </div>
    </div>
</asp:Content>
