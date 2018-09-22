/*LOG OUT*/
function LogoutViewCtrl($scope, $rootScope, $cookieStore, $location) {
    'use strict';
    $cookieStore.remove('User');
    $rootScope.User = $scope.User = null;
    $rootScope.MyBusidex = null;
    $rootScope.HasSharedCards = false;
    $rootScope.SharedCards = [];
    $rootScope.IsLoggedIn = false;

    $rootScope.SearchModel = null;

    gapi.auth.signOut();

    $rootScope.LoginModel = {
        LoginText: 'Login',
        LoginRoute: '#/account/login',
        HomeLink: '#/home'
    };
    $location.path('/home');
}
LogoutViewCtrl.$inject = ['$scope', '$rootScope', '$cookieStore', '$location'];