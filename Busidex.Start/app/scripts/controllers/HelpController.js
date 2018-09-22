
angular.module('busidexstartApp').controller('HelpController', [
    'Cache', 'CacheKeys',
    function (Cache, CacheKeys) {
        'use strict';

        var user = Cache.get(CacheKeys.User);
        var vm = this;

        vm.LoggedIn = user !== null && user !== undefined;

        if (vm.LoggedIn) {
            Cache.put(CacheKeys.LoginRoute, '#/logout');
            Cache.put(CacheKeys.LoginText, 'LOGOUT');
        } else {
            Cache.put(CacheKeys.LoginRoute, '#/login');
            Cache.put(CacheKeys.LoginText, 'LOGIN');
        }
    }
]);