<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CompareVersion.aspx.cs" MasterPageFile="~/MasterPage.master" Inherits="CompareVersion" Title="Compare Version" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<script src="Scripts/RunTest.js" type="text/javascript"></script>

<script type="text/javascript">

window.onload = initializeCompareVersion();
var strVer1 = "", strVer2 = "";
function initializeCompareVersion() {
     setInterval('UpdateTable()', 30000);
 }

 var shifted = false;
 var strIDs = "";
 $(document).bind('keyup keydown', function (e) { shifted = e.shiftKey });

 function Add_TT(Cell, RUN_ID) {
         if (shifted) { // Shift was pressed
             if (Cell.style.backgroundColor != 'green') { Cell.style.backgroundColor = 'green'; strIDs = strIDs + RUN_ID + ','; }
             else { Cell.style.backgroundColor = 'white'; strIDs = strIDs.replace(RUN_ID + ',', ''); }
         }
         else {
             var strUrl = escape(document.URL.toString());
             if (strIDs == "") { document.location.href = 'CommentRun.aspx?RUNID=' + RUN_ID + "&BackUrl=" + strUrl; }
             else { document.location.href = 'CommentRun.aspx?RUNID=' + strIDs + "&BackUrl=" + strUrl; }
         }
 }

 function ColorCell(Cell, strColor) {
     if (Cell.style.backgroundColor != 'green') {
      if (strColor == 'white') { Cell.style.cursor = 'pointer'; Cell.style.textDecoration = 'underline'; }
       else {Cell.style.textDecoration = 'none';};
     Cell.style.backgroundColor = strColor; };
 }

var OldValueResponse = "";

function UpdateDiv() {

    var e = document.getElementById("ctl00_ContentPlaceHolder1_DropDownListVersion1");
    var strVer1 = e.options[e.selectedIndex].value;

    e = document.getElementById("ctl00_ContentPlaceHolder1_DropDownListVersion2");
    var strVer2 = e.options[e.selectedIndex].value;

    e = document.getElementById("ctl00_ContentPlaceHolder1_DropDownListRequests");
    var strRequest = "";

    if (e != null) {
        strRequest = e.options[e.selectedIndex].value;
    }

    document.getElementById('NewsDiv').innerHTML = "<img src='IMAGES/Circle.GIF'/>"
   // document.getElementById('NewsDiv').style.width = '1000px';
 
 
            $.ajax({
                type: "POST",
                url: "WebService.asmx/WhatBetweenVersions",
                data: '{strVersionNew:"' + strVer1 + '",strRequest:"' + strRequest + '",strVersionOld:"' + strVer2 + '" }',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    strResponse = response.d;
                    // alert(strResponse);
                    document.getElementById('NewsDiv').innerHTML = strResponse;

                },
                error: function (xhr, ajaxOptions, thrownError) { /* alert("Error:" + xhr.responseText);*/ }
            });

}

function UpdateTable() {

    var e = document.getElementById("ctl00_ContentPlaceHolder1_DropDownListVersion1");
    var strVer1 = e.options[e.selectedIndex].value;

    e = document.getElementById("ctl00_ContentPlaceHolder1_DropDownListVersion2");
    var strVer2 = e.options[e.selectedIndex].value;


    $.ajax({
        type: "POST",
        url: "WebService.asmx/GetCompareVersionInfo",
        data: '{strVersion1:"' + strVer1 + '",strVersion2:"' + strVer2 + '" }',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            strResponse = response.d;
            if (OldValueResponse == "") OldValueResponse = strResponse;
            if (OldValueResponse != strResponse) {
                OldValueResponse = strResponse
                window.location.reload()
            }
        },
        error: function (xhr, ajaxOptions, thrownError) { /* alert("Error:" + xhr.responseText); */ }
    });
            }
   
 

