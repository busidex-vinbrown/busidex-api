/*MY ACCOUNT*/
angular.module('Busidex').controller('AccountController', [
   '$q', '$scope', '$http', 'Account', 'Users', '$location', '$routeParams', 'Analytics', 'AccountType', 'Password', 'UserName', 'Cache', 'CacheKeys',
    function ($q, $scope, $http, Account, Users, $location, $routeParams, Analytics, AccountType, Password, UserName, Cache, CacheKeys) {
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
        
        vm.Waiting = false;
        vm.Saved = vm.Error = false;
        vm.SavedMessage = vm.ErrorMessage = '';

        //#region ACCOUNT INFO
        vm.AccountInfoModel = {};
        vm.AccountInfoModel.UserInfo = {};
        vm.AccountInfoModel.AccountTypeId = 0;
        vm.AccountInfoModel.OriginalEmail = '';

        Account.get({ id: vm.User.UserId },
            function (busidexUser) {
                vm.AccountInfoModel.UserInfo = busidexUser;
                vm.AccountInfoModel.OriginalEmail = busidexUser.Email;
            },
            function () {
                window.alert('error');
            });

        vm.CheckEmailAvailability = function () {

            var deferred = $q.defer();

            Users.get({ name: vm.AccountInfoModel.UserInfo.Email },
                function (data) {
                    if (data.User === null) {
                        deferred.resolve();
                    } else {
                        deferred.reject();
                    }
                },
                function () {
                    deferred.reject();
                });

            return deferred.promise;
            
        };
        vm.SaveAccountInfo = function () {
            vm.Error = false;
            vm.Saved = false;
            if ($scope.userDataForm.Email.$dirty) {
                vm.CheckEmailAvailability().then(function(){
                    Account.update(vm.AccountInfoModel.UserInfo,
                        function () {
                            vm.Error = false;
                            vm.Saved = true;
                            vm.SavedMessage = 'Your account information has been saved!';
                        },
                        function () {
                            vm.Saved = false;
                            vm.Error = true;
                            vm.ErrorMessage = 'There was a problem saving your account information. If the problem persists, please email busidex.help@gmail.com and we will try to help.';
                        });
                }, function () {

                    vm.Saved = false;
                    vm.Error = true;
                    vm.ErrorMessage = 'That email address is already in use.';
                });
                
            } else {
                Account.update(vm.AccountInfoModel.UserInfo,
                function () {
                    vm.Error = false;
                    vm.Saved = true;
                    vm.SavedMessage = 'Your account information has been saved!';
                },
                function () {
                    vm.Saved = false;
                    vm.Error = true;
                    vm.ErrorMessage = 'There was a problem saving your account information. If the problem persists, please email busidex.help@gmail.com and we will try to help.';
                });
            }
            
        };
        //#endregion

        //#region PASSWORD CHANGE
        vm.PasswordInfo = {};
        vm.PasswordModel = {};

        vm.PasswordModel.Validate = function () {

            return (!angular.isUndefinedOrNull(vm.PasswordInfo.OldPassword) && vm.PasswordInfo.OldPassword.length > 0) &&
                (!angular.isUndefinedOrNull(vm.PasswordInfo.NewPassword) && vm.PasswordInfo.NewPassword.length > 0) &&
                (!angular.isUndefinedOrNull(vm.PasswordInfo.OldPassword) && vm.PasswordInfo.OldPassword !== vm.PasswordInfo.NewPassword) &&
                (!angular.isUndefinedOrNull(vm.PasswordInfo.NewPassword) && vm.PasswordInfo.NewPassword === vm.PasswordInfo.ConfirmPassword);
        };
        Password.get(
            function (response) {
                response.Model.UserId = vm.User.UserId;
                vm.PasswordInfo = response.Model;
            },
            function () {

            });

        vm.SavePassword = function () {
            vm.Error = false;
            vm.Saved = false;
            Password.update(vm.PasswordInfo,
                function () {
                    vm.Error = false;
                    vm.Saved = true;
                    vm.SavedMessage = 'Your password has been changed!';
                },
                function () {
                    vm.Error = true;
                    vm.Saved = false;
                    vm.ErrorMessage = 'There was a problem changing your password. If the problem persists, please email busidex.help@gmail.com and we will try to help.';
                });
            return true;
        };
        //#endregion

        //#region USER NAME CHANGE
        vm.UserNameModel = {};
        vm.UserNameModel.UserName = vm.User.UserName;
        vm.UserNameModel.NewUserName = '';
        vm.UserNameModel.UserNameOkToUse = false;
        vm.UserNameChanged = false;
        vm.BadUserName = false;
        vm.Error = false;

        vm.CheckUserNameAvailability = function () {

            if (vm.UserNameModel.UserName === undefined || vm.UserNameModel.UserName.length < 5 || vm.UserNameModel.UserName.length > 20) {
                return;
            }

            Users.get({ name: vm.UserNameModel.UserName },
                function (data) {
                    vm.UserNameModel.UserNameOkToUse = data.User === null ? 'OK' : 'USED';
                },
                function () {
                    window.alert('There was a problem validating your username');
                });
        };
        vm.SaveUserName = function () {

            vm.UserNameModel.UserNameChanged = false;
            vm.UserNameModel.BadUserName = false;
            vm.UserNameModel.Error = false;

            UserName.update({ userId: vm.User.UserId, name: vm.UserNameModel.NewUserName },
                function () {
                    vm.UserNameModel.UserNameChanged = true;
                    vm.User.UserName = vm.UserNameModel.NewUserName;
                    vm.UserNameModel.UserName = vm.UserNameModel.NewUserName;

                    Cache.put(CacheKeys.User, angular.toJson(vm.User));
                },
                function (response) {
                    if (response.status === 400 || response.status === 404) {
                        vm.UserNameModel.BadUserName = true;
                    } else {
                        vm.UserNameModel.Error = true;
                    }
                });
        };
        //#endregion

        //#region DISPLAY NAME CHANGE
        vm.DisplayNameModel = {};
        vm.DisplayNameModel.DisplayName = vm.User.DisplayName;
        vm.DisplayNameModel.NewDisplayName = '';
        vm.DisplayNameChanged = false;
        vm.Error = false;

        
        vm.SaveDisplayName = function () {

            vm.DisplayNameModel.DisplayNameChanged = false;
            vm.DisplayNameModel.Error = false;

            Account.changeDisplayName({ name: vm.DisplayNameModel.NewDisplayName },
                function () {
                    vm.DisplayNameModel.UserNameChanged = true;
                    vm.User.DisplayName = vm.DisplayNameModel.NewDisplayName;
                    vm.DisplayNameModel.DisplayName = vm.DisplayNameModel.NewDisplayName;
                    vm.DisplayNameChanged = true;
                    Cache.put(CacheKeys.User, angular.toJson(vm.User));
                    vm.DisplayNameModel.NewDisplayName = '';
                },
                function (response) {
                    if (response.status === 400 || response.status === 404) {
                        vm.DisplayNameModel.BadUserName = true;
                    } else {
                        vm.DisplayNameModel.Error = true;
                    }
                });
        };
        //#endregion
    }
]);
