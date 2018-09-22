/*CONFIRM MY CARD*/
function ConfirmCardCtrl($scope, $route) {
    'use strict';
    $scope.Token = $route.current.params.token;
}
ConfirmCardCtrl.$inject = ['$scope', '$route'];