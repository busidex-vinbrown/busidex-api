angular.module('Busidex').controller('LogoutController', [
    'Cache',
    function (Cache) {
        'use strict';
        
        Cache.nuke();
        
    }
]);