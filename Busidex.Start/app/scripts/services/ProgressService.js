
var services = services || angular.module('busidexstartApp.services', ['$location']);
services
    .service('Progress', function($location) {
        'use strict';

        var MAX = 7;
        var step;
        var pct;

        function getPercent(step) {
            if (step === MAX) {
                 return 100;
            }
            return Math.round((step / MAX) * 100, 0);
        }

        var _get = function() {
            switch ($location.path()) {
            case '/':
            {                
                step = 1;
                pct = getPercent(step);
                break;
            }
            case '/front':
            {
                pct = getPercent(1);
                step = 1;
                pct = getPercent(step);
                break;
            }
            case '/back':
            {
                pct = getPercent(2);
                step = 2;
                pct = getPercent(step);
                break;
            }
            case '/visibility':
            {
                step = 3;
                pct = getPercent(step);
                break;
            }
            case '/contactinfo':
            {
                step = 4;
                pct = getPercent(step);
                break;
            }
            case '/searchinfo':
            {
                step = 5;
                pct = getPercent(step);
                break;
            }
            case '/tags':
            {
                step = 6;
                pct = getPercent(step);

                break;
            }
            case '/addressinfo':
            {
                step = 7;
                pct = getPercent(step);
                break;
            }
            default:
            {
                pct = 0;
                step = 0;
            }
            }
            return pct;
        };
        var _msg = function() {
            return step + ' / ' + MAX;
        };
        this.message = _msg;
        this.get = _get;
    });