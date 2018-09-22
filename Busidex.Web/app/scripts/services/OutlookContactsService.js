var services = services || angular.module('Busidex.services', ['ngResource']);
services
    .factory('OutlookContacts', function ($resource) {
        'use strict';
        return $resource(ROOT + '/Contacts/Post', {}, {
            post: { method: 'POST', transformRequest: angular.identity, headers: { 'Content-Type': undefined }, data: { fd: '@fd' } }
        });
    });