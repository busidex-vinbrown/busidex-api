
angular.module('busidexstartApp').controller('StartController', [
    '$http', '$cookies', '$location', '$routeParams', '$localStorage', 'Registration', 'Cache', 'CacheKeys', 'Analytics',
    function ($http, $cookies, $location, $routeParams, $localStorage, Registration, Cache, CacheKeys, Analytics) {
        'use strict';

        Analytics.trackPage($location.path());

        Cache.put(CacheKeys.LoginRoute, '#/login');
        Cache.put(CacheKeys.LoginText, 'LOGIN');

        var vm = this;

        Cache.nuke();

        var token = $routeParams.token;
        if (token !== undefined && token !== null) {

            Registration.update({ token: token },
                function(user) {
                    _gaq.push(['_trackEvent', 'CardUpdate', 'Complete', token]);

                    Cache.put(CacheKeys.User, angular.toJson(user));
                    $http.defaults.headers.common['X-Authorization-Token'] = user.Token;

                    $location.path('/front');
                    return;
                },
                function(error) {
                    if (error === null || error === undefined || error.Message === null) {
                        error = { Message: 'There was a problem completing your registration.' };
                    }
                    vm.RegistrationErrors.push(error.Message);
                    return;
                });
        } else {
            $location.path('/login');
        }
    }
]);