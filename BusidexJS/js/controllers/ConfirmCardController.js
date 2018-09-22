/*CONFIRM MY CARD*/
function ConfirmCardCtrl($scope, $route) {

    $scope.Token = $route.current.params.token;
}
ConfirmCardCtrl.$inject = ['$scope', '$route'];