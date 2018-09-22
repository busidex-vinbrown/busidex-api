/*LOG IN PAGE*/
function LoginViewCtrl($scope, $rootScope, $http, $cookieStore, $location, Login, SharedCard, $routeParams) {

    $scope.lastForm = {};
    $scope.form = {
        UserName: '',
        Password: '',
        Token: $routeParams._t
    };

    $rootScope.User = null;
    $scope.sendForm = function (form) {

        $scope.Waiting = true;
        $scope.lastForm = angular.copy(form);
        var data = {
            'UserName': $scope.form.UserName,
            'Password': $scope.form.Password,
            'Token:': $scope.form.Token,
            'RememberMe': false

        };
        $scope.LoginErrors = [];
        Login.post($scope.form,
            function (user) {
                $scope.Waiting = false;
                $scope.resultData = user;
                $rootScope.User = user;
                $rootScope.User.RememberMe = $scope.form.RememberMe;

                var acctType = $rootScope.User !== null ? parseInt($rootScope.User.AccountTypeId) : 0;
                $rootScope.MyBusidexMenuName = parseInt($rootScope.User.AccountTypeId) === ACCT_TYPE_ORGANIZATION ? 'Edit Referrals' : 'My Busidex';
                $rootScope.MyBusidexName = $rootScope.User !== null && acctType === ACCT_TYPE_ORGANIZATION ? 'Referrals' : 'My Busidex';

                $rootScope.SearchModel = null;

                $rootScope.LoginModel = {
                    LoginText: $rootScope.User === null ? 'Login' : 'LogOut',
                    LoginRoute: $rootScope.User === null ? '#/account/login' : '#/account/logout',
                    HomeLink: $rootScope.User !== null && $rootScope.User.StartPage == 'Organization' ? '#/groups/organization/' + $rootScope.User.Organizations[0].Item2 : '#/home'
                };

                $cookieStore.put('User', user);
                $rootScope.IsLoggedIn = $rootScope.User !== null;

                var token = $rootScope.User !== null ? $rootScope.User.Token : null;
                $http.defaults.headers.common['X-Authorization-Token'] = token;

                SharedCard.get({},
                        function (data) {
                            if (data.SharedCards && data.SharedCards.length > 0) {
                                $rootScope.HasSharedCards = true;
                                $rootScope.SharedCards = [];
                                for (var i = 0; i < data.SharedCards.length; i++) {
                                    $rootScope.SharedCards.push(data.SharedCards[i]);
                                }
                                $scope.SendFromEmail = data.SharedCards[0].SendFromEmail;
                            }
                        },
                        function () {

                        });

                if (user.StartPage == 'Organization') {

                    $location.path('/groups/organization/' + user.Organizations[0].Item2);

                } else {

                    $location.path('/home');
                    $location.url($location.path()); // remove any query string parameters

                    $http({
                        method: 'GET',
                        url: ROOT + "/mycard/?id=" + user.UserId,
                        headers: { 'Content-Type': 'application/json' }
                    }).success(function (model) {

                        if ($rootScope.User.AccountTypeId == 5 || $rootScope.User.AccountTypeId == 6) {
                            var card = model.MyCards[0];
                            if (card !== null && card !== undefined &&
                            (card.Name === null || card.Name.length === 0) &&
                            (card.CompanyName === null || card.CompanyName.length === 0)) {
                                $location.path('/card/edit/' + card.CardId);
                            } else if (card === null) {
                                $location.path('/card/add/mine');
                            }
                        }
                    });
                }
            },
            function (error) {

                if (error.status == 404) {
                    $scope.LoginErrors.push('Username or password is incorrect');
                } else {
                    $scope.LoginErrors.push(error.data.Message);
                }
                $scope.Waiting = false;
            });
    };

    $scope.resetForm = function () {
        $scope.form = angular.copy($scope.lastForm);
    };
}
LoginViewCtrl.$inject = ['$scope', '$rootScope', '$http', '$cookieStore', '$location', 'Login', 'SharedCard', '$routeParams'];