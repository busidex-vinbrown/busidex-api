/*APPLICATION VERSION*/
function ApplicationVersionCtrl($scope, ApplicationVersion) {
    $scope.Version = ApplicationVersion.Version;
}
ApplicationVersionCtrl.$inject = ['$scope', 'ApplicationVersion'];
