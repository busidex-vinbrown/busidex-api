var services = services || angular.module('Busidex.services', ['ngResource']);
services
    .factory('Communications', function ($resource) {
        'use strict';
        return $resource(ROOT + '/Communications', {
            query: { method: 'GET', cache: false },
            get: { method: 'GET', cache: false, isArray: true },
            post: { method: 'POST', params: { UserId: '@UserId' }, cache: false },
            update: { method: 'PUT', cache: false },
            remove: { method: 'DELETE' }
        });
    });