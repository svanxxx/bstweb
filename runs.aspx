<%@ Page Title="Test Runs" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="runs.aspx.cs" Inherits="Runs" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<link href="css/runs.css" rel="stylesheet" />
	<script src="scripts/runs.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<div class="dropdown">
		<button class="btn btn-xs btn-primary dropdown-toggle" type="button" data-toggle="dropdown">
			Select Filters And Actions
			<span class="caret"></span>
		</button>
		<ul class="dropdown-menu">
			<li><a href="#" id="grouptests"><span class="glyphicon glyphicon-filter"></span>&nbsp; <span id="grouptestslabel">Group Results (Now UNGrouped!)</span></a></li>
			<li class="dropdown-submenu">
				<a class="menuchild" tabindex="-1" href="#"><span class="glyphicon glyphicon-filter"></span>Verified Filter<span class="caret"></span></a>
				<ul class="dropdown-menu">
					<li><a href="#" id="verified_yes">&nbsp; Show Only Verified</a></li>
					<li><a href="#" id="verified_no">&nbsp; Show Only NOT Verified</a></li>
					<li><a href="#" id="verified_all">&nbsp; Show ALL</a></li>
				</ul>
			</li>
			<li><a href="#" id="performance"><span class="glyphicon glyphicon-signal"></span>&nbsp; Show Performance Graph</a></li>
		</ul>
	</div>
	<div visible="false" runat="server" id="requestinfo" class="alert alert-danger">
		<strong>Request:</strong> bububu.
	</div>
	<asp:Table ID="TTable" runat="server" class="renderhide pagetable table table-bordered table-responsive" EnableViewState="False">
	</asp:Table>
	<div class="row helpercontainer">
		<div class="col-sm-11">
			<div class="alert alert-info">
				<strong>Info:</strong> Click on the row # cells to select / unselect rows.
			</div>
		</div>
	</div>
	<div id="testactions">
		<button id="ignorebutton" type="button" class="btn btn-warning btn-block">Ignore</button>
		<button id="verifybutton" type="button" class="btn btn-warning btn-block">Verify</button>
		<button id="commentbutton" type="button" class="btn btn-warning btn-block">Comment</button>
	</div>
	<div class="row">
		<div class="col-sm-7">
			<ul class="pagination">
			</ul>
		</div>
		<div class="col-sm-5">
			<div class="col-sm-3">
				<label for="showby">Show by:</label>
			</div>
			<div class="col-sm-6">
				<select class="form-control" id="showby">
					<option>30</option>
					<option>60</option>
					<option>150</option>
					<option>300</option>
				</select>
			</div>
		</div>
	</div>
</asp:Content>
