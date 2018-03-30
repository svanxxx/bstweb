<%@ Page Title="Activity" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="activity.aspx.cs" Inherits="Activity" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<link href="css/activity.css" rel="stylesheet" />
	<script src="scripts/activity.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<asp:Table ID="TTable" runat="server" class="pagetable table table-bordered table-responsive table-striped table-hover" EnableViewState="False">
	</asp:Table>
	<ul class="pagination">
	</ul>
</asp:Content>
