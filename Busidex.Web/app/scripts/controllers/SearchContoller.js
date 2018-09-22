
angular.module('Busidex').controller('SearchController', [
    '$location', '$http', 'Search', 'Activity', 'Analytics', '$routeParams', 'ngDialog', 'EventSource', 'CacheKeys', 'Cache', 'Busidex', 'SharedCardCart',
    function ($location, $http, Search, Activity, Analytics, $routeParams, ngDialog, EventSource, CacheKeys, Cache, Busidex, SharedCardCart) {
        'use strict';

        var vm = this;
        var user = Cache.get(CacheKeys.User);
        user = angular.fromJson(user);

        $http.defaults.headers.common['X-Authorization-Token'] = user !== null && user !== undefined ? user.Token : '';

        vm.User = user;
        vm.ShowPrivateEventMessage = false;

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

        function _reset() {
            vm.model = {};
            vm.model.SearchText = '';
            vm.model.SearchResultsMessage = '';
            vm.model.SearchModel = {};
            vm.model.SearchModel.Results = [];
            vm.ShowPrivateEventMessage = false;
        }

        function _showNoOwnerMessage() {
            window.alert('Sorry, this card has not been activated.');
        }

        function _showResultsMessage() {
            vm.model.SearchResultsMessage = '';
            if (vm.model.TagSearch === true) {
                return vm.model.SearchResultsMessage;
            }

            if (vm.model.SearchModel.Results.length === 0) {
                vm.model.SearchResultsMessage = 'Your search for &quot;' + vm.model.SearchText.split(' ').join('+') + '&quot; didn&quot;t return any results.';
            } else {
                vm.model.SearchResultsMessage = 'We searched for anything containing &quot;' + vm.model.SearchText.split(' ').join('\' or \'') + '&quot; and found ' + vm.model.SearchModel.Results.length + ' cards!';
            }

            return vm.model.SearchResultsMessage;
        }

        function searchByTag(tag) {
            vm.Waiting = true;
            _reset();
            Search.api.searchByTag({ systag: tag },
                function(model) {
                    vm.Waiting = false;
                    vm.ShowPrivateEventMessage = model.SearchModel.Results.length === 0;
                    vm.model = buildSearchResults(model);
                    vm.model.TagSearch = true;
                    vm.model.SearchText = '';
                },
                function(data) {
                    vm.resultData = data;
                    vm.Waiting = false;
                });
        }

        function searchByOrganization(orgId) {

            vm.Waiting = true;
            _reset();
            Search.api.searchByOrganization({ orgId: orgId },
            function (model) {

                vm.Waiting = false;
                vm.model = buildSearchResults(model);
                vm.model.TagSearch = true;
                vm.model.SearchText = '';
            },
            function (data) {
                vm.resultData = data;
                window.alert('Getting search results failed.');
                vm.Waiting = false;
            });
        }

        function searchByGroupName(groupName) {

            vm.Waiting = true;
            _reset();
            Search.api.searchByGroupName({ groupName: groupName },
            function (model) {

                vm.Waiting = false;
                vm.model = buildSearchResults(model);
                vm.model.TagSearch = true;
                vm.model.SearchText = '';
            },
            function (data) {
                vm.resultData = data;
                window.alert('Getting search results failed.');
                vm.Waiting = false;
            });
        }

        
        vm.IsLoggedIn = vm.User !== null;
        
        // total hack
        if (!vm.IsLoggedIn && $routeParams._o !== '11') {
            $location.path('login');
            return;
        }

        vm.SearchByTag = searchByTag;
        vm.SearchByOrganization = searchByOrganization;
        vm.SearchByGroupName = searchByGroupName;

        vm.Reset = _reset;
        vm.ShowResultsMessage = _showResultsMessage;
        vm.ShowNoOwnerMessage = _showNoOwnerMessage;
        vm.CardOnly = false;
        vm.Popup = false;

        vm.AddSharedCard = function (card) {

            SharedCardCart.put(card);
        };

        vm.AddToMyBusidex = function(card) {

            if (vm.User !== null && vm.User.UserId > 0) {
                Busidex.api.post({ userId: vm.User.UserId, cId: card.CardId },
                    function() {
                        window.alert('This card is now in your Busidex!');
                        card.ExistsInMyBusidex = true;
                    },
                    function() {
                        window.alert('There was a problem adding this card to your Busidex');
                        return false;
                    });
            }
        };

        vm.showPopup = function (card) {
            vm.selectedCard = card;
            ngDialog.open({
                closeByDocument: false,
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

         if ($routeParams._t !== null && $routeParams._t !== undefined) {
            searchByTag($routeParams._t);
            Analytics.trackEvent('Search', 'Event', $routeParams._t);
        } else if ($routeParams._o !== null && $routeParams._o !== undefined) {
            searchByOrganization($routeParams._o);
            Analytics.trackEvent('Search', 'Event', $routeParams._o);
        } else if ($routeParams._g !== null && $routeParams._g !== undefined) {
            searchByGroupName($routeParams._g);
            Analytics.trackEvent('Search', 'Event', $routeParams._g);
        }

        vm.doSearch = function() {

            vm.model.NoResults = false;

            vm.model.SearchModel.Results = [];
            if (vm.model.SearchModel.Distance === 0) {
                vm.model.SearchModel.Distance = 25;
            }
            vm.Waiting = true;
            vm.ShowPrivateEventMessage = false;
            vm.model.SearchModel.UserId = vm.User !== undefined && vm.User !== null ? vm.User.UserId : null;
            vm.model.UserId = null; //userId;
            vm.model.CardType = 1; // search for Cards only

            Analytics.trackEvent('Search', 'General', vm.model.SearchText);
            
            Search.api.post(vm.model,
                function(model) {

                    vm.Waiting = false;
                    vm.model = buildSearchResults(model);
                    vm.model.TagSearch = false;
                    vm.model.SearchText = model.SearchModel.SearchText;
                    vm.model.NoResults = vm.model.SearchModel.Results.length === 0;
                },
                function(data) {
                    vm.resultData = data;
                    window.alert('Getting search results failed.');
                    vm.Waiting = false;
                });
        };

        vm.setPopupImage = function(card) {

            if (!card.HasOwner) {
                return false;
            }

            vm.popupCard = card;
            vm.ShowAddLink = !card.ExistsInMyBusidex;
            return true;
        };
        
        _reset();
    }
]);

