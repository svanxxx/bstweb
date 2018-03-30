<%@ Page Title="Test Requests" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="requests.aspx.cs" Inherits="Requests" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<link href="css/requests.css" rel="stylesheet" />
	<script src="scripts/requests.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<asp:Table ID="TTable" runat="server" class="pagetable table table-bordered table-responsive table-striped table-hover" EnableViewState="False">
	</asp:Table>
	<div class="row">
		<div class="col-sm-7">
			<ul class="pagination">
			</ul>
		</div>
		<div class="col-sm-5">
			<div class="col-sm-4">
				<label for="showby">Show by:</label>
			</div>
			<div class="col-sm-4">
				<select class="form-control" id="showby">
					<option>30</option>
					<option>60</option>
					<option>150</option>
					<option>300</option>
				</select>
			</div>
			<div class="col-sm-4">
				<button id="showall" type="button" class="btn btn-info">Show all</button>
			</div>
		</div>
	</div>
</asp:Content>
