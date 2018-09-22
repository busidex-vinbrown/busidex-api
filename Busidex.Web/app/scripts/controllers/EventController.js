//(function() {

angular.module('Busidex').controller('EventController', [
    '$location', '$http', 'Search', 'Activity', 'Analytics', '$routeParams', 'ngDialog', 'EventSource', 'CacheKeys', 'Cache', 'Busidex', 'SharedCardCart',
    function ($location, $http, Search, Activity, Analytics, $routeParams, ngDialog, EventSource, CacheKeys, Cache, Busidexm, SharedCardCart) {
        'use strict';

        var vm = this;
        var user = Cache.get(CacheKeys.User);
        user = angular.fromJson(user);

        $http.defaults.headers.common['X-Authorization-Token'] = user !== null && user !== undefined ? user.Token : '';

        vm.User = user;
        vm.ShowPrivateEventMessage = false;
        vm.model = {
            SearchText: '',
            Results: []
        };
        vm.model.TagSearch = false;

        vm.popupCard = null;
        vm.ShowAddLink = false;
        vm.Waiting = false;
        vm.model.SearchModel = {};
        vm.model.SearchModel.Results = [];
        vm.ShowBusidexIcon = true;
        vm.CardOnly = true;

        vm.osbe = $routeParams.code.indexOf('OSBE') >= 0;
        vm.rina = $routeParams.code.indexOf('RINA') >= 0;
        vm.ten = $routeParams.code.indexOf('TEN') >= 0;

        var addActivity = function (sourceId, cardId) {

            var activity =
                {
                    CardId: cardId,
                    UserId: vm.User !== null ? vm.User.UserId : null,
                    EventSourceId: sourceId
                };
            Activity.post(activity,
                function () {
                    //console.log('event saved');
                },
                function () {
                    //console.log('event NOT saved: ' + status);
                });
        };

        function buildSearchResults(model) {
            for (var i = 0; i < model.SearchModel.Results.length; i++) {

                var thisCard = model.SearchModel.Results[i];

                if (thisCard.OwnerId !== null && thisCard.Searchable === true) {
                    thisCard.HasOwner = true;
                    addActivity(EventSource.SEARCH, thisCard.CardId);
                } else {
                    thisCard.HasOwner = false;
                }
                thisCard.FrontOrientationClass = thisCard.FrontOrientation === 'V' ? 'v_thumbnail' : 'h_thumbnail';
                thisCard.BackOrientationClass = thisCard.BackOrientation === 'V' ? 'v_thumbnail' : 'h_thumbnail';
                thisCard.ShowFront = true;
                thisCard.ShowNotes = false;
            }
            return model;
        }

        function searchByTag(tag) {
            vm.Waiting = true;
            Search.api.searchByTag({ systag: tag },
                function (model) {
                    vm.Waiting = false;
                    vm.ShowPrivateEventMessage = model.SearchModel.Results.length === 0;
                    vm.model = buildSearchResults(model);
                    vm.model.TagSearch = true;
                    vm.model.SearchText = '';
                },
                function (data) {
                    vm.resultData = data;
                    vm.Waiting = false;
                });
        }

        vm.AddSharedCard = function (card) {
            SharedCardCart.put(card);
        };

        vm.AddToMyBusidex = function (card) {

            if (vm.User !== null && vm.User.UserId > 0) {
                Busidex.api.post({ userId: vm.User.UserId, cId: card.CardId },
                    function () {
                        window.alert('This card is now in your Busidex!');
                        card.ExistsInMyBusidex = true;
                    },
                    function () {
                        window.alert('There was a problem adding this card to your Busidex');
                        return false;
                    });
            }
        };

        vm.showTerms = function () {
            ngDialog.open({
                template: '/views/fragments/eventterms.html',
                controller: ['$scope', function ($scope) {
                }]
            });
        }

        vm.showPopup = function (card) {
            vm.selectedCard = card;
            ngDialog.open({
                template: '/views/fragments/cardpopup.html',
                controller: ['$scope', function ($scope) {
                    $scope.card = card;

                    $scope.AddToMyBusidex = vm.AddToMyBusidex;

                }]
            });
        };

        vm.rotateCard = function (card) {
            card.ShowFront = !card.ShowFront;
        };

        vm.setPopupImage = function (card) {

            if (!card.HasOwner) {
                return false;
            }

            vm.popupCard = card;
            vm.ShowAddLink = !card.ExistsInMyBusidex;
            return true;
        };

        searchByTag($routeParams.code);
    }
]);
//})();