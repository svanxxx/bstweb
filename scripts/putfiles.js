function rawHtml($sce) {
	return function (val) {
		return $sce.trustAsHtml(val);
	};
}
$(function () {
	var app = angular.module('mpsapplication', []);
	app.filter('rawHtml', ['$sce', rawHtml]);
	app.controller('mpscontroller', ["$scope", "$http", function ($scope, $http) {
		$scope.output = "";
		$scope.files = [];
		$scope.isAdmin = IsAdmin();
		$scope.RUNID = getParameterByName("RUNID");
		$scope.key = getParameterByName("Guid");
		$scope.key = getParameterByName("Guid");
		$scope.testName = getParameterByName("Test_Name");
		$scope.branch = getParameterByName("gitBranch");
		$scope.user = userName();
		$scope.comments = $scope.testName + ", etalon update ";

		$scope.readonly = function () {
			return !$scope.isAdmin || inProgress() || $scope.files.length < 1 || $scope.output.length > 0;
		};
		$scope.loadData = function () {
			var taskprg = StartProgress("Loading data...");
			$http.post("WebService.asmx/getChangedFiles", JSON.stringify({ key: $scope.key }))
				.then(function (result) {
					$scope.files = result.data.d;
					$scope.files.forEach(function (f) {
						var drives = f.NEW.split("\\");
						f.name = drives[drives.length - 1];
						f.checked = true;
					});
					EndProgress(taskprg);
				});
		};
		$scope.commit = function () {
			if ($scope.readonly()) {
				return;
			}
			var indexes = [];
			$scope.files.forEach(function (f, i) {
				if (f.checked) {
					indexes.push(i);
				}
			});
			var taskprg = StartProgress("Working...");
			$http.post("WebService.asmx/commitChangedFiles", JSON.stringify({ key: $scope.key, indexes: indexes, comment: $scope.comments, runID: $scope.RUNID }))
				.then(function (result) {
					$scope.output = result.data.d.join("");
					EndProgress(taskprg);
				});
		};
		$scope.check = function (c) {
			$scope.files.forEach(function (f) {
				f.checked = c;
			});
		};
		$scope.loadData();
	}]);
});