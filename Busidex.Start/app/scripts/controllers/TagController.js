angular.module('busidexstartApp').controller('TagController', [
    '$scope', '$http', 'Cache', 'CacheKeys', '$location', 'Card', 'Analytics', '$routeParams', 'FileModifiedService',
    function ($scope, $http, Cache, CacheKeys, $location, Card, Analytics, $routeParams, FileModifiedService) {
        'use strict';

        Analytics.trackPage($location.path());

        var vm = this;
        vm.Tags = [];
        vm.TagTypes = [];
        vm.TagTypes.push('User');
        vm.TagTypes.push('System');

        vm.model = {};
        vm.ModelError = false;
        vm.loading = true;
        vm.modified = false;
        vm.editmode = $routeParams.m === 'edit';

        var card = Cache.get(CacheKeys.Card);
        var user = Cache.get(CacheKeys.User);
        user = angular.fromJson(user);
        card = angular.fromJson(card);

        $http.defaults.headers.common['X-Authorization-Token'] = user.Token;

        var data = {
            'id': user.CardId,
            'userId': 0
        };
        Card.get(data,
            function (model) {
                vm.loading = false;
                card = model.Model;
                vm.card = card;
                vm.model = model;
                if (vm.card.Tags === null) {
                    vm.card.Tags = [];
                } else {
                    for (var i = 0; i < vm.card.Tags.length; i++) {
                        vm.card.Tags[i].TagTypeId = vm.card.Tags[i].TagType;
                    }
                }

                $scope.$watch(function () {
                    return vm.card.Tags;
                }, function (newTag, oldTag) {
                    vm.modified = vm.modified || newTag.length !== oldTag.length;
                    if (!vm.modified) {
                        for (var i = 0; i < newTag.length; i++) {
                            if (newTag[i].Text !== oldTag[i].Text) {
                                vm.modified = true;
                                break;
                            }
                        }
                    }
                    checkModified();
                }, true);

                Analytics.trackEvent('CardUpdate', 'TagsUpdate', vm.card.CardId.toString());
            },
            function () {
                vm.loading = false;
                vm.card = {};
                vm.ModelError = true;
            });

        function checkModified() {
            if (vm.editmode) {
                FileModifiedService.setModified(vm.modified);
            }
        }

        vm.checkTagCount = function(card) {

            if (card === undefined) {
                return false;
            }

            var MAX_TAGS = 7;
            var tagCount = 0;
            var disabled = false;
            for (var i = 0; i < card.Tags.length; i++) {
                if (card.Tags[i].TagTypeId === 1) {
                    tagCount++;
                }
            }
            disabled = tagCount >= MAX_TAGS;
            return disabled;
        };

        vm.save = function (direction) {

            if (!vm.modified) {
                if (direction >= 0) {
                    $location.path(direction === 1 ? '/addressinfo' : '/searchinfo');
                }
                return;
            }

            vm.ModelError = false;
            Card.saveCardInfo(vm.card,
                function () {
                    if (direction >= 0) {
                        $location.path(direction === 1 ? '/addressinfo' : '/searchinfo');
                    }
                    vm.modified = false;
                    checkModified();
                },
                function () {
                    vm.ModelError = true;
                });            
        };
       
        vm.AddTag = function () {

            var found = false;
            var text = vm.NewTag;
            for (var i = 0; i < vm.card.Tags.length; i++) {
                if (vm.card.Tags[i].Text === text) {
                    found = true;
                    break;
                }
            }
            if (!found) {
                vm.card.Tags.push({
                    Text: text,
                    TagTypeId: 1
                });
            }
            vm.NewTag = '';
        };
        vm.RemoveTag = function (text) {
            for (var i = 0; i < vm.card.Tags.length; i++) {
                if (vm.card.Tags[i].Text === text) {
                    vm.card.Tags.splice(i, 1);
                    break;
                }
            }
        };
    }
]);