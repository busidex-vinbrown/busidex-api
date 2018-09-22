'use strict';

 var ROOT = 'https://www.busidexapi.com/api';
//var ROOT = 'http://local.busidexapi.com/api';

var Camera = Camera || {
    PictureSourceType: {
        PHOTOLIBRARY: 0,
        CAMERA: 1,
        SAVEDPHOTOALBUM: 2
    },
    DestinationType: {
        DATA_URL: 0,
        FILE_URI: 1,
        NATIVE_URI: 2
    },
    EncodingType: {
        JPEG: 0,
        PNG: 1
    },
    MediaType: {
        PICTURE: 0,
        VIDEO: 1,
        ALLMEDIA: 2
    },
    Direction: {
        BACK: 0,
        FRONT: 1
    }
};

var services = angular.module('Busidex.services', ['ngResource']);

services.value('version', '0.3');
services
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
    .factory('Registration', function($resource, $http) {

        //$http.defaults.headers.post['Content-Type'] = ''
        //   + 'application/x-www-form-urlencoded; charset=UTF-8';

        //$http.defaults.transformRequest = function (model) {
        //    return "model=" + JSON.stringify(model);
        //};
        return $resource(ROOT + "/Registration/", {}, {
            query: { method: 'GET' },
            get: { method: 'GET' },
            post: { method: 'POST' },
            update: { method: 'PUT', params: { token: '@token' } },
            remove: { method: 'DELETE' }
        });
    })
    /*LOGIN SERVICE*/
    .factory('LoginSvc', function($resource, $http) {

        //$http.defaults.headers.post['Content-Type'] = ''
        //   + 'application/json';
        //$http.defaults.headers.post['Content-Type'] = ''
        //   + 'application/x-www-form-urlencoded; charset=UTF-8';
        return $resource(ROOT + "/Account/Login", {}, {
            query: { method: 'GET' },
            get: { method: 'GET' },
            post: { method: 'POST', data: {}, isArray: false },
            update: { method: 'PUT' },
            remove: { method: 'DELETE' }
        });
    })
    /*SUGGESTIONS SERVICE*/
    .factory('Suggestions', function($resource, $http) {

        $http.defaults.headers.post['Content-Type'] = ''
            + 'application/json';

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
            update: { method: 'PUT', data: { data: {} }, isArray: true },
            remove: { method: 'DELETE' }
        });
    })
    /*GROUP NOTES SERVICE*/
    .factory('GroupNotes', function($resource) {

        return $resource(ROOT + "/GroupNotes", { id: '@id', notes: '@notes' }, {
            get: { method: 'GET' },
            post: { method: 'POST' },
            update: { method: 'PUT', data: { data: {} }, isArray: true },
            remove: { method: 'DELETE' }
        });
    })
    /*BUSIDEX SERVICE*/
    .factory('Busidex', function($resource) {

        return $resource(ROOT + "/Busidex", { id: '@id' }, {
            query: { method: 'GET' },
            get: { method: 'GET' },
            post: { method: 'POST' },
            update: { method: 'PUT', data: { data: {} }, isArray: true },
            remove: { method: 'DELETE' }
        });
    })
    /*GROUPS SERVICE*/
    .factory('Groups', function($resource) {

        return $resource(ROOT + "/Groups", {}, {
            get: { method: 'GET' },
            post: { method: 'POST', params: { userId: '@userId', id: '@id', cardIds: '@cardIds', description: '@description' }, isArray: true },
            update: { method: 'PUT', params: { userId: '@userId', id: '@id', cardIds: '@cardIds', description: '@description' }, isArray: true },
            remove: { method: 'DELETE' }
        });
    })
    /*GROUPSDETAIL SERVICE*/
    .factory('Busigroup', function($resource) {

        return $resource(ROOT + "/GroupDetail/", { id: '@id' }, {
            query: { method: 'GET' },
            get: { method: 'GET' },
            post: { method: 'POST' },
            update: { method: 'PUT', data: { data: {} }, isArray: true },
            remove: { method: 'DELETE' }
        });
    })
    /*ACCOUNT SERVICE*/
    .factory('Account', function($resource) {

        return $resource(ROOT + "/Account/", {}, {
            query: { method: 'GET' },
            get: { method: 'GET' },
            post: { method: 'POST' },
            update: { method: 'PUT', data: { data: {} }, isArray: true },
            remove: { method: 'DELETE' }
        });
    })
    /*PASSWORD SERVICE*/
    .factory('Password', function($resource) {

        return $resource(ROOT + "/Password/", {}, {
            query: { method: 'GET' },
            get: { method: 'GET' },
            post: { method: 'POST' },
            update: { method: 'PUT', data: { data: {} }, isArray: true },
            remove: { method: 'DELETE' }
        });
    })
    /*SETTINGS SERVICE*/
    .factory('Settings', function($resource) {

        return $resource(ROOT + "/Settings/", {}, {
            query: { method: 'GET' },
            get: { method: 'GET', id: '@userId' },
            post: { method: 'POST' },
            update: { method: 'PUT', data: { data: {} }, isArray: true },
            remove: { method: 'DELETE' }
        });
    })
    /*CARD SERVICE*/
    .factory('Card', function($resource, $http) {

        $http.defaults.headers.post['Content-Type'] = ''
            + 'application/json';

        return $resource(ROOT + "/card", {}, {
            query: { method: 'GET' },
            get: { method: 'GET' },
            post: { method: 'POST' },
            update: { method: 'PUT' },
            remove: { method: 'DELETE' }
        });
    })
    /*SHARED CARD SERVICE*/
    .factory('SharedCard', function($resource, $http) {

        $http.defaults.headers.post['Content-Type'] = ''
            + 'application/json';

        return $resource(ROOT + "/SharedCard/:userId", {}, {
            query: { method: 'GET' },
            get: { method: 'GET' },
            post: { method: 'POST' },
            update: { method: 'PUT' },
            remove: { method: 'DELETE' }
        });
    })
    /*EVENT ACTIVITY SERVICE*/
    .factory('Activity', function ($resource, $http) {

        $http.defaults.headers.post['Content-Type'] = ''
            + 'application/json';

        return $resource(ROOT + "/Activity/:userId", {}, {
            query: { url: ROOT + '/Activity/GetEventSources', method: 'GET', cache: false },
            get: { method: 'GET', cache: false },
            post: { method: 'POST', data: {}, isArray: false },
            update: { method: 'PUT' },
            remove: { method: 'DELETE' }
        });
    })
    /*MOBILE UPLOAD*/
    .factory('MobileUpload', function($resource, $http) {

        $http.defaults.headers.post['Content-Type'] = ''
            + 'application/json';

        return $resource(ROOT + "/SharedCard/:userId", {}, {
            query: { method: 'GET' },
            get: { method: 'GET' },
            post: { method: 'POST' },
            update: { method: 'PUT' },
            remove: { method: 'DELETE' }
        });
    })
    /*SEARCH SERVICE*/
    .factory('SearchFilter', function() {
        this.SearchFilters = { Name: '', CompanyName: '', Email: '', Tags: '', PhoneNumbers: '' };
    })
    /*IMAGE PATHS SERVICE*/
    .factory("ApplicationImages", function() {
        return {
            GearIcon: 'images/gear.png',
            NotesIcon: 'images/notes.png',
            LoginIcon: 'images/login.png',
            CreateAccountIcon: 'images/createaccount.png',
            Check: 'images/checkmark.png'
        };
    })
    /*PHONEGAP SERVICE*/
    .factory('PhoneGap', [
        '$q',
        '$rootScope',
        '$document',
        function($q, $rootScope, $document) {
            var deferred = $q.defer();
            $document.bind('deviceready', function() {
                $rootScope.$apply(deferred.resolve);
            });
            return {
                ready: function() {
                    return deferred.promise;
                }
            };
        }
    ])
    /*CAMERA SERVICE*/
    .factory('Camera', [
        '$q',
        '$window',
        'PhoneGap',
        function($q, $window, PhoneGap) {
            return {
                getPicture: function(onSuccess, onError, options) {
                    //PhoneGap.ready().then(function() {
                        $window.navigator.camera.getPicture(onSuccess, onError, options);
                    //});
                },
                cleanup: function(onSuccess, onError) {
                    //PhoneGap.ready().then(function() {
                        $window.navigator.camera.cleanup(onSuccess, onError);
                    //});
                },
                PictureSourceType: Camera.PictureSourceType,
                DestinationType: Camera.DestinationType,
                EncodingType: Camera.EncodingType,
                MediaType: Camera.MediaType,
                Direction: Camera.Direction
            };
        }
    ]);
    