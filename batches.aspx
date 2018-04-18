<%@ Page Title="Batches" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="batches.aspx.cs" Inherits="Batches" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<link href="css/batches.css" rel="stylesheet" />
	<script src="scripts/batches.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<div class="alert alert-info">
		<button id="addnew" type="button" class="btn">
			<span class="glyphicon glyphicon-plus"></span>
			<strong>Add new batch</strong>
		</button>
		Click on the batch data cell to <strong>EDIT</strong> the value. Click on the # cell to <strong>DELETE</strong> the batch.
	</div>
	<div id="modal" class="modal">
		<div class="modal-content">
			<div class="modal-caption alert alert-success">
				<strong>Warning:</strong> Click close button when your editing finishes and you will be prompted to save or discard
				<button id="closeeditor" type="button" class="btn">
					<span class="glyphicon glyphicon-remove"></span>
				</button>
			</div>
			<textarea id="editor"></textarea>
		</div>
	</div>
	<asp:Table ID="TTable" runat="server" class="pagetable table table-bordered table-responsive table-striped table-hover" EnableViewState="False">
	</asp:Table>
	<div class="row">
		<div class="col-sm-6">
			<ul class="pagination">
			</ul>
		</div>
		<div class="col-sm-6">
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
		</div>
	</div>
</asp:Content>
