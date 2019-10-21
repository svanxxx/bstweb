function ProcessTableData() {
	var heads = $('.pagetable thead tr:eq(0) th');
	var rows = $('.pagetable tbody tr')
	for (var h = 0; h < heads.length; h++) {
		var th = $(heads[h]);
		for (var r = 0; r < rows.length; r++) {
			var cell = $('.pagetable tbody tr:eq(' + r + ') td:eq(' + h + ')');
			var requestid = $(rows[r]).attr("requestid");
			var column = th.text();
			if (column == "F") {
				var download = "\\\\storage2\\ReleaseSocket\\Stack\\" + $(rows[r]).attr("guid") + "\\Release.zip";
				cell.append("<a href='getfile.aspx?Path=" + download + "'><img title='" + download + "' src='images/save.GIF'></img></a>");
			}
			else if (column == "C") {
				var download = "\\\\storage2\\ReleaseSocket\\Stack\\" + $(rows[r]).attr("guid") + "\\Modules.zip";
				cell.append("<a href='getfile.aspx?Path=" + download + "'><img title='" + download + "' src='images/save.GIF'></img></a>");
			}
			else if (column == "VERSION") {
				var url = "runs.aspx?R.RequestID=" + requestid;
				var ver = cell.text();
				cell.html("");
				cell.append("<a href='" + url + "'>" + ver + "</a>");
			}
			else if (column == "TTID") {
				var url = "<a class='rectst' title='Recommended tests...' href='FilesStatistics.aspx?R.RequestID=" + requestid + "'>&#8621;</a>";
				var t = cell.html();
				cell.html("");
				cell.html(parseTTLink(t) + url);
			}
			else if (column == "COMMENT") {
				var t = cell.html();
				cell.html("");
				cell.html(parseTTLink(t));
			}
			else if (column == "PROGABB") {
				var url = replaceUrlParam(window.location.href, "PROGABB", cell.text());
				var ver = cell.text();
				cell.html("");
				cell.append("<a href='" + url + "'>" + ver + "</a>");
			}
			else if (column == "BST") {
				var url = replaceUrlParam(window.location.href, "BST", cell.text());
				var val = cell.text();
				if (!val) {
					$(rows[r]).css("background-color", "yellow");
				}
				cell.html("");
				cell.append("<a href='" + url + "'>" + val + "</a>");
			}
			else if (column == "SCHED") {
				var url = replaceUrlParam("Sequence.aspx", "RequestID", requestid);
				url = replaceUrlParam(url, "BackUrl", encodeURIComponent(window.location.href));
				var btn = "<a style='width:100%' class='btn btn-info btn-xs' href='" + url + "'>Edit</a>"
				cell.append(btn);
			}
			else if (column == "IGNORE") {
				var t = cell.text();
				var cap = (t) ? "unignore" : "IGNORE";
				var sty = (t) ? "btn-warning" : "btn-info";

				var $but = $("<button style='padding:0; width:100%' type='button' class='btn " + sty + " btn-xs'>" + cap + "</button>");
				cell.html("");
				cell.append($but);
				$but.click(function () {
					if (!IsAdmin()) {
						alert("Unknown users or not administrators cannot change the data. Please login as domain user or get admin rights.");
						return;
					}
					var mess = $(this).text();
					var ignore = mess == "IGNORE";
					var answer = ignore ? confirm("Are you sure you want to ignore test request?") : confirm("Are you sure you want to UNignore test?");
					if (!answer) {
						return;
					}
					var va = ignore ? "1" : "0";
					var id = $($($(this).parent()).parent()).attr("requestid");
					getdata("IgnoreRequest", JSON.stringify({ "id": id, "str1or0": va }), function () {
						window.location.reload();
					});
				})
			}
			else if (th.text() == "TESTER") {
				var t = cell.text();
				if (!t) {
					var $but = $("<button style='padding:0; width:100%' type='button' class='btn btn-danger btn-xs'>I will test!</button>");
					cell.html("");
					cell.append($but);
					$but.click(function () {
						if (!IsAdmin()) {
							alert("Unknown users or not administrators cannot change the data. Please login as domain user or get admin rights.");
							return;
						}
						var id = $($($(this).parent()).parent()).attr("requestid");
						var answer = confirm("Are you sure you want to mark test request as handled manually by you?");
						if (!answer) {
							return;
						}
						getdata("TestRequestManually", JSON.stringify({ "id": id }), function () {
							window.location.reload();
						});
					})
				}
			}
			else if (th.text() == "TESTED") {
				var t = cell.text();
				var cap = (t) ? "untest" : "TESTED";
				var sty = (t) ? (t == "False" ? "btn-danger" : "btn-success") : "btn-info";
				var $but = $("<button style='padding:0; width:100%' type='button' class='btn " + sty + " btn-xs'>" + cap + "</button>");
				cell.html("");
				cell.append($but);
				$but.click(function () {
					if (!IsAdmin()) {
						alert("Unknown users or not administrators cannot change the data. Please login as domain user or get admin rights.");
						return;
					}
					var mess = $(this).text();
					var id = $($($(this).parent()).parent()).attr("requestid");
					if (mess == "TESTED") {
						var url = replaceUrlParam("CommentTestRun.aspx", "RequestID", id);
						url = replaceUrlParam(url, "BackUrl", encodeURIComponent(window.location.href));
						window.location.href = url;
					}
					else {
						var answer = confirm("Are you sure you want to mark test request as UNTESTED?");
						if (!answer) {
							return;
						}
						getdata("UntestRequest", JSON.stringify({ "id": id }), function () {
							window.location.reload();
						});
					}
				})
			}
		}
	}
}
function handleShowAll() {
	var sh = getParameterByName("showall");
	if (sh == "1") {
		$("#showall").text("Show unprocessed");
	} else {
		$("#showall").text("Show all");
	}
	$("#showall").click(function () {
		var sh = getParameterByName("showall");
		if (sh == "1") {
			window.location = replaceUrlParam(window.location.href, "showall", "0");
		} else {
			window.location = replaceUrlParam(window.location.href, "showall", "1");
		}
	})
}
var _lastrequest;
function lastrequest(event) {
	if (!_lastrequest) {
		_lastrequest = event.data;
		return;
	}
	if (_lastrequest != event.data) {
		window.location.reload();
	}
}
$(function () {
	addBSTSignalCallback("lastrequest", lastrequest);
	ProcessTableData();

	//shifting table to the left in case it is out of page
	$(".pagetable").removeClass("renderhide");

	var leftmarg = parseInt($(".pagetable").parent().css("margin-left"));
	var tablew = $(".pagetable").outerWidth(true);
	var divw = $(".pagetable").parent().outerWidth(true);
	leftmarg = Math.max((divw - tablew) / 2, 0);
	$(".pagetable").parent().css("margin-left", leftmarg + "px");
	if (leftmarg < 1) {
		$(".pagetable").parent().css("padding-left", "0");
	}

	handleShowAll();
})