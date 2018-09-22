
var services = services || angular.module('Busidex.services', ['ngResource']);
services
    .factory('Login', function($resource) {
        'use strict';
        return $resource(ROOT + '/Account/Login', {}, {
            query: { method: 'GET', cache: false },
            get: { method: 'GET', cache: false },
            post: { method: 'POST', isArray: false, cache: false },
            update: { url: ROOT + '/Account/LoginReset', method: 'PUT', cache: false },
            remove: { method: 'DELETE', cache: false }
        });
    });