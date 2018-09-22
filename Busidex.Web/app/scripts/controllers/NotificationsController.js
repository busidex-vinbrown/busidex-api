
angular.module('Busidex').controller('NotificationsController', [
    '$http', 'SharedCardCart', 'SharedCard', 'Cache', 'CacheKeys',
    function ($http, SharedCardCart, SharedCard, Cache, CacheKeys) {
        'use strict';

        var vm = this;
        vm.email = '';
        vm.message = '';

        var user = Cache.get(CacheKeys.User);
        user = angular.fromJson(user);
        vm.User = user;

        $http.defaults.headers.common['X-Authorization-Token'] = user !== null && user !== undefined ? user.Token : '';
        
        var notifications = angular.fromJson(Cache.get(CacheKeys.Notifications) || []);
        vm.notifications = [];

        for (var i = 0; i < notifications.length; i++) {
            var c = notifications[i].Card;
            c.message = notifications[i].Recommendation;
            c.SentFrom = notifications[i].SendFromDisplayName;
            c.FrontOrientationClass = c.FrontOrientation === 'V' ? 'v_thumbnail' : 'h_thumbnail';
            c.BackOrientationClass = c.BackOrientation === 'V' ? 'v_thumbnail' : 'h_thumbnail';
            vm.notifications.push(c);
        }

        vm.message = '';

        function saveSharedCards(model) {
            SharedCard.update({ id: 0 }, JSON.stringify(model),
                function () {

                },
                function () {

                });
        }

        vm.accept = function(idx) {
            var accepted = [];
            var declined = [];

            accepted.push(vm.notifications[idx].CardId);

            var model = { AcceptedCardIdList: accepted, DeclinedCardIdList: declined, UserId: 0, SharedWith: 0 };
            saveSharedCards(model);
            updateNotifications(idx);
        };

        function updateNotifications(idx) {
            vm.notifications.splice(idx, 1);
            Cache.put(CacheKeys.Notifications, angular.toJson(vm.notifications));
        }

        vm.remove = function (idx) {

            var accepted = [];
            var declined = [];

            declined.push(vm.notifications[idx].CardId);

            var model = { AcceptedCardIdList: accepted, DeclinedCardIdList: declined, UserId: 0, SharedWith: 0 };
            saveSharedCards(model);
            updateNotifications(idx);
        };
    }
]);