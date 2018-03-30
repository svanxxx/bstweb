<%@ Page Title="BST Log" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="log.aspx.cs" Inherits="Log" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<link href="css/log.css" rel="stylesheet" />
	<script src="scripts/log.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<div id="controlpanel" class="panel panel-default">
		<div class="col-sm-1">
			<h5>Machine:</h5>
		</div>
		<div class="col-sm-4">
			<asp:DropDownList ID="thoster" runat="server" class="thosterlist form-control" EnableViewState="false">
			</asp:DropDownList>
		</div>
		<div class="col-sm-1">
			<h5>Date:</h5>
		</div>
		<div class="col-sm-4">
			<input class="form-control" id="logdate" type="date" name="Date">
		</div>
		<div class="col-sm-1">
			<button id="showresults" type="button" class="btn btn-success btn-primary btn-sm"><span class="glyphicon glyphicon-filter"></span>Show Filter Results</button>
		</div>
	</div>
	<asp:Table ID="TTable" runat="server" class="pagetable table table-bordered table-responsive table-striped table-hover" EnableViewState="False">
	</asp:Table>
	<ul class="pagination">
		<li><a href="?page=1">1</a></li>
	</ul>
</asp:Content>
