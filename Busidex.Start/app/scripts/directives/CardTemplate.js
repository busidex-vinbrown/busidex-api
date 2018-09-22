var directives = directives || angular.module('busidexstartApp.directives', []);

directives
    .directive('card', function() {
        'use strict';
        return {
            restrict: 'A',
            replace: true,
            templateUrl: '/views/fragments/card.html',
            scope: {
                'cardsrc': '=',
                'orientationstyle': '=',
                'elementid': '=',
                'waiting': '='
            }
        };
    });