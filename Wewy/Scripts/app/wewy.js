var app = angular.module('WewyApp', ['ngRoute']);

app.config(['$routeProvider',
  function ($routeProvider) {
      $routeProvider.
        when('/Status', {
            templateUrl: 'templates/Status.html',
            controller: 'StatusCtrl'
        }).
        when('/EditStatus/:statusId', {
            templateUrl: 'templates/EditStatus.html',
            controller: 'EditStatusCtrl'
        }).
        when('/EditUser', {
            templateUrl: 'templates/EditUser.html',
            controller: 'EditUserCtrl'
        }).
        otherwise({
            redirectTo: '/Status'
        });
  }]);

app.controller('EditUserCtrl', function ($scope, $http, $window) {

    $scope.cities = [];
    $scope.isSaving = false;
    $scope.alert = "";

    $http.get("/api/City").success(function (data, status, headers, config) {
        $scope.cities = data;
    }).error(function (data, status, headers, config) {
        $scope.alert = "Oops... something went wrong";
        $scope.working = false;
    });

    $scope.save = function () {
        $scope.isSaving = true;
        $scope.alert = "Saving...";
        $http.put("/api/User", { "cityName": $scope.cityName }).success(function (data, status, headers, config) {
            $window.location.href = '/#Status';
            $scope.isSaving = false;
        }).error(function (data, status, headers, config) {
            $scope.isSaving = false;
            $scope.alert = "Oops... something went wrong";
            $scope.working = false;
        });
    };
});

app.controller('StatusCtrl', function ($scope, $http, $interval) {
    var dad = 1;
    $scope.isSendingStatus = false;
    $scope.isLoading = false;
    $scope.dateIndex = 0;
    $scope.displayMyStatuses = true;
    $scope.displaySortByDay = false;
    $scope.myUserData = null;
    $scope.loverUserData = null;

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
            status.formattedDates = [fromNow, dateLocalToCreator, dateLocalToLover];
        }
    };

    $scope.updateRelativeStatusTimes = function () {
        var status, mom, fromNow;
        var formatString = "dddd, MMM Do, h:mm a";
        for (i in $scope.statuses) {
            status = $scope.statuses[i];
            mom = moment(status.dateCreatedUtc + "Z");
            fromNow = mom.fromNow();
            status.formattedDates[0] = fromNow;
        }
    }

    $scope.bumpDateIndex = function () {
        $scope.dateIndex = ($scope.dateIndex + 1) % 3;
    }

    $scope.changeSort = function () {
        if ($scope.displaySortByDay)
        {
            // Sort by local time.
            sortFunction = function (a, b) {
                return (new Date(b.dateCreatedCreator)) - (new Date(a.dateCreatedCreator));
            };

            // Show in time local to creator.
            $scope.dateIndex = 1;
        }
        else
        {
            sortFunction = function (a, b) {
                return (new Date(b.dateCreatedUtc)) - (new Date(a.dateCreatedUtc));
            };

            // Show in relative time.
            $scope.dateIndex = 0;
        }

        $scope.statuses.sort(sortFunction);        
    }

    $scope.getUserData = function () {
        $http.get("/api/User").success(function (data, status, headers, config) {
            $scope.me = data.me;
            $scope.lover = data.lover;
            $scope.updateUserTime();
            $interval($scope.updateUserTime, 10000);
        }).error(function (data, status, headers, config) {
            $scope.isLoading = false;
            $scope.title = "Oops... something went wrong";
            $scope.working = false;
        });
    }

    $scope.updateUserTime = function () {
        var now = moment();

        var meTime = now.tz($scope.me.timeZoneName);
        $scope.me.day = meTime.format("ddd, MMM Do");
        $scope.me.time = meTime.format("h:mm a");

        var loverTime = now.tz($scope.lover.timeZoneName);
        $scope.lover.day = loverTime.format("ddd, MMM Do");
        $scope.lover.time = loverTime.format("h:mm a");
    }

    $scope.reload();
    $scope.getUserData();
    $interval($scope.updateRelativeStatusTimes, 60000);
});

app.controller('EditStatusCtrl', function ($scope, $http, $interval, $routeParams) {
    
    $scope.load = function () {
        $scope.isLoading = true;
        $http.get("/api/Status/" + $routeParams.statusId).success(function (data, status, headers, config) {
            $scope.isLoading = false;
            $scope.newStatusText = data.text;
        }).error(function (data, status, headers, config) {
            $scope.isLoading = false;
            $scope.alert = "Oops... something went wrong";
            $scope.working = false;
        });
    };

    $scope.save = function () {
        $scope.isSaving = true;
        $scope.alert = "Saving...";
        $http.put("/api/Status?id=" + $routeParams.statusId, { "text": $scope.newStatusText }).success(function (data, status, headers, config) {
            $scope.alert = "Saved.";
            $scope.isSaving = false;
        }).error(function (data, status, headers, config) {
            $scope.isSaving = false;
            $scope.alert = "Oops... something went wrong";
            $scope.working = false;
        });
    };

    $scope.load();
});
