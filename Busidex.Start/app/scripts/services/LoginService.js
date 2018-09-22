
var services = services || angular.module('busidexstartApp.services', ['ngResource']);
services
    .factory('Login', function($resource) {
        'use strict';
        return $resource(ROOT + '/Account/Login', {}, {
            query: { method: 'GET' },
            get: { method: 'GET' },
            post: { method: 'POST', isArray: false },
            update: { url: ROOT + '/Account/LoginReset', method: 'PUT' },
            remove: { method: 'DELETE' }
        });
    });