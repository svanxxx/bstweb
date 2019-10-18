<%@ Page Title="Test Requests" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="testrequests.aspx.cs" Inherits="TestRequests" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<%=System.Web.Optimization.Styles.Render("~/bundles/testrequests_css")%>
	<%=System.Web.Optimization.Scripts.Render("~/bundles/testrequests_js")%>
	<script src="<%=Settings.CurrentSettings.ANGULARCDN.ToString()%>angular.min.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<div ng-app="mpsapplication" ng-controller="mpscontroller">
		<table class="table table-bordered">
			<thead>
				<tr>
					<th>TTID</th>
				</tr>
			</thead>
			<tbody>
				<tr ng-repeat="r in requests">
					<td>{{r.TTID}}</td>
				</tr>
			</tbody>
		</table>
	</div>
</asp:Content>
