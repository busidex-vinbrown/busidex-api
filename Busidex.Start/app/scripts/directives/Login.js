var directives = directives || angular.module('busidexstartApp.directives', []);

directives
    .directive('login', [function () {
        'use strict';
        return {
            restrict: 'A',
            //controller: 'LoginFragmentController as vm',
            replace: true,
            scope: { routepath: '=', linktext: '=' },
            link: function(scope) {
                //var user = Cache.get(CacheKeys.User) || $cookies.get(CacheKeys.User) || '{}';
                //user = JSON.parse(user);
                scope.Model = {};
                scope.Model.loginRoute = scope.routepath;
                scope.Model.loginText = scope.linktext;

                
                //Cache.put(CacheKeys.LoginRoute, user.UserId === undefined ? '#/login' : '#/logout');
                //Cache.put(CacheKeys.LoginText, user.UserId === undefined ? 'LOGIN' : 'LOGOUT');
            },
            transclude: true,
            template: '<li><a href="{{Model.loginRoute}}">{{Model.loginText}}</a></li>'
        };
    }]);