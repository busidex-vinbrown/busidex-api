
var services = services || angular.module('busidexstartApp.services', ['ngResource']);
services
    .factory('Registration', function ($resource) {
        'use strict';
        return $resource(ROOT + '/Registration/', {}, {
            query: { method: 'GET' },
            get: { method: 'GET' },
            post: { url: ROOT + '/Registration/Post', method: 'POST' },
            update: { method: 'PUT', params: { token: '@token' } },
            activate: { method: 'PUT', params: {token: '@token'}, url: ROOT + '/Registration/ActivateAccount' },
            complete: {url: ROOT + '/account/UpdateOnboardingComplete', method: 'PUT'},
            remove: { method: 'DELETE' }
        });
    });