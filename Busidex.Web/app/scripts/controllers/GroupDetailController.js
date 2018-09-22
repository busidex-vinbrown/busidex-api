angular.module('Busidex').controller('GroupsController', [
    '$location', '$route', 'GroupDetail', 'GroupNotes', 'Cache', 'CacheKeys', 'Analytics',
    function ($location, $route, GroupDetail, GroupNotes, Cache, CacheKeys, Analytics) {
        'use strict';

        Analytics.trackPage($location.path());

        var vm = this;
        var user = Cache.get(CacheKeys.User);
        user = angular.fromJson(user);

        vm.User = user;

        if (vm.User === null) {
            $location.path('/login');
            return;
        }

        vm.Waiting = false;
        vm.GroupId = $route.current.params.id;

        GroupDetail.get({ id: $route.current.params.id },
            function(response) {

                vm.Group = response.Model.Busigroup;
                vm.Cards = [];
                for (var i = 0; i < response.Model.BusigroupCards.length; i++) {
                    var card = response.Model.BusigroupCards[i];
                    card.Ready = true;

                    card.OrientationClass = card.FrontOrientation === 'V' ? 'v_preview' : 'h_preview';

                    vm.Cards.push(card);
                }
            },
            function() {

            });

        vm.delete = function() {

            if (window.confirm('Are you sure you want to delete this group?')) {

                GroupDetail.remove({ id: $route.current.params.id },
                    function() {
                        $location.path('/groups/mine');
                    },
                    function() {

                    });
            }
        };

        $(document).on('change', 'textarea.groupNotes', function() {

            GroupNotes.update({ id: $(this).attr('ucId'), notes: window.escape($(this).val()) },
                function() {

                },
                function() {

                });
        });
        
        vm.showCards = function() {
            for (var i = 0; i < vm.Cards.length; i++) {
                vm.Cards[i].Ready = true;
            }
        };
    }
]);