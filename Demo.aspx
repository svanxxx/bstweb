<%@ Page Title="Demo Databases List" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Demo.aspx.cs" Inherits="Demo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
	<td>
	<asp:Panel ID="PagesPanel" runat="server" HorizontalAlign="Center">
	</asp:Panel>
	<asp:Table ID="DemoTable" runat="server" Width="100%">
		<asp:TableRow runat="server">
			<asp:TableCell runat="server" BackColor="#66FF33" Width="25%">Version</asp:TableCell>
			<asp:TableCell runat="server" BackColor="#66FF33" Width="25%">SQL</asp:TableCell>
			<asp:TableCell runat="server" BackColor="#66FF33" Width="25%">Oracle</asp:TableCell>
		</asp:TableRow>
</asp:Table>
	</td>
</asp:Content>

