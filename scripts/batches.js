var initText = "";
var batchid = "";
var batchupdater = "";

function closeEditor() {
	var newText = $("#editor").val();
	if (initText != newText) {
		var answer = confirm("The data have been changed. Would you like to save changes?");
		if (answer) {
			getdata(batchupdater, JSON.stringify({ "id": batchid, "text": newText }), function (mess) {
				window.location.reload();
			})
		}
	}
	var modal = document.getElementById('modal');
	modal.style.display = "none";
}
function editValue(calee, updater) {
	var modal = document.getElementById('modal');
	modal.style.display = "block";
	initText = calee.html();
	batchupdater = updater;
	batchid = calee.parent().attr("rowid");
	$editor = $("#editor");
	$editor.val(initText);
	$editor.focus();
}
$(function () {
	$('table tbody tr td:nth-child(3)').click(function () {
		editValue($(this), "SetBatchData");
	});
	$('table tbody tr td:nth-child(2)').click(function () {
		editValue($(this), "SetBatchName");
	});
	$('table tbody tr td:nth-child(1)').click(function () {
		var answer = confirm("Are you sure you want to delete selected batch?");
		if (answer) {
			batchid = $(this).parent().attr("rowid");
			getdata("DeleteBatch", JSON.stringify({ "id": batchid }), function (mess) {
				window.location = removeUrlParam(location.href, "id");
			})
		}
	});
	$("#closeeditor").click(function () {
		closeEditor();
	});
	$("#addnew").click(function () {
		var name = prompt("Please enter batch name", "Harry Potter");
		getdata("AddBatch", JSON.stringify({ "name": name }), function (mess) {
			window.location = replaceUrlParam(location.href, "id", mess.d);
		})
	});
	$("#editor").keyup(function (event) {
		if (event.which == 27) {
			event.preventDefault();
			closeEditor();
		}
	});
})