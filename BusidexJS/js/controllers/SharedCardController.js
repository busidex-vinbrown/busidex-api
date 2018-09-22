/*SHARED CARDS*/
function SharedCardCtrl($scope, $rootScope, $cookieStore, $http, SharedCard, $location, $route) {

    $rootScope.User = $rootScope.User || $cookieStore.get('User');

    var userId = $rootScope.User !== null ? $rootScope.User.UserId : 0;

    $scope.HasSharedCards = false;


    if ($rootScope.User !== null) {
        SharedCard.get({},
            function (data) {
                if (data.SharedCards && data.SharedCards.length > 0) {
                    $rootScope.HasSharedCards = true;
                    $rootScope.SharedCards = [];
                    for (var i = 0; i < data.SharedCards.length; i++) {
                        $rootScope.SharedCards.push(data.SharedCards[i]);
                    }

                    $scope.SendFromEmail = data.SharedCards[0].SendFromEmail;
                }
            },
            function () {

            });
    }

    $scope.AcceptSharedCards = function () {

        var accepted = [];
        var declined = [];

        for (var i = $scope.$parent.SharedCards.length - 1; i >= 0 ; i--) {
            var sharedCard = $scope.$parent.SharedCards[i];
            if (sharedCard.Accepted == "true") {
                accepted.push(sharedCard.CardId);
                $scope.$parent.SharedCards.splice(i, 1);
                $scope.SharedCards.splice(i, 1);
            }
            if (sharedCard.Accepted == "false") {
                declined.push(sharedCard.CardId);
                $scope.$parent.SharedCards.splice(i, 1);
                $scope.SharedCards.splice(i, 1);
            }
        }
        var model = { AcceptedCardIdList: accepted, DeclinedCardIdList: declined, UserId: userId, SharedWith: userId };

        SharedCard.update({ id: userId }, JSON.stringify(model),
            function (data) {

                if ($scope.$parent.SharedCards.length === 0) {
                    $rootScope.HasSharedCards = $scope.HasSharedCards = false;
                }
                $rootScope.MyBusidex = []; // force a refresh
                if ($location.path() == '/busidex/mine') {
                    $route.reload();
                } else {
                    $location.path('/busidex/mine');
                }
            },
            function (error) {

            });


    };
}
SharedCardCtrl.$inject = ['$scope', '$rootScope', '$cookieStore', '$http', 'SharedCard', '$location', '$route'];
