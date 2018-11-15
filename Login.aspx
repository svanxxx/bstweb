<%@ Page Title="Login" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="login.aspx.cs" Inherits="Login" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<meta property="og:title" content="<%=SecurityPage.GetPageOgTitle()%>">
	<meta property="og:description" content="Click to see more details">
	<meta property="og:image" content="<%=SecurityPage.GetPageOgImage()%>">
	<meta property="og:site_name" content="<%=SecurityPage.GetPageOgName()%>">
	<meta property="og:type" content="website">
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<div class="row">
		<div class="col-sm-4"></div>
		<div class="col-sm-4">
			<h2>MPS testing system</h2>
			<p>Please enter user name and password (you can use domain format: user@mps and password)</p>
			<div class="row">
				<div class="col-sm-3"></div>
				<div class="col-sm-6">
					<img src="images/img_avatar.png" alt="Avatar" style="width: 30%; margin: auto; display: block;">
					<div class="form-group">
						<label for="usr">Name:</label>
						<asp:TextBox runat="server" class="form-control" ID="usr"></asp:TextBox>
					</div>
					<div class="form-group">
						<label for="pwd">Password:</label>
						<asp:TextBox runat="server" TextMode="Password" class="form-control" ID="pwd"></asp:TextBox>
					</div>
					<h3>
						<asp:Label runat="server" ID="message"></asp:Label></h3>
					<input type="submit" class="btn btn-info" value="Login">
					<asp:CheckBox runat="server" ID="keeplogged" Text="Keep me logged in" />
				</div>
				<div class="col-sm-3"></div>
			</div>
		</div>
		<div class="col-sm-4"></div>
	</div>
</asp:Content>
