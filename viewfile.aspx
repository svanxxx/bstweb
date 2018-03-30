<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ViewFile.aspx.cs" Inherits="ViewFile" %>

<%@ Register src="Controls/BstMenuControl.ascx" tagname="BstMenuControl" tagprefix="uc1" %>

<%@ Register src="Controls/BstHeaderControl.ascx" tagname="BstHeaderControl" tagprefix="uc2" %>
<%@ Register src="Controls/BstFooterControl.ascx" tagname="BstFooterControl" tagprefix="uc3" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>BST View/Edit File</title>
<link rel="stylesheet" href="http://code.jquery.com/ui/1.10.3/themes/smoothness/jquery-ui.css" />
<script src="http://code.jquery.com/jquery-1.9.1.js"></script>
<script src="http://code.jquery.com/ui/1.10.3/jquery-ui.js"></script>
<link rel="stylesheet" href="/resources/demos/style.css" />

 <link href="CSS/Search.css" rel="stylesheet" type="text/css" />    
 <script src="Scripts/Search.js" type="text/javascript"></script>
 </head>

<body>
    <form id="ViewFileForm" runat="server">
    <div>
    
    	 <uc2:BstHeaderControl ID="BstHeaderControl1" runat="server" />
    
    	 <uc1:BstMenuControl ID="BstMenuControl1" runat="server" />
    
    	 <asp:ScriptManager ID="ScriptManager1" runat="server">
		 </asp:ScriptManager>
		 <asp:UpdatePanel ID="UpdatePanel1" runat="server">
			 <ContentTemplate>
				 <asp:Panel ID="Panel2" runat="server">
					 <asp:UpdateProgress ID="UpdateProgress1" runat="server">
						 <ProgressTemplate>
							 Updating data...
						 </ProgressTemplate>
					 </asp:UpdateProgress>
                		Search:
                     <asp:TextBox id="idSearch" runat="server"  autocomplete="off"
                          Width="80%" onKeyUp="return SearchBar(this, 'FileTextBox')"></asp:TextBox> <br/>
                     <div id="base">Loading... </div>

					 <asp:ImageButton ID="RefreshButton" runat="server" ImageUrl="~/IMAGES/Refresh.ico" 
				 onclick="RefreshButton_Click" />
					 <asp:ImageButton ID="SaveButton" runat="server" ImageUrl="~/IMAGES/save.GIF" 
						 onclick="SaveButton_Click" />
					 <asp:Label ID="FileLabel" runat="server" Text="Label"></asp:Label>
					 <asp:SqlDataSource ID="SqlDataSource1" runat="server" 
						 ConnectionString="<%$ ConnectionStrings:BST_STATISTICS192.168.0.8 %>" 
						 SelectCommand="SELECT [BATCH_NAME], [BATCH_DATA], [ID] FROM [BATCHES]">
					 </asp:SqlDataSource>
					 <table style="width: 100%;">
						 <tr>
							 <td width="90%">
								 <asp:TextBox ID="FileTextBox" runat="server" BackColor="#CCFFCC" 
									 BorderStyle="Solid" Rows="30" TextMode="MultiLine" Width="100%" onclick="SearchBar_Off()">
									 </asp:TextBox>
							 </td>
							 <td width="10%">
									<asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/ViewBatch.aspx" 
										Width="100%">Edit data...</asp:HyperLink>
									<asp:GridView ID="DataGridView" runat="server" DataSourc="SqlDataSource1" 
										Width="100%" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" 
										DataSourceID="SqlDataSource1" onrowcommand="GridView1_RowCommand" DataKeyNames="ID" 
										BorderStyle="Solid" BorderWidth="1px" CellPadding="0" Font-Names="arial" 
										Font-Size="Smaller" Height="90%" HorizontalAlign="Center" PageSize="20">
										<Columns>
											<asp:ButtonField ButtonType="Button" CommandName="add" Text="&lt;-">
												<ControlStyle Font-Names="arial" Font-Size="10pt" Height="20px" Width="20px" />
												<ItemStyle Width="20px" Font-Names="arial" Font-Size="10pt" Height="20px" 
													HorizontalAlign="Center" VerticalAlign="Middle" Wrap="True" />
											</asp:ButtonField>
											<asp:BoundField DataField="BATCH_NAME" HeaderText="BATCH_NAME" 
												SortExpression="BATCH_NAME">
											</asp:BoundField>
											<asp:BoundField DataField="BATCH_DATA" HeaderText="BATCH_DATA" 
												SortExpression="BATCH_DATA" Visible="False" />
											<asp:BoundField DataField="ID" HeaderText="ID" InsertVisible="False" 
												ReadOnly="True" SortExpression="ID" />
										</Columns>
									</asp:GridView>
							 </td>
						 </tr>
					 </table>
				 </asp:Panel>
			 </ContentTemplate>
		 </asp:UpdatePanel>
    
    	 <uc3:BstFooterControl ID="BstFooterControl1" runat="server" />
    
    </div>
    </form>
</body>
</html>
