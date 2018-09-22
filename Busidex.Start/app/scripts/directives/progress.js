var directives = directives || angular.module('busidexstartApp.directives', []);

directives
    .directive('progress', function () {
        'use strict';
        return {
            restrict: 'A',
            replace: true,
            templateUrl: '/views/fragments/progress.html',
            transclude: true,
            controller: 'ProgressController as pg'
        };
    });