</script>

  <td>
    <div>
        <table style="width: 100%; text-align:center">
            <tr>
                <td style="width: 33%">
                    <asp:DropDownList ID="DropDownListVersion1" runat="server"></asp:DropDownList>
                    <asp:DropDownList ID="DropDownListRequests"  runat="server"> </asp:DropDownList>
                </td>
                <td style="width: 33%">
                    <asp:Button ID="Compare" runat="server" Text="Compare" 
                        onclick="Compare_Click" />
                    <input id="ButtonNew" type="button" value="< What's new >" onclick="UpdateDiv()" />
                </td>

                <td style="width: 33%">
                    <asp:DropDownList ID="DropDownListVersion2" runat="server"></asp:DropDownList>
                </td>
            </tr>
            
        </table> <br />
        <div id="NewsDiv" style="width: 100%;"> </div>
 
        <asp:SqlDataSource ID="SqlDataSource1" runat="server" 
            ConnectionString="<%$ ConnectionStrings:BST_STATISTICS192.168.0.8 %>" SelectCommand="SELECT CASELIST.CNAME AS CASE_NAME,
	(
		SELECT TOP 1 TEST_NAME
		FROM TESTS
		WHERE ID = CASELIST.TESTID
		) AS TEST,
	(
		SELECT TOP 1 DBTYPE
		FROM DBTYPES
		WHERE ID = CASELIST.DBTID
		) AS DB,
	VER_LEFT.TEST_EXCEPTIONS AS EX1,
	VER_LEFT.TEST_DBERRORS AS DBE1,
	VER_LEFT.TEST_OUTPUTERRORS AS OE1,
	VER_LEFT.TEST_WARNINGS AS W1,
	VER_LEFT.TEST_ERRORS AS ERR1,
	VER_LEFT.TEST_DURATION AS DUR1,
	VER_LEFT.DOCLINK AS LINK1,
	VER_RIGHT.TEST_DBERRORS AS DBE2,
	VER_RIGHT.TEST_EXCEPTIONS AS EX2,
	VER_RIGHT.TEST_OUTPUTERRORS AS OE2,
	VER_RIGHT.TEST_WARNINGS AS W2,
	VER_RIGHT.TEST_ERRORS AS ERR2,
	VER_RIGHT.TEST_DURATION AS DUR2,
	VER_RIGHT.DOCLINK AS LINK2,
	VER_LEFT.RUN_HASH AS HASH1,
	VER_RIGHT.RUN_HASH AS HASH2,
	CASE 
		WHEN (
				VER_LEFT.RUN_HASH IS NOT NULL
				AND VER_RIGHT.RUN_HASH IS NOT NULL
				)
			THEN 0
		WHEN (
				VER_LEFT.RUN_HASH IS NULL
				AND VER_RIGHT.RUN_HASH IS NOT NULL
				)
			THEN 1
		WHEN (
				VER_LEFT.RUN_HASH IS NOT NULL
				AND VER_RIGHT.RUN_HASH IS NULL
				)
			THEN 2
		ELSE 3
		END AS ORD
FROM (
	(
		SELECT DISTINCT TEST_ID TESTID,
			TEST_CASE_ID CASEID,
			DBTYPE_ID DBTID,
			(select TEST_CASE_NAME from TEST_CASES where TEST_CASES.ID = TESTRUNS.TEST_CASE_ID) CNAME
		FROM TESTRUNS
		WHERE DBTYPE_ID IN (
				2,
				3
				)
			AND TEST_CASE_ID IS NOT NULL
		) AS CASELIST LEFT OUTER JOIN (
		SELECT TEST_DURATION,
			TEST_ERRORS,
			TEST_EXCEPTIONS,
			TEST_WARNINGS,
			TEST_OUTPUTERRORS,
			TEST_CASE_ID,
			DBTYPE_ID,
			TEST_FIPVERSIONID,
			RUN_HASH,
			TEST_DBERRORS,
			DOCLINK,
			row_number() OVER (
				PARTITION BY test_case_id,
				DBTYPE_ID ORDER BY test_case_id,
					DBTYPE_ID,
					TEST_RUN_DATE DESC
				) AS RN
		FROM TESTRUNS
		WHERE TEST_FIPVERSIONID = (
				SELECT TOP 1 ID
				FROM FIPVERSION
				WHERE VERSION = '2013.8B.318.45.147'
				)
		) AS VER_LEFT ON VER_LEFT.TEST_CASE_ID = CASELIST.CASEID
		AND VER_LEFT.DBTYPE_ID = CASELIST.DBTID
		AND VER_LEFT.rn = 1
	)
