var services = services || angular.module('Busidex.services', ['ngResource']);
services
    .factory('Account', function ($resource) {
        'use strict';

        return $resource(ROOT + '/Account/', {}, {
            query: { method: 'GET' },
            get: { url: ROOT + '/Account/Get', method: 'GET', cache: false },
            post: { method: 'POST' },
            update: { url: ROOT + '/account/UpdateUserAccount', method: 'PUT', params: { 'user': '@user' }, isArray: false },
            checkUser: { url: ROOT + '/account/CheckDeleteParams', method: 'POST', isArray: false },
            remove: { method: 'DELETE', isArray: false },
            changeDisplayName: { url: ROOT + '/account/UpdateDisplayName', method: 'PUT', params: { 'name': '@name' }, isArray: false },
            getTerms: { url: ROOT + '/account/GetUserTerms', method: 'GET', isArray: true },
            acceptTerms: { url: ROOT + '/account/AcceptUserTerms', method: 'POST' }
        });
    });