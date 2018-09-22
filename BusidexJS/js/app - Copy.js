//'use strict';
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

var window = window || {};

var GROUPTYPE_PERSONAL = 1;
var GROUPTYPE_ORGANIZATION = 2;
var GROUPTYPE_CORPORATION = 3;
var CACHE_VERSION = 16;
if (window.location.href.indexOf('local.') < 0 && window.location.href.indexOf('www.') < 0) {
    var newLocation = window.location.href;
    newLocation = newLocation.replace('http:', 'https:');
    newLocation = newLocation.replace('https://', 'https://www.');
    window.location.href = newLocation;
}

if (window.location.href.indexOf('local.') < 0 && window.location.href.indexOf('http:') >= 0) {
    window.location.href = window.location.href.replace('http:', 'https:');
}


var Busidex = angular.module('Busidex', ['ngCookies']);
Busidex.factory('Data', function () {
    return { User: null };
});
var i_errors = 0;
var i_users = 0;
var i_owners = 0;
var i_newcards = 0;
var i_popular = 0;
var i_tags = 0;
var i_unowned = 0;

function clearAdminIntervals() {
    clearInterval(i_errors);
    clearInterval(i_users);
    clearInterval(i_owners);
    clearInterval(i_newcards);
    clearInterval(i_popular);
    clearInterval(i_tags);
    clearInterval(i_unowned);
}
// Declare app level module which depends on filters, and services
angular.module('Busidex', ['Busidex.filters', 'Busidex.services', 'Busidex.directives', 'ngCookies', 'ui.bootstrap', 'ngResource', 'ngSanitize', 'ngRoute', 'ngGrid', 'ngAnimate', 'angular-google-analytics', 'ui.bootstrap.popover', 'textAngular', 'satellizer']).
    config(['$routeProvider', '$httpProvider', 'AnalyticsProvider', '$locationProvider', '$provide', function ($routeProvider, $httpProvider, AnalyticsProvider, $locationProvider, $provide) {

        //$provide.decorator('$sniffer', function ($delegate) {
        //    $delegate.history = false;
        //    return $delegate;
        //});

        //$authProvider.facebook({
        //    clientId: '657854390977827'
        //});

        //$authProvider.google({
        //    clientId: CLIENT_ID
        //});

        //$authProvider.github({
        //    clientId: '0ba2600b1dbdb756688b'
        //});

        //$authProvider.linkedin({
        //    clientId: '77cw786yignpzj'
        //});

        //$authProvider.yahoo({
        //    clientId: 'dj0yJmk9dkNGM0RTOHpOM0ZsJmQ9WVdrOVlVTm9hVk0wTkRRbWNHbzlNQS0tJnM9Y29uc3VtZXJzZWNyZXQmeD0wMA--',
        //});

        //$authProvider.twitter({
        //    url: '/auth/twitter'
        //});
        //$authProvider.loginOnSignup = true;
        //$authProvider.loginRedirect = '/';
        //$authProvider.logoutRedirect = '/';
        //$authProvider.signupRedirect = '/login';
        //$authProvider.loginUrl = '/account/login';// '/auth/login';
        //$authProvider.signupUrl = ROOT + "/Registration/Post";//'/auth/signup';
        //$authProvider.loginRoute = '/login';
        //$authProvider.signupRoute = '/signup';
        //$authProvider.tokenName = 'token';
        //$authProvider.tokenPrefix = 'satellizer'; // Local Storage name prefix
        //$authProvider.unlinkUrl = '/auth/unlink/';
        //$authProvider.authHeader = 'Authorization';

        //$authProvider.oauth2({
        //    name: 'foursquare',
        //    url: '/auth/foursquare',
        //    clientId: 'MTCEJ3NGW2PNNB31WOSBFDSAD4MTHYVAZ1UKIULXZ2CVFC2K',
        //    redirectUri: window.location.origin || window.location.protocol + '//' + window.location.host,
        //    authorizationEndpoint: 'https://foursquare.com/oauth2/authenticate'
        //});

        $provide.decorator('taOptions', ['$delegate', function (taOptions) {
            // $delegate is the taOptions we are decorating
            // here we override the default toolbars and classes specified in taOptions.
            taOptions.toolbar = [
                ['bold', 'italics', 'underline', 'ul', 'ol', 'redo', 'undo', 'clear'],
                ['justifyLeft', 'justifyCenter', 'justifyRight']//,
                //['html']
            ];
            taOptions.classes = {
                focussed: 'focussed',
                toolbar: 'btn-toolbar',
                toolbarGroup: 'btn-group',
                toolbarButton: 'btn btn-default',
                toolbarButtonActive: 'active',
                disabled: 'disabled',
                textEditor: 'form-control',
                htmlEditor: 'form-control'
            };
            return taOptions; // whatever you return will be the taOptions
        }]);

        $httpProvider.defaults.useXDomain = true;
        AnalyticsProvider.setAccount('UA-29820162-1');
        AnalyticsProvider.trackPages(true);
        //$locationProvider.html5Mode(true).hashPrefix('!');;
        //$locationProvider.html5Mode(true);
        // VANITY URLS (this won't scale. fix this some day)
        $routeProvider.when('/lizzabethbrown', { redirectTo: '/card/details/465', controller: CardDetailCtrl });

        $routeProvider
        .when('/home', { templateUrl: 'partials/home.html?' + CACHE_VERSION, controller: 'HomeViewCtrl' })
        .when('/about', { templateUrl: 'partials/about.html?' + CACHE_VERSION, controller: 'GenericViewCtrl' })
        .when('/card/confirm/:token', { templateUrl: 'partials/card/confirm.html?' + CACHE_VERSION, controller: 'ConfirmCardCtrl' })
        .when('/card/search', { templateUrl: 'partials/card/search.html?' + CACHE_VERSION, controller: 'SearchCtrl' })
        .when('/card/search/:_t', { templateUrl: 'partials/card/search.html?' + CACHE_VERSION, controller: 'SearchCtrl' })
        .when('/card/add', { templateUrl: 'partials/card/add.html?' + CACHE_VERSION, controller: 'AddCardCtrl' })
        .when('/card/add/mine', { templateUrl: 'partials/card/add.html?' + CACHE_VERSION, controller: 'AddCardCtrl' })
        .when('/card/edit/:id', { templateUrl: 'partials/card/add.html?' + CACHE_VERSION, controller: 'AddCardCtrl' })
        .when('/card/mine', { templateUrl: 'partials/card/mine.html?' + CACHE_VERSION, controller: 'MyCardsCtrl' })
        .when('/card/details/:CardId', { templateUrl: 'partials/card/details.html?' + CACHE_VERSION, controller: 'CardDetailCtrl' })

        .when('/busidex/mine', { templateUrl: 'partials/busidex/mine.html?' + CACHE_VERSION, controller: 'MyBusidexCtrl' })
        .when('/busidex/suggestions', { templateUrl: 'partials/busidex/suggestions.html?' + CACHE_VERSION, controller: 'SuggestionsCtrl' })
        .when('/busidex/survey', { templateUrl: 'partials/busidex/survey.html?' + CACHE_VERSION, controller: 'GenericViewCtrl' })

        .when('/groups/mine', { templateUrl: 'partials/groups/mine.html?' + CACHE_VERSION, controller: 'GroupsCtrl' })
        .when('/groups/details/:id', { templateUrl: 'partials/groups/details.html?' + CACHE_VERSION, controller: 'GroupDetailCtrl' })
        .when('/groups/create', { templateUrl: 'partials/groups/create.html?' + CACHE_VERSION, controller: 'CreateGroupCtrl' })
        .when('/groups/edit/:id', { templateUrl: 'partials/groups/create.html?' + CACHE_VERSION, controller: 'EditGroupCtrl' })
        
        .when('/groups/organization/:id', { templateUrl: 'partials/groups/organization.html?' + CACHE_VERSION, controller: 'OrganizationCtrl' })

        .when('/account/login', { templateUrl: 'partials/account/login.html?' + CACHE_VERSION, controller: 'LoginViewCtrl' })
        .when('/account/login/:token', { templateUrl: 'partials/account/login.html?' + CACHE_VERSION, controller: 'LoginViewCtrl' })
        .when('/account/logout', { templateUrl: 'partials/account/logout.html?' + CACHE_VERSION, controller: 'LogoutViewCtrl' })
        .when('/account/index', { templateUrl: 'partials/account/index.html?' + CACHE_VERSION, controller: 'AccountCtrl' })
        .when('/account/terms', { templateUrl: 'partials/account/terms.html?' + CACHE_VERSION, controller: 'GenericViewCtrl' })
        .when('/account/privacy', { templateUrl: 'partials/account/privacy.html?' + CACHE_VERSION, controller: 'GenericViewCtrl' })
        .when('/account/passwordrecover', { templateUrl: 'partials/account/passwordrecover.html?' + CACHE_VERSION, controller: 'PasswordRecoverCtrl' })
        .when('/account/passwordreset', { templateUrl: 'partials/account/passwordreset.html?' + CACHE_VERSION, controller: 'PasswordResetCtrl' })
        .when('/account/register', { templateUrl: 'partials/account/register.html?' + CACHE_VERSION, controller: 'RegistrationViewCtrl' })
        .when('/account/register/:token', { templateUrl: 'partials/account/register.html?' + CACHE_VERSION, controller: 'RegistrationViewCtrl' })
        .when('/account/registrationcomplete/token/:token', { templateUrl: 'partials/account/registrationcomplete.html?' + CACHE_VERSION, controller: 'RegistrationCompleteCtrl' })
        .when('/account/usernamerecover', { templateUrl: 'partials/account/usernamerecover.html?' + CACHE_VERSION, controller: 'UserNameRecoverCtrl' })
        .when('/account/recoversent', { templateUrl: 'partials/account/recoversent.html?' + CACHE_VERSION, controller: 'GenericViewCtrl' })
        .when('/account/delete/', { templateUrl: 'partials/account/delete.html?' + CACHE_VERSION, controller: 'DeleteAccountCtrl' })

        .when('/RealtorConference2014', { templateUrl: 'partials/account/welcome.html?' + CACHE_VERSION, controller: 'GenericViewCtrl' })
        .when('/bca', { templateUrl: 'partials/vanity/bca.html?' + CACHE_VERSION, controller: 'GenericViewCtrl' })
        .when('/RealtorTroopSupport', { templateUrl: 'partials/vanity/RealtorTroopSupport.html?' + CACHE_VERSION, controller: 'GenericViewCtrl' })
        .when('/osbe', { templateUrl: 'partials/vanity/osbe.html?' + CACHE_VERSION, controller: 'GenericViewCtrl' })

        .when('/settings', { templateUrl: 'partials/settings/index.html?' + CACHE_VERSION, controller: 'GenericViewCtrl' })

        .when('/admin/index', { templateUrl: 'partials/admin/index.html?' + CACHE_VERSION, controller: 'GenericViewCtrl' })
        .when('/admin/newcards', { templateUrl: 'partials/admin/newcards.html?' + CACHE_VERSION, controller: 'AdminCtrl' })
        .when('/admin/unownedcards', { templateUrl: 'partials/admin/unownedcards.html?' + CACHE_VERSION, controller: 'AdminCtrl' })
        .when('/admin/populartags', { templateUrl: 'partials/admin/populartags.html?' + CACHE_VERSION, controller: 'AdminCtrl' })
        .when('/admin/systemtags', { templateUrl: 'partials/admin/systemtags.html?' + CACHE_VERSION, controller: 'AdminCtrl' })
        .when('/admin/errors', { templateUrl: 'partials/admin/errors.html?' + CACHE_VERSION, controller: 'AdminCtrl' })
        .when('/admin/users', { templateUrl: 'partials/admin/users.html?' + CACHE_VERSION, controller: 'AdminCtrl' })
        .when('/admin/owners', { templateUrl: 'partials/admin/owners.html?' + CACHE_VERSION, controller: 'AdminCtrl' })
        .when('/admin/edit', { templateUrl: 'partials/admin/editcards.html?' + CACHE_VERSION, controller: 'AdminEditCtrl' })
        .when('/admin/communication', { templateUrl: 'partials/admin/communication.html?' + CACHE_VERSION, controller: 'AdminCtrl' })

        .when('/newsletters/index', { templateUrl: 'partials/newsletters/index.html?' + CACHE_VERSION, controller: 'GenericViewCtrl' })
        .when('/newsletters/newsletter_1', { templateUrl: 'partials/newsletters/newsletter_1.html?' + CACHE_VERSION, controller: 'GenericViewCtrl' })
        .when('/newsletters/newsletter_2', { templateUrl: 'partials/newsletters/newsletter_2.html?' + CACHE_VERSION, controller: 'GenericViewCtrl' })
        .when('/newsletters/newsletter_3', { templateUrl: 'partials/newsletters/newsletter_3.html?' + CACHE_VERSION, controller: 'GenericViewCtrl' })
        .when('/newsletters/newsletter_4', { templateUrl: 'partials/newsletters/newsletter_4.html?' + CACHE_VERSION, controller: 'GenericViewCtrl' })
        .when('/newsletters/newsletter_5', { templateUrl: 'partials/newsletters/newsletter_5.html?' + CACHE_VERSION, controller: 'GenericViewCtrl' })
        .when('/newsletters/newsletter_6', { templateUrl: 'partials/newsletters/newsletter_6.html?' + CACHE_VERSION, controller: 'GenericViewCtrl' })
        .when('/newsletters/newsletter_7', { templateUrl: 'partials/newsletters/newsletter_7.html?' + CACHE_VERSION, controller: 'GenericViewCtrl' })

        .otherwise({ redirectTo: '/home' });

}]).run(function ($rootScope, $location) {
        $rootScope.$watch(function () {
            return $location.path();
        },
         function (a) {
             //console.log('url has changed: ' + a);
             clearAdminIntervals();
             // show loading div, etc...
         });
    });

