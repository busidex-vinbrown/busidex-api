
angular.module('Busidex').controller('ShareCardController', [
    'SharedCardCart', 'SharedCard', 'Cache', 'CacheKeys',
    function (SharedCardCart, SharedCard, Cache, CacheKeys) {
        'use strict';

        var vm = this;
        vm.email = '';
        vm.message = '';

        var cards = SharedCardCart.get();
        vm.cards = angular.fromJson(cards) || [];
        vm.notifications = [];

        vm.message = '';

        vm.share = function() {

            if (vm.email.length === 0) {
                return;
            }

            var sharedCards = [];
            var user = Cache.get(CacheKeys.User);
            user = angular.fromJson(user);
            var shareDate = new Date();            

            for (var i = 0; i < vm.cards.length; i++) {
                var card = vm.cards[i];

                var sharedCard = {
                    CardId: card.CardId,
                    SendFrom: user.UserId,
                    Email: vm.email,
                    ShareWith: null,
                    SharedDate: shareDate,
                    Accepted: null,
                    Declined: null,
                    Recommendation: vm.message
                };
                sharedCards.push(sharedCard);
            }

            SharedCard.post(sharedCards,
                function() {
                    SharedCardCart.clear();
                    vm.message = '';
                    vm.email = '';
                    vm.cards = [];
                },
                function() {

                });
        };

        vm.removeFromList = function(idx) {
            SharedCardCart.remove(idx);
            vm.cards.splice(idx, 1);
        };
    }
]);