var directives = directives || angular.module('busidexstartApp.directives', []);

directives
    .directive('fileModifiedWarning', function () {
        'use strict';
        return {
            restrict: 'E',
            replace: true,
            templateUrl: '/views/fragments/file_modified_warning.html',
            transclude: true            
        };
    });