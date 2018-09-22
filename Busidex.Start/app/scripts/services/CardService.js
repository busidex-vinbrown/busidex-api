
var services = services || angular.module('busidexstartApp.services', ['ngResource']);
services
    .factory('Card', function($resource, $http) {
        'use strict';
        $http.defaults.headers.post['Content-Type'] = '' + 'application/json';

        return $resource(ROOT + '/card', {}, {
            query: { url: ROOT + '/card/GetCardCount', method: 'GET' },
            get: { method: 'GET', url: ROOT + '/card/Get', cache: false, params: {'id': '@id', 'userId': '@userId'} },
            seoResults: { url: ROOT + '/card/GetSeoCardNames', method: 'GET' },
            post: { method: 'POST', url: ROOT + '/card', cache: false, params: { 'idx': '@idx', 'imageUrl': '@imageUrl', 'orientation': '@orientation' } },
            update: { method: 'PUT' },
            saveCardImage: { method: 'PUT', url: ROOT + '/card/SaveCardImage', cache: false, params: { 'idx': '@idx', 'imageUrl': '@imageUrl', 'orientation': '@orientation' } },
            saveVisibility: { url: ROOT + '/card/SaveCardVisibility', params: { 'visibility': '@visibility' }, method: 'PUT' },
            saveCardInfo: { url: ROOT + '/card/SaveContactInfo', method: 'PUT' },
            remove: { method: 'DELETE', params: { 'id': '@id' } }
        });
    });