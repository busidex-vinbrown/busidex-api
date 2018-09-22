
var services = services || angular.module('Busidex.services', ['ngResource']);
services
    .factory('Search', function($resource) {
        'use strict';
        var vm = this;
        vm.cache = null;

        var _api = $resource(ROOT + '/Search/DoSearch', {}, {
            query: { method: 'GET' },
            get: { method: 'GET' },
            post: { url: ROOT + '/search/Search', method: 'POST', params: { userId: '@userId', model: '@model' } },
            searchByTag: { url: ROOT + '/search/SystemTagSearch', params: { systag: '@systag' }, method: 'POST' },
            searchByOrganization: { url: ROOT + '/search/OrganizationMemberSearch', params: { orgId: '@orgId' }, method: 'POST' },
            searchByGroupName: { url: ROOT + '/search/GroupCardSearch', params: { groupName: '@groupName' }, method: 'POST' },
            update: { method: 'PUT' },
            remove: { method: 'DELETE' }
        });

        vm.query = function() {
            var result = _api.get();
            vm.cache = result;
            return result;
        };
        vm.api = _api;
        return vm;
    });