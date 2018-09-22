/*REGISTRATION COMPLETE*/
function RegistrationCompleteCtrl($scope, $rootScope, $cookieStore, $route, $routeParams, Registration, $location, Analytics) {
    Analytics.trackPage($location.path());
    $scope.Waiting = false;
    $rootScope.IsLoggedIn = $rootScope.User !== null;

    $scope.RegistrationErrors = [];
    var _token = $routeParams.token;
    Registration.update({ token: _token },
        function () {
            _gaq.push(['_trackEvent', 'Registration', 'Complete', _token]);
        },
        function (error) {
            if (error === null || error === undefined || error.Message === null) {
                error = { Message: 'There was a problem completing your registration.' };
            }
            $scope.RegistrationErrors.push(error.Message);
        });
}
RegistrationCompleteCtrl.$inject = ['$scope', '$rootScope', '$cookieStore', '$route', '$routeParams', 'Registration', '$location', 'Analytics'];