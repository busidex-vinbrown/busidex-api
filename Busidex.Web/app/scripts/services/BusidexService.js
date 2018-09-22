var services = services || angular.module('Busidex.services', ['ngResource']);
services
    .factory('Busidex', function ($resource) {
        'use strict';

        var vm = this;

        var _api = $resource(ROOT + '/Busidex', { id: '@id', isMobileView: '@isMobileView' }, {
            query: { url: ROOT + '/Busidex', method: 'GET', cache: false },
            get: { url: ROOT + '/Busidex', method: 'GET', cache: false },
            post: { url: ROOT + '/busidex', method: 'POST', params: { userId: '@userId', cId: '@cId' } },
            update: { url: ROOT + 'busidex/Put', method: 'PUT', data: { data: {} }, isArray: false },
            updateCardStatus: { url: ROOT + '/busidex/UpdateUserCardStatus', method: 'PUT' },
            remove: { method: 'DELETE' }
        });

        vm.api = _api;

        var _collection = [];
        vm.collection = _collection;

        return vm;
    });