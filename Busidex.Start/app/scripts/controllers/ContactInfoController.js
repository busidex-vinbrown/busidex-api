angular.module('busidexstartApp').controller('ContactInfoController', [
    '$scope', '$http', 'Cache', 'CacheKeys', '$location', 'Card', 'PhoneNumberTypes', 'Analytics', '$routeParams', 'FileModifiedService',
    function ($scope, $http, Cache, CacheKeys, $location, Card, PhoneNumberTypes, Analytics, $routeParams, FileModifiedService) {
        'use strict';

        Analytics.trackPage($location.path());

        var vm = this;
        vm.PhoneNumberTypes = PhoneNumberTypes;
        vm.model = {};
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
            function(model) {

                vm.loading = false;
                card = model.Model;
                vm.card = card;
                vm.model = model;
                if (vm.card.PhoneNumbers === null) {
                    vm.card.PhoneNumbers = [];
                }
                if (vm.card.PhoneNumbers.length === 0) {
                    appendPhoneNumber();
                }
                Cache.put(CacheKeys.Card, angular.toJson(vm.card));

                $scope.$watch(function() {
                    return vm.card.Email;
                }, function(newEmail, oldEmail) {
                    vm.modified = vm.modified || newEmail !== oldEmail;
                    checkModified();
                });
                $scope.$watch(function() {
                    return vm.card.Url;
                }, function(newUrl, oldUrl) {
                    vm.modified = vm.modified || newUrl !== oldUrl;
                    checkModified();
                });

                $scope.$watch(function() {
                    return vm.card.PhoneNumbers;
                }, function(newPhoneNumbers, oldPhoneNumbers) {
                    vm.modified = vm.modified || newPhoneNumbers.length !== oldPhoneNumbers.length;
                    if (!vm.modified) {
                        for (var i = 0; i < newPhoneNumbers.length; i++) {
                            if (newPhoneNumbers[i].Number !== oldPhoneNumbers[i].Number ||
                                newPhoneNumbers[i].PhoneNumberTypeId !== oldPhoneNumbers[i].PhoneNumberTypeId) {
                                vm.modified = true;
                                break;
                            }
                        }
                    }
                    checkModified();
                }, true);
                Analytics.trackEvent('CardUpdate', 'ContactInfoUpdate', vm.card.CardId.toString());
            },
            function() {
                vm.loading = false;
                vm.card = card;
                vm.ModelError = true;
            });

        function checkModified() {
            if (vm.editmode) {
                FileModifiedService.setModified(vm.modified);
            }
        }

        vm.save = function(direction) {

            if (!vm.modified) {
                if (direction >= 0) {
                    $location.path(direction === 1 ? '/searchinfo' : '/visibility');
                }
                return;
            }

            Card.saveCardInfo(vm.card,
                function() {
                    if (direction >= 0) {
                        $location.path(direction === 1 ? '/searchinfo' : '/visibility');
                    }
                    vm.modified = false;
                    checkModified();
                },
                function() {
                    vm.ModelError = true;
                });
        };

        vm.AllowNext = function() {
            return vm.card.Email.length > 0 || vm.card.PhoneNumbers.length > 0;
        };

        vm.AddPhoneNumber = function() {

            for (var i = 0; i < vm.card.PhoneNumbers.length; i++) {
                if (vm.card.PhoneNumbers[i].PhoneNumberTypeId === null) {
                    vm.card.PhoneValid = false;
                    return false;
                }
            }
            vm.PhoneValid = true;

            appendPhoneNumber();

            return false;
        };
        vm.RemovePhoneNumber = function(idx) {

            if (vm.card.PhoneNumbers.length === 1) {
                return;
            }

            vm.card.PhoneNumbers.splice(idx, 1);
        };

        function appendPhoneNumber() {
            vm.card.PhoneNumbers.push({
                CardId: vm.card.CardId,
                Created: new Date(),
                Updated: new Date(),
                Number: '',
                PhoneNumberId: 0,
                PhoneNumberTypeId: 0,
                PhoneNumberType: {
                    PhoneNumberTypeId: 0,
                    Name: '',
                    Deleted: false
                },
                Extension: '',
                Deleted: false
            });
        }
    }
]);