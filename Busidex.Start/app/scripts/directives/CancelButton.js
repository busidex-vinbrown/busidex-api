var directives = directives || angular.module('busidexstartApp.directives', []);

directives
    .directive('saveAndExit', function () {
        'use strict';
        return {
            restrict: 'E',
            template: '<a ng-href="https://www.busidex.com/#/card/mine" class="btn btn-lg btn-danger" style="margin:10px">Save and Exit</a>',
            transclude: true
        };
    });