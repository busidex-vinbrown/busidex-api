/*PASSWORD RESET*/
angular.module('Busidex').controller('PasswordResetController', [
    '$http', 'Cache', 'CacheKeys', '$location', '$routeParams', 'Login', 'SharedCard',
    function ($http, Cache, CacheKeys, $location, $routeParams, Login, SharedCard) {

        'use strict';

        var vm = this;

        vm.Model = {};
        vm.Model.Email = '';
        vm.Model.Error = false;
        vm.Model.Password = '';
        vm.Model.ConfirmPassword = '';

        vm.Model.LoginData = {
            UserName: '',
            Password: '',
            TempData: $routeParams.data,
            RememberMe: false
        };

        vm.passwordsMatch = function () {

            return vm.Model.Password === vm.Model.ConfirmPassword &&
                vm.Model.Password !== null &&
                vm.Model.Password.length > 0 &&
                vm.Model.ConfirmPassword !== null &&
                vm.Model.ConfirmPassword.length > 0;
        };

        if (!vm.LoggedIn) {
            vm.Model.Login = function() {
                vm.LoggedIn = true;
                if (vm.Model.Password === vm.Model.ConfirmPassword) {
                    vm.Model.LoginData.Password = vm.Model.Password;
                    vm.Model.LoginData.UserName = vm.Model.UserName;

                    Login.update(vm.Model.LoginData,
                        function(user) {

                            vm.Waiting = false;
                            vm.resultData = user;
                            vm.User = user;

                            $http.defaults.headers.common['Set-Cookie'] = ';domain=.busidx.com';

                            Cache.put(CacheKeys.User, angular.toJson(user));

                            $http.defaults.headers.common['X-Authorization-Token'] = user.Token;

                            $http({
                                method: 'GET',
                                url: ROOT + '/mycard/?id=0',
                                headers: { 'Content-Type': 'application/json' }
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
                        },
                        function() {
                            vm.Model.Error = true;
                            vm.LoggedIn = false;
                        });
                } else {
                    vm.Model.Error = true;
                    vm.LoggedIn = false;
                }
            };
        }
    }
]);