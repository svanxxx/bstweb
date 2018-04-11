<%@ Page Title="Batches" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="batches.aspx.cs" Inherits="Batches" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
    <link href="css/batches.css" rel="stylesheet" />
    <script src="scripts/batches.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<div class="alert alert-info">
		<strong>Info:</strong> Click on the batch data cell to edit the value.
	</div>
    <div id="modal" class="modal">
        <div class="modal-content">
            <span class="close">&times;</span>
            <textarea id="editor"></textarea>
        </div>
    </div>
    <asp:Table ID="TTable" runat="server" class="pagetable table table-bordered table-responsive table-striped table-hover" EnableViewState="False">
    </asp:Table>
    <div class="row">
        <div class="col-sm-6">
            <ul class="pagination">
            </ul>
        </div>
        <div class="col-sm-6">
            <div class="col-sm-3">
                <label for="showby">Show by:</label>
            </div>
            <div class="col-sm-3">
                <select class="form-control" id="showby">
                    <option>30</option>
                    <option>60</option>
                    <option>150</option>
                    <option>300</option>
                </select>
            </div>
        </div>
    </div>
</asp:Content>
