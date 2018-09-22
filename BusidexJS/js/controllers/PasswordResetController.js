/*PASSWORD RESET*/
function PasswordResetCtrl($scope, $rootScope, $location, Login, $routeParams, $cookieStore, SharedCard, Analytics) {
    Analytics.trackPage($location.path());
    $scope.Model = {};
    $scope.Model.Email = '';
    $scope.Model.Error = false;
    $scope.Model.Password = '';
    $scope.Model.ConfirmPassword = '';

    $scope.Model.LoginData = {
        UserName: '',
        Password: '',
        TempData: $routeParams.data,
        RememberMe: false
    };

    if (!$scope.LoggedIn) {
        $scope.Model.Login = function () {
            $scope.LoggedIn = true;
            if ($scope.Model.Password == $scope.Model.ConfirmPassword) {
                $scope.Model.LoginData.Password = $scope.Model.Password;
                $scope.Model.LoginData.UserName = $scope.Model.UserName;

                Login.update($scope.Model.LoginData,
                    function (user) {

                        $scope.Waiting = false;
                        $scope.resultData = user;
                        $rootScope.User = user;
                        $rootScope.LoginModel = {
                            LoginText: $rootScope.User === null ? 'Login' : 'LogOut',
                            LoginRoute: $rootScope.User === null ? '#/account/login' : '#/account/logout',
                            HomeLink: $rootScope.User !== null && $rootScope.User.StartPage == 'Organization' ? '#/groups/organization/' + $rootScope.User.Organizations[0].Item2 : '#/home'
                        };

                        $cookieStore.put('User', user);

                        $location.path("/home");
                        $rootScope.IsLoggedIn = $rootScope.User !== null;

                        SharedCard.get({},
                            function (data) {
                                if (data.SharedCards && data.SharedCards.length > 0) {
                                    $rootScope.HasSharedCards = true;
                                    SharedCardList = data.SharedCards;
                                    $scope.SendFromEmail = data.SharedCards[0].SendFromEmail;
                                }
                            },
                            function () {

                            });
                    },
                    function () {
                        $scope.Model.Error = true;
                        $scope.LoggedIn = false;
                    });
            } else {
                $scope.Model.Error = true;
                $scope.LoggedIn = false;
            }
        };
    }
}
PasswordResetCtrl.$inject = ['$scope', '$rootScope', '$location', 'Login', '$routeParams', '$cookieStore', 'SharedCard', 'Analytics'];
