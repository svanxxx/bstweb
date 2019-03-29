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
			var users = [];
			for (var i = 0; i < $scope.users.length; i++) {
				var ch = $scope.users[i].changed;
				if (ch) {
					delete $scope.users[i].changed;
					users.push($scope.users[i]);
				}
			}
			$http.post("webservice.asmx/setBSTUsers", angular.toJson({ "users": users }))
				.then(function () {
					EndProgress(prg);
					$scope.changed = false;
				});
		};

		var taskprg = StartProgress("Loading data...");
		$scope.users = [];
		$http.post("webservice.asmx/getBSTUsers", JSON.stringify({ "active": false }))
			.then(function (result) {
				$scope.users = result.data.d;
			});
		$scope.changed = false;
		$scope.itemchanged = function (r) {
			r.changed = true;
			$scope.changed = true;
		};
		EndProgress(taskprg);
	}]);
})