<%@ Page Title="Versions Summary" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="vsummary.aspx.cs" Inherits="VSummary" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<link href="css/vsummary.css" rel="stylesheet" />
	<script src="scripts/vsummary.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<asp:Table ID="TTable" runat="server" class="pagetable table table-bordered table-responsive table-striped table-hover" EnableViewState="False">
	</asp:Table>
	<div class="alert alert-info">
		<strong>Info:</strong> Click on the line to see full list of results for the version.
	</div>
	<ul class="pagination">
	</ul>
</asp:Content>
