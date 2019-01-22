var txt1, txt2;
var comparer = new diff_match_patch();

var def_cur = -1;
function link_up() {
	if (def_cur < 0)
		return;

	def_cur--;
	var slctr = "a[name=def_" + def_cur + "]";
	if ($(slctr).length <= 0) {
		def_cur++;
		return;
	}
	$(slctr).get(0).scrollIntoView();
}
function link_down() {
	var slctr = "a[name^=def_]";
	if (def_cur > $(slctr).length - 1)
		return;

	def_cur++;
	slctr = "a[name=def_" + def_cur + "]";
	if ($(slctr).length <= 0) {
		def_cur--;
		return;
	}
	$(slctr).get(0).scrollIntoView();
}
function Compare() {
	if (txt1 == null || txt2 == null)
		return;
	if (txt1.startsWith("<!DOCTYPE"))
		txt1 = txt1.replace(/</g, "&lt;").replace(/>/g, "&gt;");
	if (txt2.startsWith("<!DOCTYPE"))
		txt2 = txt2.replace(/</g, "&lt;").replace(/>/g, "&gt;");

	ShowWaitDlg(true);
	var d = comparer.diff_main(txt1, txt2);
	comparer.diff_cleanupSemantic(d);

	var f1 = getParameterByName("file1");
	$("#fslbl1").html(f1);
	$("#fslbl1").attr("href", "showfile.aspx?file=" + f1);
	$("#fslbl1").attr("title", f1);
	var f2 = getParameterByName("file2");
	$("#fslbl2").html(f2);
	$("#fslbl2").attr("href", "showfile.aspx?file=" + f2);
	$("#fslbl2").attr("title", f2);

	var t1 = "";
	var t2 = "";

	var diffs = 0;
	for (var i = 0; i < d.length; i++) {
		var tp = d[i][0];
		var tx = d[i][1];
		var rp = tx.replace(/(?:\r\n|\r|\n)/g, '<br />');
		if (tp == 0) {
			t1 += rp
			t2 += rp;
		}
		else if (tp == 1) {
			t2 += "<mark>" + rp + "</mark>";
			t1 += "<a name = 'def_" + diffs + "'/>";
			var count = (rp.match(/<br \/>/g) || []).length;
			for (var j = 0; j < count; j++) {
				t1 += "<mark> </mark>" + "<br />";
			}
			if (count < 1)
				t1 += "<mark> </mark>";
			diffs++;
		}
		else if (tp == -1) {
			var count = (rp.match(/<br \/>/g) || []).length;
			for (var j = 0; j < count; j++) {
				t2 += "<span class='del2'> </span>" + "<br />";
			}
			t1 += "<a name = 'def_" + diffs + "'/>";
			t1 += "<del>" + rp + "</del>";
			diffs++;
		}
	}
	
	$("#cmplbl").html("Total differences: " + diffs);
	$("#fspan1").html(t1);
	$("#fspan2").html(t2);
	ShowWaitDlg(false);
}
$(function () {
	$("#home").click(function () {
		window.location = window.location.href.split("#")[0];
	})
	var file1 = getParameterByName("file1");
	var file2 = getParameterByName("file2");

	var f1proc = StartProgress("Loading file 1...")
	getdata("GetFile", JSON.stringify({ "filename": file1 }), function (mess) {
		EndProgress(f1proc);
		txt1 = mess.d;
		Compare();
	})
	var f2proc = StartProgress("Loading file 2...")
	getdata("GetFile", JSON.stringify({ "filename": file2 }), function (mess) {
		EndProgress(f2proc);
		txt2 = mess.d;
		Compare();
	})
	$(window).on('load', function () {
		$(this).trigger('resize');
	});
});