<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>BST Login</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
    	<asp:Panel ID="Panel1" runat="server" HorizontalAlign="Center">
    	<center>
			<br />
			<br />
			<asp:Label ID="Label1" runat="server" Font-Size="20pt" 
				Text="MPS BST LOGIN PAGE"></asp:Label>
			<br />
			<br />
			<br />
			<br />
			<asp:Login ID="LoginControl" runat="server" BackColor="#F7F6F3" 
				BorderColor="#E6E2D8" BorderPadding="4" BorderStyle="Solid" BorderWidth="1px" 
				DestinationPageUrl="~/index.aspx" Font-Names="Arial" Font-Size="Small" 
				ForeColor="#333333" Height="193px" 
				Width="314px" 
				
				FailureText="Your login attempt was not successful. Please try again or contact SVAN, OVAN or IVAN ." 
				RememberMeSet="True" TitleText="BST Log In" onloggingin="LoginControl_LoggingIn">
				<TextBoxStyle Font-Size="0.8em" />
				<LoginButtonStyle BackColor="#FFFBFF" BorderColor="#CCCCCC" BorderStyle="Solid" 
					BorderWidth="1px" Font-Names="Verdana" Font-Size="0.8em" ForeColor="#284775" />
				<InstructionTextStyle Font-Italic="True" ForeColor="Black" />
				<TitleTextStyle BackColor="#5D7B9D" Font-Bold="True" Font-Size="0.9em" 
					ForeColor="White" />
			</asp:Login>
		</center>
		 </asp:Panel>
    
    </div>
    </form>
</body>
</html>
