/*CREATE GROUP*/
function CreateGroupCtrl($scope, $rootScope, $cookieStore, $location, $route, Busidex, Groups) {
    'use strict';
    $scope.Waiting = false;
    $rootScope.IsLoggedIn = $rootScope.User !== null;
    $scope.ShowSelectedOnly = false;
    $rootScope.SetCurrentMenuItem('Groups');
    $scope.Group = {
        Description: '',
        CardIds: []
    };

    if ($rootScope.User === null) {
        $location.path('/account/login');
    } else {

        if (!$rootScope.MyBusidex || $rootScope.MyBusidex.length === 0) {
            Busidex.get({},
                function (response) {

                    $rootScope.MyBusidex = $scope.MyBusidex = response.MyBusidex.Busidex;

                },
                function () {
                    window.alert('error');
                });
        } else {
            $scope.MyBusidex = $rootScope.MyBusidex;
            for (var i = 0; i < $scope.MyBusidex.length; i++) {
                $scope.MyBusidex[i].Selected = false;
            }
        }

        $scope.ToggleSelected = function (card) {

            card.Selected = !card.Selected;
            if (card.Selected === true) {
                $scope.Group.CardIds.push(card.Card.CardId);
            } else {
                for (var j = 0; j < $scope.Group.CardIds.length; j++) {
                    if ($scope.Group.CardIds[j] === card.Card.CardId) {
                        $scope.Group.CardIds.splice(j, 1);
                        break;
                    }
                }
            }
        };
        $scope.save = function () {

            var data = { userId: $rootScope.User.UserId, groupTypeId: GROUPTYPE_PERSONAL, id: 0, cardIds: $scope.Group.CardIds.join(), description: $scope.Group.Description };

            Groups.post(data,
                function () {

                    $location.path('/groups/mine');

                },
                function () {
                    window.alert('error');
                });
        };
    }

}
CreateGroupCtrl.$inject = ['$scope', '$rootScope', '$cookieStore', '$location', '$route', 'Busidex', 'Groups'];