/*DELETE ACCOUNT*/
angular.module('Busidex').controller('DeleteAccountController', [
    '$http', 'Account', '$location', 'Analytics', 'Cache', 'CacheKeys',
    function($http, Account, $location, Analytics, Cache, CacheKeys) {
        'use strict';

        var vm = this;
        var user = Cache.get(CacheKeys.User);
        user = angular.fromJson(user);

        vm.User = user;

        if (vm.User === null) {
            $location.path('/login');
            return;
        }

        $http.defaults.headers.common['X-Authorization-Token'] = user.Token;

        Analytics.trackPage($location.path());

        vm.Model = {};
        vm.Model.Errors = [];

        vm.DeleteAccountModel = {
            UserName: '',
            Password: ''
        };
        vm.DeleteAccount = function() {
            var _model = vm.DeleteAccountModel;
            vm.Error = false;
            Account.checkUser(_model,
                function (data) {

                    Account.remove({ token: data.Token },
                        function () {
                            $location.path('/logout');
                        },
                        function () {

                        });
                },
                function (response) {
                    vm.Error = true;
                    if (response.status >= 400 && response.status < 500) {
                        vm.ErrorMessage = 'We couldn\'t find an account for that username and password.';
                    }
                });
        };
    }
]);
