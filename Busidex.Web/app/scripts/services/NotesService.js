var services = services || angular.module('Busidex.services', ['ngResource']);
services
    .factory('Notes', function ($resource) {
        'use strict';
        return $resource(ROOT + '/Notes', { id: '@id', notes: '@notes' }, {
            get: { method: 'GET' },
            post: { method: 'POST' },
            update: { method: 'PUT', data: { data: {} }, isArray: false },
            remove: { method: 'DELETE' }
        });
    });