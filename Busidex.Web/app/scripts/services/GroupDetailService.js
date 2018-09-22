var services = services || angular.module('Busidex.services', ['ngResource']);
services
    .factory('GroupDetail', function ($resource) {
        'use strict';

        return $resource(ROOT + '/GroupDetail/', { id: '@id' }, {
            query: { method: 'GET' },
            get: { url: ROOT + '/GroupDetail/', method: 'GET' },
            post: { method: 'POST' },
            update: { method: 'PUT', data: { data: {} }, isArray: false },
            remove: { method: 'DELETE' }
        });
    });