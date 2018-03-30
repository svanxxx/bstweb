<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="CommentTestRun.aspx.cs" Inherits="CommentTestRun" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

 
	<td>

    <table border = "0" width="100%" style="border-style:none;">
    <tr> 
      <td width="90%"> 
        <asp:Label ID="Label1" runat="server" Text="Result:" Font-Size="Medium"></asp:Label>
        <asp:Label ID="LabelError" runat="server" Text="" ForeColor="Red" Font-Size="Medium"></asp:Label>
        <br />
        <asp:RadioButton ID="TestOK" GroupName = "GIT" runat="server" AutoPostBack="true"
            BackColor = "White" Text = "You can put your code to GIT"
            Width="100%" Font-Size="Medium" 
              oncheckedchanged="RadioButton_CheckedChanged" /> <br/>
        <asp:RadioButton ID="TestFailed" GroupName = "GIT" runat="server" AutoPostBack="true"
            BackColor = "White" Text = "Your version have errors"
            Width="100%" Font-Size="Medium" 
              oncheckedchanged="RadioButton_CheckedChanged" /> <br/>
		      
      </td>
      <td>
        <asp:ImageButton ID="ImageButtonRun" runat="server" ImageUrl="IMAGES/Send%20mail.png"  Width="100%" OnClick="CommentButton_Click"  ToolTip="Send" /> <br/> <br/>
		<asp:Button ID="ClearButton" runat="server" Text="Clear" onclick="ClearButton_Click" Width="100%" />
      </td>
     </tr>
    </table>
      <asp:Label ID="UserMess" runat="server" Text="Label"></asp:Label>
		<br />
        <asp:Label ID="Label2" runat="server" Text="Comment:" Font-Size="Medium"></asp:Label> <br />
		<asp:TextBox ID="CMT" TextMode="multiline" Height="300" Width="99%" runat="server"></asp:TextBox>
		<br />
        <br />
        <br />
	</td>

    <asp:SqlDataSource ID="SqlDataSource1" runat="server" 
            ConnectionString="<%$ ConnectionStrings:BST_STATISTICS192.168.0.8 %>" SelectCommand=""></asp:SqlDataSource>

        <asp:GridView ID="GridView1" runat="server" AllowPaging="True" 
            AllowSorting="True" AutoGenerateColumns="False" DataSourceID="SqlDataSource1" 
            EnableModelValidation="True" onrowdatabound="GridView1_RowDataBound" onrowcreated="GridView1_RowCreated"
            PageSize="1000" Width="100%">
            <Columns>
                <asp:TemplateField HeaderText="CB"></asp:TemplateField>
                <asp:TemplateField HeaderText="#"></asp:TemplateField>
                <asp:BoundField DataField="TEST" HeaderText="TEST" ReadOnly="True" 
                    SortExpression="TEST" />
                <asp:BoundField DataField="CASE_NAME" HeaderText="CASE_NAME" ReadOnly="True" 
                    SortExpression="CASE_NAME" />
                <asp:TemplateField HeaderText="COMMENT "></asp:TemplateField>
                <asp:BoundField DataField="DB" HeaderText="DB" ReadOnly="True" 
                    SortExpression="DB" />
                <asp:BoundField DataField="EX1" HeaderText="EX1" SortExpression="EX1" />
                <asp:BoundField DataField="DBE1" HeaderText="DBE1" SortExpression="DBE1" />
                <asp:BoundField DataField="ERR1" HeaderText="ERR1" SortExpression="ERR1" />
                <asp:BoundField DataField="OE1" HeaderText="OE1" SortExpression="OE1" />
                <asp:BoundField DataField="W1" HeaderText="W1" SortExpression="W1" />
                <asp:BoundField DataField="DUR1" HeaderText="DUR1" SortExpression="DUR1" />
                <asp:TemplateField HeaderText="HASH1"></asp:TemplateField>
                <asp:TemplateField HeaderText="HASH2"></asp:TemplateField>
                <asp:BoundField DataField="EX2" HeaderText="EX2" SortExpression="EX2" />
                <asp:BoundField DataField="DBE2" HeaderText="DBE2" SortExpression="DBE2" />
                <asp:BoundField DataField="ERR2" HeaderText="ERR2" SortExpression="ERR2" />
                <asp:BoundField DataField="OE2" HeaderText="OE2" SortExpression="OE2" />
                <asp:BoundField DataField="W2" HeaderText="W2" SortExpression="W2" />
                <asp:BoundField DataField="DUR2" HeaderText="DUR2" SortExpression="DUR2" />
                <asp:BoundField DataField="ORD" HeaderText="ORD" ReadOnly="True" 
                    SortExpression="ORD" ApplyFormatInEditMode="True" Visible="False" />
                <asp:BoundField DataField="LINK1" HeaderText="LINK1" InsertVisible="False" 
                    SortExpression="LINK1" Visible="False" />
                <asp:BoundField DataField="LINK2" HeaderText="LINK2" SortExpression="LINK2" 
                    Visible="False" />
            </Columns>
            <PagerSettings PageButtonCount="30" Position="Top" />
        </asp:GridView>
</asp:Content>

