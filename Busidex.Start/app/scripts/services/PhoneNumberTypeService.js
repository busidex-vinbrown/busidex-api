
var services = services || angular.module('busidexstartApp.services', ['ngResource']);
services
    .factory('PhoneNumberTypes', function() {
        'use strict';
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
    });