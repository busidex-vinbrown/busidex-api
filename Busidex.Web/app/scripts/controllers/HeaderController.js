
angular.module('Busidex').controller('HeaderController', [
    '$scope', '$http', 'Cache', 'CacheKeys', '$location', 'SharedCardCart', 'SharedCard',
    function ($scope, $http, Cache, CacheKeys, $location, SharedCardCart, SharedCard) {
        'use strict';

        function getUser() {
            var user = Cache.get(CacheKeys.User);
            user = angular.fromJson(user);
            return user;
        }

        var vm = this;
        vm.User = getUser();

        vm.HasCard = false;
        vm.LoginRoute = Cache.get(CacheKeys.LoginRoute);
        vm.LoginText = Cache.get(CacheKeys.LoginText);
        vm.count = SharedCardCart.Count();
        vm.showCount = true;
        vm.LoggedIn = Cache.get(CacheKeys.User) !== null;
        vm.notifications = angular.fromJson(Cache.get(CacheKeys.Notifications) || []);
        vm.ItemIndex = -1;
        vm.organizations = [];
        
        $http.defaults.headers.common['X-Authorization-Token'] = vm.user !== null && vm.user !== undefined ? vm.user.Token : '';

        if ($location.path() !== '/card/notifications') {
            SharedCard.get(
                function(results) {
                    if (results !== null && results !== undefined && results.SharedCards !== null && results.SharedCards !== undefined) {
                        vm.notifications = [];
                        for (var i = 0; i < results.SharedCards.length; i++) {
                            vm.notifications.push(results.SharedCards[i]);
                        }
                        Cache.put(CacheKeys.Notifications, angular.toJson(vm.notifications));
                    }
                },
                function() {

                });
        }

        //#region watches  
        $scope.$watchCollection(function () {
            return $location.path();
        }, function (newPath) {
            var whitelist = [];
            whitelist.push('/bca');
            whitelist.push('/account');
            whitelist.push('/card/search');
            whitelist.push('/groups/mine');
            whitelist.push('/faq');
            whitelist.push('/login');
            whitelist.push('/card/share');
            whitelist.push('/bca');
            whitelist.push('/bnistars');
            whitelist.push('/minutemanpress');
            whitelist.push('/osbe');
            whitelist.push('/realtortroopsupport');
            whitelist.push('/riarexpo');
            whitelist.push('/card/events');
            whitelist.push('/event');

            vm.ShowHeader = vm.LoggedIn;
            if (!vm.ShowHeader) {
                for (var i = 0; i < whitelist.length; i++) {
                    if (newPath.indexOf(whitelist[i]) >= 0) {
                        vm.ShowHeader = true;
                        break;
                    }
                }
            }

            var navItems = {
                MEMBERSHIPS: 6,
                MY_ACCOUNT: 5,
                FAQ: 4,
                GROUPS: 3,
                MY_CARD: 2,
                MY_BUSIDEX: 1,
                SEARCH: 0,
                UNKNOWN: -1
            };

            if ($location.path().indexOf('/account/mine') >= 0) {
                vm.ItemIndex = navItems.MY_ACCOUNT;
            } else if ($location.path().indexOf('/faq') >= 0) {
                vm.ItemIndex = navItems.FAQ;
            } else if ($location.path().indexOf('groups/mine') >= 0) {
                vm.ItemIndex = navItems.GROUPS;
            } else if ($location.path().indexOf('groups/create') >= 0) {
                vm.ItemIndex = navItems.GROUPS;
            } else if ($location.path().indexOf('groups/edit') >= 0) {
                vm.ItemIndex = navItems.GROUPS;
            } else if ($location.path().indexOf('card/mine') >= 0) {
                vm.ItemIndex = navItems.MY_CARD;
            } else if ($location.path().indexOf('/busidex/mine') >= 0) {
                vm.ItemIndex = navItems.MY_BUSIDEX;
            } else if ($location.path().indexOf('/card/search') >= 0) {
                vm.ItemIndex = navItems.SEARCH;
            } else if ($location.path().indexOf('/card/memberships') >= 0) {
                vm.ItemIndex = navItems.MEMBERSHIPS
            } else {
                vm.ItemIndex = navItems.UNKNOWN;
            }

        });

        $scope.$watchCollection(function () {
            return Cache.get(CacheKeys.User);
        }, function (newUser) {
            vm.LoggedIn = newUser !== null && newUser !== undefined;

            if (vm.LoggedIn) {
                Cache.put(CacheKeys.LoginRoute, '#/logout');
                Cache.put(CacheKeys.LoginText, 'LOGOUT');
            } else {
                Cache.put(CacheKeys.LoginRoute, '#/login');
                Cache.put(CacheKeys.LoginText, 'LOGIN');
            }
        }, true);

        vm.GoToNotifications = function() {
            $location.path('/card/notifications');
        };

        vm.GoToSharedCards = function() {
            $location.path('/card/share');
        };

        $scope.$watchCollection(function () {
            return SharedCardCart.Count();
        }, function (newValue) {
            vm.showCount = false;
            vm.count = newValue;
            vm.showCount = true;
        });

        $scope.$watchCollection(function () {
            return vm.notifications;
        }, function (newValue) {
            vm.showCount = false;
            vm.count = newValue;
            vm.showCount = true;
        });

        $scope.$watchCollection(function () {
            return Cache.get(CacheKeys.Notifications);
        }, function (newNotifications) {
            newNotifications = angular.fromJson(newNotifications);
            vm.notifications = newNotifications;
            vm.User = getUser();
            vm.LoggedIn = vm.User !== null && vm.User !== undefined;
        });

        $scope.$watchCollection(function () {
            return Cache.get(CacheKeys.User);
        }, function (newUser) {
            var newUserObject = angular.fromJson(newUser);
            vm.User = newUserObject;
        });

        $scope.$watchCollection(function () {
            return Cache.get(CacheKeys.Card);
        }, function (newCard) {
            newCard = angular.fromJson(newCard);
            vm.HasCard = newCard !== null && newCard !== undefined && newCard.FrontFileId !== null && newCard.FrontFileId !== undefined && newCard.FrontFileId.length > 0;
        });

        $scope.$watchCollection(function () {
            return Cache.get(CacheKeys.LoginRoute);
        }, function (newRoute) {
            vm.LoginRoute = newRoute;
        });

        $scope.$watchCollection(function () {
            return Cache.get(CacheKeys.LoginText);
        }, function (newText) {
            vm.LoginText = newText;
        });
        //#endregion 
    }
]);