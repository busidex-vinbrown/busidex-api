'use strict';
/* jshint -W100 */
angular.module('Busidex.directives', []).
    directive('appVersion', ['version', function(version) {
        return function(scope, elm) {
            elm.text(version);
        };
    }])
    .directive('login', function() {
        return {
            restrict: 'E',
            controller: 'LoginPartialViewCtrl',
            replace: true,
            transclude: true,
            template: '<li><a ng-model="LoginModel" href="{{LoginModel.LoginRoute}}">{{LoginModel.LoginText}}</a></li>'
        };
    })
    .directive('homeLink', function() {
        return {
            restrict: 'E',
            controller: 'LoginPartialViewCtrl',
            replace: true,
            transclude: true,
            template: '<a ng-model="LoginModel" ng-href="{{LoginModel.HomeLink}}" ng-click="$parent.SetCurrentMenuItem(\'Home\')">Home</a>'
        };
    })
    .directive('fileUpload', function() {
        return {
            controller: 'AddCardCtrl',
            //scope: true,        //create a new scope
            link: function(scope, el, attrs) {
                el.unbind('change').bind('change', function(event) {
                    scope.IsBound = null;
                    var files = event.target.files;
                    //iterate files since 'multiple' may be specified on the element
                    for (var i = 0; i < files.length; i++) {
                        //emit event upward
                        scope.$emit('fileSelected', { file: files[i], idx: attrs.idx });
                    }
                });
            }
        };
    })
    .directive('validEmailC', function () {
        return {
            require: 'ngModel',
            link: function (scope, elm, attrs, ctrl) {
                ctrl.$parsers.unshift(function (viewValue) {
                    var emailMatch = viewValue === scope.userDataForm.Email.$viewValue;
                    ctrl.$setValidity('isMatch', emailMatch);
                    return viewValue;
                });
            }
        };
    })
    .directive('validPasswordC', function() {
        return {
            require: 'ngModel',
            link: function(scope, elm, attrs, ctrl) {
                ctrl.$parsers.unshift(function(viewValue) {
                    var pswdMatch = scope.userDataForm.ConfirmPassword.$viewValue === scope.userDataForm.Password.$viewValue;
                    ctrl.$setValidity('isMatch', pswdMatch);
                    return viewValue;
                });
            }
        };
    })
    .directive('validForm', function () {
        return {
            require: 'ngModel',
            link: function (scope, elm, attrs, ctrl) {
                ctrl.$parsers.unshift(function (viewValue) {
                    var pwdcnt = 0;
                    var emailcnt = 0;
                    var namecnt = 0;
                    
                    //if (ctrl.$name == 'DisplayName') {
                    //    scope.UserNameOkToUse = '';
                    //}
                    if (ctrl.$name === 'Email') {
                        scope.EmailOkToUse = '';
                    }
                    var displayNameMessages = {};
                    var passwordMessages = {};
                    var emailMessages = {};

                    //var regex = new RegExp('^[A-z][A-z0-9]*$');
                    if (scope.userDataForm.DisplayName !== undefined) {
                        displayNameMessages = {
                            DisplayNameRequired: {
                                Error: (scope.userDataForm.DisplayName.$viewValue === undefined || scope.userDataForm.DisplayName.$viewValue.length === 0) && (++namecnt === 1),
                                Message: 'required'
                            },
                            //ValidUserName: {
                            //    Error: (scope.userDataForm.UserName.$viewValue !== undefined && !scope.userDataForm.UserName.$viewValue.match(regex)) && (++namecnt === 1),
                            //    Message: 'Must start with a letter, letters & numbers only.'
                            //},
                            //UserNameLength: {
                            //    Error: scope.userDataForm.UserName.$viewValue !== undefined && (scope.userDataForm.UserName.$viewValue.length < 5 || scope.userDataForm.UserName.$viewValue.length > 20) && (++namecnt == 1),
                            //    Message: 'Must be between 5 and 20 characters'
                            //},
                            ErrorCount: namecnt
                        };
                    }

                    if (scope.userDataForm.Password !== undefined) {
                        passwordMessages = {
                            PasswordRequired: {
                                Error: (scope.userDataForm.Password.$viewValue === undefined || scope.userDataForm.Password.$viewValue.length === 0) && (++pwdcnt === 1),
                                Message: 'password required'
                            },
                            //PasswordMatch: {
                            //    Error: scope.userDataForm.ConfirmPassword.$viewValue !== scope.userDataForm.Password.$viewValue && (++pwdcnt === 1),
                            //    Message: 'passwords do not match'
                            //},
                            //ConfirmPasswordRequired: {
                            //    Error: (scope.userDataForm.ConfirmPassword.$viewValue === undefined || scope.userDataForm.ConfirmPassword.$viewValue.length === 0) && (++pwdcnt === 1),
                            //    Message: 'Please confirm your password'
                            //},
                            PasswordLength: {
                                Error: (scope.userDataForm.Password.$viewValue !== undefined && (scope.userDataForm.Password.$viewValue.length < 8 || scope.userDataForm.Password.$viewValue.length > 20)) && (++pwdcnt === 1),
                                Message: 'password must be between 8 and 20 characters'
                            },
                            ErrorCount: pwdcnt
                        };
                    }

                    if (scope.userDataForm.Email !== undefined) {
                        emailMessages = {
                            EmailRequired: {
                                Error: (scope.userDataForm.Email.$viewValue === undefined || scope.userDataForm.Email.$viewValue.length === 0) && (++emailcnt === 1),
                                Message: 'required'
                            },
                            ValidEmail: {
                                Error: (scope.userDataForm.Email.$viewValue !== undefined && scope.userDataForm.Email.$viewValue.indexOf('@') < 0) && (++emailcnt === 1),
                                Message: 'invalid email'
                            },
                            //EmailMatch: {
                            //    Error: scope.userDataForm.ConfirmEmail !== undefined && scope.userDataForm.ConfirmEmail.$viewValue !== scope.userDataForm.Email.$viewValue && (++emailcnt === 1),
                            //    Message: 'emails do not match'
                            //},
                            //ConfirmEmailRequired: {
                            //    Error: (scope.userDataForm.ConfirmEmail !== undefined && (scope.userDataForm.ConfirmEmail.$viewValue === undefined || scope.userDataForm.ConfirmEmail.$viewValue.length === 0)) && (++emailcnt === 1),
                            //    Message: 'Please confirm your email'
                            //},
                            ErrorCount: emailcnt
                        };
                    }
                    scope.PasswordMessages = passwordMessages;
                    scope.EmailMessages = emailMessages;
                    scope.DisplayNameMessages = displayNameMessages;

                    return viewValue;
                });
            }
        };
    })
    .directive('humanQuestion', function() {
        return {
            require: 'ngModel',
            link: function(scope, elm, attrs, ctrl) {
                ctrl.$parsers.unshift(function(viewValue) {

                    var correctAnswer = viewValue === 7;
                    ctrl.$setValidity('isCorrect', correctAnswer);
                    return viewValue;
                });
            }
        };
    })
    .directive('ngEnter', function () {
        return function (scope, element, attrs) {
            element.bind('keydown keypress', function (event) {
                if(event.which === 13) {
                    scope.$apply(function (){
                        scope.$eval(attrs.ngEnter);
                    });

                    event.preventDefault();
                }
            });
        };
    })
    .directive('adminMenu', function () {
        return {
            require: 'ngModel',
            controller: 'AdminMenuCtrl',
            restrict: 'E',
            template: '<ul id="adminMenu"><li ng-repeat="item in MenuItems"><a class="btn btn-link" ng-href="{{item.Link}}">{{item.Name}}</a></li></ul>',
            replace: true,
            transclude: true
        };
    })
    .directive('sharedcardicon', function() {
        return {
            require: 'ngModel',
            controller: 'SharedCardCtrl',
            restrict: 'E',
            template: '<li ng-model="HasSharedCards" ng-show="$parent.HasSharedCards"><a id="notificationTrigger" href="" data-target="#notificationPopup" role="button" data-toggle="modal"><img id="NotificationIcon" src="https://www.busidex.com/img/notification1.gif" alt="Someone sent you some cards!" title="Someone sent you some cards!" /></a></li>',
            replace: true,
            transclude: true
        };
    })
    .directive('notes', ['Notes', function (Notes) {
        return {
            scope: {
                card: '='
            },
            link: function ($scope) {

                $scope.card.EditingNotes = false;
                $scope.NotesCache = $scope.card.Notes;

                //console.log($scope.card.CardId);
                $scope.EditNotes = function () {
                    
                    $scope.card.EditingNotes = true;
                };

                $scope.DiscardNotes = function() {
                    $scope.card.EditingNotes = false;
                    $scope.card.Notes = $scope.NotesCache;
                };

                $scope.SaveNotes = function (id, notes) {

                    Notes.update({ id: id, notes: window.escape(notes) },
                       function () {
                           $scope.card.EditingNotes = false;
                           $scope.NotesCache = $scope.card.Notes;
                       },
                       function () {

                       });                                       
                };
            },
            restrict: 'AE',
            template: '<div>' +
                '<textarea class="Notes" ucid="{{card.UserCardId}}" data="{{card.Notes}}" cardid="{{card.CardId}}" ng-focus="EditNotes()" ng-model="card.Notes" name="Notes"></textarea>' +
                '<i ng-show="card.EditingNotes" ng-click="DiscardNotes()" class="icon-remove" style="zoom:1.5;float:right;width:15px;margin-top:-5px;"></i>' +
                '<i ng-show="card.EditingNotes" ng-click="SaveNotes(card.UserCardId, card.Notes)" class="icon-check" style="zoom:1.5;float:right;width:15px;;margin-top:-5px;"></i>' +
                '</div>',
            replace: true
        };
    }])
    .directive('fileModel', ['$parse', function ($parse) {
        return {
            restrict: 'A',
            link: function(scope, element, attrs) {
                var model = $parse(attrs.fileModel);
                var modelSetter = model.assign;
            
                element.bind('change', function(){
                    scope.$apply(function(){
                        modelSetter(scope, element[0].files[0]);
                    });
                });
            }
        };
    }])
    .directive('popoverTemplatePopup', [ '$templateCache', '$compile', function ( $templateCache, $compile ) {
        return {
            restrict: 'EA',
            replace: true,
            scope: { title: '@', content: '@', placement: '@', animation: '&', isOpen: '&' },
            templateUrl: 'card/popover-template.html',
            link: function( scope, iElement ) {
 
                var content = angular.fromJson( scope.content ),
                    template = $templateCache.get( content.templateUrl ),
                    templateScope = scope,
                    scopeElements = document.getElementsByClassName( 'ng-scope' );
 
                angular.forEach( scopeElements, function( element ) { 
                    var aScope = angular.element( element ).scope();
                    if ( aScope.$id === content.scopeId ) {
                        templateScope = aScope;
                    }
                });
 
                iElement.find('div.popover-content').html( $compile( template )( templateScope ) );
            }
        };
    }])
    .directive('popoverTemplate', [ '$tooltip', function ( $tooltip ) {
    var tooltip = $tooltip( 'popoverTemplate', 'popover', 'click' );
 
    tooltip.compile = function() {
        return {
            'pre': function( scope, iElement, iAttrs ) {
                iAttrs.$set( 'popoverTemplate', { templateUrl: iAttrs.popoverTemplate, scopeId: scope.$id } );
            },
            'post': tooltip.link
        };
    };
 
    return tooltip;
}])
    .directive('mobileicon', function() {
        return {
            require: 'ngModel',
            controller: 'MyBusidexCtrl',
            restrict: 'E',
            template: '<img ng-click="toggleMobile(card)" ng-src="{{card.MobileView && \'../../img/mobile2_32x32.png\' || \'../../img/mobile2_32x32_off.png\'}}" title="Mobile View" alt="" />',
            replace: true,
            transclude: true
        };
    })
    .directive('whenScrolled', function ($window) {
        return  function(scope, elm) {
            var sib = elm[0].nextElementSibling;
                angular.element($window).bind('scroll', function() {
                    
                    var sTop = sib.scrollTop;
                    var oHeight = sib.offsetHeight;
                  
                    var sum = (sTop + oHeight);
                    var comp = $window.document.body.scrollTop;
                    
                    if (sum + 200 >= comp) {
                        scope.$apply(elm[0].click());
                    }
                });
            };
    })
    .directive('fileModel', ['$parse', function ($parse) {
        return {
            restrict: 'A',
            scope: { method: '&cb' },
            link: function (scope, element, attrs) {
                var model = $parse(attrs.fileModel);
               // var cb = attrs.cb;
                var modelSetter = model.assign;

                element.bind('change', function () {
                    scope.$apply(function () {
                        modelSetter(scope, element[0].files[0]);
                        var _cb = scope.method(); // eval('scope.' + cb);

                        //for (_cb in scope) {
                        //    if (_cb == cb) {
                        //        break;
                        //    }
                        //}
                        if (_cb !== null && _cb !== undefined) {
                            _cb(element[0].files[0]);
                        }
                    });
                    
                });
            }
        };
    }])
    .directive('passwordStrength', function () {
        return {
            restrict: 'A',
            require: 'ngModel',
            link: function (scope, element, attrs, ngModel) {
                var indicator = element.children();
                var dots = Array.prototype.slice.call(indicator.children());
                var weakest = dots.slice(-1)[0];
                var weak = dots.slice(-2);
                var strong = dots.slice(-3);
                var strongest = dots.slice(-4);

                element.after(indicator);

                element.bind('keyup', function () {
                    var matches = {
                            positive: {},
                            negative: {}
                        },
                        counts = {
                            positive: {},
                            negative: {}
                        },
                        tmp,
                        strength = 0;//,
                        //letters = 'abcdefghijklmnopqrstuvwxyz',
                        //numbers = '01234567890',
                        //symbols = '\\!@#$%&/()=?¿',
                        //strValue = '';

                    angular.forEach(dots, function (el) {
                        el.style.backgroundColor = '#ebeef1';
                    });

                    if (ngModel.$viewValue) {
                        // Increase strength level
                        matches.positive.lower = ngModel.$viewValue.match(/[a-z]/g);
                        matches.positive.upper = ngModel.$viewValue.match(/[A-Z]/g);
                        matches.positive.numbers = ngModel.$viewValue.match(/\d/g);
                        matches.positive.symbols = ngModel.$viewValue.match(/[$-/:-?{-~!^_`\[\]]/g);
                        matches.positive.middleNumber = ngModel.$viewValue.slice(1, -1).match(/\d/g);
                        matches.positive.middleSymbol = ngModel.$viewValue.slice(1, -1).match(/[$-/:-?{-~!^_`\[\]]/g);

                        counts.positive.lower = matches.positive.lower ? matches.positive.lower.length : 0;
                        counts.positive.upper = matches.positive.upper ? matches.positive.upper.length : 0;
                        counts.positive.numbers = matches.positive.numbers ? matches.positive.numbers.length : 0;
                        counts.positive.symbols = matches.positive.symbols ? matches.positive.symbols.length : 0;

                        counts.positive.numChars = ngModel.$viewValue.length;
                        tmp += (counts.positive.numChars >= 8) ? 1 : 0;

                        counts.positive.requirements = (tmp >= 3) ? tmp : 0;
                        counts.positive.middleNumber = matches.positive.middleNumber ? matches.positive.middleNumber.length : 0;
                        counts.positive.middleSymbol = matches.positive.middleSymbol ? matches.positive.middleSymbol.length : 0;

                        // Decrease strength level
                        matches.negative.consecLower = ngModel.$viewValue.match(/(?=([a-z]{2}))/g);
                        matches.negative.consecUpper = ngModel.$viewValue.match(/(?=([A-Z]{2}))/g);
                        matches.negative.consecNumbers = ngModel.$viewValue.match(/(?=(\d{2}))/g);
                        matches.negative.onlyNumbers = ngModel.$viewValue.match(/^[0-9]*$/g);
                        matches.negative.onlyLetters = ngModel.$viewValue.match(/^([a-z]|[A-Z])*$/g);

                        counts.negative.consecLower = matches.negative.consecLower ? matches.negative.consecLower.length : 0;
                        counts.negative.consecUpper = matches.negative.consecUpper ? matches.negative.consecUpper.length : 0;
                        counts.negative.consecNumbers = matches.negative.consecNumbers ? matches.negative.consecNumbers.length : 0;

                        // Calculations
                        strength += counts.positive.numChars * 4;
                        if (counts.positive.upper) {
                            strength += (counts.positive.numChars - counts.positive.upper) * 2;
                        }
                        if (counts.positive.lower) {
                            strength += (counts.positive.numChars - counts.positive.lower) * 2;
                        }
                        if (counts.positive.upper || counts.positive.lower) {
                            strength += counts.positive.numbers * 4;
                        }
                        strength += counts.positive.symbols * 6;
                        strength += (counts.positive.middleSymbol + counts.positive.middleNumber) * 2;
                        strength += counts.positive.requirements * 2;

                        strength -= counts.negative.consecLower * 2;
                        strength -= counts.negative.consecUpper * 2;
                        strength -= counts.negative.consecNumbers * 2;

                        if (matches.negative.onlyNumbers) {
                            strength -= counts.positive.numChars;
                        }
                        if (matches.negative.onlyLetters) {
                            strength -= counts.positive.numChars;
                        }

                        strength = Math.max(0, Math.min(100, Math.round(strength)));

                        if (strength > 85) {
                            angular.forEach(strongest, function (el) {
                                el.style.backgroundColor = '#008cdd';
                            });
                        } else if (strength > 65) {
                            angular.forEach(strong, function (el) {
                                el.style.backgroundColor = '#6ead09';
                            });
                        } else if (strength > 30) {
                            angular.forEach(weak, function (el) {
                                el.style.backgroundColor = '#e09115';
                            });
                        } else {
                            weakest.style.backgroundColor = '#e01414';
                        }
                    }
                });
            },
            template: '<span class="password-strength-indicator"><span></span><span></span><span></span><span></span></span>'
        };
    });

