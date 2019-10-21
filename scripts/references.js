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
function userName() {
	return document.getElementById("username").value;
}
function killTooltips() {
	setTimeout(function () {
		var elements = document.getElementsByClassName("tooltip");
		while (elements.length > 0) {
			elements[0].parentNode.removeChild(elements[0]);
		}
	}, 500);
}
function reActivateTooltips() {
	killTooltips();
	setTimeout(function () { $('[data-toggle="tooltip"]').tooltip({ html: true, container: "body" }); }, 1000);//when data loaded - activate tooltip.
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