LEFT OUTER JOIN (
	SELECT TEST_DURATION,
		TEST_ERRORS,
		TEST_EXCEPTIONS,
		TEST_WARNINGS,
		TEST_OUTPUTERRORS,
		TEST_CASE_ID,
		DBTYPE_ID,
		TEST_FIPVERSIONID,
		RUN_HASH,
		TEST_DBERRORS,
		DOCLINK,
		row_number() OVER (
			PARTITION BY test_case_id,
			DBTYPE_ID ORDER BY test_case_id,
				DBTYPE_ID,
				TEST_RUN_DATE DESC
			) AS RN
	FROM TESTRUNS
	WHERE TEST_FIPVERSIONID = (
			SELECT TOP 1 ID
			FROM FIPVERSION
			WHERE VERSION = '2013.8B.318.46.148'
			)
	) AS VER_RIGHT ON VER_RIGHT.TEST_CASE_ID = CASELIST.CASEID
	AND VER_RIGHT.DBTYPE_ID = CASELIST.DBTID
	AND VER_RIGHT.rn = 1
ORDER BY ord,
	DB,
	TEST,
	CASE_NAME
"></asp:SqlDataSource>

        <asp:GridView ID="GridView1" runat="server" AllowPaging="True" 
            AllowSorting="True" AutoGenerateColumns="False" DataSourceID="SqlDataSource1" 
            EnableModelValidation="True" onrowdatabound="GridView1_RowDataBound" 
            PageSize="50" Width="100%">
            <Columns>
                <asp:TemplateField HeaderText="#"></asp:TemplateField>
                <asp:BoundField DataField="TEST" HeaderText="TEST" ReadOnly="True" 
                    SortExpression="TEST" />
                <asp:BoundField DataField="CASE_NAME" HeaderText="CASE_NAME" ReadOnly="True" 
                    SortExpression="CASE_NAME" />
                <asp:TemplateField HeaderText="COMMENT "></asp:TemplateField>
                <asp:BoundField HeaderText="RR" />
                <asp:BoundField DataField="DB" HeaderText="DB" ReadOnly="True" 
                    SortExpression="DB" />
                <asp:BoundField DataField="EX1" HeaderText="EX1" SortExpression="EX1" />
                <asp:BoundField DataField="DBE1" HeaderText="DBE1" SortExpression="DBE1" />
                <asp:BoundField DataField="ERR1" HeaderText="ERR1" SortExpression="ERR1" />
                <asp:BoundField DataField="OE1" HeaderText="OE1" SortExpression="OE1" />
                <asp:BoundField DataField="W1" HeaderText="W1" SortExpression="W1" />
                <asp:BoundField DataField="DUR1" HeaderText="DUR1" SortExpression="DUR1" />
                <asp:BoundField DataField="HASH1" HeaderText="HASH1" SortExpression="HASH1" />
                <asp:BoundField DataField="HASH2" HeaderText="HASH2" SortExpression="HASH2" />
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

    </div>
   	
    <asp:HiddenField ID="REPID" runat="server" />
			<center>
				<asp:Label ID="ShowByLabel" runat="server" Text="Show By:"></asp:Label>
				<asp:DropDownList ID="ShowByList" runat="server" AutoPostBack="True" 
					onselectedindexchanged="ShowByList_SelectedIndexChanged">
					<asp:ListItem>10</asp:ListItem>
					<asp:ListItem>20</asp:ListItem>
					<asp:ListItem>50</asp:ListItem>
					<asp:ListItem>100</asp:ListItem>
					<asp:ListItem>150</asp:ListItem>
					<asp:ListItem>200</asp:ListItem>
					<asp:ListItem>250</asp:ListItem>
					<asp:ListItem>300</asp:ListItem>
				</asp:DropDownList>
			</center>

   <td>
</asp:Content>

