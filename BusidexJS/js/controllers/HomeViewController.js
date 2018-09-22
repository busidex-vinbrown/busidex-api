/*HOME*/
function HomeViewCtrl($scope, $rootScope, $cookieStore, $http, $location, Feature, Busidex) {
    $rootScope.ShowFilterControls = false;
    $scope.model = {};
    $scope.model.FeaturedCard = null;
    $scope.AddToMyBusidex = addToMyBusidex;
    $scope.ExistsInMyBusidex = false;

    Feature.get({},
        function (resp) {

            $scope.model.FeaturedCard = resp.FeaturedCard;
        },
        function () {

        });

    function addToMyBusidex(id) {

        if ($rootScope.User === null) {
            $location.path('/account/login');
            return;
        }

        Busidex.post({ userId: $rootScope.User.UserId, cId: id },
            function () {
                alert("This card is now in your Busidex!");
                $scope.ExistsInMyBusidex = true;
            },
            function () {
                alert('There was a problem adding this card to your Busidex');
                return false;
            });
    }

    $rootScope.SetCurrentMenuItem('Home');
}
HomeViewCtrl.$inject = ['$scope', '$rootScope', '$cookieStore', '$http', '$location', 'Feature', 'Busidex'];