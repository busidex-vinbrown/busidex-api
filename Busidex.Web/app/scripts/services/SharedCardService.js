var services = services || angular.module('Busidex.services', ['ngResource']);
services
    .factory('SharedCard', function ($resource) {
        'use strict';
        return $resource(ROOT + '/SharedCard/:userId', {}, {
            query: { method: 'GET', cache: false },
            getSample: { url: ROOT + '/SharedCard/GetSample', method: 'GET', cache: false },
            get: { method: 'GET', cache: false },
            post: { url: ROOT + '/SharedCard/Post', method: 'POST', isArray: false },
            sendTest: { url: ROOT + '/SharedCard/SendTestEmail', method: 'POST', isArray: false },
            update: { method: 'PUT' },
            remove: { method: 'DELETE' }
        });
    });