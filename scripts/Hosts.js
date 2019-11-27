$(function () {
	var app = angular.module('mpsapplication', []);
	app.controller('mpscontroller', ["$scope", "$http", "$interval", function ($scope, $http, $interval) {
		$scope.readonly = !IsAdmin();
		$scope.hosts = [];

		$scope.isAdmin = IsAdmin();
		$scope.fetchData = function (d) {
			$scope.hosts = d;
			$scope.hosts.forEach(function (m) {
				if (m.PCPING) {
					m.PCPING = StringToDateTime(m.PCPING);
				}
				m.INFOSHORT = m.INFO.split(",").slice(0, 2).join(",");
				m.INFO = m.INFO.replace(/,/g, "\n");
				m.COLOR = "";
			});
			$scope.updateStatus();
			reActivateTooltips();
		};
		$scope.loadData = function () {
			var loadprg = StartProgress("Synchronizing data...");
			$http.post("machines.asmx/getHosts", JSON.stringify({}))
				.then(function (result) {
					$scope.fetchData(result.data.d);
					EndProgress(loadprg);
				});
		};
		$scope.vnc = function (machine) {
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
			if ($scope.hosts) {
				var now = new Date();
				$scope.hosts.forEach(function (m) {
					var color = "white";
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
		$scope.startStopHost = function (id, start) {
			var loadprg = StartProgress("Updating machine...");
			$http.post("machines.asmx/startStopHost", JSON.stringify({ id: id, start: start }))
				.then(function (result) {
					$scope.fetchData(result.data.d);
					EndProgress(loadprg);
				});
		};
		$scope.fetchData(JSON.parse(document.getElementById("inidata").value));
		$scope.offline = JSON.parse(document.getElementById("offline").value);

		$scope.deleteHost = function (id) {
			var answer = confirm("Are you sure you want to delete host?");
			if (answer) {
				var loadprg = StartProgress("Updating machine...");
				$http.post("machines.asmx/DeleteHost", JSON.stringify({ id: id }))
					.then(function (result) {
						$scope.offline = result.data.d;
						for (var i = $scope.hosts.length - 1; i >= 0; i--) {
							if ($scope.hosts[i].ID === id) {
								$scope.hosts.splice(i, 1);
								break;
							}
						}
						EndProgress(loadprg);
					});
			}
		};
		$scope.online = function (id) {
			var loadprg = StartProgress("Updating machine...");
			$http.post("machines.asmx/OnlineHost", JSON.stringify({ id: id }))
				.then(function (result) {
					$scope.fetchData(result.data.d);
					for (var i = $scope.offline.length - 1; i >= 0; i--) {
						if ($scope.offline[i].ID === id) {
							$scope.offline.splice(i, 1);
							break;
						}
					}
					EndProgress(loadprg);
				});
		};

		$interval(function () {
			$scope.loadData();
		}, 30000);
		$interval(function () {
			$scope.updateStatus();
		}, 10000);
	}]);
})