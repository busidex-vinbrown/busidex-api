
angular.module('busidexstartApp').controller('NextStepsController', [
    'Registration', 'Cache', 'CacheKeys', '$http', 'Analytics', '$location',
    function (Registration, Cache, CacheKeys, $http, Analytics, $location) {
        'use strict';

        Analytics.trackPage($location.path());

        var user = Cache.get(CacheKeys.User);
        user = angular.fromJson(user);
        $http.defaults.headers.common['X-Authorization-Token'] = user.Token;

        Registration.complete(
                function () {
                    Analytics.trackEvent('CardUpdate', 'CardUpdateComplete', user.UserId.toString());
                },
                function () {
                    window.alert('error');
                });
    }
]);