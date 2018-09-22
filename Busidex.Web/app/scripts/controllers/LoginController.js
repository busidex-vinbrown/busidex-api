(function () {
    'use strict';

    angular.module('Busidex').controller('LoginController', [
        '$scope', '$http', '$location', 'Login', '$routeParams', 'Cache', 'CacheKeys', 'SharedCard',
        function ($scope, $http, $location, Login, $routeParams, Cache, CacheKeys, SharedCard) {

            var vm = this;

            vm.lastForm = {};
            vm.form = {
                UserName: '',
                Password: '',
                Token: $routeParams._t,
                EventTag: $routeParams._e,
                AcceptSharedCards: $routeParams._a || false
            };

            if (window.location.href.indexOf('local') < 0) {
                vm.cookieOptions = { domain: 'busidex.com', path: 'busidex.com' };
            } else {
                vm.cookieOptions = {};
            }
            vm.doLogin = function (form) {

                Cache.nuke();

                vm.Waiting = true;
                vm.lastForm = angular.copy(form);
                vm.LoginErrors = [];
                Login.post(vm.form,
                    function (user) {
                        vm.Waiting = false;
                        vm.resultData = user;
                        var user = angular.fromJson(user);

                        $http.defaults.headers.common['Set-Cookie'] = ';domain=.busidx.com';

                        Cache.put(CacheKeys.User, angular.toJson(user));

                        $http.defaults.headers.common['X-Authorization-Token'] = user.Token;

                        if (user.StartPage === 'Organization') {
                            /*
                                ORGANIZATION ADMIN
                                If this user is an organization admin, redirect them to their organization page
                            */
                            window.location = 'https://org.busidex.com/start/' + user.Organizations[0].Item2;
                        } else {
                            /*
                                REGULAR USER
                                Load the app normally
                            */
                            $http({
                                method: 'GET',
                                cache: false,
                                url: ROOT + '/mycard/mine?id=0',
                                headers: { 'Content-Type': 'application/json', 'X-Authorization-Token': user.Token }
                            }).success(function (model) {

                                var card = model.MyCards[0];
                                if (card !== null && card !== undefined) {
                                    card.FrontSrc = 'https://az381524.vo.msecnd.net/cards/' + card.FrontFileId + '.' + card.FrontFileType.replace('.', '');
                                    card.BackSrc = 'https://az381524.vo.msecnd.net/cards/' + card.BackFileId + '.' + card.BackFileType.replace('.', '');
                                } else {
                                    card = {};
                                    card.FrontOrientation = 'H';
                                    card.BackOrientation = 'H';
                                    card.FrontSrc = null;
                                    card.BackSrc = null;
                                    vm.card = card;
                                }
                                Cache.put(CacheKeys.Card, angular.toJson(card));

                                var notifications = [];
                                SharedCard.get(
                                    function (results) {
                                        if (results !== null && results !== undefined && results.SharedCards !== null && results.SharedCards !== undefined) {
                                            for (var i = 0; i < results.SharedCards.length; i++) {
                                                notifications.push(results.SharedCards[i]);
                                            }
                                            Cache.put(CacheKeys.Notifications, angular.toJson(notifications));
                                        }
                                    },
                                    function () {

                                    });

                                $location.path('/main');
                                $location.url($location.path()); // remove any query string parameters
                            });
                        }
                    },
                    function (error) {

                        if (error.status === 404) {
                            vm.LoginErrors.push('Username or password is incorrect');
                        } else {
                            vm.LoginErrors.push(error.data.Message);
                        }
                        vm.Waiting = false;
                    });
            };
        }
    ]);
})();