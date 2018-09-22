/*USERNAME RECOVERY*/
function UserNameRecoverCtrl($scope, $rootScope, $location, Users, Analytics) {
    'use strict';
    Analytics.trackPage($location.path());
    $scope.Model = {};
    $scope.Model.Email = '';
    $scope.Model.BadEmail = false;
    $scope.Model.Error = false;

    $scope.Model.SendEmail = function () {
        Users.post({ email: $scope.Model.Email },
            function () {
                $location.path('/account/recoversent');
            },
            function (response) {
                if (response.status === 400 || response.status === 404) {
                    $scope.Model.BadEmail = true;
                } else {
                    $scope.Model.Error = true;
                }
            });
    };
}
UserNameRecoverCtrl.$inject = ['$scope', '$rootScope', '$location', 'Users', 'Analytics'];