/*EDIT GROUP*/
angular.module('Busidex').controller('EditGroupController', [
    '$http', '$cookieStore', '$location', '$route', 'Busidex', 'Groups', 'GroupDetail', 'Analytics', 'Cache', 'CacheKeys',
    function ($http, $cookieStore, $location, $route, Busidex, Groups, GroupDetail, Analytics, Cache, CacheKeys) {
        'use strict';

        Analytics.trackPage($location.path());

        var vm = this;
        var user = Cache.get(CacheKeys.User);
        user = angular.fromJson(user);
        
        vm.User = user;

        vm.EditModes = {
            Add: 1,
            Update: 2
        };        

        if (vm.User === null) {
            $location.path('/login');
            return;
        }

        vm.Waiting = false;

        vm.ShowSelectedOnly = false;

        vm.group = {
            GroupId: 0,
            Description: '',
            CardIds: []
        };

        $http.defaults.headers.common['X-Authorization-Token'] = user.Token;

        vm.EditMode = $route.current.params.id !== undefined ? vm.EditModes.Update : vm.EditModes.Add;

        if (vm.EditMode === vm.EditModes.Update) {
            GroupDetail.get({ id: $route.current.params.id },
                function(response) {

                    vm.group = response.Model.Busigroup;
                    vm.group.CardIds = [];
                    vm.Cards = response.Model.BusigroupCards;

                    Busidex.api.get({ all: true },
                        function(model) {

                            vm.Busidex = model.MyBusidex.Busidex;

                            for (var b = 0; b < vm.Busidex.length; b++) {
                                var busidexCard = vm.Busidex[b];
                                busidexCard.OrientationClass = busidexCard.Card.FrontOrientation === 'V' ? 'v_preview' : 'h_preview';
                                for (var c = 0; c < vm.Cards.length; c++) {
                                    var groupCard = vm.Cards[c];
                                    if (busidexCard.CardId === groupCard.CardId) {
                                        busidexCard.Selected = true;
                                        vm.group.CardIds.push(groupCard.CardId);
                                        break;
                                    }
                                }
                            }
                        },
                        function() {

                        });

                },
                function() {
                   
                });
        } else {
            Busidex.api.get({ all: true },
                function (model) {

                    vm.Busidex = model.MyBusidex.Busidex;

                    for (var b = 0; b < vm.Busidex.length; b++) {
                        var busidexCard = vm.Busidex[b];
                        busidexCard.OrientationClass = busidexCard.Card.FrontOrientation === 'V' ? 'v_preview' : 'h_preview';
                    }
                },
                function () {

                });
        }

        vm.ToggleSelected = function(card) {

            card.Selected = !card.Selected;
            if (card.Selected === true) {
                vm.group.CardIds.push(card.Card.CardId);
            } else {
                for (var i = 0; i < vm.group.CardIds.length; i++) {
                    if (vm.group.CardIds[i] === card.Card.CardId) {
                        vm.group.CardIds.splice(i, 1);
                        break;
                    }
                }
            }
        };

        vm.save = function() {

            vm.SaveError = false;

            var data = {
                userId: vm.User.UserId,
                groupTypeId: vm.EditMode === vm.EditModes.Add ? 1 : vm.group.GroupTypeId,
                id: vm.EditMode === vm.EditModes.Add ? 0 : vm.group.GroupId,
                cardIds: vm.group.CardIds.join(),
                description: vm.group.Description
            };

            if (vm.EditMode === vm.EditModes.Add) {

                Groups.post(data,
                    function () {
                        $location.path('/groups/mine');
                    },
                    function () {
                        vm.SaveError = true;
                    });
            } else {
                Groups.update(data,
                     function () {
                         $location.path('/groups/mine');
                     },
                     function () {
                         vm.SaveError = true;
                     });
            }
            
        };
    }
]);