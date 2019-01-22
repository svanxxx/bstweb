var _callsettname = "JColResizer0";
function getTabKey() {
	return GetPageName() + _callsettname;
}
$(function () {
	var tables = $(".table-colresizable");
	if (tables.length > 0) {
		sessionStorage[_callsettname] = localStorage[getTabKey()];
		tables.colResizable({
			liveDrag: true,
			postbackSafe: true,
			onResize: function (target) {
				setTimeout(function () {
					localStorage[getTabKey()] = sessionStorage[_callsettname];
				}, 1000);
			}
		});
	}
});