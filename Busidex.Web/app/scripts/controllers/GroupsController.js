angular.module('Busidex').controller('GroupsController', [
   '$http', '$location', 'Groups', 'Analytics', 'GroupDetail', 'Cache', 'CacheKeys',
    function ($http, $location, Groups, Analytics, GroupDetail, Cache, CacheKeys) {
        'use strict';

        Analytics.trackPage($location.path());
        
        var vm = this;
        var user = Cache.get(CacheKeys.User);
        user = angular.fromJson(user);

        vm.User = user;

        $http.defaults.headers.common['X-Authorization-Token'] = user.Token;

        if (vm.User === null) {
            $location.path('/login');
            return;
        }

        var setCurrentTab = function (currentTab) {

            for (var tab in vm.Tabs) {
                if (vm.Tabs.hasOwnProperty(tab)) {
                    vm.Tabs[tab] = false;
                }
            }
            vm.Tabs[currentTab] = true; 
        };

        var getGroup = function(groupId) {

            GroupDetail.get({ id: groupId },
                function(response) {
                    vm.Group = {};
                    vm.Cards = [];
                    vm.Group = response.Model.Busigroup;

                    setCurrentTab(vm.Group.Description);
                    for (var i = 0; i < response.Model.BusigroupCards.length; i++) {
                        var card = response.Model.BusigroupCards[i];

                        card.Card.FrontOrientationClass = card.FrontOrientation === 'V' ? 'v_preview' : 'h_preview';
                        card.Card.ShowFront = true;
                        vm.Cards.push(card);
                    }
                },
                function() {

                });
        };
       
        function resetGroup(groupId) {
            vm.Cards = [];
            getGroup(groupId);
        }

        vm.Waiting = false;
        vm.GetGroup = resetGroup;
        vm.Tabs = {};
        vm.GroupId = 0;
        vm.GroupHelp = 'BusiGroups and Memberships are ways you can organize your cards. Use a BusiGroup if the group is just for you to see. Choose from any card in your Busidex collection to add to the group. Memberships are groups that all members of the group can see. The person that creates the group has control over who is included.';

        function selectFirstGroup() {
            setCurrentTab(vm.Tabs[vm.groups[0].Description]);
            getGroup(vm.groups[0].GroupId);
        }

        Groups.get({ id: vm.User.UserId },
            function(response) {
                vm.groups = response.Model;
                for (var i = 0; i < vm.groups.length; i++) {
                    vm.Tabs[vm.groups[i].Name] = false;
                }

                if (vm.groups.length > 0) {
                    selectFirstGroup();
                }
            },
            function() {

            });

        vm.DeleteGroup = function(groupId) {

            if (window.confirm('Are you sure you want to delete this group?')) {

                Groups.remove({ id: groupId },
                    function() {

                        for (var i = 0; i < vm.groups.length; i++) {
                            if (vm.groups[i].GroupId === groupId) {
                                vm.groups.splice(i, 1);
                                break;
                            }
                        }
                        if (vm.groups.length > 0) {
                            selectFirstGroup();
                        } 
                    },
                    function() {

                    });
            }
        };        
    }
]);