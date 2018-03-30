<%@ Page Title="Test Runs" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="runs.aspx.cs" Inherits="Runs" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<link href="css/runs.css" rel="stylesheet" />
	<script src="scripts/runs.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<div visible="false" runat="server" id="requestinfo" class="alert alert-danger">
		<strong>Request:</strong> bububu.
	</div>
	<asp:Table ID="TTable" runat="server" class="renderhide pagetable table table-bordered table-responsive" EnableViewState="False">
	</asp:Table>
	<div class="row helpercontainer">
		<div class="col-sm-10">
			<div class="alert alert-info">
				<strong>Info:</strong> Click on the row # cells to select / unselect rows.
			</div>
		</div>
		<div class="col-sm-1">
			<button id="performance" type="button" class="btn btn-danger">Performance</button>
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
			<div class="col-sm-3">
				<select class="form-control" id="showby">
					<option>30</option>
					<option>60</option>
					<option>150</option>
					<option>300</option>
				</select>
			</div>
			<div class="col-sm-3">
				<button id="grouptests" type="button" class="btn btn-info">Group Results</button>
			</div>
		</div>
	</div>
</asp:Content>
