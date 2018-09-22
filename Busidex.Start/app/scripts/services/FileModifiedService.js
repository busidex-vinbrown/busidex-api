

var services = services || angular.module('busidexstartApp.services');
services
    .factory('FileModifiedService', function ($uibModal) {
        'use strict';

        var _modified = false;
        var modalDefaults = {
            backdrop: 'static',
            keyboard: true,
            modalFade: true,
            size: 'lg',
            templateUrl: '/views/fragments/file_modified_warning.html'
        };

       return {
            setModified: function(val) {
                _modified = val;
            },
            isModified: function() {
                return _modified;
            },
            showWarning: function (newUrl) {
                
                return _show(newUrl);
            }
        };

        function _show(newUrl) {
           
            if (!modalDefaults.controller) {
                modalDefaults.controller = ['$scope', '$uibModalInstance', '$location', function ($scope, $uibModalInstance, $location) {
                   
                    $scope.cancel = function () {
                        _modified = true;
                        $uibModalInstance.dismiss('cancel');
                    };
                    $scope.continue = function () {
                        _modified = false;
                        $uibModalInstance.close();
                        $location.path(newUrl);
                    };
                }];
            }

            return $uibModal.open(modalDefaults).result;
        }
    });