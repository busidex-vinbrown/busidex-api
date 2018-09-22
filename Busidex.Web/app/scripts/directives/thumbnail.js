var directives = directives || angular.module('Busidex.directives', []);

directives
    .directive('thumbnail', function() {
        'use strict';
        return {
            restrict: 'A',
            replace: true,
            controller: 'ThumbnailController as vm',
            templateUrl: '/views/fragments/thumbnail.html',
            scope: {
                card: '=card'
            }
        };
    });