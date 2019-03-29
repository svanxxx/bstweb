<%@ Page Title="Users" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="users.aspx.cs" Inherits="Users" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<%=System.Web.Optimization.Styles.Render("~/bundles/users_css")%>
	<%=System.Web.Optimization.Scripts.Render("~/bundles/users_js")%>
	<script src="<%=Settings.CurrentSettings.ANGULARCDN.ToString()%>angular.min.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<div ng-app="mpsapplication" ng-controller="mpscontroller">
		<div class="alert alert-danger savebutton btn-group-vertical" ng-cloak ng-show="changed">
			<button type="button" class="btn btn-lg btn-info" ng-click="save()">Save</button>
			<button type="button" class="btn btn-lg btn-danger" ng-click="discard()">Discard</button>
		</div>
		<div class="table-responsive">
			<table class="table table-hover table-bordered">
				<thead class="thead-dark">
					<tr class="info">
						<th>Name</th>
						<th>Phone</th>
						<th>Login</th>
						<th>Password</th>
						<th>Admin</th>
						<th>Guest</th>
						<th>Retired</th>
					</tr>
				</thead>
				<tbody>
					<tr ng-repeat="u in users | orderBy : ['RETIRED','USER_NAME']" class="{{u.changed?'data-changed':''}}">
						<td>
							<input ng-disabled="readonly" class="intable-data-input" type="text" ng-model="u.USER_NAME" ng-change="itemchanged(u)">
						</td>
						<td>
							<input ng-disabled="readonly" class="intable-data-input" type="text" ng-model="u.PHONE" ng-change="itemchanged(u)">
						</td>
						<td>
							<input ng-disabled="readonly" class="intable-data-input" type="text" ng-model="u.LOGIN" ng-change="itemchanged(u)">
						</td>
						<td>
							<input ng-disabled="readonly" class="intable-data-input" type="password" ng-model="u.PASSWORD" ng-change="itemchanged(u)" placeholder="********">
						</td>
						<td align="center">
							<input ng-disabled="readonly" type="checkbox" ng-model="u.ISADMIN" ng-change="itemchanged(u)">
						</td>
						<td align="center">
							<input ng-disabled="readonly" type="checkbox" ng-model="u.ISGUEST" ng-change="itemchanged(u)">
						</td>
						<td align="center">
							<input ng-disabled="readonly" type="checkbox" ng-model="u.RETIRED" ng-change="itemchanged(u)">
						</td>
					</tr>
				</tbody>
			</table>
		</div>
	</div>
</asp:Content>
