function ReloadTabeFromServer() {
	getdata("GetHosts", "", function (mess) {
		Reload(mess.d);
	});
}
function CheckClass(row, classn) {
	if (row.attr("class") != classn) {
		row.attr("class", classn);
	}
}
function BusyTimer() {
	$('#hosts > tbody > tr').each(function (i, obj) {
		var row = $(obj);
		var start = row.attr("start");
		var ping = row.attr("ping");
		var curr = new Date();
		curr = curr.getTime();

		var pdiff = (curr - ping) / 1000; //sec
		var diff = (curr - start) / 1000 / 60;
		var hours = Math.floor(diff / 60);
		var mins = Math.floor(diff - hours * 60);

		if (pdiff > 120) {
			CheckClass(row, "noworkmachine");
		}
		else {
			$('td:eq(' + $("th[bsttype=timer]").index() + ')', row).html(("0" + hours).slice(-2) + ":" + ("0" + mins).slice(-2));
			CheckClass(row, "freemachine");
		}
	});
}
function VNC(machine) {
	var coo = $.cookie("user_know_to_run_vnc");
	if (!coo) {
		alert('This is a warning message. You will see it only once. Note: you have to click VNC column header cell to see instructions and download archive with required files.');
		$.cookie("user_know_to_run_vnc", 1, { expires: 365 });
	}
	var textToSaveAsBlob = new Blob([""], { type: "text/plain" });
	var Link = document.createElement("a");
	Link.download = machine + ".vnc";
	Link.innerHTML = "Download File";
	Link.href = window.URL.createObjectURL(textToSaveAsBlob);
	Link.onclick = destroyClickedElement;
	Link.style.display = "none";
	document.body.appendChild(Link);
	Link.click();
	senddata("FeedLog", JSON.stringify({ "str": "VNC viewer has been launched for machine: '" + machine + "'" }));
	CheckForLog();
}
function destroyClickedElement(event) {
	document.body.removeChild(event.target);
}
function instruction() {
	alert('You should have realvnc viewer installed first. Now you will need to download script and executable. First save exe and script files on some place and run script.');
	//		e.preventDefault();
	window.open('/add/starter.zip');
}
function StartHost(machine) {
	getdata("StartHost", JSON.stringify({ "host": machine }), function (mess) {
		var imgindex = $("th[bsttype=starit]").index()
		var img = $('tr[machine=' + machine + '] td:eq(' + imgindex + ') img');
		img.attr('src', 'images/process.gif');
		img.attr('width', '16');
		img.attr('height', '16');
		setTimeout(function () { ReloadTabeFromServer(); }, 10000);
		CheckForLog();
	})
}
function DelM(machine) {
	var answer = confirm("Are you sure you want to delete machine '" + machine + "'?");
	if (!answer) {
		return;
	}
	getdata("DelM", JSON.stringify({ "machine": machine }), function (mess) {
		var imgindex = $("th[bsttype=starit]").index()
		var img = $('tr[machine=' + machine + '] td:eq(' + imgindex + ') img');
		img.attr('src', 'images/process.gif');
		img.attr('width', '16');
		img.attr('height', '16');
		setTimeout(function () { ReloadTabeFromServer(); }, 10000);
		CheckForLog();
	})
}
function StopHost(machine) {
	var answer = confirm("Are you sure you want to stop machine '" + machine + "'?");
	if (!answer) {
		return;
	}
	getdata("StopHost", JSON.stringify({ "host": machine }), function (mess) {
		var imgindex = $("th[bsttype=stopit]").index()
		var img = $('tr[machine=' + machine + '] td:eq(' + imgindex + ') img');
		img.attr('src', 'images/process.gif');
		img.attr('width', '16');
		img.attr('height', '16');
		setTimeout(function () { ReloadTabeFromServer(); }, 10000);
		CheckForLog();
	})
}
function Reload(data) {
	$("#hosts tbody tr").remove();
	var start = true;
	for (var k = 0; k < data.length; k++) {
		var pc = data[k];

		var comma = pc.Info.indexOf(",");
		if (comma > -1) {
			comma = pc.Info.indexOf(",", comma + 1);
		}
		if (comma == -1) {
			comma = 30;
		}
		var IPcomma = pc.IP.indexOf(",");
		var ethernetproblem = (pc.IP.indexOf("FAIL") > -1) ? " class='ethernetproblem' " : "";
		IPcomma = (IPcomma == -1) ? pc.IP.length : IPcomma

		var list = pc.List;
		if (list) {
			list = "";
			var mlist = pc.List.split(',');
			for (var m = 0; m < mlist.length; m++) {
				var pname = mlist[m].trim();
				if (pname) {
					var url = "runs.aspx?P.PCNAME=" + pname;
					list += "<a href='" + url + "'>" + pname + "</a>" + "&nbsp";
				}
			}
		}

		var row =
			"<tr start='" + pc.Started + "' ping='" + pc.Pcping + "' machine='" + pc.Name + "'>"
			+ "<td>" + (k + 1) + "</td>"
			+ "<td class='machine'><a href='machines.aspx?machines=" + pc.List.replace(/ /g, "") + "'>" + pc.Name + "</a></td>"
			+ "<td class='del' onclick=\"DelM('" + pc.Name + "')\"><img src='/images/delete.gif'></td>"
			+ "<td class='vnc' onclick=\"VNC('" + pc.Name + "')\"><img src='/images/vnc.png'></td>"
			+ "<td class='pau' onclick=\"StopHost('" + pc.Name + "')\"><img src='/images/sign_stop.png'></td>"
			+ "<td class='pau' onclick=\"StartHost('" + pc.Name + "')\"><img src='/images/sign_start.png'></td>"
			+ "<td></td>"
			+ "<td>" + list + "</td>"
			+ "<td title='" + pc.Info.replace(/,/g, "\n") + "'>" + pc.Info.substring(0, comma) + "</td>"
			+ "<td " + ethernetproblem + "title='" + pc.IP.replace(/,/g, "\n") + "'>" + pc.IP.substring(0, IPcomma) + "</td>"
			+ "<td>" + pc.MAC + "</td>"
			+ "</tr>";
		if (!start) {
			$('#hosts tbody tr:last').after(row);
		}
		else {
			$('#hosts tbody').append(row);
		}
		$(".machine").contextmenu(function () {
			alert("Handler for .contextmenu() called.");
		});
	}
	BusyTimer();
}
var _host_lastupdated;
function host_lastupdated() {
	if (!_host_lastupdated) {
		_host_lastupdated = event.data;
		return;
	}
	if (_host_lastupdated != event.data) {
		_host_lastupdated = event.data;
		ReloadTabeFromServer();
	}
}
$(function () {
	addBSTSignalCallback("host_lastupdated", host_lastupdated);

	var header = "<tr>"
		+ "<th>#</th>"
		+ "<th>Machine</th>"
		+ "<th>D</th>"
		+ "<th class='vnc' onclick=\"instruction()\">VNC</th>"
		+ "<th bsttype=stopit>STO</th>"
		+ "<th bsttype=starit>STA</th>"
		+ "<th bsttype=timer>GO</th>"
		+ "<th>Children</th>"
		+ "<th>Info</th>"
		+ "<th>IP</th>"
		+ "<th>MAC</th>"
		+ "</tr>";
	$("#hosts thead").append(header);

	ReloadTabeFromServer();

	setInterval(BusyTimer, 10000);
})