

// A custom cache backed by:
// 1. cookies
// 2. local storage
// 3. in-memory hash table
var services = services || angular.module('busidexregister.services', ['$rootScope', '$localStorage', '$cookies', '$http', 'CacheKeys']);
services
    .service('Cache', function ($rootScope, $localStorage, $cookies, $http, CacheKeys) {
        'use strict';
        var cache = {};

        var cookieOptions;
        if (window.location.href.indexOf('local') < 0) {
            cookieOptions = { domain: 'busidex.com', path: 'busidex.com', secure: true };
        } else {
            cookieOptions = {};
        }

        this.put = function(key, value) {
            var oldValue = this.get(key);
            cache[key] = value;
            $localStorage[key] = value;
            if (key !== CacheKeys.Card) {
                try {
                    $cookies.put(key, value, cookieOptions); // might be too big for a cookie
                } catch (e) {
                    console.log('could not store cookie: ' + key + '. reason: ' + e );
                }
            }
            $rootScope.$broadcast(
                'cache.item.updated',
                { key: key, newValue: value, oldValue: oldValue });
        };

        var _remove = function(key) {

            var value = _get(key);

            cache[key] = null;
            delete cache[key];
            $cookies.remove(key);

            $rootScope.$broadcast(
                'cache.item.removed', { key: key, value: value });
        };

        this.remove = _remove;

        this.nuke = function () {

            $http.defaults.headers.common['X-Authorization-Token'] = null;

            var cookies = $cookies.getAll();
            
            for (var cookie in cookies) {
                $cookies.remove(cookie, cookieOptions);
            }
            cache = {};
            $localStorage.$reset();
        };

        var _get = function(key) {
            return $cookies.get(key) || $localStorage[key] || cache[key] || null;
        };
        this.get = _get;

    }).service('CacheKeys', function() {
        'use strict';
        return {
            Card: 'card',
            User: 'user',
            LoginRoute: 'login-route',
            LoginText: 'login-text',
            Registration: 'registration'
        };
    });