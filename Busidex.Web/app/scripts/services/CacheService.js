
// A custom cache backed by:
// 1. cookies
// 2. local storage
// 3. in-memory hash table
var services = services || angular.module('Busidex.services', ['$localStorage', '$cookies', '$http']);
services
    .service('Cache', function ($localStorage, $cookies, $http) {
        'use strict';
        var cache = {};

        var cookieOptions;
        if (window.location.href.indexOf('local') < 0) {
            cookieOptions = { domain: 'busidex.com', path: 'busidex.com', secure: true };
        } else {
            cookieOptions = {};
        }

        this.put = function(key, value) {
            //var oldValue = this.get(key);
            cache[key] = value;
            $localStorage[key] = value;
            try {
                $cookies.put(key, value, cookieOptions); // might be too big for a cookie
            } catch (e) {

            }
            //$rootScope.$broadcast(
            //    'cache.item.updated',
            //    { key: key, newValue: value, oldValue: oldValue });
        };

        var _remove = function(key) {

            //var value = _get(key);

            cache[key] = null;
            delete cache[key];
            $cookies.remove(key);

            //$rootScope.$broadcast(
            //    'cache.item.removed', { key: key, value: value });
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
            return cache[key] || $cookies.get(key) || $localStorage[key] || null;
        };
        this.get = _get;

    }).service('CacheKeys', function() {
        'use strict';
        return {
            Card: 'card',
            User: 'user',
            LoginRoute: 'login-route',
            LoginText: 'login-text',
            SharedCards: 'shared-cards',
            Notifications: 'notifications'
        };
    });