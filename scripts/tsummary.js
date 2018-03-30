function RebuildLinks() {
	var ver = $(".versionlist").val();
	$(".pagetable tbody tr").each(function (i, obj) {
		var cell = $(this).find('td:eq(1)');
		var test = cell.text();
		cell.html("<a href='runs.aspx?V.VERSION=" + ver + "&T.TEST_NAME=" + test + "'>" + test + "</a>");
	});
}
$(function () {
	RebuildLinks();
	$(".versionlist").change(function () {
		RebuildLinks();
	})
})