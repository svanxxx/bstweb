<%@ Page Title="Hosts" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="hosts.aspx.cs" Inherits="Hosts" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<link href="css/hosts.css" rel="stylesheet" />
	<script src="scripts/hosts.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
	<table id="hosts" class="pagetable table table-bordered table-responsive">
		<thead>
		</thead>
		<tbody>
		</tbody>
	</table>
  <div class="alert alert-warning">
    <strong>Warning!</strong> In case of problem with LAN adapter "IP" column will be displayed in yellow color.
  </div>
	<button id="showlog" type="button" class="btn btn-info" data-toggle="collapse" data-target="#log">Log...</button>
	<textarea readonly class="collapse form-control" rows="5" id="log" style="font-family: monospace;">
				Log data will appear soon automatically...
	</textarea>
</asp:Content>
