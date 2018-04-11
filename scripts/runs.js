var selclass = "bstselectedrow";

function rerun(e) {
	var user = $(".bstusername").text();

	var img = $(event.target);
	var td = img.parent();
	var command = td.attr("title");
	var row = td.parent();
	var runid = row.attr("runid");
	var requestid = row.attr("requestid");
	AskRerun(requestid, command, user, runid);
}
function CheckIgnoreEnable() {
	if ($("." + selclass).length > 0) {
		$("#testactions").css("display", "block");
	} else {
		$("#testactions").css("display", "none");
	}
}
function GetSelectedRows() {
	var commaSeparatedIDs = "";
	$("." + selclass).each(function () {
		commaSeparatedIDs += $(this).attr("runid") + ",";
	})
	if (commaSeparatedIDs.length > 0) {
		commaSeparatedIDs = commaSeparatedIDs.substring(0, commaSeparatedIDs.length - 1);
	}
	return commaSeparatedIDs;
}
function CommentSelected() {
	var commaSeparatedIDs = GetSelectedRows();
	if (commaSeparatedIDs == "") {
		return;
	}

	var mess = prompt("Please enter your comment (set it 'empty' to clean all comments from all users):", "empty");
	if (!mess) {
		return;
	}

	var fproc = StartProgress("Updating tests...")
	getdata("CommentTests", JSON.stringify({ "commaSeparatedIDs": commaSeparatedIDs, "mess": mess }), function (mess) {
		EndProgress(fproc);
		if (mess.d != "OK") {
			alert(mess.d)
		}
		else {
			location.reload();
			StartProgress("Reloading data...")
		}
	})
}
function ProcessSelected(func) {
	var commaSeparatedIDs = GetSelectedRows();
	if (commaSeparatedIDs == "")
		return;

	if (!confirm("Are you sure you want to " + func + " tests: " + commaSeparatedIDs + "?"))
		return;

	var fproc = StartProgress("Updating tests...")
	getdata(func, JSON.stringify({ "commaSeparatedIDs": commaSeparatedIDs }), function (mess) {
		EndProgress(fproc);
		if (mess.d != "OK") {
			alert(mess.d)
		}
		else {
			location.reload();
			StartProgress("Reloading data...")
		}
	})
}
function MarkCommentsWithImg() {
	var table = $(".pagetable")[0];
	var vcol = -1;
	for (var ic = 0; ic < table.rows[0].cells.length; ic++) {
		if (table.rows[0].cells[ic].innerHTML == "Comment") {
			vcol = ic;
			break;
		}
	}
	for (var ir = 1; vcol > -1 && ir < table.rows.length; ir++) {
		var ctext = table.rows[ir].cells[vcol].innerHTML;
		if (ctext.indexOf(":") > -1) {
			var iname = ctext.substring(0, ctext.indexOf(":"));
			var img = "<img title='" + iname + "'class='imgpers' src='images/persons/" + iname + ".jpg'></img>";
			table.rows[ir].cells[vcol].innerHTML = img + ctext.substring(ctext.indexOf(":"), ctext.length);
		}
	}
}
function ProcessTableData() {
	var heads = $('.pagetable thead tr:eq(0) th');
	var rows = $('.pagetable tbody tr')
	for (var h = 0; h < heads.length; h++) {
		var th = $(heads[h]);
		var filter = th.attr("filter");
		var filtertype = th.attr("filtertype");

		if (filter && filtertype == "string") {
			for (var r = 0; r < rows.length; r++) {
				var cell = $('.pagetable tbody tr:eq(' + r + ') td:eq(' + h + ')');
				var text = cell.text();
				cell.text("");
				cell.append("<a href='?" + filter + "=" + text + "&R.REPEATED=<>2'>&lt;.&gt;</a>");
				var url = replaceUrlParam(location.href, filter, text);
				cell.append("<a href='" + url + "'>" + text + "</a>");
			}
		}
		else if (filter && filtertype == "int") {
			th.addClass("cellfilter");
			if (location.href.indexOf(filter) != -1) {
				th.css("background-color", "#ff00004f");
				th.css("color", "black");
			} else {
				th.css("background-color", "#0000ff42");
				th.css("color", "white");
			}

			th.click(function (event) {
				var h = $(event.target);
				var filter = h.attr("filter");
				if (location.href.indexOf(filter) != -1) {
					window.location = removeUrlParam(filter, location.href);
				} else {
					window.location = replaceUrlParam(location.href, filter, "<>0");
				}
			});
		}
	}
}
$(function () {
	ProcessTableData();
	MarkCommentsWithImg();

	//shifting table to the left in case it is out of page
	$(".pagetable").removeClass("renderhide");
	var leftmarg = parseInt($(".pagetable").parent().css("margin-left"));
	var tablew = $(".pagetable").outerWidth(true);
	var divw = $(".pagetable").parent().outerWidth(true);
	leftmarg = Math.max((divw - tablew) / 2, 0);
	$(".pagetable").parent().css("margin-left", leftmarg + "px");

	var g = getParameterByName("R.REPEATED");
	if (!g || g == "0") {
		$('#grouptests').text("Ungroup Results");
	}
	
	$('#grouptests').click(function () {
		var g = getParameterByName("R.REPEATED");
		if (!g) {
			g = "0";
		}
		if (g == "0") {
			g = "<>2"
		} else {
			g = "0"
		}
		window.location = replaceUrlParam(location.href, "R.REPEATED", g);
	})

	$(".pagetable thead tr th:first-child").click(function (event) {
		if ($("." + selclass).length > 0) {
			$(".pagetable tbody tr td:first-child").each(function () {
				var row = $($(this).parent());
				row.removeClass(selclass);
			})
		} else {
			$(".pagetable tbody tr td:first-child").each(function () {
				var row = $($(this).parent());
				row.addClass(selclass);
			})
		}
		CheckIgnoreEnable();
	});

	$(".pagetable tbody tr td:first-child").click(function (event) {
		var td = $(event.target);
		var row = $(td.parent());
		if (row.hasClass(selclass)) {
			row.removeClass(selclass);
		} else {
			row.addClass(selclass);
		}
		CheckIgnoreEnable();
	});
	$(".pagetable thead tr th:nth-child(18)").click(function (event) {
		var $rows = $(".hashspan");
		if ($rows.length > 0) {
			$rows.removeClass("hashspan");
			$rows.addClass("hashspanfull");
		} else {
			$rows = $(".hashspanfull");
			$rows.removeClass("hashspanfull");
			$rows.addClass("hashspan");
		}
	});
	$("#ignorebutton").click(function () {
		ProcessSelected("IgnoreTests")
	})
	$("#commentbutton").click(function () {
		CommentSelected()
	})
	$("#verifybutton").click(function () {
		ProcessSelected("VerifyTests")
	})
	$("#performance").click(function () {
		var url = window.location.href;
		window.location.href = url.replace(GetPageName(), "runsperformance.aspx");
	})
})