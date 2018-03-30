<%@ Control Language="C#" AutoEventWireup="true" CodeFile="BstFooterControl.ascx.cs" Inherits="Controls_BstFooterControl" %>
    
    	<asp:Panel ID="FooterPanel" runat="server" Height="35px" 
			 HorizontalAlign="Left" BackImageUrl="~/IMAGES/greengradreverse.jpeg" 
	BorderColor="Yellow" BorderStyle="Solid" BorderWidth="1px">
			<asp:Table ID="UserTable" runat="server" Font-Names="Arial" Font-Size="7pt" 
				ForeColor="Yellow" BorderStyle="None">
			</asp:Table>
		   <asp:Table ID="Table1" runat="server" Width="100%" BorderStyle="None" 
				BorderWidth="0px">
				<asp:TableRow runat="server" BorderStyle="None" BorderWidth="0">
					<asp:TableCell ID="LabelCell" runat="server" BorderStyle="None" Font-Italic="True" Font-Names="Arial" Font-Size="7pt" ForeColor="Yellow" BorderWidth="0">Resource Engendering Systems. Quality Control Page.</asp:TableCell>
					<asp:TableCell ID="DateCell" runat="server" BorderStyle="None" Font-Italic="True" Font-Names="Arial" Font-Size="9pt" ForeColor="Yellow" BorderWidth="0" HorizontalAlign="Right">01.01.0001</asp:TableCell>
					<asp:TableCell runat="server" BorderStyle="None" BorderWidth="0"><asp:Image ID="Image1" runat="server" ImageUrl="~/IMAGES/gear.JPG" Width="10" ImageAlign="Right" />
</asp:TableCell>
				</asp:TableRow>
			</asp:Table>
		 </asp:Panel>
    
    