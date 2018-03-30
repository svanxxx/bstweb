<%@ Page Title="Machines management" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="machines.aspx.cs" Inherits="Machines" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<link href="css/machines.css" rel="stylesheet" />
	<script src="scripts/machines.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
	<table id="machines" class="pagetable table table-bordered table-responsive">
		<thead>
		</thead>
		<tbody>
		</tbody>
	</table>
	<button id="showlog" type="button" class="btn btn-info" data-toggle="collapse" data-target="#log">Log...</button>
	<textarea readonly class="collapse form-control" rows="5" id="log" style="font-family: monospace;">
				Log data will appear soon automatically...
	</textarea>
</asp:Content>
