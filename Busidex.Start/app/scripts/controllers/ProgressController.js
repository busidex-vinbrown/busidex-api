
angular.module('busidexstartApp').controller('ProgressController', [
    '$scope', 'Progress', '$location',
    function ($scope, Progress, $location) {
        'use strict';

        var vm = this;
        
        $scope.$watch(function () {
            return $location.path();
        }, function () {
            vm.progress = Progress.get();
            vm.message = Progress.message();
        }, true);
    }
]);