<%@ Page Title="Hosts" Language="C#" MasterPageFile="~/Master2.Master" AutoEventWireup="true" Inherits="SecurityPage" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<%=System.Web.Optimization.Styles.Render("~/bundles/hosts_css")%>
	<%=System.Web.Optimization.Scripts.Render("~/bundles/hosts_js")%>
	<script src="<%=Settings.CurrentSettings.ANGULARCDN.ToString()%>angular.min.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<input type="hidden" id="inidata" value='<%=Newtonsoft.Json.JsonConvert.SerializeObject(HostState.EnumUsed()) %>'>
	<input type="hidden" id="offline" value='<%=Newtonsoft.Json.JsonConvert.SerializeObject(OfflineHost.Enum()) %>'>
	<div ng-app="mpsapplication" ng-controller="mpscontroller">
		<div class="row">
			<div class="col-sm-0 col-lg-1">
			</div>
			<div class="col-sm-11 col-lg-10">
				<div class="table-responsive">
					<table class="table table-hover table-bordered shadow" style="font-size: 90%">
						<thead class="thead-dark">
							<tr class="info">
								<th>#</th>
								<th>Name</th>
								<th>D</th>
								<th>VNC</th>
								<th>STO</th>
								<th>STA</th>
								<th>GO</th>
								<th>Children</th>
								<th>Info</th>
								<th>IP</th>
								<th>MAC</th>
							</tr>
						</thead>
						<tbody>
							<tr ng-repeat="h in hosts | orderBy : ['NAME']" style="background-color: {{h.COLOR}}">
								<td class="td-center">{{$index+1}}</td>
								<td><a href="machines.aspx?machines={{h.CHILDREN.join()}}">{{h.NAME}}</a></td>
								<td class="td-center" ng-click="deleteHost(h.ID)" data-toggle="tooltip" title="Delete {{h.NAME}}"><i style="cursor: pointer" class="fas fa-skull-crossbones"></i></td>
								<td class="td-center"><a href ng-click="vnc(h.IP.split(',')[0].split(' ')[0])">
									<img src="/images/vnc.png" data-toggle="tooltip" title="VNC {{h.NAME}}"></img></a></td>
								<td class="td-center" ng-click="startStopHost(h.ID, false)" data-toggle="tooltip" title="Stop {{h.NAME}}"><a href><i class="text-danger far fa-stop-circle"></i></a></td>
								<td class="td-center" ng-click="startStopHost(h.ID, true)" data-toggle="tooltip" title="Start {{h.NAME}}"><a href><i class="text-success fas fa-play"></i></a></td>
								<td class="td-center small">{{h.STARTED}}</td>
								<td>
									<a ng-repeat="m in h.CHILDREN" href="runs.aspx?P.PCNAME={{m}}">
										<span class="badge badge-light p-0">{{m}}
										</span>
									</a>
								</td>
								<td class="td-center"><span title="{{h.INFO}}">{{h.INFOSHORT}}</span></td>
								<td>{{h.IP}}</td>
								<td class="td-center">{{h.MAC}}</td>
							</tr>
						</tbody>
					</table>
				</div>
				<div class="d-flex mb-3 small shadow">
					<button type="button" class="btn btn-sm">Standby</button>
					<button type="button" class="btn btn-sm" style="background-color: lightgray">Offline</button>
					<div class="btn-group ml-auto">
						<button type="button" class="btn btn-sm btn-outline-secondary dropdown-toggle" data-toggle="dropdown">
							Add
						</button>
						<div class="dropdown-menu">
							<div ng-click="online(o.ID)" style="cursor:pointer" ng-repeat="o in offline" class="dropdown-item" >{{o.NAME}}</div>
						</div>
					</div>
				</div>
			</div>
			<div class="col-sm-1 col-lg-1">
			</div>
		</div>
	</div>
</asp:Content>
