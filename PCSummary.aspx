<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PCSummary.aspx.cs" Inherits="PCSummary" %>

<%@ Register src="Controls/BstMenuControl.ascx" tagname="BstMenuControl" tagprefix="uc1" %>

<%@ Register src="Controls/BstHeaderControl.ascx" tagname="BstHeaderControl" tagprefix="uc2" %>

<%@ Register src="Controls/BstFooterControl.ascx" tagname="BstFooterControl" tagprefix="uc3" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<link rel="stylesheet" type="text/css" href="CSS/Tables.css" />
	<title>BST PC Summary</title>
</head>
<body>
    <form id="PCSummaryform" runat="server">
    <div>
    
    	 <uc2:BstHeaderControl ID="BstHeaderControl1" runat="server" />
    
    	 <asp:SqlDataSource ID="SqlDataSource1" runat="server" 
             ConnectionString="<%$ ConnectionStrings:BST_STATISTICS192.168.0.8 %>" 
             DeleteCommand="DELETE FROM [PCS] WHERE [ID] = @original_ID" 
             InsertCommand="INSERT INTO [PCS] ([PCNAME], [PCIP], [UNUSED], [IPCONFIG], [PCINFO], [DBTYPE], [PHYSICAL], [NO3DV], [PARENT_PC]) VALUES (@PCNAME, @PCIP, @UNUSED, @IPCONFIG, @PCINFO, @DBTYPE, @PHYSICAL, @NO3DV, @PARENT_PC)" 
             OldValuesParameterFormatString="original_{0}" 
             SelectCommand="SELECT [ID], [PCNAME], [PCIP], [UNUSED], [IPCONFIG], [PCINFO], [DBTYPE], [PHYSICAL], [NO3DV], [PARENT_PC] FROM [PCS] ORDER BY [UNUSED], [PCNAME]" 
             UpdateCommand="UPDATE [PCS] SET [PCNAME] = @PCNAME, [PCIP] = @PCIP, [UNUSED] = @UNUSED, [IPCONFIG] = @IPCONFIG, [PCINFO] = @PCINFO, [DBTYPE] = @DBTYPE, [PHYSICAL] = @PHYSICAL, [NO3DV] = @NO3DV, [PARENT_PC] = @PARENT_PC WHERE [ID] = @original_ID">
             <DeleteParameters>
                 <asp:Parameter Name="original_ID" Type="Int32" />
             </DeleteParameters>
             <InsertParameters>
                 <asp:Parameter Name="PCNAME" Type="String" />
                 <asp:Parameter Name="PCIP" Type="String" />
                 <asp:Parameter Name="UNUSED" Type="Boolean" />
                 <asp:Parameter Name="IPCONFIG" Type="String" />
                 <asp:Parameter Name="PCINFO" Type="String" />
                 <asp:Parameter Name="DBTYPE" Type="String" />
                 <asp:Parameter Name="PHYSICAL" Type="Boolean" />
                 <asp:Parameter Name="NO3DV" Type="Boolean" />
                 <asp:Parameter Name="PARENT_PC" Type="String" />
             </InsertParameters>
             <UpdateParameters>
                 <asp:Parameter Name="PCNAME" Type="String" />
                 <asp:Parameter Name="PCIP" Type="String" />
                 <asp:Parameter Name="UNUSED" Type="Boolean" />
                 <asp:Parameter Name="IPCONFIG" Type="String" />
                 <asp:Parameter Name="PCINFO" Type="String" />
                 <asp:Parameter Name="DBTYPE" Type="String" />
                 <asp:Parameter Name="PHYSICAL" Type="Boolean" />
                 <asp:Parameter Name="NO3DV" Type="Boolean" />
                 <asp:Parameter Name="PARENT_PC" Type="String" />
                 <asp:Parameter Name="original_ID" Type="Int32" />
             </UpdateParameters>
         </asp:SqlDataSource>
		 <uc1:BstMenuControl ID="BstMenuControl1" runat="server" />
		 <asp:ScriptManager ID="ScriptManager1" runat="server">
		 </asp:ScriptManager>
		 <asp:UpdatePanel ID="UpdatePanel1" runat="server">
			 <ContentTemplate>
				 <asp:UpdateProgress ID="UpdateProgress1" runat="server">
					 <ProgressTemplate>
						 Updating data...
					 </ProgressTemplate>
				 </asp:UpdateProgress>
				 <asp:GridView ID="GridView1" runat="server" AllowSorting="True" 
                     AutoGenerateColumns="False" DataKeyNames="ID" DataSourceID="SqlDataSource1" 
                     EnableModelValidation="True" onrowdatabound="GridView1_RowDataBound" 
                     Width="100%">
                     <Columns>
                         <asp:CommandField ShowEditButton="True" />
                         <asp:BoundField DataField="ID" HeaderText="ID" InsertVisible="False" 
                             ReadOnly="True" SortExpression="ID" />
                         <asp:BoundField DataField="PCNAME" HeaderText="PCNAME" 
                             SortExpression="PCNAME" />
                         <asp:BoundField DataField="PCIP" HeaderText="PCIP" SortExpression="PCIP" />
                         <asp:CheckBoxField DataField="UNUSED" HeaderText="UNUSED" 
                             SortExpression="UNUSED" />
                         <asp:BoundField DataField="IPCONFIG" HeaderText="IPCONFIG" 
                             SortExpression="IPCONFIG" />
                         <asp:BoundField DataField="PCINFO" HeaderText="PCINFO" 
                             SortExpression="PCINFO" />
                         <asp:BoundField DataField="DBTYPE" HeaderText="DBTYPE" 
                             SortExpression="DBTYPE" />
                         <asp:CheckBoxField DataField="PHYSICAL" HeaderText="PHYSICAL" 
                             SortExpression="PHYSICAL" />
                         <asp:CheckBoxField DataField="NO3DV" HeaderText="NO3DV" 
                             SortExpression="NO3DV" />
                         <asp:BoundField DataField="PARENT_PC" HeaderText="PARENT_PC" 
                             SortExpression="PARENT_PC" />
                     </Columns>
                 </asp:GridView>
                 <br />
			 </ContentTemplate>
		 </asp:UpdatePanel>
    
    	 <uc3:BstFooterControl ID="BstFooterControl1" runat="server" />
    
    </div>
    </form>
</body>
</html>
