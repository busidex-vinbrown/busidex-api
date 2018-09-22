
angular.module('busidexregister').controller('HeaderController', [
    '$scope', '$http', 'Cache', 'CacheKeys',
    function ($scope, $http, Cache, CacheKeys) {
        'use strict';

        function getUser() {
            var user = Cache.get(CacheKeys.User);
            user = angular.fromJson(user);
            return user;
        }

        var vm = this;
        vm.User = getUser();

        $http.defaults.headers.common['X-Authorization-Token'] = vm.user !== null && vm.user !== undefined ? vm.user.Token : '';

    }
]);