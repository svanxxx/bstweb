<%@ Page Title="Settings" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="settings.aspx.cs" Inherits="settings" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<%=System.Web.Optimization.Styles.Render("~/bundles/settings_css")%>
	<%=System.Web.Optimization.Scripts.Render("~/bundles/settings_js")%>
	<script src="<%=Settings.CurrentSettings.ANGULARCDN.ToString()%>angular.min.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<div ng-app="mpsapplication" ng-controller="mpscontroller">
		<div class="alert alert-danger savebutton btn-group-vertical" ng-cloak ng-show="changed">
			<button type="button" class="btn btn-lg btn-info" ng-click="save()">Save</button>
			<button type="button" class="btn btn-lg btn-danger" ng-click="discard()">Discard</button>
		</div>
		<table class="table table-hover table-bordered">
			<thead>
				<tr class="info">
					<th>Name</th>
					<th>Value</th>
				</tr>
			</thead>
			<tbody>
				<tr ng-repeat="s in settings | orderBy : 's.NAME'" class="{{s.changed?'data-changed':''}}">
					<td>{{s.NAME}}</td>
					<td><input ng-disabled="readonly" class="intable-data-input" type="text" ng-model="s.VALUE" ng-change="itemchanged(s)"></td>
				</tr>
			</tbody>
		</table>
	</div>
</asp:Content>
