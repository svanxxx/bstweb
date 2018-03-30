function ProcessText(txt) {
	var $parentdiv = $("#filecontainer");
	var splitted = txt.split("\n");
	for (var i = 0; i < splitted.length; i++) {
		var line = splitted[i];
		/*if (line.trim() == "") {
			$parentdiv.append("<br>");
			continue;
		}*/
		var tabs = line.split("\t");
		var colscount = tabs.length;
		if (colscount == 1) {
			var $div = "<pre>" + splitted[i] + "</pre>";
			$parentdiv.append($div);
		}
		else {
			var $table = $("<table></table>");
			$parentdiv.append($table);
			for (; i < splitted.length; i++) {
				line = splitted[i];
				var tabs = line.split("\t");
				var colscount2 = tabs.length;
				if (colscount2 != colscount) { //this is not our table
					i--;
					break;
				}
				var $row = $("<tr></tr>");
				$table.append($row);
				for (var c = 0; c < colscount2; c++) {
					var tx = tabs[c] == "" ? "&nbsp" : tabs[c];
					var $td = "<td>" + tx + "</td>";
					$row.append($td);
				}
			}
			$parentdiv.append("<br>");
		}
	}
}
$(function () {
	var $cfile = $('span[id*="filetoshow"]');
	var file = encodeURI($cfile.text());
	var f1proc = StartProgress("Loading file ...")
	getdata("GetFile", JSON.stringify({ "filename": file }), function (mess) {
		EndProgress(f1proc);
		var f2proc = StartProgress("Processing ...")
		ProcessText(mess.d);
		EndProgress(f2proc);
	})
})