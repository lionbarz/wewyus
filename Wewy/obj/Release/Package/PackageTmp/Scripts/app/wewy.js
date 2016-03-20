var app = angular.module('WewyApp', ['ngRoute']);

app.config(['$routeProvider',
  function ($routeProvider) {
      $routeProvider.
        when('/Group/:groupId', {
            templateUrl: 'templates/Group.html',
            controller: 'GroupCtrl'
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
        when('/EditGroup/:groupId', {
            templateUrl: 'templates/EditGroup.html',
            controller: 'EditGroupCtrl'
        }).
        otherwise({
            redirectTo: '/Groups'
        });
  }]);

app.controller('EditUserCtrl', function ($scope, $http, $window) {

    $scope.cities = [];
    $scope.userCity = "";
    $scope.isSaving = false;
    $scope.alert = "Loading...";

    $http.get("/api/City").success(function (data, status, headers, config) {
        $scope.alert = "";
        $scope.cities = data.cityNames;
        $scope.cityName = data.userCityName;
    }).error(function (data, status, headers, config) {
        $scope.alert = "Oops... something went wrong";
        $scope.working = false;
    });

    $scope.save = function () {
        mixpanel.track("Change city");
        $scope.isSaving = true;
        $scope.alert = "Saving...";
        $http.put("/api/User", { "cityName": $scope.cityName }).success(function (data, status, headers, config) {
            $scope.alert = "Saved."
            $scope.isSaving = false;
        }).error(function (data, status, headers, config) {
            $scope.isSaving = false;
            $scope.alert = "Oops... something went wrong";
            $scope.working = false;
        });
    };
});

app.controller('GroupCtrl', function ($scope, $http, $interval, $routeParams) {
    var dad = 1;
    $scope.isSendingStatus = false;
    $scope.thingsLoading = 0;
    $scope.displayMyStatuses = true;
    $scope.displaySortByDay = false;
    $scope.myUserData = null;
    $scope.loverUserData = null;
    $scope.groupId = $routeParams.groupId;

    $scope.reload = function () {
        mixpanel.track("Reload group");
        $scope.thingsLoading++;
        $http.get("/api/Status?groupId=" + $scope.groupId).success(function (data, status, headers, config) {
            $scope.thingsLoading--;
            $scope.statuses = data;
            $scope.colorStatuses();
            $scope.updateTimes();
        }).error(function (data, status, headers, config) {
            $scope.thingsLoading--;
            $scope.alert = "Oops... something went wrong";
        });
    };

    $scope.submitNewStatus = function () {
        mixpanel.track("Post new status");
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
            $scope.colorStatuses();
        }).error(function (data, status, headers, config) {
            $scope.isSendingStatus = false;
            $scope.alert = "Oops... something went wrong";
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
        $scope.thingsLoading++;
        $http.get("/api/Group?groupId=" + $scope.groupId).success(function (data, status, headers, config) {
            $scope.thingsLoading--;
            $scope.group = data;
            $scope.assignMemberColors();
            $scope.hashMembersById();
            $scope.updateUserTime();
            $interval($scope.updateUserTime, 10000);
            $scope.reload();
        }).error(function (data, status, headers, config) {
            $scope.thingsLoading--;
            if (status == 500) {
                $scope.alert = "Oops... something went wrong.";
            }
            else {
                $scope.alert = "The group doesn't exist or you don't have access.";
            }
        });
    }

    $scope.updateUserTime = function () {
        var now = moment();

        for (var member of $scope.group.members) {
            var memberTime = now.tz(member.timeZoneName);
            member.day = memberTime.format("ddd, MMM Do");
            member.time = memberTime.format("h:mm a");
        }
    }

    $scope.assignMemberColors = function () {
        // Assign a color to each member and start repeating if more members than colors.
        var colors = ["darkblue", "darkred",  "darkgoldenrod", "darkolivegreen", "black", "darkgrey"];
        for (var i=0; i < $scope.group.members.length; i++) {
            $scope.group.members[i].color = colors[i % colors.length];
        }
    }

    $scope.hashMembersById = function () {
        $scope.group.membersById = [];

        // For faster lookup when coloring statuses.
        for (var member of $scope.group.members) {
            $scope.group.membersById[member.id] = member;
        }
    }

    $scope.colorStatuses = function () {
        for (var status of $scope.statuses) {
            status.color = $scope.group.membersById[status.creatorId].color;
        }
    }

    $scope.getGroupData();
    $interval($scope.updateRelativeStatusTimes, 60000);
});

app.controller('EditStatusCtrl', function ($scope, $http, $interval, $routeParams, $window) {
    
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
        mixpanel.track("Edit status");
        $scope.isSaving = true;
        $scope.alert = "Saving...";
        $http.put("/api/Status?id=" + $routeParams.statusId, { "text": $scope.newStatusText }).success(function (data, status, headers, config) {
            $scope.alert = "Saved.";
            $scope.isSaving = false;
            $window.history.back();
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
            $scope.generateGroupPreview();
        }).error(function (data, status, headers, config) {
            $scope.isLoading = false;
            $scope.alert = "Oops... something went wrong";
            $scope.working = false;
        });
    };

    // Make a list of the members as the preview.
    $scope.generateGroupPreview = function () {
        for (var group of $scope.groups) {
            group.preview = group.members.map(function (e) { return e.name; }).join(', ');
        }
    }

    $scope.load();
});

app.controller('CreateGroupCtrl', function ($scope, $http, $interval, $location) {

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
                $scope.alert = $scope.newMemberEmail + " hasn't registered on this site.";
            } 
        }).error(function (data, status, headers, config) {
            $scope.isLoading = false;
            if (data && data.message) {
                $scope.alert = data.message;
            } else {
                $scope.alert = "Oops... something went wrong";
            }
        });
    };

    $scope.saveGroup = function () {
        mixpanel.track("Create group");

        if (!$scope.groupName)
        {
            $scope.alert = "Please specify group name.";
            return;
        }

        $scope.alert = "Creating group...";
        $scope.isSaving = true;

        $http.post("/api/Group", { "name": $scope.groupName, "members": $scope.members }).success(function (data, status, headers, config) {
            if (data.id) {
                $scope.alert = "Saved.";
                $scope.isSaving = false;
                $location.path("/Group/" + data.id);
            } else {
                $scope.alert = "Something went wrong.";
                $scope.isSaving = false;
            }
        }).error(function (data, status, headers, config) {
            $scope.isSaving = false;
            if (data && data.message) {
                $scope.alert = data.message;
            } else {
                $scope.alert = "Oops... something went wrong";
            }
        });
    };
});

app.controller('EditGroupCtrl', function ($scope, $http, $interval, $routeParams, $window) {

    $scope.group = null;

    $scope.load = function () {
        $scope.isLoading = true;
        $scope.alert = "Loading...";
        $http.get("/api/Group?groupId=" + $routeParams.groupId).success(function (data, status, headers, config) {
            $scope.isLoading = false;
            $scope.group = data;
            $scope.alert = "";
        }).error(function (data, status, headers, config) {
            $scope.isLoading = false;
            $scope.alert = "Oops... something went wrong";
        });
    };

    $scope.save = function () {
        mixpanel.track("Change group name");
        $scope.isSaving = true;
        $scope.alert = "Saving...";
        $http.put("/api/Group?groupId=" + $routeParams.groupId, $scope.group).success(function (data, status, headers, config) {
            $scope.alert = "Saved.";
            $scope.isSaving = false;
            $window.history.back();
        }).error(function (data, status, headers, config) {
            $scope.isSaving = false;
            $scope.alert = "Oops... something went wrong";
            $scope.working = false;
        });
    };

    $scope.load();
});