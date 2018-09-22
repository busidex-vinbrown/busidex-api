/*jslint unused: false, strict: false */

angular.isUndefinedOrNull = function (val) {
    'use strict';
    return angular.isUndefined(val) || val === null;
};

var player = player || {};
var ROOT = 'https://www.busidexapi.com/api';
var API_ID = '994108869461.apps.googleusercontent.com';
var API_KEY = 'gLo08X7faByUWKaw0QTVZGJ8';
var CLIENT_ID = '904413034329-99devb4k7i835kpfqdmc23f018huhbiv.apps.googleusercontent.com';
if (window.location.href.indexOf('local') >= 0) {
    ROOT = 'http://local.busidexapi.com/api';
    API_ID = '994108869461-pcffqeebsao1npi6atlba6tup5bf15va.apps.googleusercontent.com';
    API_KEY = 'o94YbjE7sKGGt8gpUYWyU8Xx';
    CLIENT_ID = '904413034329-ia3qla70dmtm9t8uijefjcs56h0dtu3t.apps.googleusercontent.com';
}
var ACCT_TYPE_BASIC = 1;
var ACCT_TYPE_PROFESSIONAL = 5;
var ACCT_TYPE_BETA = 6;
var ACCT_TYPE_ORGANIZATION = 7;

var GROUPTYPE_PERSONAL = 1;
var GROUPTYPE_ORGANIZATION = 2;
var GROUPTYPE_CORPORATION = 3;
//if (window.location.href.indexOf('local.') < 0 && window.location.href.indexOf('www.') < 0 && window.TEST === undefined) {
//    var newLocation = window.location.href;
//    newLocation = newLocation.replace('http:', 'https:');
//    newLocation = newLocation.replace('https://', 'https://www.');
//    window.location.href = newLocation;
//}

if (window.location.href.indexOf('local.') < 0 && window.location.href.indexOf('localhost') < 0 && window.location.href.indexOf('http:') >= 0) {
    window.location.href = window.location.href.replace('http:', 'https:');
}

var Busidex = angular.module('Busidex', [
        'Busidex.services',
        'Busidex.directives',
        'ngAnimate',
        'ngAria',
        'ngCookies',
        'ngMessages',
        'ngResource',
        'ngRoute',
        'ngSanitize',
        'ngTouch',
        'ngStorage',
       // 'br.fullpage',
        'angular-google-analytics',
        'infinite-scroll',
        'toggle-switch',
        'ngDialog',
        'chart.js',
        'ui.bootstrap'
]).config([
        '$routeProvider', '$httpProvider', 'AnalyticsProvider', '$locationProvider', '$provide',
        function ($routeProvider, $httpProvider, AnalyticsProvider) {

            'use strict';

            AnalyticsProvider.setAccount('UA-29820162-1');
            AnalyticsProvider.trackPages(true);
            AnalyticsProvider.trackUrlParams(true);
            AnalyticsProvider.useDisplayFeatures(true);

            $routeProvider
                .when('/lizzabethbrown', { templateUrl: 'views/main.html', controller: 'VanityUrlController' })
                .when('/login', { templateUrl: 'views/login.html' })
                .when('/login/:token', { templateUrl: 'views/login.html' })
                .when('/logout', { templateUrl: 'views/logout.html' })
                .when('/main', { templateUrl: 'views/main.html' })
                .when('/home', { templateUrl: 'views/main.html' })
                .when('/faq', { templateUrl: 'views/faq.html' })
                .when('/releases', { templateUrl: 'views/releases.html' })
                .when('/quickshare', { templateUrl: 'views/quickshare.html', controller: 'QuickShareController' })
                .when('/reportabuse', { templateUrl: 'views/reportabuse.html' })
                .when('/busidex/mine', { templateUrl: 'views/busidex/mine.html' })
                .when('/card/search', { templateUrl: 'views/card/search.html' })
                .when('/card/events', { templateUrl: 'views/card/events.html' })
                .when('/card/share', { templateUrl: 'views/card/share.html' })
                .when('/card/mine', { templateUrl: 'views/card/mine.html' })
                .when('/card/notifications', { templateUrl: 'views/card/notifications_list.html' })
                .when('/account/register', { templateUrl: 'views/account/register.html' })
                .when('/account/terms', { templateUrl: 'views/account/terms.html' })
                .when('/account/privacy', { templateUrl: 'views/account/privacy.html' })
                .when('/account/passwordrecover', { templateUrl: 'views/account/passwordrecover.html' })
                .when('/account/passwordreset', { templateUrl: 'views/account/passwordreset.html' })
                .when('/account/recoversent', { templateUrl: 'views/account/recoversent.html' })
                .when('/account/mine', { templateUrl: 'views/account/mine.html' })
                .when('/account/deactivate', { templateUrl: 'views/account/delete.html' })

                .when('/events/:code', { templateUrl: 'views/events/event.html' })

                .when('/groups/mine', { templateUrl: 'views/groups/mine.html' })
                .when('/groups/details/:id', { templateUrl: 'views/groups/details.html'})
                .when('/groups/create', { templateUrl: 'views/groups/create.html' })
                .when('/groups/edit/:id', { templateUrl: 'views/groups/create.html' })

                .when('/groups/organization/:id', { templateUrl: 'views/groups/organization.html'})

                .when('/RealtorConference2014', { templateUrl: 'views/vanity/riarexpo.html' })
                .when('/bca', { templateUrl: 'views/vanity/bca.html' })
                .when('/RealtorTroopSupport', { templateUrl: 'views/vanity/RealtorTroopSupport.html' })
                .when('/osbe', { templateUrl: 'views/vanity/osbe.html' })
                .when('/ten', { templateUrl: 'views/vanity/ten.html' })
                .when('/rina', { templateUrl: 'views/vanity/rina.html' })
                .when('/bnistars', { templateUrl: 'views/vanity/bnistars.html' })
                .when('/riarexpo', { templateUrl: 'views/vanity/riarexpo.html?' })
                .when('/minutemanpress', { templateUrl: 'views/vanity/minutemanpress.html' })

                .otherwise({ redirectTo: '/main' });
        }
    ])
    .run(function ($rootScope, $location, Analytics, Cache, CacheKeys, $routeParams) {
        'use strict';
        $rootScope.$watch(function () {            
            return $location.path();
        },
         function (oldPath, newPath) {
             //console.log('$routeParams._o: ' + $routeParams._o);

             if (oldPath.indexOf('main') < 0 &&
                 oldPath.indexOf('login') < 0 &&
                 oldPath.indexOf('password') < 0 &&
                 oldPath.indexOf('faq') < 0 &&
                 oldPath.indexOf('search') < 0 &&
                 oldPath.indexOf('terms') < 0 &&
                 oldPath.indexOf('osbe') < 0 &&
                 oldPath.indexOf('ten') < 0 &&
                 oldPath.indexOf('rina') < 0 &&
                 oldPath.length > 2) {
                 var user = Cache.get(CacheKeys.User);
                 user = angular.fromJson(user);

                 if (user === null) {
                     $location.path('/login');
                     return;
                 }
             }
             if (oldPath !== newPath) {
                 Analytics.trackPage($location.path());
                 //console.log('Moved to path: ' + $location.path());
             }
         });
    });

