<%@ Page Title="Commands" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="commands.aspx.cs" Inherits="Commands" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<link href="css/commands.css" rel="stylesheet" />
	<script src="scripts/commands.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<button id="savebutton" type="submit" class="btn btn-primary"><span class='glyphicon glyphicon-save'></span>Save</button>
	<textarea class="form-control" id="commandstext" runat="server" rows="35"></textarea>
</asp:Content>
