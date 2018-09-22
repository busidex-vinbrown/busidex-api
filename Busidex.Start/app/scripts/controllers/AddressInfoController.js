
angular.module('busidexstartApp').controller('AddressInfoController', [
    '$scope', '$http', 'Cache', 'CacheKeys', '$location', 'Card', 'StateCodes', 'Analytics', '$routeParams', 'FileModifiedService',
    function ($scope, $http, Cache, CacheKeys, $location, Card, StateCodes, Analytics, $routeParams, FileModifiedService) {
        'use strict';

        Analytics.trackPage($location.path());

        var vm = this;
        vm.model = {};
        vm.loading = false;
        vm.StateCodes = StateCodes;
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

        /**
         * @param {id}
         * @param {cardId}
         * @param {addr1}
         * @param {addr2}
         * @param {city}
         * @param {state}
         * @param {zip} 
         */
        var Address = function(id, cardId, addr1, addr2, city, state, zip) {

            if (state === null) {
                state = {
                    StateCodeId: 0,
                    Code: '',
                    Name: ''
                };
            }
            var self = this;
            self.CardAddressId = id;
            self.CardId = cardId;
            self.Address1 = addr1;
            self.Address2 = addr2;
            self.City = city;
            self.State = {
                StateCodeId: state.StateCodeId,
                Code: state.Code,
                Name: state.Name
            };
            self.ZipCode = zip;
            self.Deleted = false;
            self.DeleteSrc = '../../img/delete.png';
            self.Display = function() {
                return self.Address1 + ' ' +
                    self.Address2 + ' ' +
                    self.City + ' ' +
                    (self.State !== null ? self.State.Code : '') +
                    (self.ZipCode !== null && self.ZipCode.length > 0 ? ', ' : ' ') +
                    self.ZipCode;
            };
            self.Selected = false;
        };

        Card.get(data,
            function(model) {
                vm.loading = false;
                card = model.Model;

                var addresses = [];
                for (var i = 0; i < model.Model.Addresses.length; i++) {
                    var a = model.Model.Addresses[i];
                    addresses.push(
                        new Address(a.CardAddressId, a.CardId, a.Address1, a.Address2, a.City, a.State, a.ZipCode)
                    );
                }
                card.Addresses = addresses;

                vm.card = card;
                vm.model = model;
                if (vm.card.Addresses === undefined || vm.card.Addresses === null) {
                    vm.card.Addresses = [];
                    vm.card.Addresses.push(new Address());
                }

                Cache.put(CacheKeys.Card, angular.toJson(vm.card));

                $scope.$watch(function() {
                    return vm.card.Addresses[0];
                }, function(newAddress, oldAddress) {
                    vm.modified = vm.modified || newAddress !== oldAddress;
                    checkModified();
                }, true);
                Analytics.trackEvent('CardUpdate', 'AddressInfoUpdate', vm.card.CardId.toString());
            },
            function() {
                vm.loading = false;
                vm.card = {};
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
                    if (!user.OnboardingComplete) {
                        $location.path(direction === 1 ? '/nextsteps' : '/tags');
                    } else {
                        if (direction === 1) {
                            window.location.href = SITEROOT + '/#/card/mine';
                        } else {
                            $location.path('/tags');
                        }
                    }
                }
                return;
            }

            vm.ModelError = false;
            Card.saveCardInfo(vm.card,
                function() {
                    if (direction > 0) {
                        if (!user.OnboardingComplete) {
                            $location.path(direction === 1 ? '/nextsteps' : '/tags');
                        } else {
                            if (direction === 1) {
                                window.location.href = SITEROOT + '/#/card/mine';
                            } else {
                                $location.path('/tags');
                            }
                        }
                    }
                    vm.modified = false;
                    checkModified();
                },
                function() {
                    vm.ModelError = true;
                });

        };
    }
]);