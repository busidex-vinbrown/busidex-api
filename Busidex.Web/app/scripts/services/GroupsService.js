var services = services || angular.module('Busidex.services', ['ngResource']);
services
    .factory('Groups', function ($resource) {
        'use strict';

        return $resource(ROOT + '/Groups', {}, {
            get: { method: 'GET' },
            post: { method: 'POST', params: { userId: '@userId', groupTypeId: '@groupTypeId', id: '@id', cardIds: '@cardIds', description: '@description' }, isArray: false },
            update: { method: 'PUT', params: { userId: '@userId', groupTypeId: '@groupTypeId', id: '@id', cardIds: '@cardIds', description: '@description' }, isArray: false },
            remove: { method: 'DELETE', params: {id: '@id'}, cache: false }
        });
    });