/*MY BUSIDEX*/
angular.module('Busidex').controller('MyBusidexCardController', [
    'Notes', 'SharedCardCart',
    function (Notes, SharedCardCart) {
        'use strict';

        var vm = this;
        //var user = Cache.get(CacheKeys.User);
        //user = angular.fromJson(user);

        //vm.User = user;

        vm.EditNotes = function (card) {

            Notes.update({ id: card.UserCardId, notes: window.escape(card.Notes) },
                function () {

                },
                function () {

                });
        };
        
        vm.rotateCard = function (card) {
            card.ShowFront = !card.ShowFront;
        };

        vm.AddSharedCard = function (card) {
            SharedCardCart.put(card);
        };

        vm.CardIsInSharingCart = function (id) {
            return SharedCardCart.exists(id);
        };

    }]);
