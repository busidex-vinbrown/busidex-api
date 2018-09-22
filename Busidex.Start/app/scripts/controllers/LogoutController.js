angular.module('busidexstartApp').controller('LogoutController', [
    '$window', '$location', 'Cache',
    function ($window, $location, Cache) {
        'use strict';

        Cache.nuke();

        $window.location = 'https://www.busidex.com/#/logout';
       
    }
]);