var directives = directives || angular.module('Busidex.directives', []);

directives
    .directive('mybusidexcard', function() {
        'use strict';
        return {
            restrict: 'A',
            replace: true,
            controller: 'MyBusidexCardController as vm',
            templateUrl: '/views/fragments/mybusidexcard.html',
            scope: {
                card: '=card'
            }
        };
    });