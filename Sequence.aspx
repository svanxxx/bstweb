<%@ Page Title="Sequence" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
	CodeFile="Sequence.aspx.cs" Inherits="Sequence" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
	<link href="CSS/Search.css" rel="stylesheet" type="text/css" />
	<script src="http://mps.resnet.com/cdn/jquery/jquery-3.2.1.min.js"></script>
	<script src="http://mps.resnet.com/cdn/mpshelper.js"></script>
	<script src="scripts/Common.js"></script>
	<script src="Scripts/Search.js" type="text/javascript"></script>
	<script src="Scripts/RunTest.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
	<asp:Label ID="LabelError" runat="server" Visible="False" ForeColor="Red" Font-Size="Larger"></asp:Label>
	<td>Search:
        <asp:TextBox ID="idSearch" runat="server" autocomplete="off" Width="90%" onKeyUp="return SearchBar(this, 'ctl00_ContentPlaceHolder1_FileTextBox')"></asp:TextBox>
		<br />
		<div id="base">
			Loading...
		</div>
		<asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:BST_STATISTICS192.168.0.8 %>"
			SelectCommand="SELECT [BATCH_NAME], [BATCH_DATA], [ID] FROM [BATCHES] order by [BATCH_NAME]"></asp:SqlDataSource>
		<table width="100%">
			<tr>
				<td width="90%">
					<table width="100%">
						<tr>
							<td>
								<asp:TextBox class="testsequencetext" ID="FileTextBox" runat="server" BackColor="#CCFFCC" BorderStyle="Solid"
									Rows="30" TextMode="MultiLine" Width="99%" Height="100%" onclick="SearchBar_Off()">
								</asp:TextBox>
							</td>
						</tr>
						<tr>
							<td>
								<table width="100%">
									<tr>
										<td>Before:
										</td>
										<td>After:
										</td>
									</tr>
									<tr>
										<td>
											<asp:TextBox ID="TextBoxBefore" runat="server" BackColor="#CCFFCC" BorderStyle="Solid"
												Rows="2" TextMode="MultiLine" Width="99%" Height="100%" onclick="SearchBar_Off()">
											</asp:TextBox>
										</td>
										<td>
											<asp:TextBox ID="TextBoxAfter" runat="server" BackColor="#CCFFCC" BorderStyle="Solid"
												Rows="2" TextMode="MultiLine" Width="99%" Height="100%" onclick="SearchBar_Off()">
											</asp:TextBox>
										</td>
									</tr>
								</table>
							</td>
						</tr>
					</table>
				</td>
				<td width="10%">
					<table>
						<tr>
							<asp:ImageButton ID="ImageButtonRun" runat="server" ImageUrl="IMAGES/Save_Big.jpg"
								OnClick="ImageButton_Run" />
						</tr>
						<tr>
							<asp:ImageButton ID="ImageButtonStopRequest" runat="server" ImageUrl="IMAGES/sign_Stop.png"
								OnClick="ImageButton_StopRequest" OnClientClick="return confirm('This will stop all tests for request. Are you sure?')" />
						</tr>
						<tr>
							<b>Priority: </b>
							<asp:DropDownList ID="PriorityList" runat="server" Width="100%">
								<asp:ListItem Text="1 (Low)" Value="1"></asp:ListItem>
								<asp:ListItem Text="2 (Programmer big release)" Value="2"> </asp:ListItem>
								<asp:ListItem Text="3 (Release)" Value="3" Selected="true"> </asp:ListItem>
								<asp:ListItem Text="4 (Programmer)" Value="4"> </asp:ListItem>
								<asp:ListItem Text="5 (High)" Value="5"> </asp:ListItem>
							</asp:DropDownList>
						</tr>
                        <tr>
                            <td>
                                <asp:CheckBox ID="RemoveIdenticalTests" runat="server" Checked="true" Text="Remove Identical Tests" Font-Bold="true" />
                            </td>
                            <td>
                                <asp:CheckBox ID="RemoveIdenticalGroupsOfTests" runat="server" Checked="true" Text="Remove Identical Groups Of Tests" Font-Bold="true" />
                            </td>
                        </tr>
						<tr>
							<asp:GridView ID="DataGridView" runat="server" DataSourc="SqlDataSource1" Width="100%"
								AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" DataSourceID="SqlDataSource1"
								OnRowCommand="GridView1_RowCommand" DataKeyNames="ID" BorderStyle="Solid" BorderWidth="1px"
								CellPadding="0" Font-Names="arial" Font-Size="Smaller" Height="80%" HorizontalAlign="Center"
								PageSize="18">
								<PagerSettings Position="Top" />
								<Columns>
									<asp:ButtonField ButtonType="Button" CommandName="add" Text="&lt;-">
										<ControlStyle Font-Names="arial" Font-Size="10pt" Height="20px" Width="20px" />
										<ItemStyle Width="20px" Font-Names="arial" Font-Size="10pt" Height="20px" HorizontalAlign="Center"
											VerticalAlign="Middle" Wrap="True" />
									</asp:ButtonField>
									<asp:BoundField DataField="BATCH_NAME" HeaderText="BATCH_NAME" SortExpression="BATCH_NAME"></asp:BoundField>
									<asp:BoundField DataField="BATCH_DATA" HeaderText="BATCH_DATA" SortExpression="BATCH_DATA"
										Visible="False" />
									<asp:BoundField DataField="ID" HeaderText="ID" InsertVisible="False" ReadOnly="True"
										SortExpression="ID" />
								</Columns>
							</asp:GridView>
							<asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/ViewBatch.aspx" Width="100%">Edit data...</asp:HyperLink>
						</tr>
					</table>
				</td>
			</tr>
		</table>
	</td>
	<b>Run now tests: </b>
	<asp:Table ID="TableStop" Width="100%" runat="server" border="1">
		<asp:TableHeaderRow>
			<asp:TableHeaderCell>Command </asp:TableHeaderCell>
			<asp:TableHeaderCell>Thoster </asp:TableHeaderCell>
			<asp:TableHeaderCell>Stop </asp:TableHeaderCell>
		</asp:TableHeaderRow>
	</asp:Table>
	<b>Recommended tests: </b>
	<asp:HiddenField ID="OriginalData" runat="server" />
	<asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:BST_STATISTICS192.168.0.8 %>"
		SelectCommand=""></asp:SqlDataSource>
	<asp:GridView ID="GridView1" runat="server" AllowPaging="True" AllowSorting="True"
		AutoGenerateColumns="False" DataSourceID="SqlDataSource2" EnableSortingAndPagingCallbacks="True"
		PageSize="50" Width="100%">
		<PagerSettings PageButtonCount="20" Position="Top" />
		<Columns>
			<asp:BoundField DataField="CODEFILENAME" HeaderText="CODEFILENAME" SortExpression="CODEFILENAME" />
			<asp:BoundField DataField="TEST_CASE_NAME" HeaderText="TEST_CASE_NAME" SortExpression="TEST_CASE_NAME" />
			<asp:BoundField DataField="FAILURES" HeaderText="FAILURES" ReadOnly="True" SortExpression="FAILURES" />
		</Columns>
	</asp:GridView>
	Files:
    <br />
	<asp:TextBox ID="TextAreaMessage" TextMode="multiline" Height="100" Width="99.5%"
		runat="server" ReadOnly="True" /></asp:TextBox><br />
	<td>
</asp:Content>
