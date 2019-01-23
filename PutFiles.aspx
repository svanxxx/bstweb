<%@ Page Title="Push Files" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="PutFiles.aspx.cs" Inherits="PutFiles" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<%=System.Web.Optimization.Styles.Render("~/bundles/putfiles_css")%>
	<%=System.Web.Optimization.Scripts.Render("~/bundles/putfiles_js")%>
	<script src="<%=Settings.CurrentSettings.ANGULARCDN.ToString()%>angular.min.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<div ng-app="mpsapplication" ng-controller="mpscontroller">
		<div class="row">
			<h3>Push Files: new etalons</h3>
			<div class="row">
				<div class="col-sm-10">
					<b>Branch: </b><span>{{branch}}</span><br />
					<b>User Name: </b><span>{{user}}</span><br />
					<label for="comment">Comment:</label>
				</div>
				<div class="col-sm-2">
					<button type="button" ng-click="commit()" ng-disabled="readonly()" class="btn btn-primary">Commit & Push</button>
				</div>
			</div>
			<textarea ng-disabled="readonly()" class="form-control" rows="5" id="comment" ng-model="comments"></textarea>
		</div>
		<div class="row" ng-show="output.length>0">
			<h3>Output:</h3>
			<pre><code ng-bind-html="output | rawHtml"></code></pre>
		</div>
		<div class="row">
			<ul ng-disabled="readonly()" class="list-group list-group-flush">
				<li class="list-group-item" ng-repeat="f in files" style="background-color:{{f.checked?'transparent':'lightgray'}}">
					<button ng-disabled="readonly()" type="button" class="btn btn-success btn-xs" ng-click="f.checked = !f.checked;">
						<span class="glyphicon glyphicon-check" ng-show="f.checked"></span>
						<span class="glyphicon glyphicon-unchecked" ng-show="!f.checked"></span>
					</button>
					<b>{{f.name}}</b>
					<a class="btn btn-success btn-xs" href="merge.aspx?{{f.ETA}}&{{f.NEW}}" target="_blank">Compare</a>
					<br />
					<span class="label label-info">New File:</span><span style="word-wrap: break-word">{{f.NEW}}</span><br />
					<span class="label label-info">Old File:</span><span style="word-wrap: break-word">{{f.ETA}}</span><br />
					<span class="label label-info">BST:</span><span style="word-wrap: break-word">{{f.BST}}</span>
				</li>
			</ul>
		</div>
	</div>
</asp:Content>
