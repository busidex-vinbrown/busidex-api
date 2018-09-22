/*APPLICATION VERSION*/
function ApplicationVersionCtrl($scope, ApplicationVersion) {
    'use strict';
    $scope.Version = ApplicationVersion.Version;
}
ApplicationVersionCtrl.$inject = ['$scope', 'ApplicationVersion'];
