<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ShowFile.aspx.cs" Inherits="ShowFile" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Show file</title>
	<script src="http://mps.resnet.com/cdn/jquery/jquery-3.2.1.min.js"></script>
	<script src="http://mps.resnet.com/cdn/mpshelper.js"></script>
	<script src="scripts/common.js"></script>
	<script>
		$(function () {
			var f1proc = StartProgress("Loading file...")
			getdata("GetFile", JSON.stringify({ "filename": getParameterByName("file") }), function (mess) {
				EndProgress(f1proc);
				document.open();
				document.write(mess.d.replace(/(?:\r\n|\r|\n)/g, '<br />'));
				document.close();
			})
		})
	</script>
</head>
<body>
	<form id="form1" runat="server">
		<div>
		</div>
	</form>
</body>
</html>
