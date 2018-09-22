var directives = directives || angular.module('Busidex.directives', []);

directives
    .directive('sharedCart', function () {
        'use strict';
        return {
            restrict: 'A',
            replace: true,
            templateUrl: '/views/fragments/sharedcart.html',
            transclude: true
        };
    });