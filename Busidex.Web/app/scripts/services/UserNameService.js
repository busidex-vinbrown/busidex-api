var services = services || angular.module('Busidex.services', ['ngResource']);
services
    .factory('UserName', function ($resource) {
        'use strict';

        return $resource(ROOT + "/User/", {}, {
            query: { method: 'GET', cache: false },
            get: { method: 'GET', cache: false },
            post: { method: 'POST' },
            update: { url: ROOT + '/User/ChangeUserName/', method: 'PUT', cache: false, params: { userId: '@userId', name: '@name' } },
            remove: { method: 'DELETE' }
        });
    });