'use strict';

//angular.module('PhoneGap', []).factory('PhoneGap', [
//  '$q',
//  '$rootScope',
//  '$document',
//  function ($q, $rootScope, $document) {
//      var deferred = $q.defer();
//      $document.bind('deviceready', function () {
//          $rootScope.$apply(deferred.resolve);
//      });
//      return {
//          ready: function () {
//              return deferred.promise;
//          }
//      };
//  }
//]).run([
//  'PhoneGap',
//  function (PhoneGap) {
//  }
//]);
var ACCT_TYPE_PROFESSIONAL = 5;
angular.module('myApp', ['Busidex.filters', 'Busidex.services', 'Busidex.directives', 'ngCookies', 'ngResource', 'ngTouch', 'ngSanitize', 'ngRoute', 'ajoslin.mobile-navigate'])
    .config(['$routeProvider', '$httpProvider', function ($routeProvider, $httpProvider) {
        $routeProvider
            .when("/mine/", { templateUrl: "partials/busidex/mine.html", controller: 'MyBusidexCtrl' })
            .when("/mine/:sync", { templateUrl: "partials/busidex/mine.html", controller: 'MyBusidexCtrl' })
            .when("/card/add", { templateUrl: "partials/card/add.html", controller: 'AddCardCtrl' })
            .when("/search", { templateUrl: "partials/busidex/search.html", controller: 'SearchCtrl' })
            .when("/favorites/:gId", { templateUrl: "partials/busidex/mine.html", controller: 'MyBusidexCtrl' })
            .when("/login", { templateUrl: "partials/account/login.html", controller: 'LoginPartialViewCtrl' })
            .when("/terms", { templateUrl: "partials/account/terms.html", controller: 'MainCtrl' })
            .when("/register", { templateUrl: "partials/account/register.html", controller: 'RegistrationViewCtrl' })
            .when("/", { templateUrl: "partials/home.html", controller: 'MainCtrl' })
            .otherwise({ redirectTo: "/" });
    }]).config(function($compileProvider) {
        $compileProvider.aHrefSanitizationWhitelist(/^\s*(https?|ftp|mailto|file|tel):/);
    })
.run(function ($route, $http, $templateCache) {
    angular.forEach($route.routes, function (r) {
        if (r.templateUrl) {
            $http.get(r.templateUrl, { cache: $templateCache });
        }
    });
});



