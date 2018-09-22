'use strict';

if (window.location.href.indexOf('local.') < 0 && window.location.href.indexOf('http:') >= 0 && window.location.href.indexOf('localhost') < 0) {
    window.location.href = window.location.href.replace('http:', 'https:');
}

var ROOT = 'https://www.busidexapi.com/api';
var SITEROOT = 'https://www.busidex.com';
var API_ID = '994108869461.apps.googleusercontent.com';
var FILEPICKER_KEY = 'Ax0T0Zu9Rm2KxuigGAksQz';
var API_KEY = 'gLo08X7faByUWKaw0QTVZGJ8';
var CLIENT_ID = '904413034329-99devb4k7i835kpfqdmc23f018huhbiv.apps.googleusercontent.com';

var ACCT_TYPE_BASIC = 1;
var ACCT_TYPE_PROFESSIONAL = 5;
var ACCT_TYPE_BETA = 6;
var ACCT_TYPE_ORGANIZATION = 7;

if (window.location.href.indexOf('local') >= 0 ) {
    ROOT = 'http://local.busidexapi.com/api';
    SITEROOT = 'http://local.busidex.com';
    API_ID = '994108869461-pcffqeebsao1npi6atlba6tup5bf15va.apps.googleusercontent.com';
    API_KEY = 'o94YbjE7sKGGt8gpUYWyU8Xx';
    CLIENT_ID = '904413034329-ia3qla70dmtm9t8uijefjcs56h0dtu3t.apps.googleusercontent.com';
    FILEPICKER_KEY = 'APTFmbO8TTqaGzJMukAZrz';

    ACCT_TYPE_BASIC = 1;
    ACCT_TYPE_PROFESSIONAL = 5;
    ACCT_TYPE_BETA = 6;
    ACCT_TYPE_ORGANIZATION = 7;
}
/**
 * @ngdoc overview
 * @name busidexstartApp
 * @description
 * # busidexstartApp
 *
 * Main module of the application.
 */
angular
    .module('busidexstartApp', [
        'busidexstartApp.services',
        'busidexstartApp.directives',
        'ngAnimate',
        'ngAria',
        'ngCookies',
        'ngMessages',
        'ngResource',
        'ngRoute',
        'ngSanitize',
        'ngTouch',
        'ngStorage',
        'ui.bootstrap',
        'angular-google-analytics'
    ])
    .config(function ($routeProvider, AnalyticsProvider) {

        AnalyticsProvider.setAccount('UA-29820162-1');
        AnalyticsProvider.trackPages(true);
        AnalyticsProvider.trackUrlParams(true);
        AnalyticsProvider.useDisplayFeatures(true);

        $routeProvider
            .when('/start', { templateUrl: 'views/redirect.html', controller: 'StartController'})
            .when('/login', { templateUrl: 'views/login.html'})
            .when('/logout', { templateUrl: 'views/logout.html'})
            .when('/front', { templateUrl: 'views/frontimage.html' })
            .when('/front/:_u', { templateUrl: 'views/frontimage.html' })
            .when('/back', { templateUrl: 'views/backimage.html'})
            .when('/visibility', { templateUrl: 'views/visibility.html'})
            .when('/contactinfo', { templateUrl: 'views/contactinfo.html'})
            .when('/searchinfo', { templateUrl: 'views/searchinfo.html' })
            .when('/tags', { templateUrl: 'views/tags.html' })
            .when('/addressinfo', { templateUrl: 'views/addressinfo.html' })
            .when('/help', { templateUrl: 'views/help.html' })
            .when('/nextsteps', { templateUrl: 'views/nextsteps.html' })
            .otherwise({ redirectTo: '/front' });
    })
    .run(function ($rootScope, $location, FileModifiedService) {

        $rootScope.$on('$locationChangeStart', function (event) {
            //console.log('route changing');
            if (FileModifiedService.isModified()) {
                FileModifiedService.showWarning($location.path());
                event.preventDefault();
            }
            //console.log('done');
        });

        $rootScope.$watch(function() {
                return $location.path();
            },
            function() {
                //console.log('url has changed: ' + a);
                // show loading div, etc...
                //if (FileModifiedService.isModified()) {
                //    FileModifiedService.showWarning();
                //}
            });
    });
