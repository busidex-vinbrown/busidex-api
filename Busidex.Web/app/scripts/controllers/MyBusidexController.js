(function () {
    'use strict';
angular.module('Busidex').controller('MyBusidexController', [
    '$scope', '$http', '$location', 'Busidex', 'SharedCard', 'Activity', 'Analytics', 'Cache', 'CacheKeys', '$timeout',
    function ($scope, $http, $location, Busidex, SharedCard, Activity, Analytics, Cache, CacheKeys, $timeout) {
        
        var vm = this;
        var user = Cache.get(CacheKeys.User);
        user = angular.fromJson(user);

        vm.User = user;

        if (vm.User === null) {
            $location.path('/login');
            return;
        }
        function addLetter() {
            vm.dictionary.push(vm.alphabet[vm.idx++]);
            if (vm.idx < vm.alphabet.length) {
                $timeout(addLetter, 100);
            }
        }
        vm.alphabet = 'abcdefghijklmnopqrstuvwxyz';
        vm.idx = 0;
        vm.dictionary = [];
        vm.dictionary.push('ALL');
        vm.ShowRemove = true;

        $timeout(addLetter, 1000);
        
        $http.defaults.headers.common['X-Authorization-Token'] = user.Token;    

        vm.model = {};
        vm.model.cards = [];
        vm.model.cardCache = [];
        vm.showDetails = false;
        vm.dictionaryVal = '';

        vm.setFilterVal = function (letter) {
            vm.dictionaryVal = letter;
        };

        vm.customFilter = function(card) {
            if (vm.dictionaryVal.length === 0 || vm.dictionaryVal === 'ALL') {
                return true;
            }
            var exp = new RegExp(vm.dictionaryVal, 'g');
            return (card.Name !== null && card.Name.toLowerCase().substring(0, 1).match(exp)) ||
                (card.CompanyName !== null && card.CompanyName.toLowerCase().substring(0, 1).match(exp));
        };          

        vm.Waiting = true;
        vm.Error = false;
        vm.ShowBusidexIcon = false;
        vm.q = '';
        vm.loading = true;

        vm.AddActivity = function (sourceId, cardId) {

            var activity =
            {
                CardId: cardId,
                UserId: vm.User.UserId,
                EventSourceId: sourceId
            };
            Activity.post(activity,
                function() {
                    console.log('event saved');
                },
                function(status) {
                    console.log('event NOT saved: ' + status);
                });
        };

        var Address = function (id, cardId, addr1, addr2, city, state, zip) {
            
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
            self.Display = function () {
                return self.Address1 + ' ' +
                    self.Address2 + ' ' +
                    self.City + ' ' +
                    (self.State !== null ? self.State.Code : '') +
                    (self.ZipCode !== null && self.ZipCode.length > 0 ? ', ' : ' ') +
                    self.ZipCode;
            };
            self.Selected = false;
        };

        Busidex.api.get({ all: true },
            function(data) {

                if (Busidex.collection.length !== data.MyBusidex.Busidex.length || Busidex.collection.length === 0) {
                    for (var i = 0; i < data.MyBusidex.Busidex.length; i++) {

                        var thisCard = data.MyBusidex.Busidex[i].Card;
                        thisCard.FrontOrientationClass = thisCard.FrontOrientation === 'V' ? 'thumbnail-v_thumbnail' : 'thumbnail-h_thumbnail';
                        thisCard.BackOrientationClass = thisCard.BackOrientation === 'V' ? 'thumbnail-v_thumbnail' : 'thumbnail-h_thumbnail';

                        if (thisCard.Addresses !== null && thisCard.Addresses.length > 0) {
                            var a = thisCard.Addresses[0];
                            thisCard.Address = new Address(a.CardAddressId, a.CardId, a.Address1 || '', a.Address2 || '', a.City || '', a.State, a.ZipCode || '');
                        } else {
                            thisCard.Address = null;
                        }

                        thisCard.MapInfo = 'http://maps.google.com/maps?daddr={' + (thisCard.Address !== null ? thisCard.Address.Display() : '') + '}';
                        thisCard.Share = false;
                        thisCard.Notes = data.MyBusidex.Busidex[i].Notes;
                        thisCard.ShowFront = true;
                        thisCard.UserCardId = data.MyBusidex.Busidex[i].UserCardId;
                        thisCard.ShowNotes = true;
                        thisCard.ShowRemove = true;

                        Busidex.collection.push(thisCard);
                    }
                } else {
                    window.console.log(angular.toJson(data));
                }
                vm.model.cards = Busidex.collection;

                vm.Waiting = false;
                vm.loading = false;

            }, function(data) {
                vm.resultData = data;
                vm.Waiting = false;
                vm.loading = false;
                vm.Error = true;
                window.console.log('Error: ' + angular.toJson(data));
            });

        $scope.$watchCollection(function () {
            return Busidex.collection;
        }, function (newCollection) {
            vm.model.cards = newCollection;
        }, true);
    }]);
})();