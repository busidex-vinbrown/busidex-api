var directives = directives || angular.module('busidexregister.directives', []);

directives
    .directive('matchEmail', function () {
        'use strict';
        return {
            require: 'ngModel',
            link: function (vm, elm, attrs, ctrl) {

                
                ctrl.$validators.matchEmail = function () {

                    if (vm.userDataForm.Email.$viewValue === undefined || vm.userDataForm.ConfirmEmail.$viewValue === undefined) {
                        return true;
                    }
                    var matching = vm.userDataForm.Email.$viewValue === vm.userDataForm.ConfirmEmail.$viewValue;
                    //vm.userDataForm.$setValidity(!vm.userDataForm.$invalid && matching);
                    return matching;
                };

                //vm.$watch(vm.userDataForm.Email.ngModel, function () {
                //    return vm.userDataForm.Email.$viewValue;
                //}, function () {
                //    ctrl.validate();
                //    //ctrl.$setValidity(isValid);
                //}, true);

                //vm.$watch(attrs.ngModel, function () {
                //    return vm.userDataForm.ConfirmEmail.$viewValue;
                //}, function () {
                //    ctrl.validate();
                //    //ctrl.$setValidity(isValid);
                //}, true);
            }
        };
    });