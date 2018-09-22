'use strict';
/*jslint unused: false */
describe('Controller: HomeViewCtrl', function () {
    beforeEach(module('Busidex'));
   
    var HomeViewCtrl, scope;

    // Initialize the controller and a mock scope
    beforeEach(inject(function ($rootScope, $controller, $cookieStore, $http, $location, Feature, Busidex) {

        $rootScope.SetCurrentMenuItem = function () { return true; };
        scope = $rootScope.$new();
        HomeViewCtrl = $controller('HomeViewCtrl', {
            $scope: scope
        });
    }));

    it('should attach a list of awesomeThings to the scope', function () {
        expect(scope.ExistsInMyBusidex).toBe(false);
    });
});
