//'use strict'
var services = angular.module('Busidex.services', ['ngResource']);

//services.value('version', '0.3');
services    
    /*APPLICATION VERSION*/
    .factory('ApplicationVersion', function($resource) {

        return { Version: '1.0.0.36924' };
        //return $resource(ROOT + "/ApplicationVersion", {}, {
        //    get: { method: 'GET', data: {}, isArray: false },
        //    post: { method: 'POST' },
        //    update: { method: 'PUT' },
        //    remove: { method: 'DELETE' }
        //});
    })
    /*PHONE NUMBER TYPES SERVICE*/
    .factory('phoneNumberTypes', function() {
        return [
            {
                PhoneNumberTypeId: 1,
                Name: 'Business',
                Deleted: false
            }, {
                PhoneNumberTypeId: 2,
                Name: 'Home',
                Deleted: false
            }, {
                PhoneNumberTypeId: 3,
                Name: 'Mobile',
                Deleted: false
            }, {
                PhoneNumberTypeId: 4,
                Name: 'Fax',
                Deleted: false
            }, {
                PhoneNumberTypeId: 5,
                Name: 'Toll Free',
                Deleted: false
            }, {
                PhoneNumberTypeId: 6,
                Name: 'eFax',
                Deleted: false
            }, {
                PhoneNumberTypeId: 7,
                Name: 'Other',
                Deleted: false
            }, {
                PhoneNumberTypeId: 8,
                Name: 'Direct',
                Deleted: false
            }, {
                PhoneNumberTypeId: 9,
                Name: 'Voice Mail',
                Deleted: false
            }, {
                PhoneNumberTypeId: 10,
                Name: 'Business 2',
                Deleted: false
            }
        ];
    })
    /*STATE CODE SERVICE*/
    .factory('stateCodes', function() {
        return [
            {
                StateCodeId: 1,
                Code: 'AL',
                Name: 'Alabama'
            }, {
                StateCodeId: 2,
                Code: 'AK',
                Name: 'Alaska'
            }, {
                StateCodeId: 3,
                Code: 'AZ',
                Name: 'Arizona'
            }, {
                StateCodeId: 4,
                Code: 'AR',
                Name: 'Arkansas'
            }, {
                StateCodeId: 5,
                Code: 'CA',
                Name: 'California'
            }, {
                StateCodeId: 6,
                Code: 'CO',
                Name: 'Colorado'
            }, {
                StateCodeId: 7,
                Code: 'CT',
                Name: 'Connecticut'
            }, {
                StateCodeId: 8,
                Code: 'DE',
                Name: 'Delaware'
            }, {
                StateCodeId: 9,
                Code: 'DC',
                Name: 'District Of Columbia'
            }, {
                StateCodeId: 10,
                Code: 'FL',
                Name: 'Florida'
            }, {
                StateCodeId: 11,
                Code: 'GA',
                Name: 'Georgia'
            }, {
                StateCodeId: 12,
                Code: 'HI',
                Name: 'Hawaii'
            }, {
                StateCodeId: 13,
                Code: 'ID',
                Name: 'Idaho'
            }, {
                StateCodeId: 14,
                Code: 'IL',
                Name: 'Illinois'
            }, {
                StateCodeId: 15,
                Code: 'IN',
                Name: 'Indiana'
            }, {
                StateCodeId: 16,
                Code: 'IA',
                Name: 'Iowa'
            }, {
                StateCodeId: 17,
                Code: 'KS',
                Name: 'Kansas'
            }, {
                StateCodeId: 18,
                Code: 'KY',
                Name: 'Kentucky'
            }, {
                StateCodeId: 19,
                Code: 'LA',
                Name: 'Louisiana'
            }, {
                StateCodeId: 20,
                Code: 'ME',
                Name: 'Maine'
            }, {
                StateCodeId: 21,
                Code: 'MD',
                Name: 'Maryland'
            }, {
                StateCodeId: 22,
                Code: 'MA',
                Name: 'Massachusetts'
            }, {
                StateCodeId: 23,
                Code: 'MI',
                Name: 'Michigan'
            }, {
                StateCodeId: 24,
                Code: 'MN',
                Name: 'Minnesota'
            }, {
                StateCodeId: 25,
                Code: 'MS',
                Name: 'Mississippi'
            }, {
                StateCodeId: 26,
                Code: 'MO',
                Name: 'Missouri'
            }, {
                StateCodeId: 27,
                Code: 'MT',
                Name: 'Montana'
            }, {
                StateCodeId: 28,
                Code: 'NE',
                Name: 'Nebraska'
            }, {
                StateCodeId: 29,
                Code: 'NV',
                Name: 'Nevada'
            }, {
                StateCodeId: 30,
                Code: 'NH',
                Name: 'New Hampshire'
            }, {
                StateCodeId: 31,
                Code: 'NJ',
                Name: 'New Jersey'
            }, {
                StateCodeId: 32,
                Code: 'NM',
                Name: 'New Mexico'
            }, {
                StateCodeId: 33,
                Code: 'NY',
                Name: 'New York'
            }, {
                StateCodeId: 34,
                Code: 'NC',
                Name: 'North Carolina',
            }, {
                StateCodeId: 35,
                Code: 'ND',
                Name: 'North Dakota'
            }, {
                StateCodeId: 36,
                Code: 'OH',
                Name: 'Ohio'
            }, {
                StateCodeId: 37,
                Code: 'OK',
                Name: 'Oklahoma'
            }, {
                StateCodeId: 38,
                Code: 'OR',
                Name: 'Oregon'
            }, {
                StateCodeId: 39,
                Code: 'PA',
                Name: 'Pennsylvania'
            }, {
                StateCodeId: 40,
                Code: 'RI',
                Name: 'Rhode Island'
            }, {
                StateCodeId: 41,
                Code: 'SC',
                Name: 'South Carolina'
            }, {
                StateCodeId: 42,
                Code: 'SD',
                Name: 'South Dakota'
            }, {
                StateCodeId: 43,
                Code: 'TN',
                Name: 'Tennessee'
            }, {
                StateCodeId: 44,
                Code: 'TX',
                Name: 'Texas'
            }, {
                StateCodeId: 45,
                Code: 'UT',
                Name: 'Utah'
            }, {
                StateCodeId: 46,
                Code: 'VT',
                Name: 'Vermont'
            }, {
                StateCodeId: 47,
                Code: 'VA',
                Name: 'Virginia'
            }, {
                StateCodeId: 48,
                Code: 'WA',
                Name: 'Washington'
            }, {
                StateCodeId: 49,
                Code: 'WV',
                Name: 'West Virginia'
            }, {
                StateCodeId: 50,
                Code: 'WI',
                Name: 'Wisconsin'
            }, {
                StateCodeId: 51,
                Code: 'WY',
                Name: 'Wyoming'
            }
        ];
    })
    /*FILE SIZE INFO SERVICE*/
    .factory('fileSizeInfoContent', function() {
        return 'The maximum file size for a card is 150KB. If your image size is too large, you can use image editing software to reduce the ' +
            'file size (there are lots available, some of them free).';
    })
    /*REGISTRATION SERVICE*/
    .factory('Registration', function($resource) {
        return $resource(ROOT + "/Registration/", {}, {
            query: { method: 'GET' },
            get: { method: 'GET' },
            post: { url: ROOT + '/Registration/Post', method: 'POST' },
            update: { method: 'PUT', params: { token: '@token' } },
            remove: { method: 'DELETE' }
        });
    })
    /*LOGIN SERVICE*/
    .factory('Login', function($resource) {

        return $resource(ROOT + "/Account/Login", {}, {
            query: { method: 'GET' },
            get: { method: 'GET' },
            post: { method: 'POST', isArray: false },
            update: {url: ROOT + '/Account/LoginReset', method: 'PUT' },
            remove: { method: 'DELETE' }
        });
    })
    /*SUGGESTIONS SERVICE*/
    .factory('Suggestions', function($resource, $http) {

        $http.defaults.headers.post['Content-Type'] = '' + 'application/json';

        return $resource(ROOT + "/Suggestions", {}, {
            get: { method: 'GET' },
            post: { method: 'POST', data: { data: {} }, isArray: false },
            update: { method: 'PUT', params: { id: '@id' }, isArray: false },
            remove: { method: 'DELETE' }
        });
    })
    /*NOTES SERVICE*/
    .factory('Notes', function($resource) {

        return $resource(ROOT + "/Notes", { id: '@id', notes: '@notes' }, {
            get: { method: 'GET' },
            post: { method: 'POST' },
            update: { method: 'PUT', data: { data: {} }, isArray: false },
            remove: { method: 'DELETE' }
        });
    })
    /*GROUP NOTES SERVICE*/
    .factory('GroupNotes', function($resource) {

        return $resource(ROOT + "/GroupNotes", { id: '@id', notes: '@notes' }, {
            get: { method: 'GET' },
            post: { method: 'POST' },
            update: { method: 'PUT', data: { data: {} }, isArray: false },
            remove: { method: 'DELETE' }
        });
    })
    /*BUSIDEX SERVICE*/
    .factory('Busidex', function($resource) {

        return $resource(ROOT + "/Busidex", { id: '@id', isMobileView: '@isMobileView' }, {
            query: { method: 'GET' },
            get: { method: 'GET' },
            post: { url: ROOT + '/busidex', method: 'POST', params: {userId: '@userId', cId: '@cId'} },
            update: { url: ROOT + 'busidex/Put', method: 'PUT', data: { data: {} }, isArray: false },
            updateCardStatus: { url: ROOT + '/busidex/UpdateUserCardStatus', method: 'PUT' },
            remove: { method: 'DELETE' }
        });
    })
    /*MY BUSIDEX LIST*/
    .factory('MyBusidex', function() {
        return function(d) {
            var data = d || {};
            return data;
        };
    })
    /*GROUPS SERVICE*/
    .factory('Groups', function($resource) {

        return $resource(ROOT + "/Groups", {}, {
            get: { method: 'GET' },
            post: { method: 'POST', params: { userId: '@userId', groupTypeId: '@groupTypeId', id: '@id', cardIds: '@cardIds', description: '@description' }, isArray: false },
            update: { method: 'PUT', params: { userId: '@userId', groupTypeId: '@groupTypeId', id: '@id', cardIds: '@cardIds', description: '@description' }, isArray: false },
            remove: { method: 'DELETE' }
        });
    })
    /*GROUPSDETAIL SERVICE*/
    .factory('Busigroup', function($resource) {

        return $resource(ROOT + "/GroupDetail/", { id: '@id' }, {
            query: { method: 'GET' },
            get: { method: 'GET' },
            post: { method: 'POST' },
            update: { method: 'PUT', data: { data: {} }, isArray: false },
            remove: { method: 'DELETE' }
        });
    })
    /*ORGANIZATION SERVICE*/
    .factory('Organizations', function ($resource, $rootScope) {
        
        return $resource(ROOT + "/Organization", {}, {
            get: { url: ROOT + '/Organization/Get', method: 'GET', params: { organizationId: '@organizationId' }, isArray: false },
            select: { url: ROOT + '/Organization/Get', method: 'GET', params: { organizationId: '@organizationId' }, isArray: false },
            getMembers: { url: ROOT + '/Organization/GetMembers', method: 'GET', params: { organizationId: '@organizationId' }, isArray: false },
            getGuests: { url: ROOT + '/Organization/GetGuests', method: 'GET', params: { organizationId: '@organizationId' }, isArray: false },
            getReferrals: { url: ROOT + '/Organization/GetReferrals', method: 'GET', params: { organizationId: '@organizationId' }, isArray: false },
            post: { method: 'POST', isArray: false },
            addMember: { url: ROOT + '/Organization/AddOrganaizationCard', method: 'POST', params: { organizationId: '@organizationId', cardId: '@cardId' }, isArray: false },
            update: { method: 'PUT', isArray: false },            
            remove: { url: ROOT + '/Organization/DeleteOrganaizationCard', method: 'DELETE' }
            
        });
    })
    /*FILE UPLOAD SERVICE*/
    .factory('FileUpload', function ($http, $rootScope) {

        return {
            UploadFile: function(file, uploadUrl, callback) { //note the callback argument
                var token = $rootScope.User !== null ? $rootScope.User.Token : null;
                var fd = new FormData();
                fd.append('file', file);
                $http.post(uploadUrl, fd, {
                        transformRequest: angular.identity,
                        headers: { 'Content-Type': undefined, 'X-Authorization-Token': token }
                    })
                    .success(function() {
                        callback();
                    })
                    .error(function() {
                    });

            }
        };
    })
    /*ACCOUNT SERVICE*/
    .factory('Account', function($resource) {

        return $resource(ROOT + "/Account/", {}, {
            query: { method: 'GET' },
            get: { method: 'GET' },
            post: { method: 'POST' },
            update: { url: ROOT + '/account/UpdateUserAccount', method: 'PUT', params: { 'user': '@user' }, isArray: false },
            checkUser: {url: ROOT + '/account/CheckDeleteParams', method: 'POST', isArray: false },
            remove: { method: 'DELETE', isArray: false },
            getTerms: { url: ROOT + '/account/GetUserTerms', method: 'GET', isArray: true },
            acceptTerms: {url: ROOT + '/account/AcceptUserTerms',  method: 'POST' }
        });
    })
    /*PASSWORD SERVICE*/
    .factory('Password', function($resource) {

        return $resource(ROOT + "/Password/", {}, {
            query: { method: 'GET' },
            get: { method: 'GET' },
            post: { method: 'POST' },
            update: { method: 'PUT', data: { data: {} }, isArray: false },
            remove: { method: 'DELETE' }
        });
    })
    /*SETTINGS SERVICE*/
    .factory('Settings', function($resource) {

        return $resource(ROOT + "/Settings/", {}, {
            query: { method: 'GET' },
            get: { method: 'GET', id: '@userId' },
            post: { method: 'POST' },
            update: { method: 'PUT', data: { data: {} }, isArray: false },
            remove: { method: 'DELETE' }
        });
    })
    /*SEARCH SERVICE*/
    .factory('Search', function ($resource) {

        return $resource(ROOT + "/Search/DoSearch", {}, {
            query: { method: 'GET' },
            get: { method: 'GET' },
            post: { url: ROOT + '/search/Search', method: 'POST', params: { userId: '@userId', model: '@model' } },
            searchByTag: { url: ROOT + '/search/SystemTagSearch', params: { systag: '@systag' }, method: 'POST' },
            searchByOrganization: { url: ROOT + '/search/OrganizationMemberSearch', params: { orgId: '@orgId' }, method: 'POST' },
            searchByGroupName: { url: ROOT + '/search/GroupCardSearch', params: { groupName: '@groupName' }, method: 'POST' },
            update: { method: 'PUT' },
            remove: { method: 'DELETE' }
        });
    })
    /*FEATURE SERVICE*/
    .factory('Feature', function ($resource, $http) {

        $http.defaults.headers.post['Content-Type'] = '' + 'application/json';

        return $resource(ROOT + "/card/GetFeaturedCard", {}, {
            query: { method: 'GET' },
            get: { method: 'GET' }
        });
    })
    /*CARD SERVICE*/
    .factory('Card', function($resource, $http) {

        $http.defaults.headers.post['Content-Type'] = '' + 'application/json';

        return $resource(ROOT + "/card", {}, {
            query: { url: ROOT + '/card/GetCardCount', method: 'GET' },
            get: { method: 'GET' },
            seoResults: { url: ROOT + '/card/GetSeoCardNames', method: 'GET' },
            post: { method: 'POST' },
            update: { method: 'PUT' },
            remove: { method: 'DELETE', params: { 'id': '@id'} }
        });
    })
    /*SHARED CARD SERVICE*/
    .factory('SharedCard', function($resource) {
        return $resource(ROOT + "/SharedCard/:userId", {}, {
            query: { method: 'GET', cache: false },
            getSample: {url: ROOT + '/SharedCard/GetSample',  method: 'GET', cache: false},
            get: { method: 'GET', cache: false },
            post: { url: ROOT + '/SharedCard/Post', method: 'POST', isArray: false },
            sendTest: { url: ROOT + '/SharedCard/SendTestEmail', method: 'POST', isArray: false },
            send30DaySharedCardReminders: { url: ROOT + '/SharedCard/Send30DaySharedCardReminder', method: 'GET', isArray: false },
            update: { method: 'PUT' },
            remove: { method: 'DELETE' }
        });
    })
    /*SHARED CARD EMAIL PREVIEW SERVICE*/
    .factory('SharedCardPreview', function ($resource) {

        return $resource(ROOT + "/SharedCard/Preview", {}, {
            query: { method: 'GET', cache: false },
            get: { method: 'GET', cache: false },
            post: { method: 'POST', data: {}, isArray: false },
            update: { method: 'PUT' },
            remove: { method: 'DELETE' }
        });
    })
    /*EVENT ACTIVITY SERVICE*/
    .factory('Activity', function ($resource, $http) {

        $http.defaults.headers.post['Content-Type'] = '' + 'application/json';

        return $resource(ROOT + "/Activity/:userId", {}, {
            query: {url: ROOT + '/Activity/GetEventSources', method: 'GET', cache: false },
            get: { method: 'GET', cache: false },
            post: { method: 'POST', data: {}, isArray: false },
            update: { method: 'PUT' },
            remove: { method: 'DELETE' }
        });
    })
    /*ACCOUNT TYPE SERVICE*/
    .factory('AccountType', function ($resource, $http) {

        $http.defaults.headers.post['Content-Type'] = '' + 'application/json';

        return $resource(ROOT + "/AccountType", {}, {
            query: { method: 'GET', cache: false },
            get: { method: 'GET', cache: false },
            post: { method: 'POST', data: {}, isArray: false },
            update: { method: 'PUT', params: { userAccountId: '@userAccountId', accountTypeId: '@accountTypeId' } },
            remove: { method: 'DELETE' }
        });
    })
    /*SEARCH FILTER SERVICE*/
    .factory('SearchFilter', function() {
        this.SearchFilters = { Name: '', CompanyName: '', Email: '', Tags: '', PhoneNumbers: '' };
    })
    /*USER*/
    .factory('Users', function ($resource) {

        return $resource(ROOT + "/User/", { }, {
            query: { method: 'GET', cache: false },
            get: { method: 'GET', cache: false },
            post: { url: ROOT + '/User/RecoverUserName/', method: 'POST', data: {}, isArray: false, params: { email: '@email' } },
            update: { url: ROOT + '/User/RecoverPassword/', method: 'PUT', cache: false, params: { email: '@email' } },
            remove: { method: 'DELETE' }
        });
    })
    /*USERNAME*/
    .factory('UserName', function ($resource) {

        return $resource(ROOT + "/User/", {}, {
            query: { method: 'GET', cache: false },
            get: { method: 'GET', cache: false },
            post: { method: 'POST' },
            update: { url: ROOT + '/User/ChangeUserName/', method: 'PUT', cache: false, params: { userId: '@userId', name: '@name' } },
            remove: { method: 'DELETE' }
        });
    })
    /*COMMUNICATIONS*/
    .factory('Communications', function ($resource) {
        return $resource(ROOT + "/Communications/", {}, {
            query: { method: 'GET', cache: false },
            get: { method: 'GET', cache: false, isArray: true },
            post: { method: 'POST', params: { UserId: '@UserId' }, cache: false },
            update: { method: 'PUT', cache: false },
            remove: { method: 'DELETE' }
        });
    })
    /*TAGS*/
    .factory('Tags', function ($resource) {
        return $resource(ROOT + "/Tags/", {}, {
            query: { method: 'GET', cache: false },
            get: {  url: ROOT + '/Tags/GetTags/',params: { type: '@type', tag: '@tag' }, method: 'GET', cache: false, isArray: true },
            post: { method: 'POST' },
            update: { method: 'PUT', cache: false },
            remove: { method: 'DELETE' }
        });
    })
    /*OUTLOOK CONTACTS SERVICE*/
    .factory('OutlookContacts', function ($resource, $http) {
        return $resource(ROOT + "/Contacts/Post", {}, {
            post: { method: 'POST', transformRequest: angular.identity, headers: { 'Content-Type': undefined }, data: {fd: '@fd'} }
        });
    })
    /*GOOGLESEARCH*/
    .factory('GoogleLogin', ['$http', '$rootScope', '$q', function ($http, $rootScope, $q) {
        var clientId = '904413034329-jqioibnppbiudk171imtqg4bmoqdjcq3.apps.googleusercontent.com',
            apiKey = 'jhLnUCxOhBuwcsQOEtGRA-7F',
            scopes = 'https://www.google.com/m8/feeds/contacts',     //'https://www.googleapis.com/auth/userinfo.email https://www.google.com/m8/feeds',
            domain = 'http://local.busidex.com',
            userEmail,
            deferred = $q.defer();

        this.login = function() {
            gapi.auth.authorize({ client_id: clientId, scope: scopes, immediate: false, hd: domain }, this.handleAuthResult);

            return deferred.promise;
        };

        this.handleClientLoad = function () {
            gapi.client.setApiKey(apiKey);
            gapi.auth.init(function () { });
            window.setTimeout(checkAuth, 1);
        };

        this.checkAuth = function () {
            gapi.auth.authorize({ client_id: clientId, scope: scopes, immediate: true, hd: domain }, this.handleAuthResult);
        };

        this.handleAuthResult = function (authResult) {
            if (authResult && !authResult.error) {
                var data = {};
                gapi.client.load('oauth2', 'v3', function () {
                    var request = gapi.client.oauth2.userinfo.get();
                    request.execute(function (resp) {
                        $rootScope.$apply(function () {
                            data.email = resp.email;
                            data.contacts = resp.contacts;
                        });
                    });
                });
                deferred.resolve(data);
            } else {
                deferred.reject('error');
            }
        };

        this.handleAuthClick = function (event) {
            gapi.auth.authorize({ client_id: clientId, scope: scopes, immediate: false, hd: domain }, this.handleAuthResult);
            return false;
        };

        return this;
    }])
    /*DEBOUNCE*/
    .factory('$debounce',['$rootScope', '$browser', '$q', '$exceptionHandler', function ($rootScope, $browser, $q, $exceptionHandler) {
            var deferreds = {},
                methods = {},
                uuid = 0;

            function debounce(fn, delay, invokeApply) {
                var deferred = $q.defer(),
                    promise = deferred.promise,
                    skipApply = (angular.isDefined(invokeApply) && !invokeApply),
                    timeoutId,
                    cleanup,
                    methodId,
                    bouncing = false;

                // check we dont have this method already registered
                angular.forEach(methods, function(value, key) {
                    if (angular.equals(methods[key].fn, fn)) {
                        bouncing = true;
                        methodId = key;
                    }
                });

                // not bouncing, then register new instance
                if (!bouncing) {
                    methodId = uuid++;
                    methods[methodId] = { fn: fn };
                } else {
                    // clear the old timeout
                    deferreds[methods[methodId].timeoutId].reject('bounced');
                    $browser.defer.cancel(methods[methodId].timeoutId);
                }

                var debounced = function() {
                    // actually executing? clean method bank
                    delete methods[methodId];

                    try {
                        deferred.resolve(fn());
                    } catch (e) {
                        deferred.reject(e);
                        $exceptionHandler(e);
                    }

                    if (!skipApply) $rootScope.$apply();
                };

                timeoutId = $browser.defer(debounced, delay);

                // track id with method
                methods[methodId].timeoutId = timeoutId;

                cleanup = function(reason) {
                    delete deferreds[promise.$$timeoutId];
                };

                promise.$$timeoutId = timeoutId;
                deferreds[timeoutId] = deferred;
                promise.then(cleanup, cleanup);

                return promise;
            }


            // similar to angular's $timeout cancel
            debounce.cancel = function(promise) {
                if (promise && promise.$$timeoutId in deferreds) {
                    deferreds[promise.$$timeoutId].reject('canceled');
                    return $browser.defer.cancel(promise.$$timeoutId);
                }
                return false;
            };

            return debounce;
        }
    ]);