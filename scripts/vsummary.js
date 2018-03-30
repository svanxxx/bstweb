$(function () {
	$(".versionrow").click(function () {
		var ver = $($(this).children()[1]).text();
		window.location.href = "runs.aspx?R.REPEATED=<>2&V.VERSION=" + ver;
	});
})