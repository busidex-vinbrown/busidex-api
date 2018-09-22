/*REGISTRATION*/
function RegistrationViewCtrl($scope, $http, $rootScope, $cookieStore, Registration, $route, $routeParams, Users, $location, Analytics) {

    /*
    registration scenarios:
    1. http://local.busidex.com/#/account/register?token=<guid> send owner token
    2. http://local.busidex.com/#/account/register?sId=<guid> organization invitation
    3. http://local.busidex.com/#/account/register?promo_code=<code> invite with promo code (system tag)
    */

    Analytics.trackPage($location.path());
    $scope.Waiting = true;
    $rootScope.IsLoggedIn = $rootScope.User !== null;

    var token = $route.current.params.token;
    var inviteCardToken = $route.current.params.sId;

    $scope.ShowOwnerCard = false;
    $scope.CurrentStep = 1;
    $scope.RegistrationErrors = [];
    $scope.Model = {};

    $http.defaults.useXDomain = true;

    $scope.UserNameOkToUse = '';
    $scope.EmailOkToUse = '';

    Registration.get({ id: token },
        function (model) {
            model.UserName = '';
            model.Email = '';
            model.ConfirmEmail = '';
            model.Password = '';
            model.ConfirmPassword = '';
            model.HumanQuestion = "How many letters are there in the word Busidex? (Hint: use a number, don't spell out the word.)";
            model.AccountTypeId = 6;
            model.ShowOwnerCard = model.Card !== null;
            model.HasBackImage = model.Card !== null && model.Card.BackFileId.toUpperCase() !== 'B66FF0EE-E67A-4BBC-AF3B-920CD0DE56C6';
            model.CardOwnerToken = token;
            model.PromoCode = $routeParams.promo_code;
            model.PromoCodeValid = true;
            model.IsMobile = false;
            model.InviteCardToken = inviteCardToken;
            model.ReferredBy = '';
            model.ReferredByPerson = '';
            model.ReferredByOther = '';

            model.ReferralTypes = {
                Email: 'Email',
                Shared: 'Shared',
                Personal: 'Personal',
                Other: 'Other'
            };

            if (model.Card !== null) {
                model.FrontOrientationClass = model.Card.FrontOrientation == "H" ? "h_preview" : "v_preview";
                model.BackOrientationClass = model.Card.BackOrientation == "H" ? "h_preview" : "v_preview";

                model.Card.CompanyName = encodeURIComponent(model.Card.CompanyName);
                model.Card.Name = encodeURIComponent(model.Card.Name);
                model.Card.Title = encodeURIComponent(model.Card.Title);
            }
            $scope.Model = model;

            $scope.Waiting = false;
        },
        function (error) {
            $scope.Waiting = false;
        });

    function checkReferredTypeValid(model) {

        var retVal = '';
        model.MissingPersonalReference = false;
        model.MissingOtherReference = false;

        switch (model.ReferredBy) {
            case model.ReferralTypes.Email: // all good
            case model.ReferralTypes.Shared: // all good
            case model.ReferralTypes.Personal:
                {
                    if (model.ReferredByPerson.trim() === '') {
                        model.MissingPersonalReference = true;
                        retVal = 'Please fill in who referred you to us';
                    }
                    break;
                }
            case model.ReferralTypes.Other:
                {
                    if (model.ReferredByOther.trim() === '') {
                        model.MissingOtherReference = true;
                        retVal = 'Please fill in who referred you to us';
                    }
                    break;
                }
            default:
                {
                    retVal = 'Please indicate how you heard about us';
                    break;
                }
        }
        return retVal;
    }

    function formatReferredBy(model) {
        switch (model.ReferredBy) {
            case model.ReferralTypes.Email: // all good
            case model.ReferralTypes.Shared: // all good
            case model.ReferralTypes.Personal:
                {
                    model.ReferredBy += ': ' + model.ReferredByPerson;
                    break;
                }
            case model.ReferralTypes.Other:
                {
                    model.ReferredBy += ': ' + model.ReferredByOther;
                }
        }
    }

    $scope.CheckPromoCode = function () {


        $scope.Model.PromoCodeValid = false;

    };

    $scope.CheckUserNameAvailability = function () {

        if ($scope.Model.UserName === undefined || $scope.Model.UserName.length < 5) {
            return;
        }

        Users.get({ name: $scope.Model.UserName },
            function (data) {
                $scope.UserNameOkToUse = data.User === null ? 'OK' : 'USED';
            },
            function () {
                alert('There was a problem validating your username');
            });
    };

    $scope.CheckEmailAvailability = function () {

        if ($scope.Model.Email === undefined || $scope.Model.Email.length < 5) {
            return;
        }

        Users.get({ name: $scope.Model.Email },
            function (data) {
                $scope.EmailOkToUse = data.User === null ? 'OK' : 'USED';
            },
            function () {
                alert('There was a problem validating your username');
            });
    };

    $scope.Regsister = function () {

        var referredTypeError = checkReferredTypeValid($scope.Model);
        if (referredTypeError !== '') {
            alert(referredTypeError);
            return;
        }

        formatReferredBy($scope.Model);

        $scope.RegistrationErrors = [];

        $http.defaults.headers.post['Content-Type'] = '' + 'application/x-www-form-urlencoded; charset=UTF-8';
        $http.defaults.transformRequest = function (model) {
            return "model=" + JSON.stringify(model);
        };


        Registration.post($scope.Model,
            function () {
                $scope.SetStep(4);
            },
            function (error) {
                $scope.RegistrationErrors.push(error.data.Message);
            });
    };

    _gaq.push(['_trackEvent', 'Registration', 'Step', 'Page Load']);

    $scope.SetStep = function (step) {
        $scope.CurrentStep = step;
        _gaq.push(['_trackEvent', 'Registration', 'Step', step.toString()]);
    };

    //INSPECTLET CODE
    if (window.location.href.indexOf('local.') < 0) {
        window.__insp = window.__insp || [];
        __insp.push(['wid', 128335322]);
        (function () {
            function __ldinsp() {
                var insp = document.createElement('script');
                insp.type = 'text/javascript';
                insp.async = true;
                insp.id = "inspsync";
                insp.src = ('https:' == document.location.protocol ? 'https' : 'http') + '://cdn.inspectlet.com/inspectlet.js';
                var x = document.getElementsByTagName('script')[0];
                x.parentNode.insertBefore(insp, x);
            }

            if (window.attachEvent) {
                window.attachEvent('onload', __ldinsp);
            } else {
                window.addEventListener('load', __ldinsp, false);
            }
        })();
    }
    // END INSPECTLET CODE
}
RegistrationViewCtrl.$inject = ['$scope', '$http', '$rootScope', '$cookieStore', 'Registration', '$route', '$routeParams', 'Users', '$location', 'Analytics'];
