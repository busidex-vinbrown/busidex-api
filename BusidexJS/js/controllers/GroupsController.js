/*GROUPS (LIST)*/
function GroupsCtrl($scope, $rootScope, $cookieStore, $location, Groups, Analytics, Busigroup) {
    Analytics.trackPage($location.path());
    $scope.Waiting = false;
    $scope.GetGroup = resetGroup;
    $scope.Tabs = {};
    $scope.GroupId = 0;
    $rootScope.SetCurrentMenuItem('Groups');
    $rootScope.ShowFilterControls = false;
    $scope.GroupHelp = 'BusiGroups and Memberships are ways you can organize your cards. Use a BusiGroup if the group is just for you to see. Choose from any card in your Busidex collection to add to the group. Memberships are groups that all members of the group can see. The person that creates the group has control over who is included.';

    function resetGroup(groupId) {
        //$scope.Group = {};
        $scope.Cards = [];

        //setTimeout('getGroup(' + groupId + ')',1000);
        getGroup(groupId);
    }
    getGroup = function (groupId) {

        Busigroup.get({ id: groupId },
            function (response) {
                $scope.Group = {};
                $scope.Cards = [];
                $scope.Group = response.Model.Busigroup;

                setCurrentTab($scope.Group.Description);
                for (var i = 0; i < response.Model.BusigroupCards.length; i++) {
                    var card = response.Model.BusigroupCards[i];

                    card.OrientationClass = card.FrontOrientation == 'V' ? 'v_preview' : 'h_preview';

                    $scope.Cards.push(card);
                }
            },
            function () {

            });
    };

    var setCurrentTab = function (currentTab) {

        for (var tab in $scope.Tabs) {
            if ($scope.Tabs.hasOwnProperty(tab)) {
                $scope.Tabs[tab] = false;
            }
        }
        $scope.Tabs[currentTab] = true;
    };

    if ($rootScope.User === null) {
        $location.path("/account/login");
    } else {
        Groups.get({ id: $rootScope.User.UserId },
            function (response) {
                $scope.Groups = response.Model;
                for (var i = 0; i < $scope.Groups.length; i++) {
                    $scope.Tabs[$scope.Groups[i].Name] = false;
                }

                if ($scope.Groups.length > 0) {
                    setCurrentTab($scope.Tabs[$scope.Groups[0].Description]);
                    getGroup($scope.Groups[0].GroupId);
                }
            },
            function () {

            });

        $scope.DeleteGroup = function (groupId) {

            if (confirm('Are you sure you want to delete this group?')) {

                Groups.remove({ id: groupId },
                    function () {

                        for (var i = 0; i < $scope.Groups.length; i++) {
                            if ($scope.Groups[i].GroupId == groupId) {
                                $scope.Groups.splice(i, 1);
                                break;
                            }
                        }
                    },
                    function () {

                    });
            }
        };
    }
}
GroupsCtrl.$inject = ['$scope', '$rootScope', '$cookieStore', '$location', 'Groups', 'Analytics', 'Busigroup'];