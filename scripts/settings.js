$(function () {
	var app = angular.module('mpsapplication', []);
	app.filter('passwordFilter', function () {
		return function (input) {
			var split = input.split('');
			var result = "";
			for (var i = 0; i < split.length; i++) {
				result += "*";
			}
			return result;
		};
	});
	app.controller('mpscontroller', ["$scope", "$http", function ($scope, $http) {
		$scope.readonly = !IsAdmin();
		$scope.discard = function () {
			window.location.reload();
		};
		$scope.save = function () {
			var prg = StartProgress("Saving data...");
			var arr = $scope.settings;
			for (var i = 0; i < arr.length; i++) {
				$scope.settObj[arr[i].NAME] = arr[i].VALUE;
			}
			$http.post("webservice.asmx/setSettings", angular.toJson({ s: $scope.settObj }))
				.then(function () {
					EndProgress(prg);
					$scope.loadData();
				});
		};

		$scope.loadData = function () {
			var taskprg = StartProgress("Loading data...");
			$scope.settings = [];
			$scope.changed = false;
			$scope.settObj = {};
			$http.post("webservice.asmx/getSettings", JSON.stringify({}))
				.then(function (result) {
					$scope.settObj = result.data.d;
					for (var propName in $scope.settObj) {
						if (propName === "__type") {
							continue;
						}
						$scope.settings.push({ NAME: propName, VALUE: $scope.settObj[propName] });
					}
					EndProgress(taskprg);
				});
		};
		$scope.itemchanged = function (r) {
			r.changed = true;
			$scope.changed = true;
		};

		$scope.loadData();
	}]);
});