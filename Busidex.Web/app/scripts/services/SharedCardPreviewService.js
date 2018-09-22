var services = services || angular.module('Busidex.services', ['ngResource']);
services
    .factory('SharedCardPreview', function ($resource) {
        'use strict';
        return $resource(ROOT + '/SharedCard/Preview', {
            query: { method: 'GET', cache: false },
            get: { method: 'GET', cache: false },
            post: { method: 'POST', data: {}, isArray: false },
            update: { method: 'PUT' },
            remove: { method: 'DELETE' }
        });
    });