$(function () {
	var app = angular.module('mpsapplication', []);
	app.controller('mpscontroller', ["$scope", "$http", "$interval", function ($scope, $http, $interval) {
		$scope.readonly = !IsAdmin();
		var loadprg = StartProgress("Loading data...");
		$scope.machines = [];

		$scope.isAdmin = IsAdmin();
		$scope.fetchData = function (d) {
			$scope.machines = d;
			$scope.machines.forEach(function (m) {
				if (m.STARTED) {
					m.STARTED = StringToDateTime(m.STARTED);
				}
				if (m.PCPING) {
					m.PCPING = StringToDateTime(m.PCPING);
				}
				m.COLOR = "";
			});
			$scope.updateStatus();
			reActivateTooltips();
		};
		$scope.loadData = function () {
			$http.post("machines.asmx/getMachines", JSON.stringify({}))
				.then(function (result) {
					$scope.fetchData(result.data.d);
					EndProgress(loadprg);
				});
		};
		$scope.vncStart = function () {
			alert('You should have realvnc viewer installed first. Now you will need to download script and executable. First save exe and script files on some place and run script.');
			window.open('/add/starter.zip');
		};
		$scope.vnc = function (machine) {
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
			Link.onclick = function (event) {
				document.body.removeChild(event.target);
			};
			Link.style.display = "none";
			document.body.appendChild(Link);
			Link.click();
			$http.post("WebService.asmx/FeedLog", JSON.stringify({ "str": "VNC viewer has been launched for machine: '" + machine + "'" }));
		};
		$scope.updateStatus = function () {
			if ($scope.machines) {
				var now = new Date();
				$scope.machines.forEach(function (m) {
					var diff;
					var color = m.STATUS ? "yellow" : "white";
					if (m.STARTED) {
						diff = (now - m.STARTED) / 1000 / 60;
						if (diff > 60)
							color = "burlywood";
						var hours = "" + Math.floor(diff / 60);
						var mins = "" + Math.floor(diff - hours * 60);
						var newRUN = "" + hours.padStart(2, "0") + ":" + mins.padStart(2, "0");
						if (newRUN !== m.RUN) {
							m.RUN = newRUN;
						}
					}
					if (m.PCPING) {
						diff = (now - m.PCPING) / 1000;
						if (diff > 70) {
							color = "LightGray";
						}
					}
					if (m.COLOR != color) {
						m.COLOR = color;
					}
				});
			}
		};
		$scope.pauseOnOff = function (id) {
			var loadprg = StartProgress("Updating machine...");
			$http.post("machines.asmx/PauseOnOff", JSON.stringify({ id: id }))
				.then(function (result) {
					$scope.fetchData(result.data.d);
					EndProgress(loadprg);
				});
		};
		$scope.loadData();
		$interval(function () {
			$scope.loadData();
		}, 30000);
		$interval(function () {
			$scope.updateStatus();
		}, 10000);
	}]);
})