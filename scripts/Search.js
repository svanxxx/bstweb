var idCommandField = ""
function AddTest(strTestCMD) {
	var strTest = strTestCMD;
	strTest = strTest.replace(new RegExp("`", 'g'), '"')
	var oldtext = $(".testsequencetext").val();
	$(".testsequencetext").val(oldtext + strTest + "\n");
}
function SearchBar_Off() {
	var div = document.getElementById('base');
	div.style.visibility = 'hidden';
}
function SearchBar(eSearch, idField) {

	// get id Command Field
	idCommandField = idField;

	var strText = eSearch.value;
	var div = document.getElementById('base');

	div.innerHTML = "Loading...";

	var length = strText.length;

	if (length < 2) { div.style.visibility = 'hidden'; }
	else {
		div.style.visibility = 'visible';
		div.style.marginLeft = '45px';
		div.style.marginTop = '100px';

		getdata("GetCommands", JSON.stringify({ "strSearch": strText }), function (mess) {
			strResponse = mess.d;
			document.getElementById('base').innerHTML = strResponse;
		})
	}
}
