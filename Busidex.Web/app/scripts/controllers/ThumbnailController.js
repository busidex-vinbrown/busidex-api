angular.module('Busidex').controller('ThumbnailController', [
    'Notes', 'SharedCardCart', 'ngDialog', 'Busidex', 'Cache', 'CacheKeys',
    function (Notes, SharedCardCart, ngDialog, Busidex, Cache, CacheKeys) {
        'use strict';

        var vm = this;
        var user = Cache.get(CacheKeys.User);
        user = angular.fromJson(user);

        vm.User = user;

        vm.showPopup = function (card) {
            vm.selectedCard = card;
            ngDialog.open({
                template: '/views/fragments/mybusidexcard.html',
                controller: ['$scope', function ($scope) {

                    card.FrontOrientationClass += (card.FrontOrientation === 'V' ? ' v_thumbnail' : ' h_thumbnail');
                    card.BackOrientationClass += (card.BackOrientation === 'V' ? ' v_thumbnail' : ' h_thumbnail');

                    $scope.vm = {};
                    $scope.card = card;
                    $scope.vm.AddToMyBusidex = vm.AddToMyBusidex;
                    $scope.vm.rotateCard = vm.rotateCard;
                    $scope.vm.AddSharedCard = vm.AddSharedCard;
                    $scope.vm.CardIsInSharingCart = vm.CardIsInSharingCart;
                    $scope.vm.EditNotes = vm.EditNotes;
                    $scope.vm.RemoveCard = vm.RemoveCard;
                    $scope.vm.User = vm.User;
                }]
            });
        };

        vm.RemoveCard = function (cardId, userId) {

            if (window.confirm('Remove this card from your collection?')) {
                Busidex.api.remove({ id: cardId, userId: userId },
                    function () {
                        for (var i = 0; i < Busidex.collection.length; i++) {
                            if (Busidex.collection[i].CardId === cardId) {
                                Busidex.collection.splice(i, 1);
                                break;
                            }
                        }
                    },
                    function () {
                        window.alert('There was a problem removing this card.');
                    });
            }
        };

        vm.EditNotes = function (card) {

            Notes.update({ id: card.UserCardId, notes: window.escape(card.Notes) },
                function () {

                },
                function () {

                });
        };

        vm.AddSharedCard = function (card) {

            SharedCardCart.put(card);
        };
        
        vm.rotateCard = function (card) {
            card.ShowFront = !card.ShowFront;
        };

        vm.CardIsInSharingCart = function (id) {
            return SharedCardCart.exists(id);
        };

    }
]);
