﻿var app = angular.module('WewyApp', ['ngRoute']);

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
            templateUrl: 'templates/EditGroup.html',
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

});

app.controller('GroupCtrl', function ($scope, $http, $timeout, $routeParams) {
    var dad = 1;
    $scope.isSendingStatus = false;
    $scope.isLoading = true;
    $scope.displayMyStatuses = true;
    $scope.displaySortByDay = false;
    $scope.myUserData = null;
    $scope.loverUserData = null;
    $scope.groupId = $routeParams.groupId;

    // When the statuses were last refreshed here in the client.
    $scope.statusesLastUpdateDate = null;
    $scope.statusesLastUpdateText = "never";

    $scope.updateStatusesLastUpdateText = function () {
        $timeout($scope.updateStatusesLastUpdateText, 10000);

        if (!$scope.statusesLastUpdateDate) {
            $scope.statusesLastUpdateText = "never";
        } else {
            var mom = moment($scope.statusesLastUpdateDate)
            $scope.statusesLastUpdateText = mom.fromNow();
        }
    }

    $scope.reload = function () {
        mixpanel.track("Reload group");
        $scope.isLoading = true;
        $http.get("/api/Status?groupId=" + $scope.groupId).success(function (data, status, headers, config) {
            $scope.isLoading = false;
            $scope.statuses = data;
            $scope.statusesLastUpdateDate = new Date();
            $scope.statusesLastUpdateText = "just now";
            $scope.colorStatuses();
            $scope.updateTimes();
            $timeout($scope.getGroupData, 60000);
        }).error(function (data, status, headers, config) {
            $scope.isLoading = false;
            $scope.alert = "Oops... something went wrong";
        });
    };

    $scope.submitNewStatus = function () {
        mixpanel.track("Post new status");
        var text = $scope.newStatusText;

        if (!text) {
            return;
        }

        var requestBody = {
            "text": text,
            "position": $scope.position,
            "TimezoneOffsetMinutes": -(new Date).getTimezoneOffset()
        };

        $scope.isSendingStatus = true;
        $http.post("/api/Status?groupId=" + $scope.groupId, requestBody).success(function (data, status, headers, config) {
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
                var loc = null;
                if (view.city) {
                    loc = "(" + view.city + ", " + view.viewerName + ")";
                } else {
                    loc = "(" + view.viewerName + ")";
                }
                status.formattedDates.push(moment(view.viewTimeLocal).format(formatString) + " " + loc);
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

        $timeout($scope.updateRelativeStatusTimes, 60000);
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
        $scope.isLoading = true;
        $http.get("/api/Group?groupId=" + $scope.groupId).success(function (data, status, headers, config) {
            $scope.group = data;
            $scope.assignMemberColors();
            $scope.hashMembersById();
            $scope.updateUserTime(false);
            $scope.reload();
        }).error(function (data, status, headers, config) {
            $scope.isLoading = false;
            $scope.warning = "Oops... couldn't get group info.";
        });
    }

    $scope.updateUserTime = function (schedule) {
        var now = moment.utc();

        if (schedule) {
            $timeout(function () { $scope.updateUserTime(true) }, 60000);
        }

        if (!$scope.group || !$scope.group.members)
        {
            return;
        }

        for (var member of $scope.group.members) {
            var memberTime = now.utcOffset(member.timezoneOffsetMinutes);
            member.day = memberTime.format("ddd, MMM Do");
            member.time = memberTime.format("h:mm a");
        }    
    }

    $scope.assignMemberColors = function () {
        // Assign a color to each member and start repeating if more members than colors.
        var colors = ["darkblue", "darkred",  "darkgoldenrod", "darkolivegreen", "pink", "orange"];
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
            var member = $scope.group.membersById[status.creatorId];
            if (member) {
                status.color = $scope.group.membersById[status.creatorId].color;
            }
            else {
                // The member has left the group.
                status.color = "black";
            }
        }
    }

    $scope.storePosition = function (geo) {
        $scope.alert = "Successfully got your location!";
        $scope.position = { "latitude": geo.coords.latitude, "longitude": geo.coords.longitude };
    }

    $scope.positionError = function (error) {
        var message = "";
        switch (error.code) {
            case error.PERMISSION_DENIED:
                message = "You denied the request for geolocation."
                break;
            case error.POSITION_UNAVAILABLE:
                message = "Your location information is unavailable."
                break;
            case error.TIMEOUT:
                message = "The request to get your location timed out."
                break;
            case error.UNKNOWN_ERROR:
                message = "An unknown error occurred with geolocation."
                break;
        }
        $scope.warning = message;
    }

    if (navigator.geolocation) {
        $scope.alert = "Looking for your location...";
        navigator.geolocation.getCurrentPosition($scope.storePosition, $scope.positionError);
    } else {
        $scope.warning = "Your browser doesn't support geolocation. Use a newer browser!";
    }

    $scope.getGroupData();
    $scope.updateRelativeStatusTimes();
    $scope.updateUserTime(true);
    $scope.updateStatusesLastUpdateText();
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

        var requestBody = {
            "text": $scope.newStatusText,
            "position": $scope.position,
            "timezoneOffset": - (new Date).getTimezoneOffset()
        };

        $http.put("/api/Status?id=" + $routeParams.statusId, requestBody).success(function (data, status, headers, config) {
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

app.controller('EditGroupCtrlBase', function ($scope, $http, $interval, $routeParams, $window, $location) {
    $scope.user = null;
    $scope.group = null;
    $scope.isModified = false;

    var memberExists = function (email) {
        for (var member of $scope.group.members) {
            if (member.email === email) {
                return true;
            }
        }
        return false;
    };

    $scope.addMember = function () {
        if (!$scope.newMemberEmail) {
            $scope.alert = "Please enter the email of the person you wish to add to the group.";
            return;
        }

        if (memberExists($scope.newMemberEmail)) {
            $scope.alert = "You added that person already.";
            $scope.newMemberEmail = "";
            return;
        }

        if ($scope.user && $scope.user.email === $scope.newMemberEmail) {
            $scope.alert = "You are already a member of this group.";
            $scope.newMemberEmail = "";
            return;
        }

        $scope.alert = "Adding...";
        $http.get("/api/User?email=" + encodeURI($scope.newMemberEmail)).success(function (data, status, headers, config) {
            if (data.name) {
                $scope.group.members.push(data);
                $scope.isModified = true;
                $scope.alert = "";
                $scope.newMemberEmail = "";
            }
            else {
                $scope.alert = $scope.newMemberEmail + " hasn't registered on this site.";
            }
        }).error(function (data, status, headers, config) {
            if (data && data.message) {
                $scope.alert = data.message;
            } else {
                $scope.alert = "Oops... something went wrong";
            }
        });
    };

    $scope.removeMember = function (member) {
        var index = $scope.group.members.indexOf(member);
        if (index > -1) {
            $scope.isModified = true;
            $scope.group.members.splice(index, 1);
        }
    }

    $scope.isValid = function () {
        return $scope.group &&
            $scope.group.name &&
            $scope.group.members.length > 0 &&
            !($scope.newMemberEmail && $scope.newMemberEmail.length > 0);
    }

    $scope.cancel = function () {
        $window.history.back();
    }
});

app.controller('CreateGroupCtrl', function ($scope, $controller, $http, $interval, $routeParams, $window, $location) {
    $controller('EditGroupCtrlBase', { $scope: $scope });
    $scope.isEditMode = false;
    $scope.title = "Create Group";
    $scope.saveButtonTitle = "Create";
    $scope.alert = "";
    $scope.group = { members: [], isUserAdmin: true };
    $scope.isLoading = false;

    $scope.save = function () {
        mixpanel.track("Create group");

        if (!$scope.group.name) {
            $scope.alert = "Please specify a name for the group.";
            return;
        }

        if ($scope.group.members.length < 1) {
            $scope.alert = "Please add someone to the group.";
            return;
        }

        $scope.alert = "Creating group...";
        $scope.isSaving = true;

        $http.post("/api/Group", $scope.group).success(function (data, status, headers, config) {
            if (data.id) {
                $scope.alert = "Saved.";
                $scope.isSaving = false;
                $scope.isModified = false;
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

app.controller('EditGroupCtrl', function ($scope, $controller, $http, $interval, $routeParams, $window, $location) {
    $controller('EditGroupCtrlBase', { $scope: $scope });
    $scope.isEditMode = true;
    $scope.title = "Edit Group";
    $scope.saveButtonTitle = "Save";
    $scope.alert = "";
    $scope.isLoading = true;
    
    $scope.load = function () {
        $scope.isLoading = true;
        $scope.alert = "Loading...";
        $http.get("/api/Group?groupId=" + $routeParams.groupId).success(function (data, status, headers, config) {
            $scope.isLoading = false;
            $scope.group = data;
            $scope.alert = "";
            for (var mem of $scope.group.members) {
                if ($scope.group.admin && mem.id === $scope.group.admin.id) {
                    mem.isAdmin = true;
                }
            }
        }).error(function (data, status, headers, config) {
            $scope.isLoading = false;
            $scope.alert = "Oops... something went wrong";
        });
    };

    $scope.save = function () {
        mixpanel.track("Edit group");
        $scope.isSaving = true;
        $scope.alert = "Saving...";
        $http.put("/api/Group?groupId=" + $routeParams.groupId, $scope.group).success(function (data, status, headers, config) {
            $scope.alert = "Saved.";
            $scope.isSaving = false;
            $scope.isModified = false;
            $window.history.back();
        }).error(function (data, status, headers, config) {
            $scope.isSaving = false;
            $scope.alert = "Oops... something went wrong";
        });
    };

    $scope.delete = function () {
        var result = $window.confirm("Are you sure you want to leave this group?");
        if (result) {
            $scope.confirmedDelete();
        }
    }

    $scope.confirmedDelete = function () {
        mixpanel.track("Leave group");
        $scope.isSaving = true;
        $scope.alert = "Leaving group...";
        $http.delete("/api/Group/" + $routeParams.groupId, $scope.group).success(function (data, status, headers, config) {
            $scope.alert = "You have left this group.";
            $scope.isSaving = false;
            $location.path("/Groups");
        }).error(function (data, status, headers, config) {
            $scope.isSaving = false;
            $scope.alert = "Oops... something went wrong";
        });
    }

    $http.get("/api/User").success(function (data, status, headers, config) {
        $scope.user = data;
    }).error(function (data, status, headers, config) {
        // The user is only used to not let the user add himself to the group.
        // No big deal if it fails. We don't need to show an error message.
    });

    $scope.load();
});