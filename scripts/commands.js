$(function () {
	$("#savebutton").click(function () {
		var answer = confirm("Are you sure you want to save all commands?");
		if (!answer) {
			return;
		}
		document.aspnetForm.submit();
	});
})