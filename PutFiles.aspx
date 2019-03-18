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
				<div class="col-sm-6">
					<b>Branch: </b><span>{{branch}}</span><br />
					<b>User Name: </b><span>{{user}}</span><br />
					<label for="comment">Comment:</label>
				</div>
				<div class="col-sm-4">
					<div class="btn-group btn-group-sm">
						<button ng-click="check(true)" ng-disabled="readonly()" type="button" class="btn btn btn-default"><i class="glyphicon glyphicon-ok"></i>Check All</button>
						<button ng-click="check(false)" ng-disabled="readonly()" type="button" class="btn btn btn-default"><i class="glyphicon glyphicon-remove"></i>UnCheck All</button>
					</div>
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
				<li class="list-group-item" ng-repeat="f in files" style="background-color: {{f.checked?'transparent':'lightgray'}}">
					<div class="input-group">
						<div class="input-group-btn">
							<button ng-click="f.checked = !f.checked;" ng-disabled="readonly()" class="btn btn-default" type="button">
								<i class="glyphicon glyphicon-check" ng-show="f.checked"></i>
								<i class="glyphicon glyphicon-unchecked" ng-show="!f.checked"></i>
							</button>
						</div>
						<span class="form-control">{{f.name}}</span>
						<span data-toggle="tooltip" title="{{f.BST}}" class="input-group-addon">BST</span>
						<a class="btn btn-default btn-xs input-group-addon" href="getfile.aspx?Path={{f.ETA}}" target="_blank">Old File</a>
						<a class="btn btn-default btn-xs input-group-addon" href="getfile.aspx?Path={{f.NEW}}" target="_blank">New File</a>
						<a class="btn btn-default btn-xs input-group-addon" href="merge.aspx?{{f.ETA}}&{{f.NEW}}" target="_blank">Compare</a>
					</div>
				</li>
			</ul>
		</div>
	</div>
</asp:Content>
