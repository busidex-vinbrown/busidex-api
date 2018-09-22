/*EDIT GROUP*/
function EditGroupCtrl($scope, $rootScope, $cookieStore, $location, $route, Busidex, Groups, Busigroup) {

    $scope.Waiting = false;
    $rootScope.IsLoggedIn = $rootScope.User !== null;
    $scope.ShowSelectedOnly = false;
    $rootScope.SetCurrentMenuItem('Groups');
    $scope.Group = {
        GroupId: 0,
        Description: '',
        CardIds: []
    };

    if ($rootScope.User === null) {
        $location.path("/account/login");
    } else {

        Busigroup.get({ id: $route.current.params.id },
        function (response) {

            $scope.Group = response.Model.Busigroup;
            $scope.Group.CardIds = [];
            $scope.Cards = response.Model.BusigroupCards;

            Busidex.get({},
            function (bdexResponse) {

                $rootScope.MyBusidex = $scope.MyBusidex = bdexResponse.MyBusidex.Busidex;

                for (var b = 0; b < $scope.MyBusidex.length; b++) {
                    var busidexCard = $scope.MyBusidex[b];
                    for (var c = 0; c < $scope.Cards.length; c++) {
                        var groupCard = $scope.Cards[c];
                        if (busidexCard.CardId == groupCard.CardId) {
                            busidexCard.Selected = true;
                            $scope.Group.CardIds.push(groupCard.CardId);
                            break;
                        }
                    }
                }
            },
            function () {
                alert('error');
            });

        },
        function () {

        });

        $scope.ToggleSelected = function (card) {

            card.Selected = !card.Selected;
            if (card.Selected === true) {
                $scope.Group.CardIds.push(card.Card.CardId);
            } else {
                for (var i = 0; i < $scope.Group.CardIds.length; i++) {
                    if ($scope.Group.CardIds[i] == card.Card.CardId) {
                        $scope.Group.CardIds.splice(i, 1);
                        break;
                    }
                }
            }
        };
        $scope.save = function () {

            var data = { userId: $rootScope.User.UserId, groupTypeId: $scope.Group.GroupTypeId, id: $scope.Group.GroupId, cardIds: $scope.Group.CardIds.join(), description: $scope.Group.Description };

            Groups.update(data,
                function () {
                    $location.path("/groups/mine");
                },
                function () {
                    alert('error');
                });
        };
    }
}
EditGroupCtrl.$inject = ['$scope', '$rootScope', '$cookieStore', '$location', '$route', 'Busidex', 'Groups', 'Busigroup'];