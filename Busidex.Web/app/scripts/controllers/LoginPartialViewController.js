﻿/*LOG IN*/
function LoginPartialViewCtrl($scope, $rootScope) {
    'use strict';
    $rootScope.MyBusidex = $rootScope.MyBusidex || [];
    $scope.Waiting = false;
    $rootScope.IsLoggedIn = $rootScope.User !== null;

}
LoginPartialViewCtrl.$inject = ['$scope', '$rootScope', '$cookieStore', '$routeParams'];