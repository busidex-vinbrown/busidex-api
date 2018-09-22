/*MY ACCOUNT*/
function AccountCtrl($scope, $rootScope, $cookieStore, Account, Users, $location, $routeParams, Analytics, AccountType, Settings, Password, UserName) {

    Analytics.trackPage($location.path());
    $rootScope.IsLoggedIn = $rootScope.User !== null;
    $rootScope.ShowFilterControls = false;
    $scope.DeleteAccount = deleteAccount;

    $scope.User = $rootScope.User;

    $scope.Tabs = {};
    $scope.Tabs.info = false;
    $scope.Tabs.settings = false;
    $scope.Tabs.password = false;
    $scope.Tabs.username = true;
    $scope.Tabs.accounttype = false;
    $scope.Tabs.abuse = false;

    $scope.SetCurrentTab = function (currentTab) {

        for (var tab in $scope.Tabs) {
            if ($scope.Tabs.hasOwnProperty(tab)) {
                $scope.Tabs[tab] = false;
            }
        }
        $scope.Tabs[currentTab] = true;
    };
    $scope.SetCurrentTab('info');
    $scope.Waiting = false;

    //ACCOUNT INFO
    $scope.AccountInfoModel = {};
    $scope.AccountInfoModel.UserInfo = {};
    $scope.AccountInfoModel.AccountTypeId = 0;
    $scope.AccountInfoModel.Saved = $scope.AccountInfoModel.Error = false;
    $scope.AccountInfoModel.EmailOkToUse = '';
    $scope.AccountInfoModel.OriginalEmail = "";

    Account.get({ id: $rootScope.User.UserId },
        function (busidexUser) {
            $scope.AccountInfoModel.UserInfo = busidexUser;
            $scope.AccountInfoModel.OriginalEmail = busidexUser.Email;
        },
        function () {
            alert('error');
        });

    $scope.CheckEmailAvailability = function () {

        if ($scope.AccountInfoModel.UserInfo.Email == $scope.OriginalEmail) {
            $scope.AccountInfoModel.EmailOkToUse = '';
            return;
        }

        if ($scope.AccountInfoModel.UserInfo.Email === undefined ||
            $scope.AccountInfoModel.UserInfo.Email.length < 5 ||
            $scope.AccountInfoModel.UserInfo.Email.length > 20) {
            return;
        }

        Users.get({ name: $scope.AccountInfoModel.UserInfo.Email },
            function (data) {
                $scope.AccountInfoModel.EmailOkToUse = data.User === null ? 'OK' : 'USED';
            },
            function () {
                alert('There was a problem validating your username');
            });
    };
    $scope.SaveAccountInfo = function () {
        Account.update($scope.AccountInfoModel.UserInfo,
        function (busidexUser) {

        },
        function () {

        });
    };

    function deleteAccount() {

        $location.path('/account/delete');
    }

    // ACCOUNT TYPE
    $scope.AccountTypeModel = {};
    AccountType.get({ id: $rootScope.User.UserId },
        function (results) {

            $scope.Plans = results.Plans;
            for (var i = 0; i < $scope.Plans.length; i++) {
                $scope.Plans[i].Selected = $scope.Plans[i].AccountTypeId == results.SelectedPlanId;
                if ($scope.Plans[i].Selected) {
                    $scope.AccountTypeModel.AccountTypeId = $scope.Plans[i].AccountTypeId;
                }
            }

        },
        function (error) {
        });

    $scope.SaveAccountType = function () {

        $scope.AccountTypeModel.Saved = $scope.AccountTypeModel.Error = false;
        AccountType.update({ userAccountId: $rootScope.User.UserAccountId, accountTypeId: $scope.AccountTypeModel.AccountTypeId },
        function () {
            $scope.AccountTypeModel.Saved = true;

            $rootScope.User.AccountTypeId = $scope.AccountTypeModel.AccountTypeId;
            $cookieStore.put('User', $rootScope.User);
        },
        function (response) {
            $scope.AccountTypeModel.Saved = false;
            $scope.AccountTypeModel.Error = true;
            console.log(response.status);
        });
    };

    // SETTINGS
    $scope.SettingsInfo = {};
    Settings.get({ id: $rootScope.User.UserId },
        function (response) {
            response.Model.UserId = $rootScope.User.UserId;
            $scope.SettingsInfo = response.Model;
        },
        function () {

        });

    $scope.SaveSettings = function () {

        Settings.update($scope.SettingsInfo,
            function () {
                $location.path("/account/index");
            },
            function () {
                alert('settings not changed');
            });
        return true;
    };

    // PASSWORD CHANGE
    $scope.PasswordInfo = {};
    $scope.PasswordModel = {};

    $scope.PasswordModel.Validate = function () {

        return ($scope.PasswordInfo.OldPassword !== null && $scope.PasswordInfo.OldPassword.length > 0) &&
            ($scope.PasswordInfo.NewPassword !== null && $scope.PasswordInfo.NewPassword.length > 0) &&
            $scope.PasswordInfo.OldPassword != $scope.PasswordInfo.NewPassword &&
            $scope.PasswordInfo.NewPassword == $scope.PasswordInfo.ConfirmPassword;
    };
    Password.get(
        function (response) {
            response.Model.UserId = $rootScope.User.UserId;
            $scope.PasswordInfo = response.Model;
        },
        function () {

        });

    $scope.SavePassword = function () {
        Password.update($scope.PasswordInfo,
            function () {

            },
            function () {
                alert('password not changed');
            });
        return true;
    };

    // USER NAME CHANGE
    $scope.UserNameModel = {};
    $scope.UserNameModel.UserName = $rootScope.User.UserName;
    $scope.UserNameModel.NewUserName = '';
    $scope.UserNameModel.UserNameOkToUse = false;
    $scope.UserNameChanged = false;
    $scope.BadUserName = false;
    $scope.Error = false;

    $scope.CheckUserNameAvailability = function () {

        if ($scope.UserNameModel.UserName === undefined || $scope.UserNameModel.UserName.length < 5 || $scope.UserNameModel.UserName.length > 20) {
            return;
        }

        Users.get({ name: $scope.UserNameModel.UserName },
            function (data) {
                $scope.UserNameModel.UserNameOkToUse = data.User === null ? 'OK' : 'USED';
            },
            function () {
                alert('There was a problem validating your username');
            });
    };
    $scope.SaveUserName = function () {

        $scope.UserNameModel.UserNameChanged = false;
        $scope.UserNameModel.BadUserName = false;
        $scope.UserNameModel.Error = false;

        UserName.update({ userId: $rootScope.User.UserId, name: $scope.UserNameModel.NewUserName },
            function () {
                $scope.UserNameModel.UserNameChanged = true;
                $rootScope.User.UserName = $scope.UserNameModel.NewUserName;
                $scope.UserNameModel.UserName = $scope.UserNameModel.NewUserName;

                $cookieStore.put('User', $rootScope.User);
            },
            function (response) {
                if (response.status == 400 || response.status == 404) {
                    $scope.UserNameModel.BadUserName = true;
                } else {
                    $scope.UserNameModel.Error = true;
                }
            });
    };
}
AccountCtrl.$inject = ['$scope', '$rootScope', '$cookieStore', 'Account', 'Users', '$location', '$routeParams', 'Analytics', 'AccountType', 'Settings', 'Password', 'UserName'];
