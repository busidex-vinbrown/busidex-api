
var services = services || angular.module('Busidex.services', ['$rootScope', 'Cache', 'CacheKeys']);
services
    .service('SharedCardCart', function($rootScope, Cache, CacheKeys) {
        'use strict';

        var vm = this;

        function _checkIfExists(id) {
            var exists = false;

            var cart = Cache.get(CacheKeys.SharedCards);
            if (cart !== null) {
                cart = angular.fromJson(cart);
                for (var i = 0; i < cart.length; i++) {
                    var cardId = cart[i].CardId;
                    if (cardId === id) {
                        exists = true;
                        break;
                    }
                }
            }
            return exists;
        }

        vm.exists = function(id) {
            return _checkIfExists(id);
        };

        vm.Count = function() {
            var cart = Cache.get(CacheKeys.SharedCards) || [];
            cart = angular.fromJson(cart);
            return cart.length;
        };

        vm.get = function() {
            var cart = Cache.get(CacheKeys.SharedCards) || [];
            cart = angular.fromJson(cart);

            return cart;
        };

        vm.put = function(card) {
            if (!_checkIfExists(card.CardId)) {
                var cart = Cache.get(CacheKeys.SharedCards) || [];
                cart = angular.fromJson(cart);

                cart.push({
                    CardId: card.CardId,
                    FrontFileId: card.FrontFileId,
                    FrontType: card.FrontType,
                    Name: card.Name,
                    CompanyName: card.CompanyName,
                    FrontOrientationClass: card.FrontOrientationClass
                });
                Cache.put(CacheKeys.SharedCards, angular.toJson(cart));

                $rootScope.$broadcast(
                    'shared-card.put',
                    { key: CacheKeys.SharedCards, newValue: vm.Count(), oldValue: vm.Count() - 1 });
            }
        };

        vm.remove = function(idx) {
            var cart = Cache.get(CacheKeys.SharedCards) || [];
            cart = angular.fromJson(cart);

            if (idx <= cart.length) {
                cart.splice(idx, 1);

                $rootScope.$broadcast(
                    'shared-card.remove',
                    { key: CacheKeys.SharedCards, newValue: vm.Count(), oldValue: vm.Count() + 1 });
            }
            Cache.put(CacheKeys.SharedCards, angular.toJson(cart));
        };

        vm.clear = function() {
            Cache.put(CacheKeys.SharedCards, []);

            var count = vm.Count();
            $rootScope.$broadcast(
                'shared-card.remove',
                { key: CacheKeys.SharedCards, newValue: 0, oldValue: count });
        };


    });