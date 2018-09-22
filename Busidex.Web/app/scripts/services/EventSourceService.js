
var services = services || angular.module('Busidex.services', ['ngResource']);
services
    .factory('EventSource', function() {
        'use strict';

    return {
        SHARE: 1,
        ADD: 2,
        CALL: 3,
        MAP: 4,
        EMAIL: 5,
        SEARCH: 6,
        WEBSITE: 7,
        GROUP: 8,
        DETAILS: 9
    };
});