function ShowWaitDlg(bShow) {
	if (bShow !== void 0 && !bShow) {
		$("#waitDlg").empty();
		$("#waitDlg").dialog('destroy');
	}
	else {
		$("<div id='waitDlg'><center><img src='images/process.gif' width='24' style='height: 24px' /><p><h3>Please wait...</h3></p></center></div>").dialog({
			resizable: false,
			closeOnEscape: false,
			draggable: false,
			minHeight: 50,
			minWidth: 50,
			modal: true
		}).siblings('.ui-dialog-titlebar').hide();
	}
}
function parseTTLink(txt) {
	var ttids = txt.match("TT\\d+");
	if (ttids && ttids.length > 0) {
		var ttid = ttids[0];
		var id = ttid.split("TT")[1];
		return txt.replace(ttid, "<a title='" + ttid + "' href='http://mps.resnet.com/taskmanager/showtask.aspx?ttid=" + id + "'>" + ttid + "</a>");
	}
	return txt;
}
//
function senddata(func, params) {
	var webMethod = GetSitePath() + "WebService.asmx/" + func;
	$.ajax({
		type: "POST",
		url: webMethod,
		data: params,
		contentType: "application/json; charset=utf-8",
		dataType: "json",
		error: function (e) {
			$("#systemmessages").text(e.responseText);
		}
	});
}
function getdata(func, params, proc) {
	var webMethod = GetSitePath() + "WebService.asmx/" + func;
	$.ajax({
		type: "POST",
		url: webMethod,
		data: params,
		contentType: "application/json; charset=utf-8",
		dataType: "json",
		success: function (mess) {
			proc(mess);
		},
		error: function (e) {
			$("#systemmessages").text(e.responseText);
		}
	});
}
function getdata2(func, params, proc, fproc) {
	var webMethod = GetSitePath() + "WebService.asmx/" + func;
	$.ajax({
		type: "POST",
		url: webMethod,
		data: params,
		contentType: "application/json; charset=utf-8",
		dataType: "json",
		success: function (mess) {
			proc(mess);
		},
		error: function (e) {
			fproc();
		}
	});
}
function convertDateShort(d) {
	function pad(s) { return (s < 10) ? '0' + s : s; }
	return [pad(d.getDate()), pad(d.getMonth() + 1), d.getFullYear()].join('');
}
function ClearTextSelection() {
	if (window.getSelection) {
		if (window.getSelection().empty) {  // Chrome
			window.getSelection().empty();
		} else if (window.getSelection().removeAllRanges) {  // Firefox
			window.getSelection().removeAllRanges();
		}
	} else if (document.selection) {  // IE?
		document.selection.empty();
	}
}
function exportTableToCSV($table, filename) {

	var $rows = $table.find('tr:has(td):visible'),
		// Temporary delimiter characters unlikely to be typed by keyboard
		// This is to avoid accidentally splitting the actual contents
		tmpColDelim = String.fromCharCode(11), // vertical tab character
		tmpRowDelim = String.fromCharCode(0), // null character

		// actual delimiter characters for CSV format
		colDelim = '","',
		rowDelim = '"\r\n"',

		// Grab text from table into CSV formatted string
		csv = '"' + $rows.map(function (i, row) {
			var $row = $(row),
				$cols = $row.find('td');

			return $cols.map(function (j, col) {
				var $col = $(col),
					text = $col.text();

				return text.replace('"', '""'); // escape double quotes

			}).get().join(tmpColDelim);

		}).get().join(tmpRowDelim)
			.split(tmpRowDelim).join(rowDelim)
			.split(tmpColDelim).join(colDelim) + '"',

		// Data URI
		csvData = 'data:application/csv;charset=utf-8,' + encodeURIComponent(csv);

	$(this).attr({
		'download': filename,
		'href': csvData,
		'target': '_blank'
	});
}
var lastupdatedLog = "";
function ReloadLogTabeFromServer() {
	getdata("GetLog", "", function (mess) {
		var txt = "";
		for (var i = 0; i < mess.d.length; i++) {
			txt += mess.d[i] + "\n";
		}
		$("#log").val(txt);
	});
}
function CheckForLog() {
	var logel = $("#log");
	if (logel.length < 1)
		return;

	if (logel.is(":visible") == false)
		return;

	getdata("GetLastLog", "", function (mess) {
		if (mess.d != lastupdatedLog) {
			lastupdatedLog = mess.d;
			ReloadLogTabeFromServer();
		}
	})
}
function AddPagerPage(pager, index, title) {
	var url = replaceUrlParam(location.href, "page", index);
	var html = "<li><a href='" + url + "'>" + title + "</a></li>";
	var el = $(html).appendTo(pager);
	return el;
}
function RerunTest(ReqestID, CommandName, UserName, TestRunID) {
	var fproc = StartProgress("Updating tests...")
	getdata("RunTest", JSON.stringify({ "strReqestID": ReqestID, "strCommandName": CommandName, "UserName": UserName, "TestRunID": TestRunID }), function (mess) {
		EndProgress(fproc);
		if (mess.d != "OK") {
			alert(mess.d)
		}
		location.reload();
		StartProgress("Reloading data...")
	})
}
function AskRerun(ReqestID, CommandName, UserName, TestRunID) {
	var newCommandName = CommandName.replace(/`/g, '"');
	var chCommand = prompt(" You reRun command is : " + newCommandName + " \n You can change reRun command:", newCommandName);
	if (chCommand != null) {
		chCommand = chCommand.replace(/"/g, '`');
		RerunTest(ReqestID, chCommand, UserName, TestRunID);
	}
}
var showbycook = "showby@" + GetPageName();
function ProcessShowBy() {
	var shb = $('#showby');
	if (shb.length < 1) {
		return;
	}
	var showby = getParameterByName("showby");
	if (showby) {
		$('#showby').val(showby);
	}
	else {
		var c = $.cookie(showbycook);
		if (c) {
			$('#showby').val(c);
		}
	}
	$('#showby').on('change', function () {
		window.location = replaceUrlParam(location.href, "showby", this.value);
		$.cookie(showbycook, this.value, { expires: 365 });
	})
}
$(function () {
	ProcessShowBy();

	setInterval(CheckForLog, 10000);
	var table = $(".pagetable");
	var pager = $("ul.pagination");
	if (table.length && pager.length) {
		pager.html("");
		var pages = table.attr("pages");

		var activpage = 1;
		if (getParameterByName("page") != "") {
			activpage = parseInt(getParameterByName("page"));
		}

		var p1 = activpage < 10 ? 1 : Math.floor((activpage) / 10) * 10;
		var p2 = p1 == 1 ? 9 : p1 + 9;
		p2 = Math.min(p2, pages);

		AddPagerPage(pager, 1, "&laquo");
		AddPagerPage(pager, activpage <= 10 ? 1 : activpage - 10, "&lt");

		for (var i = p1; i <= p2; i++) {
			var el = AddPagerPage(pager, i, i);
			if (activpage == i) {
				var a = $(el.children()[0]);
				a.css("background-color", "black");
				a.css("color", "white");
			}
		}

		AddPagerPage(pager, activpage + 10 > pages ? pages : activpage + 10, "&gt");
		AddPagerPage(pager, pages, "&raquo");
	}
})