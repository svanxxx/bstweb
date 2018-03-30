<%@ Page Title="Compare Files" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="compareimg.aspx.cs" Inherits="Compareimg" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<link rel="stylesheet" href="css/compareimg.css" />
	<script src="scripts/compareimg.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<div class="row">
		<div id="imgleft" class="imgcol col-md-6">
			<button id="getfile1but" type='button' class='panbut btn btn-info btn-xs'></button>
			<button id="draw1but" type='button' class='panbut btn btn-success btn-xs'>Highlight Differences</button>
			<canvas id="canvas1" width="0" height="0"></canvas>
		</div>
		<div id="imgright" class="imgcol col-md-6">
			<button id="getfile2but" type='button' class='panbut btn btn-info btn-xs'></button>
			<button id="draw2but" type='button' class='panbut btn btn-success btn-xs'>Highlight Differences</button>
			<canvas id="canvas2" width="0" height="0"></canvas>
		</div>
	</div>
</asp:Content>
