'use strict';

if (window.location.href.indexOf('local.') < 0 && window.location.href.indexOf('http:') >= 0 && window.location.href.indexOf('localhost') < 0) {
    window.location.href = window.location.href.replace('http:', 'https:');
}

var ROOT = 'https://www.busidexapi.com/api';
var SITEROOT = 'https://www.busidex.com';
var API_ID = '994108869461.apps.googleusercontent.com';
var API_KEY = 'gLo08X7faByUWKaw0QTVZGJ8';
var CLIENT_ID = '904413034329-99devb4k7i835kpfqdmc23f018huhbiv.apps.googleusercontent.com';

var ACCT_TYPE_BASIC = 1;
var ACCT_TYPE_PROFESSIONAL = 5;
var ACCT_TYPE_BETA = 6;
var ACCT_TYPE_ORGANIZATION = 7;

if (window.location.href.indexOf('local') >= 0) {
    ROOT = 'http://local.busidexapi.com/api';
    SITEROOT = 'http://local.busidex.com';
    API_ID = '994108869461-pcffqeebsao1npi6atlba6tup5bf15va.apps.googleusercontent.com';
    API_KEY = 'o94YbjE7sKGGt8gpUYWyU8Xx';
    CLIENT_ID = '904413034329-ia3qla70dmtm9t8uijefjcs56h0dtu3t.apps.googleusercontent.com';    
    ACCT_TYPE_BASIC = 1;
    ACCT_TYPE_ORGANIZATION = 7;
    ACCT_TYPE_PROFESSIONAL = 5;
    ACCT_TYPE_BETA = 6;
}
/**
 * @ngdoc overview
 * @name busidexregister
 * @description
 * # busidexregister
 *
 * Main module of the application.
 */
angular
    .module('busidexregister', [
        'busidexregister.services',
        'busidexregister.directives',
        'ngAnimate',
        'ngAria',
        'ngCookies',
        'ngMessages',
        'ngResource',
        'ngRoute',
        'ngSanitize',
        'ngTouch',
        'ngStorage',
        'angular-google-analytics',
        'ngMessages',
        'ui.bootstrap'
    ])
    .config(function ($routeProvider, AnalyticsProvider) {

        AnalyticsProvider.setAccount('UA-29820162-1');
        AnalyticsProvider.trackPages(true);
        AnalyticsProvider.trackUrlParams(true);
        AnalyticsProvider.useDisplayFeatures(true);

        $routeProvider
            .when('/name', { templateUrl: 'views/name.html' })
            .when('/accounttype', { templateUrl: 'views/accounttype.html' })
            .when('/finish', { templateUrl: 'views/finish.html' })
            .when('/name/:token', { templateUrl: 'views/name.html' })
            .when('/accounttype/:token', { templateUrl: 'views/accounttype.html' })
            .when('/finish/:token', { templateUrl: 'views/finish.html' })
            .otherwise({ redirectTo: '/name' });
    })
    .run(function ($rootScope, $location, Analytics) {
        $rootScope.$watch(function() {
                return $location.path();
            },
            function (oldPath, newPath) {
                if (oldPath !== newPath) {
                    Analytics.trackPage($location.path());
                    //console.log('Moved to path: ' + $location.path());
                }
            });
    });
