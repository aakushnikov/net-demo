<%@ Page Title="CheckPatterns" Language="C#" MasterPageFile="~/Site.Master" 
    ValidateRequest="false"
    AutoEventWireup="true" CodeBehind="CheckPatternsWeb.aspx.cs"
    Inherits="Crawler.Web.Host.CheckPatternsWeb" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Patterns</h2>
    <br><br><br><br>
    <asp:Table ID="TableMain" runat="server" Width="80%">
        <asp:TableRow runat="server" Width="100%">
            <asp:TableCell runat="server" Height="20%">
                <asp:Table ID="Table1" runat="server">
                    <asp:TableRow runat="server">
                        <asp:TableCell runat="server">
                            <asp:Label ID="Label1" runat="server" Text="Url:"></asp:Label>
                        </asp:TableCell>
                        <asp:TableCell runat="server" Width="80%">
                            <asp:TextBox runat="server" ID="txtUrl" Width="540px"></asp:TextBox>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow runat="server">
                        <asp:TableCell runat="server">
                            <asp:Label ID="Label2" runat="server" Text="CodePage:"></asp:Label>
                        </asp:TableCell>
                        <asp:TableCell runat="server" Width="80%">
                            <asp:TextBox runat="server" ID="txtCodePage" Width="540px"></asp:TextBox>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow runat="server">
                        <asp:TableCell runat="server">
                            <asp:Label ID="Label3" runat="server" Text="Use Broser:"></asp:Label>
                        </asp:TableCell>
                        <asp:TableCell runat="server" Width="80%">
                            <asp:CheckBox runat="server" ID="chUseBrowser" Width="540px"></asp:CheckBox>
                        </asp:TableCell>
                    </asp:TableRow>
                    
                </asp:Table>
            </asp:TableCell>
        </asp:TableRow>

        <asp:TableRow runat="server" Height="50%">
            <asp:TableCell runat="server">
                <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0">

                    <asp:View ID="Validate" runat="server" ViewStateMode="Enabled">
                        <h3>Validation</h3>                        
                        
                        <asp:Button ID="btnParse" runat="server" Text="Parse" OnClick="btnParse_OnClick"  Width="220px"  />
                        <br>
                        <asp:Button ID="btnLoad" runat="server" Text="LoadPattern" OnClick="btnLoad_OnClick"  Width="220px" />
                        
                    </asp:View>
                    
                    <asp:View ID="View2" runat="server"></asp:View>
                </asp:MultiView>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow runat="server">
            <asp:TableCell runat="server">
                <input id="txtResult" runat="server" type="text" style="width: 100%; height: 220px" />
            </asp:TableCell>
        </asp:TableRow>
        
        <asp:TableRow runat="server">
            <asp:TableCell runat="server">
                <%--<asp:GridView ID="GridView1" runat="server"  Width="50%"></asp:GridView>--%>
                <asp:TreeView ID="treeResult" runat="server" Width="50%"></asp:TreeView>
                <%--<asp:TextBox ID="txtResult" runat="server" Height="220px" Width="100%"></asp:TextBox>--%>
            </asp:TableCell>
        </asp:TableRow>
        
    </asp:Table>



</asp:Content>
