<%@ Page Title="Test Runs Performance" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="runsperformance.aspx.cs" Inherits="RunsPerformance" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<link href="css/runsperformance.css" rel="stylesheet" />
	<script src="scripts/runsperformance.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<svg xmlns="http://www.w3.org/2000/svg" class="graph" role="img">
		<g class="grid x-grid" id="xGrid">
			<line x1="5%" x2="5%" y1="5%" y2="95%"></line>
		</g>
		<g class="grid y-grid" id="yGrid">
			<line x1="5%" x2="95%" y1="95%" y2="95%"></line>
		</g>
		<g class="labels x-labels">
			  <% for (int i = 0; i < 11; i++)
				  { %>
					<text x="<%:(5 + i * 9).ToString()%>%" y="100%"><%:_channel.getLabelXFor(i / 10.0)%></text>
			  <% } %>
			<text x="50%" y="97.5%" class="label-title">Date</text>
		</g>
		<g class="labels y-labels">
			  <% for (int i = 0; i < 11; i++)
				  { %>
					<text y="<%:(5 + i * 9).ToString()%>%" x="4%"><%:_channel.getLabelYFor((10 - i) / 10.0)%></text>
			  <% } %>
			<text y="5%" x="10%" class="label-title">Time, hr</text>
		</g>
		<g class="data" data-setname="performanxe">
		  <% for (int i = 0; i < _channel.count; i++)
			  { %>
				<circle class="data-point <%:_channel.getPointCss(i)%>" cx="<%:_channel.getScreenPtXAt(i).ToString("0.00")%>%" cy="<%:_channel.getScreenPtYAt(i).ToString("0.00")%>%" data-y="<%:_channel.getLabelYAt(i)%>" data-x="<%:_channel.getLabelXAt(i)%>" data-info="<%:_channel.getPointInfo(i)%>" r="4"></circle>
        <% } %>
		</g>
	</svg>
</asp:Content>
