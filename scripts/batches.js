var initText = "";
var batchid = "";

function closeEditor() {
	var newText = $("#editor").val();
	if (initText != newText) {
		var answer = confirm("The data have been changed. Would you like to save changes?");
		if (answer) {
			getdata("SetBatchData", JSON.stringify({ "id": batchid, "text": newText }), function (mess) {
				window.location.reload();
			})
		}
	}
	var modal = document.getElementById('modal');
	modal.style.display = "none";
}

$(function () {
	$('table tbody tr td:nth-child(3)').click(function () {
		var modal = document.getElementById('modal');
		modal.style.display = "block";
		initText = $(this).html();
		batchid = $(this).parent().attr("rowid");
		$("#editor").val(initText);
	});
	$(".close").click(function () {
		closeEditor();
	});
	$("#editor").keyup(function (event) {
		if (event.which == 27) {
			event.preventDefault();
			closeEditor();
		}
	});
})