var directives = directives || angular.module('busidexstartApp.directives', []);

directives
    .directive('header', function () {
        'use strict';
        return {
            restrict: 'A',
            replace: true,
            templateUrl: '/views/fragments/header.html',
            transclude: true,
            controller: 'HeaderController as vm'
        };
    });