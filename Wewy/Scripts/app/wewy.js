var app = angular.module('WewyApp', ['ngRoute']);

app.config(['$routeProvider',
  function ($routeProvider) {
      $routeProvider.
        when('/Status', {
            templateUrl: 'templates/Status.html',
            controller: 'StatusCtrl'
        }).
        otherwise({
            redirectTo: '/Status'
        });
  }]);

app.controller('StatusCtrl', function ($scope, $http) {
    var dad = 1;
    $scope.loverName = "?"
    $scope.title = "Your lover is " + $scope.loverName;
    $scope.isSendingStatus = false;
    $scope.isLoading = false;
    $scope.dateIndex = 0;
    $scope.displayMyStatuses = true;

    $http.get("/api/Lover").success(function (data, status, headers, config) {
        $scope.loverName = data.name;
    }).error(function (data, status, headers, config) {
        $scope.title = "Oops... something went wrong";
        $scope.working = false;
    });

    $scope.reload = function () {
        $scope.isLoading = true;
        $http.get("/api/Status").success(function (data, status, headers, config) {
            $scope.isLoading = false;
            $scope.statuses = data;
            $scope.updateTimes();
        }).error(function (data, status, headers, config) {
            $scope.isLoading = false;
            $scope.title = "Oops... something went wrong";
            $scope.working = false;
        });
    };

    $scope.reload();

    $scope.submitNewStatus = function () {
        var text = $scope.newStatusText;
        if (!text) {
            return;
        }
        $scope.isSendingStatus = true;
        $http.post("/api/Status", { "text": text }).success(function (data, status, headers, config) {
            $scope.statuses.unshift(data);
            $scope.newStatusText = "";
            $scope.updateTimes();
            $scope.isSendingStatus = false;
        }).error(function (data, status, headers, config) {
            $scope.isSendingStatus = false;
            $scope.title = "Oops... something went wrong";
            $scope.working = false;
        });
    };

    $scope.updateTimes = function () {
        var status,
            mom,
            fromNow,
            dateLocalToCreator,
            dateLocalToLover;

        var formatString = "dddd, MMM Do, h:mm a";

        for (i in $scope.statuses)
        {
            status = $scope.statuses[i];
            mom = moment(status.dateCreatedUtc + "Z"); 
            fromNow = mom.fromNow();
            dateLocalToCreator = moment(status.dateCreatedCreator).format(formatString) + " (" + status.creatorCity + ")";
            dateLocalToLover = moment(status.dateCreatedLover).format(formatString) + " (" + status.loverCity + ")";
            status.createDates = [fromNow, dateLocalToCreator, dateLocalToLover];
        }
    };

    $scope.bumpDateIndex = function () {
        $scope.dateIndex = ($scope.dateIndex + 1) % 3;
    }
});
