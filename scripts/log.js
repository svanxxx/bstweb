$(function () {
	$("#showresults").click(function () {
		var inputDate = new Date($("#logdate").val());
		window.location.href = GetPageName() + "?date=" + DateToString(inputDate) + "&thoster=" + $(".thosterlist").val();
	})

	var d = getParameterByName("date");
	if (!d) {
		d = DateToString(new Date());
	}

	var t = getParameterByName("thoster");
	if (t) {
		$(".thosterlist").val(t);
	}

	var date = StringToDate(d);
	var day = ("0" + date.getDate()).slice(-2);
	var month = ("0" + (date.getMonth() + 1)).slice(-2);
	var today = date.getFullYear() + "-" + (month) + "-" + (day);
	$("#logdate").val(today)
})