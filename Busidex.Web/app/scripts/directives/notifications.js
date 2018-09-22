var directives = directives || angular.module('Busidex.directives', []);

directives
    .directive('notifications', function () {
        'use strict';
        return {
            restrict: 'A',
            replace: true,
            templateUrl: '/views/fragments/notifications.html',
            transclude: true
        };
    });