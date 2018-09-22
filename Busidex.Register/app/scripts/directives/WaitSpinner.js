var directives = directives || angular.module('busidexregister.directives', []);

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