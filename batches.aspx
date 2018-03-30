<%@ Page Title="Batches" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="batches.aspx.cs" Inherits="Batches" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<link href="css/batches.css" rel="stylesheet" />
	<script src="scripts/batches.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<div id="modal" class="modal">
		<div class="modal-content">
			<span class="close">&times;</span>
			<textarea></textarea>
		</div>
	</div>
	<asp:Table ID="TTable" runat="server" class="pagetable table table-bordered table-responsive table-striped table-hover" EnableViewState="False">
	</asp:Table>
	<ul class="pagination">
	</ul>
</asp:Content>
