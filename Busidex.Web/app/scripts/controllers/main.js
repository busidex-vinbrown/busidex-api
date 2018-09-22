'use strict';

/**
 * @ngdoc function
 * @name busidexwebApp.controller:MainCtrl
 * @description
 * # MainCtrl
 * Controller of the busidexwebApp
 */
angular.module('busidexwebApp')
  .controller('MainCtrl', ['$anchorScroll'], function ($scope) {
    $scope.awesomeThings = [
      'HTML5 Boilerplate',
      'AngularJS',
      'Karma'
    ];
  });
