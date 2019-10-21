$(function () {
	var app = angular.module('mpsapplication', []);
	app.controller('mpscontroller', ["$scope", "$http", function ($scope, $http) {
		$scope.requests = [];
		$scope.page = 1;
		$scope.by = 50;
		$scope.inc = function () {
			$scope.page++;
			$scope.loadData();
		};
		$scope.dec = function () {
			if ($scope.page < 2) {
				return;
			}
			$scope.page--;
			$scope.loadData();
		};
		$scope.loadData = function () {
			var taskprg = StartProgress("Loading data...");
			$http.post("TestRequests.asmx/getRequests", JSON.stringify({}))
				.then(function (result) {
					$scope.requests = result.data.d;
					EndProgress(taskprg);
				});
		};
		$scope.loadData();
	}]);
});