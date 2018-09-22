var directives = directives || angular.module('Busidex.directives', []);

directives
    .directive('login', [function () {
        'use strict';
        return {
            restrict: 'A',
            replace: true,
            scope: { routepath: '=', linktext: '=' },
            link: function(scope) {
                scope.Model = {};
                scope.Model.loginRoute = scope.routepath;
                scope.Model.loginText = scope.linktext;
            },
            transclude: true,
            template: '<li><a href="{{Model.loginRoute}}">{{Model.loginText}}</a></li>'
        };
    }]);