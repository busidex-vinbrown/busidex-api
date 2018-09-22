var services = services || angular.module('Busidex.services', ['ngResource']);
services
    .factory('Users', function ($resource) {
        'use strict';
        return $resource(ROOT + '/User/', {}, {
            query: { method: 'GET', cache: false },
            get: { method: 'GET', cache: false },
            post: { url: ROOT + '/User/RecoverUserName/', method: 'POST', data: {}, isArray: false, params: { email: '@email' } },
            update: { url: ROOT + '/User/RecoverPassword/', method: 'PUT', cache: false, params: { email: '@email' } },
            remove: { method: 'DELETE' },
            fromCache: {
                
            }
        });
    });