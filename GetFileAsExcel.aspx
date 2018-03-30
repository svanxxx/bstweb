<%@ Page Title="Show File As Excel" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="GetFileAsExcel.aspx.cs" Inherits="GetFileAsExcel" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<link href="css/getfileasexcel.css" rel="stylesheet" />
	<script src="scripts/getfileasexcel.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
	<asp:Label ID="ErrorMessage" runat="server" Visible="false" />
	<asp:Label ID="filetoshow" runat="server" />
	<hr>
	<div id="filecontainer"></div>
</asp:Content>
