var f1proc;
var f2proc;
var _imagesloaded = 0;

function get1Ctx(num) {
	var canvas = document.getElementById('canvas' + num);
	return canvas.getContext("2d");
}
function showDiff(but) {
	var $but = $(but);
	var $column = $but.parent();
	var canvas = $column.children("canvas")[0];
	var ct = canvas.getContext("2d");
	ct.clearRect(0, 0, canvas.width, canvas.height);

	if ($but.attr("mode") == 2) {
		ct.drawImage($("#diff")[0], 0, 0, ct.canvas.width, ct.canvas.height);
		$but.attr("mode", "1");
	}
	else if ($but.attr("mode") == 1) {
		ct.drawImage($column.children("img")[0], 0, 0, ct.canvas.width, ct.canvas.height);
		$but.attr("mode", "0");
	}
	else {
		ct.drawImage($column.children("img")[0], 0, 0, ct.canvas.width, ct.canvas.height);
		ct.drawImage($("#diff")[0], 0, 0, ct.canvas.width, ct.canvas.height);
		$but.attr("mode", "2");
	}
}
function processImage(img, prog) {
	_imagesloaded++;
	EndProgress(prog);

	var $image = $(img);
	$image.hide();

	if (_imagesloaded < 2) {
		return;
	}

	var ctx1 = get1Ctx(1);
	var ctx2 = get1Ctx(2);

	if (ctx1.canvas.width != ctx2.canvas.width || ctx1.canvas.height != ctx2.canvas.height) {
		alert("Images differ in size - cannot compare!");
	}

	$image1 = $("#img1");
	ctx1.canvas.width = $image1.width();
	ctx1.canvas.height = $image1.height();
	ctx1.drawImage($image1[0], 0, 0, ctx1.canvas.width, ctx1.canvas.height);

	$image2 = $("#img2");
	ctx2.canvas.width = $image2.width();
	ctx2.canvas.height = $image2.height();
	ctx2.drawImage($image2[0], 0, 0, ctx2.canvas.width, ctx2.canvas.height);

	var imgData1 = ctx1.getImageData(0, 0, ctx1.canvas.width, ctx1.canvas.height);
	var imgData2 = ctx2.getImageData(0, 0, ctx2.canvas.width, ctx2.canvas.height);
	for (var i = 0; i < imgData1.data.length; i += 4) {
		if (imgData1.data[i] != imgData2.data[i] ||
			imgData1.data[i + 1] != imgData2.data[i + 1] ||
			imgData1.data[i + 2] != imgData2.data[i + 2] ||
			imgData1.data[i + 3] != imgData2.data[i + 3]) {
			imgData1.data[i    ] = 255;
			imgData1.data[i + 1] = 0;
			imgData1.data[i + 2] = 0;
			imgData1.data[i + 3] = 255;
		}
		else {
			imgData1.data[i    ] = 0;
			imgData1.data[i + 1] = 0;
			imgData1.data[i + 2] = 0;
			imgData1.data[i + 3] = 0;
		}
	}

	var cdiff = document.createElement('canvas');
	cdiff.width = ctx1.canvas.width;
	cdiff.height = ctx1.canvas.height;
	var codiff = cdiff.getContext("2d");
	codiff.putImageData(imgData1, 0, 0);

	var dataURL = cdiff.toDataURL("image/png");
	var el = document.getElementById('diff');
	el.src = dataURL;
	el.width = ctx1.canvas.width;
	el.height = ctx1.canvas.height;
	document.body.appendChild(el);

	var commonwidth = $image1.parent().width();
	var coe = $image1.width() / commonwidth;

	ctx1.canvas.width = commonwidth;
	ctx1.canvas.height = $image1.height() / coe;
	ctx1.drawImage($image1[0], 0, 0, ctx1.canvas.width, ctx1.canvas.height);

	coe = $image2.width() / commonwidth;

	ctx2.canvas.width = commonwidth;
	ctx2.canvas.height = $image2.height() / coe;
	ctx2.drawImage($image2[0], 0, 0, ctx2.canvas.width, ctx2.canvas.height);
}
$(function () {
	var f1 = getParameterByName("file1");
	var f2 = getParameterByName("file2");
	if (!f1 || !f2) {
		alert("Files are not specified!")
		return;
	}

	var file1 = "getfile.aspx?Path=" + f1.replace(/"/g, "");
	var file2 = "getfile.aspx?Path=" + f2.replace(/"/g, "");

	var $getfile1but = $("#getfile1but");
	$getfile1but.attr("title", f1);
	$getfile1but.text(f1);
	var $draw1but = $("#draw1but");
	$draw1but.click(function () {
		showDiff(this);
	})

	var $getfile2but = $("#getfile2but");
	$getfile2but.attr("title", f1);
	$getfile2but.text(f1);
	var $draw2but = $("#draw2but");
	$draw2but.click(function () {
		showDiff(this);
	})

	f1proc = StartProgress("Loading file 1...")
	f2proc = StartProgress("Loading file 2...")

	var $i1 = $("<img id='img1' src='" + file1 + "'>");
	$i1.appendTo("#imgleft");

	var $i2 = $("<img id='img2' src='" + file2 + "'>");
	$i2.appendTo("#imgright");

	var $idiff = $("<img style='display:none' id='diff'>");
	document.body.appendChild($idiff[0]);

	$i1.one("load", function () {
		processImage(this, f1proc);
	})

	$i2.one("load", function () {
		processImage(this, f2proc);
})
});
