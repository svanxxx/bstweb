<%@ Control Language="C#" AutoEventWireup="true" CodeFile="BstMenuControl.ascx.cs" Inherits="BstMenu" Debug="true" %>


    <%-- JavaScript. --%>
<script src='Scripts/post_url1.js' type='text/javascript'></script>
<script src='Scripts/jquery-1.4.1.min.js' type='text/javascript'></script>

<script type="text/javascript">

    var ColorIndex = 2;
    var bChangeColor = 0;
    var count = "0";

    var cFirstColor = 'transparent';
    var cFlickColor = 'Red';


    window.onload = initialize();

    function initialize() {

        CheckFipVersion();
     setInterval('CheckFipVersion()', 10000);
     setInterval('FlickeringVersion()', 1000);

  //   div = document.getElementsByClassName('TestRequests')[0];
  //   cFirstColor = div.style.backgroundColor();

 }

 function FlickeringVersion() {
  var div = document.getElementsByClassName('TestRequests')[0];
  if (bChangeColor) {
      if (ColorIndex % 2 == 0) {
          div.style.backgroundColor = cFlickColor;
          div.innerHTML = "Requests (" + count + ")"
      }
      else div.style.backgroundColor = cFirstColor;
      ColorIndex = ColorIndex + 1;
  }
  else {
      div.style.backgroundColor = cFirstColor;
      div.innerHTML = "Requests";
  }
 }

 function CheckFipVersion() {


     $.ajax({
         type: "POST",
         url: "WebService.asmx/GetTestRequests",
         contentType: "application/json; charset=utf-8",
         dataType: "json",
         success: function (response) {
             strResponse = response.d;
             var arrRes = strResponse.split(",")


             if (arrRes[0] != "0") { bChangeColor = 1; cFlickColor = 'Red'; count = arrRes[0]; }
             else if (arrRes[1] != " 0") { bChangeColor = 1; cFlickColor = 'Blue'; count = arrRes[1].replace(" ", ""); }
             else bChangeColor = 0;

         },
         error: function (xhr, ajaxOptions, thrownError) { /*alert("Error:" + xhr.responseText); */ }
     });
 }

</script>

<asp:Table ID="HeaderTable" runat="server" GridLines="Both" 
	HorizontalAlign="Center" Width="100%" Height="20pt" ForeColor="#003300">
	<asp:TableRow runat="server" Height="20pt">
		<asp:TableCell runat="server" Width="14.28%" Wrap="False" 
		HorizontalAlign="Center" BorderStyle="Groove" BorderWidth="5" VerticalAlign="Middle" BackColor="Black" ForeColor="White">
			<asp:Panel ID="Panel1" runat="server" BackImageUrl="../IMAGES/tabselected.JPG">
				<asp:LinkButton ID="LinkButton1" runat="server" Font-Bold="True">Bst Home</asp:LinkButton></asp:Panel>	
		</asp:TableCell>
	</asp:TableRow>
</asp:Table>