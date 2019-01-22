var _admin = undefined;
function IsAdmin() {
	if (typeof _admin === "undefined") {
		_admin = document.getElementById("isadmin").value;
	}
	return _admin === "True";
}
function userID() {
	return parseInt(document.getElementById("userid").value);
}
function reActivateTooltips() {
	setTimeout(function () { $('[data-toggle="tooltip"]').tooltip(); }, 2000);//when data loaded - activate tooltip.
}
function copyurl() {
	var $temp = $("<input>");
	$("body").append($temp);
	$temp.val(window.location.href).select();
	document.execCommand("copy");
	$temp.remove();
	var p = StartProgress("The link has been copied.");
	setTimeout(function () { EndProgress(p); }, 2000);
}
$(function () {
	reActivateTooltips();
});