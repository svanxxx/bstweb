<%@ Control Language="C#" AutoEventWireup="true" CodeFile="BSidebar.ascx.cs" Inherits="Controls_WebUserControl" %>
<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
	<ContentTemplate>
		<asp:Panel ID="Panel1" runat="server" Height="507px">
			<asp:Panel ID="Panel2" runat="server" HorizontalAlign="Center"
				BackColor="#66FF33">
				<asp:Button ID="Button1" runat="server" Text="BST" Width="100%" />
				<asp:ImageButton ID="ImageButton1" runat="server" Height="30px"
					ImageUrl="~/IMAGES/favicon.ico" Width="30px" PostBackUrl="~/index.aspx" />
				<br />
				<asp:HyperLink ID="HyperLink1" runat="server">BST Home</asp:HyperLink>
				<br />
				<br />
				<asp:ImageButton ID="ImageButton2" runat="server" Height="30px"
					ImageUrl="~/IMAGES/PCS.ICO" Width="30px" PostBackUrl="~/machines.aspx" />
				<br />
				<asp:HyperLink ID="HyperLink2" runat="server">Handle PCs</asp:HyperLink>
				<br />
				<br />
				<asp:ImageButton ID="ImageButton3" runat="server" Height="30px"
					ImageUrl="~/IMAGES/DB.ico" Width="30px"
					PostBackUrl="http://<% =<%= BSTStat.globalIPAddress %> %>/bst/DEMO/" />
				<br />
				<asp:HyperLink ID="HyperLink3" runat="server">DEMO DBs</asp:HyperLink>
				<br />
				<br />
				<asp:ImageButton ID="ImageButton4" runat="server" Height="30px"
					ImageUrl="~/IMAGES/tests.ico" Width="30px" PostBackUrl="~/TSummary.aspx" />
				<br />
				<asp:HyperLink ID="HyperLink4" runat="server">Tests Summary</asp:HyperLink>
				<br />
				<br />
				<asp:ImageButton ID="ImageButton5" runat="server" Height="30px"
					ImageUrl="~/IMAGES/versions.ico" Width="30px" PostBackUrl="~/VSummary.aspx" />
				<br />
				<asp:HyperLink ID="HyperLink5" runat="server">Versions Summary</asp:HyperLink>
				<br />
				<br />
				<asp:ImageButton ID="ImageButton6" runat="server" Height="30px"
					ImageUrl="~/IMAGES/PCS.ICO" Width="30px" PostBackUrl="~/PCSummary.aspx" />
				<br />
				<asp:HyperLink ID="HyperLink6" runat="server">PCs Summary</asp:HyperLink>
				<br />
				<br />
				<asp:ImageButton ID="ImageButton7" runat="server" Height="30px"
					ImageUrl="~/IMAGES/gear.jpeg" Width="30px" PostBackUrl="~/Components.aspx" />
				<br />
				<asp:HyperLink ID="HyperLink7" runat="server">Components</asp:HyperLink>
				<br />
				<br />
				<asp:ImageButton ID="ImageButton8" runat="server" Height="30px"
					ImageUrl="~/IMAGES/BookGreen.ico" Width="30px" PostBackUrl="~/Documents.aspx" />
				<br />
				<asp:HyperLink ID="HyperLink8" runat="server">Documents</asp:HyperLink>
			</asp:Panel>
			<asp:Panel ID="Panel3" runat="server" HorizontalAlign="Center"
				BackColor="#66CCFF">
				<asp:Button ID="Button2" runat="server" Text="TR" Width="100%" />
				<br />
				<asp:ImageButton ID="ImageButton9" runat="server" Height="30px"
					ImageUrl="~/IMAGES/gear.jpeg" Width="30px"
					PostBackUrl="http://<% =BSTStat.globalIPAddress %>/tr/index.aspx" />
				<br />
				<asp:HyperLink ID="HyperLink9" runat="server"
					NavigateUrl="http://<% =BSTStat.globalIPAddress %>/tr/index.aspx">WAYW Now?</asp:HyperLink>
			</asp:Panel>
		</asp:Panel>
	</ContentTemplate>
</asp:UpdatePanel>

