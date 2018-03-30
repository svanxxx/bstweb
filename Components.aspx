<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Components.aspx.cs" Inherits="Components" %>

<%@ Register src="Controls/BstFooterControl.ascx" tagname="BstFooterControl" tagprefix="uc1" %>
<%@ Register src="Controls/BstMenuControl.ascx" tagname="BstMenuControl" tagprefix="uc2" %>
<%@ Register src="Controls/BstHeaderControl.ascx" tagname="BstHeaderControl" tagprefix="uc3" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<link rel="stylesheet" type="text/css" href="CSS/Tables.css" />
	<title>Components</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    	<uc3:BstHeaderControl ID="BstHeaderControl1" runat="server" />
		 <uc2:BstMenuControl ID="BstMenuControl1" runat="server" />
			<asp:SqlDataSource ID="SqlDataSource3" runat="server" 
			 ConnectionString="<%$ ConnectionStrings:BST_STATISTICS192.168.0.8 %>" 
			 DeleteCommand="DELETE FROM [COMPONENTS] WHERE [ID] = @ID" 
			 InsertCommand="INSERT INTO [COMPONENTS] ([COMPONENTNAME], [HASTEST], [TTID], [ISMODULE], [URLFILTER]) VALUES (@COMPONENTNAME, @HASTEST, @TTID, @ISMODULE, @URLFILTER)" 
			 ProviderName="<%$ ConnectionStrings:BST_STATISTICS192.168.0.8.ProviderName %>" 
			 SelectCommand="SELECT * FROM [COMPONENTS] ORDER BY [HASTEST] DESC, [COMPONENTNAME]" 
			 
			 
			 UpdateCommand="UPDATE [COMPONENTS] SET [COMPONENTNAME] = @COMPONENTNAME, [HASTEST] = @HASTEST, [TTID] = @TTID, [ISMODULE] = @ISMODULE, [URLFILTER] = @URLFILTER WHERE [ID] = @ID">
				<DeleteParameters>
					<asp:Parameter Name="ID" Type="Int32" />
				</DeleteParameters>
				<UpdateParameters>
					<asp:Parameter Name="COMPONENTNAME" Type="String" />
					<asp:Parameter Name="HASTEST" Type="Boolean" />
					<asp:Parameter Name="TTID" Type="Int64" />
					<asp:Parameter Name="ISMODULE" Type="Boolean" />
					<asp:Parameter Name="URLFILTER" Type="String" />
					<asp:Parameter Name="ID" Type="Int32" />
				</UpdateParameters>
				<InsertParameters>
					<asp:Parameter Name="COMPONENTNAME" Type="String" />
					<asp:Parameter Name="HASTEST" Type="Boolean" />
					<asp:Parameter Name="TTID" Type="Int64" />
					<asp:Parameter Name="ISMODULE" Type="Boolean" />
					<asp:Parameter Name="URLFILTER" Type="String" />
				</InsertParameters>
		 </asp:SqlDataSource>
&nbsp;<asp:ScriptManager ID="ScriptManager1" runat="server">
		 </asp:ScriptManager>
		 <asp:UpdatePanel ID="UpdatePanel1" runat="server">
			 <ContentTemplate>
				 <asp:UpdateProgress ID="UpdateProgress1" runat="server">
				 	<ProgressTemplate>
						Updating Data...
					 </ProgressTemplate>
				 </asp:UpdateProgress>
				 <asp:Panel ID="Panel2" runat="server" HorizontalAlign="Center">
					 <asp:Button ID="AddButton0" runat="server" onclick="Button1_Click" 
						 Text="Add New Record (see red row)" />
				 </asp:Panel>
				<asp:GridView ID="GridView1" runat="server" DataKeyNames="ID" DataSourceID="SqlDataSource3" 
					HorizontalAlign="Center" onrowdatabound="GridView1_RowDataBound" AllowSorting="True" 
					 AutoGenerateColumns="False">
					<Columns>
						<asp:CommandField ShowDeleteButton="True" ShowEditButton="True" />
						<asp:BoundField DataField="ID" HeaderText="ID" InsertVisible="False" 
							ReadOnly="True" SortExpression="ID" />
						<asp:BoundField DataField="COMPONENTNAME" HeaderText="COMPONENTNAME" 
							SortExpression="COMPONENTNAME" />
						<asp:CheckBoxField DataField="HASTEST" HeaderText="HASTEST" 
							SortExpression="HASTEST" />
						<asp:BoundField DataField="TTID" HeaderText="TTID" SortExpression="TTID" />
						<asp:CheckBoxField DataField="ISMODULE" HeaderText="ISMODULE" 
							SortExpression="ISMODULE" />
						<asp:BoundField DataField="URLFILTER" HeaderText="URL" SortExpression="URLFILTER" />
					</Columns>
				</asp:GridView>
			 	<asp:Panel ID="Panel1" runat="server" HorizontalAlign="Center">
					<asp:Button ID="AddButton" runat="server" onclick="Button1_Click" 
						Text="Add New Record (see red row)" />
				 </asp:Panel>
				 <br />
			 </ContentTemplate>
		 </asp:UpdatePanel>
		 <uc1:BstFooterControl ID="BstFooterControl1" runat="server" />
    </div>
    </form>
</body>
</html>
