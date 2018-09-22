var services = services || angular.module('Busidex.services', ['ngResource']);
services
    .factory('Password', function ($resource) {
        'use strict';

        return $resource(ROOT + '/Password/', {}, {
            query: { method: 'GET' },
            get: { method: 'GET' },
            post: { method: 'POST' },
            update: { method: 'PUT', data: { data: {} }, isArray: false },
            remove: { method: 'DELETE' }
        });
    });