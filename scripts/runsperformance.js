$(function () {
	$("circle").each(function () {
		var t = document.createElementNS("http://www.w3.org/2000/svg", "title")
		t.textContent = "date: " + this.dataset.x + " duration: " + this.dataset.y;
		this.appendChild(t)
	});

	$('circle').click(function () {
		var el = $(this)[0];
		getdata2("GetTestRunUrl", JSON.stringify({ "id": el.dataset.info }), function (result) {
			window.location.href = result.d;
		})
	});

	$(".graph").height(0.8*(window.innerHeight - $(".navbar").height()));

	window.onresize = function () {
		$(".graph").height(0.8 * (window.innerHeight - $(".navbar").height()));
	}
})