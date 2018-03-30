<%@ Page Title="Files Statistics" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="FilesStatistics.aspx.cs" Inherits="FilesStatistics" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
	<td>
 Filter by file: <br/>
 <asp:TextBox id="TextAreaMessage" TextMode="multiline" Height="100" Width="99.5%" 
			runat="server" /><br/></asp:TextBox>
 <asp:Button ID="btn_Filter" runat="server" Text="Set Filter" 
            onclick="btn_Filter_Click"/> 
 <asp:Button ID="btn_Filter_Off" runat="server" Text="Off Filter" Visible="False" 
            onclick="btn_Filter_Off_Click" /> <br/>
<asp:HiddenField ID="OriginalData" runat="server" />

		<asp:SqlDataSource ID="SqlDataSource1" runat="server" 
			ConnectionString="<%$ ConnectionStrings:BST_STATISTICS192.168.0.8 %>" SelectCommand=""></asp:SqlDataSource>
		<asp:GridView ID="GridView1" runat="server" AllowPaging="True" 
			AllowSorting="True" AutoGenerateColumns="False" DataSourceID="SqlDataSource1" 
			EnableSortingAndPagingCallbacks="True" PageSize="50" Width="100%">
			<PagerSettings PageButtonCount="20" Position="Top" />
			<Columns>
				<asp:BoundField DataField="CODEFILENAME" HeaderText="CODEFILENAME" 
					SortExpression="CODEFILENAME" />
				<asp:BoundField DataField="TEST_CASE_NAME" HeaderText="TEST_CASE_NAME" 
					SortExpression="TEST_CASE_NAME" />
				<asp:BoundField DataField="FAILURES" HeaderText="FAILURES" ReadOnly="True" 
					SortExpression="FAILURES" />
			</Columns>
		</asp:GridView>
	</td>
</asp:Content>

