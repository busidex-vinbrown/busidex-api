/*DELETE ACCOUNT*/
function DeleteAccountCtrl($scope, $rootScope, $cookieStore, Account, $location, Analytics) {

    Analytics.trackPage($location.path());

    $scope.User = $rootScope.User;
    $scope.Model = {};
    $scope.Model.Errors = [];

    $scope.DeleteAccountModel = {
        UserName: '',
        Password: ''
    };
    $scope.DeleteAccount = deleteAccount;

    function deleteAccount() {
        var _model = $scope.DeleteAccountModel;
        Account.checkUser(_model,
            function (data) {

                Account.remove({ token: data.Token },
                    function () {
                        $location.path('/account/logout');
                    },
                    function () {

                    });
            },
            function (response) {
                if (response.status == 401) {
                    $scope.Model.Errors.push(response.data.Message);
                }
            });
    }
}
DeleteAccountCtrl.$inject = ['$scope', '$rootScope', '$cookieStore', 'Account', '$location', 'Analytics'];