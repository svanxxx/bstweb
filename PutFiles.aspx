<%@ Page ValidateRequest="false" Title="Put File(s) as Etalon" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="PutFiles.aspx.cs" Inherits="PutFiles" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
	<div>
		<b>Branch: </b>
		<asp:Label ID="Branch" runat="server" Text="Label"></asp:Label>
		<br />
		<b>User Name: </b>
		<asp:Label ID="UserMess" runat="server" Text="Label"></asp:Label>
		<br />
		<b>
			<asp:Label ID="LabelMessage" runat="server" Text="Message:"></asp:Label></b>
		<asp:Label ID="LabelMsg" runat="server" ForeColor="Green" Text=""></asp:Label>
		<asp:Label ID="LabelErrorMessage" runat="server" ForeColor="Red" Text=""><br/></asp:Label>
		<asp:TextBox ID="TextAreaMessage" TextMode="multiline" Height="100" Width="400" runat="server" /><br />
		<asp:Button ID="Button1" runat="server" Text="Commit & Push" OnClick="Button_Click" />
		<asp:Label ID="LabelInformation" runat="server" Text=""><br/></asp:Label>
		<asp:Label ID="LabelError" runat="server" ForeColor="Red" Text=""></asp:Label>
		<asp:Label ID="LabelFile" runat="server" Text="Label"></asp:Label>
		<asp:Label ID="LabelErrorFile" runat="server" ForeColor="Red" Text=""></asp:Label>
		<asp:CheckBoxList ID="lstFilesCheckBox" runat="server"></asp:CheckBoxList>
		<br />
		<asp:Label ID="LabelDependentFiles" runat="server" Text=""></asp:Label>
		<asp:HiddenField ID="OriginalData" runat="server" />
	</div>
</asp:Content>
