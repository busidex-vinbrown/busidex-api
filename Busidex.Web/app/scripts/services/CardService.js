var services = services || angular.module('Busidex.services', ['ngResource']);
services
    .factory('Card', function ($resource) {
        'use strict';
        return $resource(ROOT + '/card', {
            query: { url: ROOT + '/card/GetCardCount', method: 'GET' },
            get: { url: ROOT + '/card', method: 'GET', cache: false },
            seoResults: { url: ROOT + '/card/GetSeoCardNames', method: 'GET' },
            post: { method: 'POST' },
            update: { method: 'PUT' },
            remove: { method: 'DELETE', params: { 'id': '@id' } }
        });
    });