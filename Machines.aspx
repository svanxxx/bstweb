<%@ Page Title="Machines" Language="C#" MasterPageFile="~/Master2.Master" AutoEventWireup="true" Inherits="SecurityPage" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<%=System.Web.Optimization.Styles.Render("~/bundles/machines_css")%>
	<%=System.Web.Optimization.Scripts.Render("~/bundles/machines_js")%>
	<script src="<%=Settings.CurrentSettings.ANGULARCDN.ToString()%>angular.min.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<input type="hidden" id="inidata" value='<%=Newtonsoft.Json.JsonConvert.SerializeObject(MachineState.EnumUsed()) %>'>
	<div ng-app="mpsapplication" ng-controller="mpscontroller">
		<div class="row">
			<div class="col-sm-0 col-lg-1">
			</div>
			<div class="col-sm-11 col-lg-10">
				<div class="table-responsive">
					<table class="table table-hover table-bordered shadow" style="font-size: 90%">
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
							<tr ng-repeat="m in machines | filter: { VISIBLE : true } | orderBy : ['NAME']" style="background-color: {{m.COLOR}}">
								<td class="p-0 td-center">{{$index+1}}</td>
								<td class="p-0"><a href="/runs.aspx?R.REPEATED=<>2&P.PCNAME={{m.NAME}}">{{m.NAME}}</a></td>
								<td class="p-0">{{m.SCHEDULES}}</td>
								<td class="p-0 td-center"><a href ng-click="vnc(m.IP)">
									<img src="/images/vnc.png" data-toggle="tooltip" title="VNC {{m.NAME}}"></img></a></td>
								<td class="p-0 td-center" style="cursor: pointer; white-space: nowrap" ng-click="pauseOnOff(m.ID)" ng-disabled="!isAdmin" data-toggle="tooltip" title="{{m.PAUSEDBY ? 'Resume' : 'Pause'}}  {{m.NAME}}">
									<img ng-show="m.PAUSEDBY" width="20" height="20" src="<%=Settings.CurrentSettings.USERIMGURL.ToString()%>{{m.PAUSEDBY}}"></img>
									<i ng-show="m.PAUSEDBY" class="fas fa-play"></i>
									<i ng-show="!m.PAUSEDBY" class="fas fa-pause-circle"></i>
								</td>
								<td class="p-0 td-center" ng-click="changeState(m.ID, '<%= MachineState.MachineStatus.Stop.ToString() %>')" data-toggle="tooltip" title="Stop all tests on {{m.NAME}}"><a href><i class="text-danger far fa-stop-circle"></i></a></td>
								<td class="p-0 td-center" ng-click="changeState(m.ID, '<%= MachineState.MachineStatus.Shutdown.ToString() %>')" data-toggle="tooltip" title="Shutdown machine {{m.NAME}}"><a href><i class="text-danger fas fa-power-off"></i></a></td>
								<td class="p-0 td-center" ng-click="changeState(m.ID, '<%= MachineState.MachineStatus.SSGET.ToString() %>')" data-toggle="tooltip" title="Get git {{m.NAME}}"><a href><i class="fas fa-code-branch"></i></a></td>
								<td class="p-0 td-center" ng-click="changeState(m.ID, '<%= MachineState.MachineStatus.Restart.ToString() %>')" data-toggle="tooltip" title="Restart {{m.NAME}}"><a href><i class="text-success fas fa-recycle"></i></a></td>
								<td class="p-0 td-center" data-toggle="tooltip" title="Show {{m.NAME}} Logs"><a href="/Log.aspx?thoster={{m.NAME}}"><i class="fas fa-file-alt"></i></a></td>
								<td class="p-0">{{m.RUN}}</td>
								<td class="p-0">{{m.STATUS}}</td>
								<td class="p-0"><a href="/runs.aspx?V.VERSION={{m.VERSION}}">{{m.VERSION}}</a></td>
							</tr>
						</tbody>
					</table>
				</div>
				<div class="d-flex mb-3 small shadow">
					<button type="button" class="btn btn-sm flex-fill">Standby</button>
					<button type="button" class="btn btn-sm flex-fill" style="background-color: lightgray">Offline</button>
					<button type="button" class="btn btn-sm flex-fill" style="background-color: yellow">Running</button>
					<button type="button" class="btn btn-sm flex-fill" style="background-color: burlywood">Slow</button>
				</div>
			</div>
			<div class="col-sm-1 col-lg-1">
			</div>
		</div>
	</div>
</asp:Content>
