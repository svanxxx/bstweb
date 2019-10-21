<%@ Page Title="Compare Files" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="Compare.aspx.cs" Inherits="Compare" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<script src="scripts/diff_match_patch.js"></script>
	<%=System.Web.Optimization.Styles.Render("~/bundles/compare_css")%>
	<%=System.Web.Optimization.Scripts.Render("~/bundles/compare_js")%>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<div id="search_differences">
		<div onclick="link_up()">
			<img src="images/arrow_up.png" width="32" height="32" alt="Previous Error" />
		</div>
		<div onclick="link_down()">
			<img src="images/arrow_down.png" width="32" height="32" alt="Next Error" />
		</div>
	</div>
	<a href="#" target="_blank" class="filebutton btn btn-info btn-xs col-lg-12" id="fslbl1"></a>
	<a href="#" target="_blank" class="filebutton btn btn-info btn-xs col-lg-12" id="fslbl2"></a>
	<div class="row">
		<div class="col-sm-6">
			<span class="content" id="fspan1">File 1:</span>
		</div>
		<div class="col-sm-6">
			<span class="content" id="fspan2">File 2:</span>
		</div>
	</div>
</asp:Content>
