var services = services || angular.module('Busidex.services', ['ngResource']);
services
    .factory('Activity', function($resource) {
        'use strict';

        return $resource(ROOT + '/Activity', {}, {
            query: { url: ROOT + '/Activity/GetEventSources', method: 'GET', cache: false },
            get: { url: ROOT + '/Activity/Get', params: { 'cardId': '@cardId', 'month': '@month' }, method: 'GET', cache: false },
            post: { method: 'POST', data: {}, isArray: false },
            update: { method: 'PUT' },
            remove: { method: 'DELETE' }
        });
    });