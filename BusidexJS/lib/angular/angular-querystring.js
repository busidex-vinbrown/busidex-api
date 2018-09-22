/**
 * Convert url parameters to JavaScript Object for
 * easy data extraction
 *
 * created by Nir Kaufman
 * il.linkedin.com/in/nirkaufman/
 *
 */
var qs = angular.module('Qs', []);

/**
 * This service will return the Query object that
 * contain all the url parameters passed.
 * it depends on the global angular $window object
 * that build in the library
 */
qs.factory('Query', function ($window) {

    var query_string = {};
    var query = $window.location.search.substring(1);
    var vars = query.split("&");

    for (var i = 0; i < vars.length; i++) {

        var pair = vars[i].split("=");

        if (typeof query_string[pair[0]] === "undefined") {
            query_string[pair[0]] = pair[1];
        } else if (typeof query_string[pair[0]] === "string") {
            query_string[pair[0]] = [ query_string[pair[0]], pair[1] ];
        } else {
            query_string[pair[0]].push(pair[1]);
        }
    }
    return query_string;
})