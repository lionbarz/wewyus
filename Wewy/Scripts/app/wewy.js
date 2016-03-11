var app = angular.module('WewyApp', ['ngRoute']);

app.config(['$routeProvider',
  function ($routeProvider) {
      $routeProvider.
        when('/Status/:groupId', {
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
        when('/Groups', {
            templateUrl: 'templates/Groups.html',
            controller: 'GroupsCtrl'
        }).
        when('/CreateGroup', {
            templateUrl: 'templates/CreateGroup.html',
            controller: 'CreateGroupCtrl'
        }).
        otherwise({
            redirectTo: '/Groups'
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

app.controller('StatusCtrl', function ($scope, $http, $interval, $routeParams) {
    var dad = 1;
    $scope.isSendingStatus = false;
    $scope.isLoading = false;
    $scope.displayMyStatuses = true;
    $scope.displaySortByDay = false;
    $scope.myUserData = null;
    $scope.loverUserData = null;
    $scope.groupId = $routeParams.groupId;

    $scope.reload = function () {
        $scope.isLoading = true;
        $http.get("/api/Status?groupId=" + $scope.groupId).success(function (data, status, headers, config) {
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
        $http.post("/api/Status?groupId=" + $scope.groupId, { "text": text }).success(function (data, status, headers, config) {
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

        var formatString = "ddd, MMM Do, h:mm a";

        for (i in $scope.statuses)
        {
            status = $scope.statuses[i];
            mom = moment(status.dateCreatedUtc + "Z"); 
            fromNow = mom.fromNow();
            status.formattedDates = [fromNow];

            for (j in status.views) {
                var view = status.views[j];
                status.formattedDates.push(moment(view.viewTimeLocal).format(formatString) + " (" + view.cityName + ", " + view.viewerName + ")");
                status.dateIndex = 0;
            }
        }
    };

    $scope.updateRelativeStatusTimes = function () {
        var status, mom, fromNow;
        for (i in $scope.statuses) {
            status = $scope.statuses[i];
            mom = moment(status.dateCreatedUtc + "Z");
            fromNow = mom.fromNow();
            status.formattedDates[0] = fromNow;
        }
    }

    $scope.changeSort = function () {
        if ($scope.displaySortByDay)
        {
            // Sort by local time.
            sortFunction = function (a, b) {
                return (new Date(b.dateCreatedCreator)) - (new Date(a.dateCreatedCreator));
            };

            // Show in time local to creator.
            // TODO: Index 1 is no longer the creator necessarily.
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

    $scope.getGroupData = function () {
        $http.get("/api/Group?groupId=" + $scope.groupId).success(function (data, status, headers, config) {
            $scope.group = data;
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

        for (i in $scope.group.members) {
            var member = $scope.group.members[i];
            var memberTime = now.tz(member.timeZoneName);
            member.day = memberTime.format("ddd, MMM Do");
            member.time = memberTime.format("h:mm a");
        }
    }

    $scope.reload();
    $scope.getGroupData();
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

app.controller('GroupsCtrl', function ($scope, $http, $interval) {

    $scope.groups = [];

    $scope.load = function () {
        $scope.isLoading = true;
        $http.get("/api/Group").success(function (data, status, headers, config) {
            $scope.isLoading = false;
            $scope.groups = data;
        }).error(function (data, status, headers, config) {
            $scope.isLoading = false;
            $scope.alert = "Oops... something went wrong";
            $scope.working = false;
        });
    };

    $scope.load();
});

app.controller('CreateGroupCtrl', function ($scope, $http, $interval, $window) {

    $scope.members = [];
    $scope.alert = "";

    $scope.addMember = function () {
        $scope.alert = "Adding...";
        $http.get("/api/User?email=" + encodeURI($scope.newMemberEmail)).success(function (data, status, headers, config) {
            if (data.name) {
                $scope.members.push(data);
                $scope.alert = "";
                $scope.newMemberEmail = "";
            }
            else {
                $scope.alert = $scope.newMemberEmail + " doesn't exist.";
            } 
        }).error(function (data, status, headers, config) {
            $scope.isLoading = false;
            $scope.alert = "Oops... something went wrong";
            $scope.working = false;
        });
    };

    $scope.saveGroup = function () {
        if (!$scope.groupName)
        {
            $scope.alert = "Please specify group name.";
            return;
        }

        $scope.alert = "Creating group...";

        $http.post("/api/Group", { "name": $scope.groupName, "members": $scope.members }).success(function (data, status, headers, config) {
            if (data.id) {
                $scope.alert = "Saved.";
                $scope.isSaving = false;
                $location.path("/Status?groupId=" + data.groupId);
            } else {
                $scope.alert = "Something went wrong.";
                $scope.isSaving = false;
            }
        }).error(function (data, status, headers, config) {
            $scope.isSaving = false;
            $scope.alert = "Oops... something went wrong";
            $scope.working = false;
        });
    };

});