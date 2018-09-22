angular.module('Busidex').controller('VanityUrlController', [
    '$location', 'ngDialog',
    function ($location, ngDialog) {
        'use strict';

        var vm = this;

        vm.showTerms = function () {
            
            ngDialog.open({
                template: '/views/fragments/eventterms.html',
                controller: ['$scope', function ($scope) {
                }]
            });
        }

        switch ($location.path()) {
            case '/lizzabethbrown':
                {
                    window.location.href = 'https://profile.busidex.com/details/Lizzabeth-Brown-Keller-Williams-Realty';
                    break;
                }
        }
    }
]);