<%@ Page Title="Machines" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="machines2.aspx.cs" Inherits="Machines" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<%=System.Web.Optimization.Styles.Render("~/bundles/machines2_css")%>
	<%=System.Web.Optimization.Scripts.Render("~/bundles/machines2_js")%>
	<script src="<%=Settings.CurrentSettings.ANGULARCDN.ToString()%>angular.min.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<div ng-app="mpsapplication" ng-controller="mpscontroller">
		<div class="table-responsive">
			<table class="table table-hover table-bordered">
				<thead class="thead-dark">
					<tr class="info">
						<th class="td-center">#</th>
						<th class="td-center">Name</th>
						<th class="td-center">T</th>
						<th class="td-center" style="cursor: pointer" ng-click="vncStart()">VNC</th>
						<th class="td-center">P/R</th>
						<th class="td-center">ST</th>
						<th class="td-center">SH</th>
						<th class="td-center">GI</th>
						<th class="td-center">RE</th>
						<th class="td-center">LO</th>
						<th class="td-center">Go</th>
						<th class="td-center">Current</th>
						<th class="td-center">Version</th>
					</tr>
				</thead>
				<tbody>
					<tr ng-repeat="m in machines | orderBy : ['NAME']" style="background-color: {{m.COLOR}}">
						<td class="p-0">{{$index+1}}</td>
						<td class="p-0"><a href="/runs.aspx?R.REPEATED=<>2&P.PCNAME={{m.NAME}}">{{m.NAME}}</a></td>
						<td class="p-0">{{m.TESTS}}</td>
						<td class="p-0 td-center"><a href ng-click="vnc(m.IP)"><img src="/images/vnc.png" data-toggle="tooltip" title="VNC {{m.NAME}}"></img></a></td>
						<td class="p-0 td-center" style="cursor: pointer; white-space: nowrap" ng-click="pauseOnOff(m.ID)" ng-disabled="!isAdmin" data-toggle="tooltip" title="{{m.PAUSEDBY ? 'Resume' : 'Pause'}}  {{m.NAME}}">
							<img ng-show="m.PAUSEDBY" width="20" height="20" src="<%=Settings.CurrentSettings.USERIMGURL.ToString()%>{{m.PAUSEDBY}}"></img>
							<i ng-show="m.PAUSEDBY" class="fas fa-play"></i>
							<i ng-show="!m.PAUSEDBY" class="fas fa-pause-circle"></i>
						</td>

						<td class="p-0 td-center" data-toggle="tooltip" title="Stop all tests on {{m.NAME}}"><a href><i class="text-danger far fa-stop-circle"></i></a></td>
						<td class="p-0 td-center" data-toggle="tooltip" title="Shutdown machine {{m.NAME}}"><a href><i class="text-danger fas fa-power-off"></i></a></td>
						<td class="p-0 td-center"><a href><i class="fas fa-code-branch"></i></a></td>
						<td class="p-0 td-center"><a href><i class="text-success fas fa-recycle"></i></a></td>

						<td class="p-0 td-center" data-toggle="tooltip" title="Show {{m.NAME}} Logs"><a href="/Log.aspx?thoster={{m.NAME}}"><i class="fas fa-file-alt"></i></a></td>
						<td class="p-0">{{m.RUN}}</td>
						<td class="p-0">{{m.STATUS}}</td>
						<td class="p-0"><a href="/runs.aspx?V.VERSION={{m.VERSION}}">{{m.VERSION}}</a></td>
					</tr>
				</tbody>
			</table>
		</div>
	</div>
</asp:Content>
