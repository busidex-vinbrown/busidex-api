var services = services || angular.module('Busidex.services', ['ngResource']);
services
    .factory('AccountType', function ($resource) {
        'use strict';

        return $resource(ROOT + "/AccountType", {}, {
            query: { method: 'GET', cache: false },
            get: { url: ROOT + "/AccountType/Get", method: 'GET', cache: false },
            post: { method: 'POST', data: {}, isArray: false },
            update: { method: 'PUT', params: { userAccountId: '@userAccountId', accountTypeId: '@accountTypeId' } },
            remove: { method: 'DELETE' }
        });
    });