angular.module('busidexstartApp').controller('SearchInfoController', [
    '$scope', '$http', 'Cache', 'CacheKeys', '$location', 'Card', 'Analytics', '$routeParams', 'FileModifiedService',
    function ($scope, $http, Cache, CacheKeys, $location, Card, Analytics, $routeParams, FileModifiedService) {
        'use strict';

        Analytics.trackPage($location.path());

        var vm = this;

        vm.model = {};
        vm.ModelError = false;
        vm.loading = true;
        vm.modified = false;
        vm.editmode = $routeParams.m === 'edit';

        var card = Cache.get(CacheKeys.Card);
        var user = Cache.get(CacheKeys.User);
        user = angular.fromJson(user);
        card = angular.fromJson(card);

        $http.defaults.headers.common['X-Authorization-Token'] = user.Token;

        var data = {
            'id': user.CardId,
            'userId': 0
        };
        Card.get(data,
            function (model) {
                vm.loading = false;
                card = model.Model;
                vm.card = card;
                vm.model = model;

                $scope.$watch(function () {
                    return vm.card.Name;
                }, function (newName, oldName) {
                    vm.modified = vm.modified || newName !== oldName;
                    checkModified();
                });
                $scope.$watch(function () {
                    return vm.card.CompanyName;
                }, function (newCompanyName, oldCompanyName) {
                    vm.modified = vm.modified || newCompanyName !== oldCompanyName;
                    checkModified();
                });
                $scope.$watch(function () {
                    return vm.card.Title;
                }, function (newTitle, oldTitle) {
                    vm.modified = vm.modified || newTitle !== oldTitle;
                    checkModified();
                });

                Analytics.trackEvent('CardUpdate', 'SearchInfoUpdate', vm.card.CardId.toString());
            },
            function () {
                vm.loading = false;
                vm.card = {};
                vm.ModelError = true;
            });

        function checkModified() {
            if (vm.editmode) {
                FileModifiedService.setModified(vm.modified);
            }
        }

        vm.save = function (direction) {

            if (!vm.modified) {
                if (direction >= 0) {
                    $location.path(direction === 1 ? '/tags' : '/contactinfo');
                }
                return;
            }

            vm.ModelError = false;
            Card.saveCardInfo(vm.card,
                function () {
                    if (direction >= 0) {
                        $location.path(direction === 1 ? '/tags' : '/contactinfo');
                    }
                    vm.modified = false;
                    checkModified();
                },
                function () {
                    vm.ModelError = true;
                });            
        };
    }
]);