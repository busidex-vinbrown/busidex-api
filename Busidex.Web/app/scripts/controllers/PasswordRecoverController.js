angular.module('Busidex').controller('PasswordRecoverController', [
    'Cache', 'CacheKeys', 'Analytics', '$location', 'Users',
    function (Cache, CacheKeys, Analytics, $location, Users) {
        'use strict';

        var vm = this;
        var user = Cache.get(CacheKeys.User);
        user = angular.fromJson(user);

        vm.User = user;
        
        vm.Model = {};
        vm.Model.Email = '';
        vm.Model.Error = false;
        vm.Model.BadEmail = false;

        if (!vm.EmailSent) {
            vm.Model.SendEmail = function () {

                vm.EmailSent = true;
                Users.update({ email: vm.Model.Email },
                    function () {
                        $location.path('/account/recoversent');
                    },
                    function (response) {
                        if (response.status === 400 || response.status === 404) {
                            vm.Model.BadEmail = true;
                        } else {
                            vm.Model.Error = true;
                        }

                    });
            };
        }
    }
]);

