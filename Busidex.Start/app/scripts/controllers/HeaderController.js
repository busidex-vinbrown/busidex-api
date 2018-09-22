
angular.module('busidexstartApp').controller('HeaderController', [
    '$scope', 'Cache', 'CacheKeys', '$routeParams', '$location',
    function ($scope, Cache, CacheKeys, $routeParams, $location) {
        'use strict';

        var vm = this;
        vm.LoggedIn = false;
        vm.HasCard = false;
        vm.LoginRoute = Cache.get(CacheKeys.LoginRoute);
        vm.LoginText = Cache.get(CacheKeys.LoginText);
        

        $scope.$watch(function () {
            return Cache.get(CacheKeys.User);
        }, function (newUser) {
            vm.LoggedIn = newUser !== null && newUser !== undefined;

            if (vm.LoggedIn) {
                Cache.put(CacheKeys.LoginRoute, '#/logout');
                Cache.put(CacheKeys.LoginText, 'LOGOUT');
            } else {
                Cache.put(CacheKeys.LoginRoute, '#/login');
                Cache.put(CacheKeys.LoginText, 'LOGIN');
            }
        }, true);

        $scope.$watch(function () {
            return $routeParams.m;
        }, function (newval) {
            vm.startmode = newval !== 'edit';
            vm.editmode = newval === 'edit';
            vm.mode = newval;
        }, true); 

        $scope.$watchCollection(function() {
            return $location.path();
        }, function() {
            var navItems = {
                FRONT_IMAGE: 1,
                BACK_IMAGE: 2,
                VISIBILITY: 3,
                CONTACT_INFO: 4,
                SEARCH_INFO: 5,
                TAGS: 6,
                ADDRESS: 7,
                UNKNOWN: -1
            };

            if ($location.path().indexOf('/front') >= 0) {
                vm.ItemIndex = navItems.FRONT_IMAGE;
            } else if ($location.path().indexOf('/back') >= 0) {
                vm.ItemIndex = navItems.BACK_IMAGE;
            } else if ($location.path().indexOf('/visibility') >= 0) {
                vm.ItemIndex = navItems.VISIBILITY;
            } else if ($location.path().indexOf('/contactinfo') >= 0) {
                vm.ItemIndex = navItems.CONTACT_INFO;
            } else if ($location.path().indexOf('/searchinfo') >= 0) {
                vm.ItemIndex = navItems.SEARCH_INFO;
            } else if ($location.path().indexOf('/tags') >= 0) {
                vm.ItemIndex = navItems.TAGS;
            } else if ($location.path().indexOf('/addressinfo') >= 0) {
                vm.ItemIndex = navItems.ADDRESS;
            } else {
                vm.ItemIndex = navItems.UNKNOWN;
            }
        });

        $scope.$watch(function () {
            return Cache.get(CacheKeys.Card);
        }, function (newCard) {
            newCard = angular.fromJson(newCard);
            vm.HasCard = newCard !== null && newCard !== undefined && newCard.FrontFileId !== null && newCard.FrontFileId !== undefined && newCard.FrontFileId.length > 0;
        }, true);

        $scope.$watch(function () {
            return Cache.get(CacheKeys.LoginRoute);
        }, function (newRoute) {
            vm.LoginRoute = newRoute;
        }, true);

        $scope.$watch(function () {
            return Cache.get(CacheKeys.LoginText);
        }, function (newText) {
            vm.LoginText = newText;
        }, true);
    }
]);