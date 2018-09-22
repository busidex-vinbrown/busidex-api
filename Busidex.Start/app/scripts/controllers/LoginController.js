angular.module('busidexstartApp').controller('LoginController', [
    '$scope', '$http', '$location', 'Login', '$routeParams', 'Cache', 'CacheKeys', 'Registration', 'Analytics',
    function ($scope, $http, $location, Login, $routeParams, Cache, CacheKeys, Registration, Analytics) {
        'use strict';

        Analytics.trackPage($location.path());

        Cache.nuke();

        var vm = this;

        vm.lastForm = {};
        vm.form = {
            UserName: '',
            Password: '',
            Token: $routeParams._t,
            EventTag: $routeParams._e,
            AcceptSharedCards: $routeParams._a || false
        };

        _gaq.push(['_trackEvent', 'CardUpdate', 'Login']);

        if (window.location.href.indexOf('local') < 0) {
            vm.cookieOptions = { domain: 'busidex.com', path: 'busidex.com' };
        } else {
            vm.cookieOptions = {};
        }
        vm.doLogin = function(form) {

            vm.Waiting = true;
            vm.lastForm = angular.copy(form);
            vm.LoginErrors = [];
            Login.post(vm.form,
                function(user) {
                    vm.Waiting = false;
                    vm.resultData = user;

                    $http.defaults.headers.common['Set-Cookie'] = ';domain=.busidx.com';

                    Cache.put(CacheKeys.User, angular.toJson(user));

                    $http.defaults.headers.common['X-Authorization-Token'] = user.Token;

                    $http({
                        method: 'GET',
                        url: ROOT + '/mycard/?id=0',
                        headers: { 'Content-Type': 'application/json' }
                    }).success(function(model) {

                        var card = model.MyCards[0];
                        if (card !== null && card !== undefined) {

                            var frontType = card.FrontFileType || '';
                            var backType = card.BackFileType || '';
                            card.FrontSrc = 'https://az381524.vo.msecnd.net/cards/' + card.FrontFileId + '.' + frontType.replace('.', '');
                            card.BackSrc = 'https://az381524.vo.msecnd.net/cards/' + card.BackFileId + '.' + backType.replace('.', '');
                        } else {
                            card = {};
                            card.FrontOrientation = 'H';
                            card.BackOrientation = 'H';
                            card.FrontSrc = null; 
                            card.BackSrc = null;
                            vm.card = card;
                        }
                        Cache.put(CacheKeys.Card, angular.toJson(card));

                       // console.log('m: ' + $routeParams.m);
                        if ($routeParams.m !== undefined) {
                            $location.path('/front').search('m', $routeParams.m);
                        } else {
                            $location.path('/front');
                        }
                        
                        //$location.url($location.path()); // remove any query string parameters
                    });

                },
                function(error) {

                    if (error.status === 404) {
                        vm.LoginErrors.push('Username or password is incorrect');
                    } else {
                        vm.LoginErrors.push(error.data.Message);
                    }
                    vm.Waiting = false;
                });
        };

        if ($routeParams.token !== undefined) {
            Registration.activate({ token: $routeParams.token },
                    function (buser) {
                        $http.defaults.headers.common['Set-Cookie'] = ';domain=.busidx.com';

                        Cache.put(CacheKeys.User, angular.toJson(buser));

                        $http.defaults.headers.common['X-Authorization-Token'] = buser.Token;

                        $location.path('/front');
                    },
                    function () {
                        $location.path('/login');
                    });
        }
    }
]);