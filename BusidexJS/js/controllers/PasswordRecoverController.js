/*PASSWORD RECOVERY*/
function PasswordRecoverCtrl($scope, $rootScope, $location, Users, Analytics) {
    Analytics.trackPage($location.path());
    $scope.Model = {};
    $scope.Model.Email = '';
    $scope.Model.Error = false;
    $scope.Model.BadEmail = false;

    if (!$scope.EmailSent) {
        $scope.Model.SendEmail = function () {

            $scope.EmailSent = true;
            Users.update({ email: $scope.Model.Email },
                function () {
                    $location.path('/account/recoversent');
                },
                function (response) {
                    if (response.status == 400 || response.status == 404) {
                        $scope.Model.BadEmail = true;
                    } else {
                        $scope.Model.Error = true;
                    }

                });
        };
    }
}
PasswordRecoverCtrl.$inject = ['$scope', '$rootScope', '$location', 'Users', 'Analytics'];
