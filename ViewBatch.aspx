<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="ViewBatch.aspx.cs" Inherits="ViewBatch" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
	<td>
		<asp:SqlDataSource ID="SqlDataSource1" runat="server" 
			ConnectionString="<%$ ConnectionStrings:BST_STATISTICS192.168.0.8 %>" 
			OldValuesParameterFormatString="original_{0}" 
			
			
			SelectCommand="SELECT [ID], [BATCH_NAME], [BATCH_DATA], [BATCH_COMM], [PC_NAME] FROM [BATCHES] ORDER BY [ID] DESC" 
			DeleteCommand="DELETE FROM [BATCHES] WHERE [ID] = @original_ID" 
			InsertCommand="INSERT INTO [BATCHES] ([BATCH_NAME], [BATCH_DATA], [BATCH_COMM], [PC_NAME]) VALUES (@BATCH_NAME, @BATCH_DATA, @BATCH_COMM, @PC_NAME)" 
			UpdateCommand="UPDATE [BATCHES] SET [BATCH_NAME] = @BATCH_NAME, [BATCH_DATA] = @BATCH_DATA, [BATCH_COMM] = @BATCH_COMM, [PC_NAME] = @PC_NAME WHERE [ID] = @original_ID">
			<DeleteParameters>
				<asp:Parameter Name="original_ID" Type="Int32" />
			</DeleteParameters>
			<UpdateParameters>
				<asp:Parameter Name="BATCH_NAME" Type="String" />
				<asp:Parameter Name="BATCH_DATA" Type="String" />
				<asp:Parameter Name="BATCH_COMM" Type="String" />
				<asp:Parameter Name="PC_NAME" Type="String" />
				<asp:Parameter Name="original_ID" Type="Int32" />
			</UpdateParameters>
			<InsertParameters>
				<asp:Parameter Name="BATCH_NAME" Type="String" />
				<asp:Parameter Name="BATCH_DATA" Type="String" />
				<asp:Parameter Name="BATCH_COMM" Type="String" />
				<asp:Parameter Name="PC_NAME" Type="String" />
			</InsertParameters>
		</asp:SqlDataSource>
        <asp:Panel ID="ButtonsPanel" runat="server" HorizontalAlign="Center">
			<asp:HyperLink ID="CompactMode" runat="server" BackColor="Silver" 
				BorderColor="#999999" BorderStyle="Outset" BorderWidth="2px" Font-Names="Arial" 
				Font-Size="8pt" Height="10pt" Width="100px">Compact Mode</asp:HyperLink>
		</asp:Panel>
		<asp:Button ID="AddButton" runat="server" onclick="AddButton_Click" 
			Text="Add new" />
		<asp:GridView ID="GridView1" runat="server" AllowPaging="True" 
			AllowSorting="True" AutoGenerateColumns="False" DataKeyNames="ID" 
			DataSourceID="SqlDataSource1" Width="100%" 
			onrowdatabound="GridView1_RowDataBound" PageSize="300">
			<Columns>
				<asp:CommandField ShowDeleteButton="True" ShowEditButton="True" >
					<HeaderStyle Width="5%" />
					<ItemStyle Wrap="True" />
				</asp:CommandField>
				<asp:BoundField DataField="ID" HeaderText="ID" ReadOnly="True" 
					SortExpression="ID" >
					<HeaderStyle Width="3%" />
				</asp:BoundField>
				<asp:TemplateField HeaderText="BATCH_NAME" SortExpression="BATCH_NAME">
					<EditItemTemplate>
						<asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("BATCH_NAME") %>' Width="98%"></asp:TextBox>
					</EditItemTemplate>
					<ItemTemplate>
						<asp:Label ID="Label2" runat="server" Text='<%# Bind("BATCH_NAME") %>' Width="99%"></asp:Label>
					</ItemTemplate>
					<HeaderStyle Width="22%" />
				</asp:TemplateField>
				<asp:TemplateField HeaderText="BATCH_DATA" SortExpression="BATCH_DATA">
					<EditItemTemplate>
						<asp:TextBox ID="DataTextBox" runat="server" Text='<%# Bind("BATCH_DATA") %>' TextMode="MultiLine" Height="100%" Width="98%"></asp:TextBox>
					</EditItemTemplate>
					<ItemTemplate>
						<asp:Label ID="DataDridLabel" runat="server" Text='<%# Bind("BATCH_DATA") %>' Width="99%"></asp:Label>
					</ItemTemplate>
					<HeaderStyle Width="50%" />
				</asp:TemplateField>
				<asp:TemplateField SortExpression="BATCH_COMM" HeaderText="BATCH_COMM">
					<EditItemTemplate>
						<asp:TextBox ID="CommTextBox" runat="server" Text='<%# Bind("BATCH_COMM") %>' TextMode="MultiLine" Height="100%" Width="98%"></asp:TextBox>
					</EditItemTemplate>
					<ItemTemplate>
						<asp:Label ID="CommDridLabel" runat="server" Text='<%# Bind("BATCH_COMM") %>' Width="99%"></asp:Label>
					</ItemTemplate>
					<HeaderStyle Width="10%" />
				</asp:TemplateField>
				<asp:TemplateField SortExpression="PC_NAME" HeaderText="PC_NAME">
					<EditItemTemplate>
						<asp:TextBox ID="PCTextBox" runat="server" Text='<%# Bind("PC_NAME") %>' TextMode="MultiLine" Height="100%" Width="98%"></asp:TextBox>
					</EditItemTemplate>
					<ItemTemplate>
						<asp:Label ID="PCDridLabel" runat="server" Text='<%# Bind("PC_NAME") %>' Width="99%"></asp:Label>
					</ItemTemplate>
					<HeaderStyle Width="10%" />
				</asp:TemplateField>
			</Columns>
		</asp:GridView>
	</td>
</asp:Content>

