var directives = directives || angular.module('busidexregister.directives', []);

directives
    .directive('validForm', function () {
        'use strict';
        return {
            require: 'ngModel',
            controller: 'RegistrationController as vm',
            transclude: false,
            link: function (vm, elm, attrs, ctrl) {
              
                ctrl.$parsers.unshift(function (viewValue) {
                    var pwdcnt = 0;
                    var emailcnt = 0;
                    var namecnt = 0;

                    if (ctrl.$name === 'Email') {
                        vm.EmailOkToUse = '';
                    }
                    var displayNameMessages = {};
                    var passwordMessages = {};
                    var emailMessages = {};

                    //if (vm.userDataForm.DisplayName !== undefined) {
                        displayNameMessages = {
                            DisplayNameRequired: {
                                Error: (vm.userDataForm.DisplayName.$viewValue === undefined || vm.userDataForm.DisplayName.$viewValue.length === 0) && (++namecnt === 1),
                                Message: 'required'
                            },
                            ErrorCount: namecnt
                        };
                    //}

                    //if (vm.userDataForm.Password !== undefined) {
                        passwordMessages = {
                            InvalidCharacters: {
                                Error: (vm.userDataForm.Password.$viewValue.indexOf('&') > 0 || vm.userDataForm.Password.$viewValue.indexOf('%') > 0) && (++pwdcnt === 1),
                                Message: 'only letters or numbers are allowed'
                            },
                            PasswordRequired: {
                                Error: (vm.userDataForm.Password.$viewValue === undefined || vm.userDataForm.Password.$viewValue.length === 0) && (++pwdcnt === 1),
                                Message: 'password required'
                            },
                            PasswordLength: {
                                Error: (vm.userDataForm.Password.$viewValue !== undefined && (vm.userDataForm.Password.$viewValue.length < 8 || vm.userDataForm.Password.$viewValue.length > 20)) && (++pwdcnt === 1),
                                Message: 'password must be between 8 and 20 characters'
                            },
                            ErrorCount: pwdcnt
                        };
                    //}

                    //if (vm.userDataForm.Email !== undefined) {
                        emailMessages = {
                            EmailRequired: {
                                Error: (vm.userDataForm.Email.$viewValue === undefined || vm.userDataForm.Email.$viewValue.length === 0) && (++emailcnt === 1),
                                Message: 'required'
                            },
                            ValidEmail: {
                                Error: (vm.userDataForm.Email.$viewValue !== undefined && vm.userDataForm.Email.$viewValue.indexOf('@') < 0) && (++emailcnt === 1),
                                Message: 'invalid email'
                            },
                            EmailMatch: {
                                Error: vm.userDataForm.ConfirmEmail !== undefined && vm.userDataForm.ConfirmEmail.$viewValue !== vm.userDataForm.Email.$viewValue && (++emailcnt === 1),
                                Message: 'emails do not match'
                            },
                            ConfirmEmailRequired: {
                                Error: (vm.userDataForm.ConfirmEmail !== undefined && (vm.userDataForm.ConfirmEmail.$viewValue === undefined || vm.userDataForm.ConfirmEmail.$viewValue.length === 0)) && (++emailcnt === 1),
                                Message: 'Please confirm your email'
                            },
                            ErrorCount: emailcnt
                        };
                    //}
                    vm.PasswordMessages = passwordMessages;
                    vm.EmailMessages = emailMessages;
                    vm.DisplayNameMessages = displayNameMessages;
                   
                    return viewValue;
                });
            }
        };
    });