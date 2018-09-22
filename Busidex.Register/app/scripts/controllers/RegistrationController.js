angular.module('busidexregister').controller('RegistrationController', [
    '$scope', '$route', '$http', 'Registration', 'Users', '$routeParams', '$location', 'Cache', 'CacheKeys',
    function ($scope, $route, $http, Registration, Users, $routeParams, $location, Cache, CacheKeys){
        'use strict';

        /*
    registration scenarios:
    1. http://local.busidex.com/#/account/register?token=<guid> send owner token
    2. http://local.busidex.com/#/account/register?sId=<guid> organization invitation
    3. http://local.busidex.com/#/account/register?promo_code=<code> invite with promo code (system tag)
    */
        var vm = this;

        vm.Waiting = true;

        var token = $route.current.params.token;
        var inviteCardToken = $route.current.params.sId;

        vm.ShowOwnerCard = false;
        vm.RegistrationErrors = [];
        var cachedModel = Cache.get(CacheKeys.Registration) || {};
        vm.Model = angular.fromJson(cachedModel);

        $http.defaults.useXDomain = true;

        $http.defaults.headers.common['Set-Cookie'] = ';domain=.busidx.com';
        $http.defaults.headers.common['X-Authorization-Token'] = '';

        vm.EmailOkToUse = '';
        vm.UserNameOkToUse = '';
	    console.log('test');
        // only load the model data once
        if (vm.Model.DisplayName === null || vm.Model.DisplayName === undefined || vm.Model.DisplayName.length === 0) {
            Registration.get({ id: token },
                function (model) {

                    model.ReferralTypes = {
                        Email: 'Email',
                        Shared: 'Shared',
                        Personal: 'Personal',
                        Other: 'Other'
                    };

                    model.DisplayName = '';
                    model.Email = '';
                    model.ConfirmEmail = '';
                    model.Password = '';
                    model.ConfirmPassword = '';
                    model.HumanQuestion = 'How many letters are there in the word Busidex? (Hint: use a number, don\'t spell out the word.)';
                    model.AccountTypeId = 6;
                    model.ShowOwnerCard = model.Card !== null;
                    model.HasBackImage = model.Card !== null && model.Card.BackFileId.toUpperCase() !== 'B66FF0EE-E67A-4BBC-AF3B-920CD0DE56C6';
                    model.CardOwnerToken = token;
                    model.PromoCode = $routeParams.promo_code;
                    model.PromoCodeValid = true;
                    model.IsMobile = false;
                    model.Agree = false;
                    model.InviteCardToken = inviteCardToken;
                    model.ReferredBy = (token && token.length > 0) ? model.ReferralTypes.Other : '';
                    model.ReferredByPerson = '';
                    model.ReferredByOther = (token && token.length > 0) ? 'Invited by Lizzabeth Brown' : '';

                    if (model.Card !== null) {
                        model.FrontOrientationClass = model.Card.FrontOrientation === 'H' ? 'h_preview' : 'v_preview';
                        model.BackOrientationClass = model.Card.BackOrientation === 'H' ? 'h_preview' : 'v_preview';

                        model.Card.CompanyName = encodeURIComponent(model.Card.CompanyName);
                        model.Card.Name = encodeURIComponent(model.Card.Name);
                        model.Card.Title = encodeURIComponent(model.Card.Title);
                    }
                    vm.Model = model;

                    vm.Waiting = false;
	                console.log('Owner Token:', model.CardOwnerToken);
                },
                function() {
                    vm.Waiting = false;
                });
        } else {
            vm.Waiting = false;
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

        vm.MoveTo = function (path) {
            Cache.put(CacheKeys.Registration, angular.toJson(vm.Model));
            $location.path(path);
        };

        vm.CheckPromoCode = function () {
            vm.Model.PromoCodeValid = false;
            };

        vm.CheckEmailAvailability = function () {

            if (vm.Model.Email === undefined || vm.Model.Email.length < 5) {
                return;
            }

            Users.get({ name: vm.Model.Email },
                function (data) {
                    vm.EmailOkToUse = data.User === null ? 'OK' : 'USED';
                },
                function () {
                    window.alert('There was a problem validating your username');
                });
        };

        vm.CheckUserNameAvailability = function () {

            if (vm.Model.DisplayName === undefined || vm.Model.DisplayName.length < 3) {
                return;
            }

            Users.get({ name: vm.Model.DisplayName },
                function (data) {
                    vm.UserNameOkToUse = data.User === null ? 'OK' : 'USED';
                },
                function () {
                    window.alert('There was a problem validating your username');
                });
        };

        vm.ClearConfrimEmail = function() {
            vm.Model.ConfirmEmail = '';
        };

        vm.Regsister = function () {

            vm.Waiting = true;

            formatReferredBy(vm.Model);

            vm.RegistrationErrors = [];

            $http.defaults.headers.post['Content-Type'] = '' + 'application/x-www-form-urlencoded; charset=UTF-8';
            $http.defaults.headers.post['Access-Control-Allow-Origin'] = '' + 'local.busidex.com';
            $http.defaults.transformRequest = function (model) {
                return 'model=' + JSON.stringify(model);
            };

            vm.Model.Password = encodeURIComponent(vm.Model.Password);
            vm.Model.ConfirmPassword = encodeURIComponent(vm.Model.ConfirmPassword);

            Registration.post(vm.Model,
                function (response) {
                    vm.Waiting = false;
                    Cache.nuke();
                    if (window.location.href.indexOf('local.') >= 0) {
                        window.location.href = 'http://local.start.busidex.com/#/front?token=' + response.Token;
                    } else {
                        window.location.href = 'https://start.busidex.com/#/front?token=' + response.Token;
                    }
                    
                },
                function (error) {
                    vm.Waiting = false;
                    vm.RegistrationErrors.push(error.data.Message);
                });
        };
    }
]); 