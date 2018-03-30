var BstSignal;
function openSignal() {
	if (!BstSignal) {
		if (!document.bstSignalEvents || document.bstSignalEvents.length < 1) {
			return;
		}
		var events = "";
		for (var i = 0; i < document.bstSignalEvents.length; i++) {
			events += (i == 0 ? "" : ",") + document.bstSignalEvents[i];
		}
		BstSignal = new EventSource('BstSignal.ashx?events=' + events);
		for (var iEv = 0; iEv < document.bstSignalEvents.length; iEv++) {
			for (var j = 0; j < document.bstSignalCallbacks[iEv].length; j++) {
				BstSignal.addEventListener(document.bstSignalEvents[iEv], document.bstSignalCallbacks[iEv][j], false);
			}
		}
	}
}
function closeSignal() {
	if (BstSignal) {
		BstSignal.close();
		BstSignal = null;
	}
}
function addBSTSignalCallback(event, callback) {
	if (!document.bstSignalEvents) {
		document.bstSignalEvents = [event];
		document.bstSignalCallbacks = [[callback]];
		return;
	}
	else {
		var index = document.bstSignalEvents.indexOf(event);
		if (index == -1) {
			document.bstSignalEvents.push(event);
			document.bstSignalCallbacks.push([callback]);
		} else {
			var calls = document.bstSignalCallbacks[index];
			calls.push(callback);
		}
	}
}
function requestsstate(event) {
	var $sig = $("#requestsignal");
	var vers = event.data.split("/")
	var done = parseInt(vers[2]) - parseInt(vers[1]);
	$sig.text("Requests (" + vers[0] + "/" + done + "/" + vers[2] + ")");
	$sig.attr("title", "Incoming: " + vers[0] + ", Processed: " + done + ", In progress: " + vers[1] + ", Total: " + vers[2]);
	if (vers[0] > 0) {
		$sig.addClass("requestsignal1");
		$sig.removeClass("requestsignal2");
	} else if (done > 0) {
		$sig.addClass("requestsignal2");
		$sig.removeClass("requestsignal1");
	} else {
		$sig.removeClass("requestsignal1");
		$sig.removeClass("requestsignal2");
	}
}
$(function () {
	if ($("#requestsignal").length > 0) {
		addBSTSignalCallback("requestsstate", requestsstate);
	}
	document.addEventListener('visibilitychange', function () {
		if (document.hidden) {
			closeSignal();
		} else {
			openSignal();
		}
	});
	setTimeout(openSignal, 2000);
})