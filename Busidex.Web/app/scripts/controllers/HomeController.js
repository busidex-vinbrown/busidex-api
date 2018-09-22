/*HOME*/
angular.module('Busidex').controller('HomeController', [
    'Cache', 'CacheKeys',
    function(Cache, CacheKeys) {
        'use strict';

        var vm = this;
        var user = Cache.get(CacheKeys.User);
        user = angular.fromJson(user);

        vm.User = user;
        vm.ShowLinks = vm.User === null || vm.User === undefined;
        
        var messages = [];
        messages.push('MAKE A CONNECTION');
        messages.push('MAKE A NEW CONTACT');
        messages.push('MAKE A REFERRAL');
        messages.push('NEVER LOSE A REFERRAL');
        messages.push('GIVE A REFERRAL');
        messages.push('NEVER LOSE A CONTACT');
        messages.push('MAKE A CONNECTION');

        vm.HomePageMessage = messages[new Date().getDay()];
    }
]);
