
var services = services || angular.module('busidexstartApp.services', ['ngResource']);
services
    .factory('Activity', function($resource, $http) {
        'use strict';
        $http.defaults.headers.post['Content-Type'] = '' + 'application/json';

        return $resource(ROOT + '/Activity/:userId', {}, {
            query: { url: ROOT + '/Activity/GetEventSources', method: 'GET', cache: false },
            get: { method: 'GET', cache: false },
            post: { method: 'POST', data: {}, isArray: false },
            update: { method: 'PUT' },
            remove: { method: 'DELETE' }
        });
    });