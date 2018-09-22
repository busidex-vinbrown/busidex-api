var directives = directives || angular.module('busidexstartApp.directives', []);

directives
    .directive('waitSpinner', function() {
        'use strict';
        return {
            restrict: 'A',
            replace: true,
            scope: {
                'waiting': '='
            }
        };
    });