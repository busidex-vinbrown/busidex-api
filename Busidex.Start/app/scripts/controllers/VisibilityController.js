angular.module('busidexstartApp').controller('VisibilityController', [
    '$scope', '$http', 'Cache', 'CacheKeys', '$location', 'Card', 'Analytics', '$routeParams', 'FileModifiedService',
    function ($scope, $http, Cache, CacheKeys, $location, Card, Analytics, $routeParams, FileModifiedService) {
        'use strict';

        Analytics.trackPage($location.path());

        var vm = this;

        vm.FileError = false;
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

                $scope.$watch(function () {
                    return vm.card.Visibility;
                }, function (newValue, oldValue) {
                    vm.modified = newValue !== oldValue;
                    checkModified();
                });

                Analytics.trackEvent('CardUpdate', 'VisibilityUpdate', vm.card.CardId.toString());
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

        vm.SetVisibility = function(v) {
            if (vm.card !== undefined) {
                vm.card.Visibility = v;
            } 
        };

        vm.AllowNext = function () {
            return vm.card !== undefined && vm.card.Visibility > 0;
        };

        vm.save = function(direction) {

            if (vm.card.Visibility > 0) {
                if (vm.modified) {
                    Card.saveVisibility({ visibility: vm.card.Visibility },
                        function () {
                            if (direction >= 0) {
                                $location.path('/contactinfo');
                            }
                            vm.modified = false;
                            checkModified();
                        },
                        function() {
                            vm.ModelError = true;
                        });
                } else {
                    if (direction >= 0) {
                        $location.path('/contactinfo');
                    }
                }
            }
        };
    }
]);