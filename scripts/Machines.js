var lastupdated = "";
var lastreloadedtable = new Date();

function ReloadTabeFromServer() {
	getdata("GetMachines", "", function (mess) {
		Reload(mess.d);
	});
}
function CheckClass(row, classn) {
	if (row.attr("class") != classn) {
		row.attr("class", classn);
	}
}
function BusyTimer() {
	$('#machines > tbody > tr').each(function (i, obj) {
		var row = $(obj);
		var start = row.attr("start");
		var ping = row.attr("ping");
		var curr = new Date();
		curr = curr.getTime();

		var pdiff = (curr - ping) / 1000;
		var diff = (curr - start) / 1000 / 60;
		var hours = Math.floor(diff / 60);
		var mins = Math.floor(diff - hours * 60);

		if (pdiff > 121) {
			CheckClass(row, "noworkmachine");
		}
		else if (start) {

			$('td:eq(' + $("th[bsttype=timer]").index() + ')', row).html(("0" + hours).slice(-2) + ":" + ("0" + mins).slice(-2));
			if (hours > 2) {
				CheckClass(row, "verylongworkmachine");
			}
			else if (hours >= 1) {
				CheckClass(row, "longworkmachine");
			}
			else {
				CheckClass(row, "workmachine");
			}
		}
		else {
			CheckClass(row, "freemachine");
		}
	});
}
function CheckForResults() {
	getdata("GetLastRun", "", function (mess) {
		var curr = new Date();
		var diff = (curr.getTime() - lastreloadedtable.getTime()) / 1000 / 60;//one minute
		if (mess.d != lastupdated || diff > 1) {
			lastupdated = mess.d;
			ReloadTabeFromServer();
		}
	})
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
}
function destroyClickedElement(event) {
	document.body.removeChild(event.target);
}
function manageMachine(machine, manageFunction, thStyle) {
	getdata(manageFunction, JSON.stringify({ "machine": machine }), function (mess) {
		var imgindex = $("th[bsttype=" + thStyle + "]").index()
		var img = $('tr[machine=' + machine + '] td:eq(' + imgindex + ') img');
		img.attr('src', 'images/process.gif');
		img.attr('width', '16');
		img.attr('height', '16');
		setTimeout(function () { ReloadTabeFromServer(); }, 2000);
	})
}
function StopMachine(machine) {
	manageMachine(machine, "StopMachine", "stopit");
}
function ShutMachine(machine) {
	manageMachine(machine, "ShutMachine", "shutit");
}
function GetGit(machine) {
	manageMachine(machine, "GetGit", "getgit");
}
function ChangeState(machine) {
	manageMachine(machine, "ChangeState", "pausit");
}
function Restart(machine) {
	manageMachine(machine, "Restart", "restit");
}
function instruction() {
	alert('You should have realvnc viewer installed first. Now you will need to download script and executable. First save exe and script files on some place and run script.');
	//		e.preventDefault();
	window.open('/add/starter.zip');
}
function Reload(data) {
	lastreloadedtable = new Date();
	$("#machines tbody tr").remove();
	var start = true;
	for (var k = 0; k < data.length; k++) {
		var pc = data[k];

		var mlist = getParameterByName("machines");
		if (mlist == "")
			mlist = [];
		else
			mlist = mlist.split(',');
		var inlist = false;
		for (var j = 0; j < mlist.length; j++) {
			if (mlist[j].toUpperCase() === pc.Name.toUpperCase()) {
				inlist = true;
			}
		}
		if (mlist.length > 0 && inlist == false) {
			continue;
		}

		var txt = pc.Current.substring(12, pc.Current.length);
		var imgpause = "sign_pause";
		if (pc.Pausedby)
			imgpause = "sign_play";
		var row =
			"<tr start='" + pc.Started + "' ping='" + pc.Pcping + "' machine='" + pc.Name + "'>"
			+ "<td>" + (k + 1) + "</td>"
			+ "<td><a href='/runs.aspx?R.REPEATED=<>2&P.PCNAME=" + pc.Name + "'>" + pc.Name + "</a></td>"
			+ "<td>" + pc.Tests + "</td>"
			+ "<td title='VNC' class='vnc' onclick=\"VNC('" + pc.Name + "')\"><img src='/images/vnc.png'></td>"
			+ "<td title='Pause/Resume' class='pau' onclick=\"ChangeState('" + pc.Name + "')\"><img src='/images/" + imgpause + ".png'>" + pc.Pausedby + "</td>"
			+ "<td title='Stop Machine' class='pau' onclick=\"StopMachine('" + pc.Name + "')\"><img src='/images/sign_stop.png'></td>"
			+ "<td title='Shutdown Machine' class='pau' onclick=\"ShutMachine('" + pc.Name + "')\"><img src='/images/SIGN_OFF.png'></td>"
			+ "<td title='Get GIT' class='pau' onclick=\"GetGit('" + pc.Name + "')\"><img src='/images/sign_getgit.png'></td>"
			+ "<td title='Restart' class='pau' onclick=\"Restart('" + pc.Name + "')\"><img src='/images/Sign_rerun_green.png'></td>"
			+ "<td title='Show Log' class='pau'><a href='Log.aspx?thoster=" + pc.Name + "'><img src='/images/report.png'><a></td>"
			+ "<td></td>"
			+ "<td>" + txt + "</td>"
			+ "<td><a href='/runs.aspx?V.VERSION=" + pc.Version + "'>" + pc.Version + "</a></td>"
			+ "</tr>";
		if (!start) {
			$('#machines tbody tr:last').after(row);
		}
		else {
			$('#machines tbody').append(row);
		}
	}
	BusyTimer();
}
$(function () {

	var header = "<tr>"
		+ "<th>#</th>"
		+ "<th>Machine</th>"
		+ "<th>T</th>"
		+ "<th class='vnc' onclick=\"instruction()\">VNC</th>"
		+ "<th bsttype=pausit>P/R</th>"
		+ "<th bsttype=stopit>ST</th>"
		+ "<th bsttype=shutit>SH</th>"
		+ "<th bsttype=getgit>GI</th>"
		+ "<th bsttype=restit>RE</th>"
		+ "<th>LO</th>"
		+ "<th bsttype=timer>GO</th>"
		+ "<th>Current</th>"
		+ "<th>Version</th>"
		+ "</tr>";
	$("#machines thead").append(header);

	ReloadTabeFromServer();

	setInterval(CheckForResults, 10000);
	setInterval(BusyTimer, 10000);
})