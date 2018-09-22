angular.module('busidexstartApp').controller('CardImageController', [
    '$scope', '$rootScope', 'Cache', 'CacheKeys', 'Card', '$http', '$location', 'Registration', '$routeParams', 'Analytics', 'FileModifiedService',
    function ($scope, $rootScope, Cache, CacheKeys, Card, $http, $location, Registration, $routeParams, Analytics, FileModifiedService) {
        'use strict';

        Analytics.trackPage($location.path());

        var vm = this;

        vm.files = [];
        vm.files.push('');
        vm.files.push('');
        vm.FileError = false;
        vm.ModelError = false;
        vm.waiting = false;
        vm.loading = false;
        vm.modified = false;
        vm.index = $location.path() === '/front' ? 0 : 1;
        vm.showErrorDetails = false;
        vm.errorDetails = [];
        vm.showAdditionalErrorInfo = showAdditionalErrorInfo;

        var card = Cache.get(CacheKeys.Card);
        var user = Cache.get(CacheKeys.User);
        
        user = angular.fromJson(user);
        card = angular.fromJson(card);

        vm.user = user;
        vm.editmode = $routeParams.m === 'edit';

        if ($routeParams._u !== undefined) {
            $http.defaults.headers.common['X-Authorization-Token'] = $routeParams._u;
        }

        function showAdditionalErrorInfo() {
            vm.showErrorDetails = !vm.showErrorDetails;
        }

        function addWatches() {
            $scope.$watch(function() {
                return vm.card.FrontOrientation;
            }, function(newOrientation, oldOrientation) {
                vm.modified = vm.modified || newOrientation !== oldOrientation;
                checkModified();
            });
            $scope.$watch(function() {
                return vm.card.BackOrientation;
            }, function(newOrientation, oldOrientation) {
                vm.modified = vm.modified || newOrientation !== oldOrientation;
                checkModified();
            });
            $scope.$watch(function() {
                return vm.card.FrontSrc;
            }, function(newName, oldName) {
                vm.modified = vm.modified || newName !== oldName;
                checkModified();
            });
            $scope.$watch(function() {
                return (vm.card !== null && vm.card !== undefined) ? vm.card.BackSrc : '';
            }, function(newName, oldName) {
                vm.modified = vm.modified || newName !== oldName;
                checkModified();
            });
        }

        function checkModified() {
            if (vm.editmode) {
                FileModifiedService.setModified(vm.modified);
            }
        }

        function bind() {
            vm.AllowNext = function() {
                return vm.card !== undefined && vm.card.FrontSrc !== null &&
                    vm.user !== null &&
                    vm.user.Token !== null && vm.user.Token !== undefined;
            };

            vm.save = function(idx, direction) {

                if (!vm.modified) {
                    if (direction >= 0) {
                        if (idx === 0) {
                            $rootScope.progress = Math.max($rootScope.progress, 34);
                            $location.path(direction === 1 ? '/back' : '/front');
                        } else {
                            $location.path(direction === 1 ? '/visibility' : '/front');
                            vm.loading = false;
                        }
                    }
                    return;
                }

                vm.loading = true;

                $rootScope.progress = Math.max($rootScope.progress, 34);
                vm.FileError = false;
                vm.ModelError = false;
                vm.idx = idx;

                Card.saveCardImage({
                        idx: vm.idx,
                        imageUrl: encodeURIComponent(vm.files[vm.idx]),
                        orientation: (idx === 0 ? vm.card.FrontOrientation : vm.card.BackOrientation)
                    },
                    function() {
                        Cache.put(CacheKeys.Card, angular.toJson(vm.card));
                        vm.loading = false;
                        vm.CardSaved = true;
                        if (direction >= 0) {
                            if (vm.idx === 0) {
                                $location.path(direction === 1 ? '/back' : '/front');
                            } else {
                                $location.path(direction === 1 ? '/visibility' : '/front');
                            }
                        }
                        vm.modified = false;
                        checkModified();

                    },
                    function() {
                        vm.FileError = true;
                        vm.ModelError = false;
                        vm.loading = false;
                    });
            };

            vm.ImagePreview = function(idx) {

                vm.idx = idx;
                filepicker.setKey(FILEPICKER_KEY);
                filepicker
                    .pick({
                            services: ['COMPUTER', 'CONVERT', 'FACEBOOK', 'WEBCAM', 'GOOGLE_DRIVE', 'GMAIL'],
                            mimetype: 'image/*',
                            container: 'modal',
                            imageQuality: 70,
                            maxSize: '256000',
                            // customCss: 'https://start.busidex.com/app/styles/filepicker.css',
                            cropForce: false
                        },
                        function(blob) {

                            $scope.$apply(function() {
                                if (vm.idx === 0) {
                                    vm.card.FrontSrc = vm.card.FrontFileId = blob.url;
                                } else {
                                    vm.card.BackSrc = vm.card.BackFileId = blob.url;
                                }
                                vm.files[vm.idx] = blob.url;
                            });

                        },
                        function(FPError) {
                            console.log(FPError.toString());
                        }
                    );
            };
        }

        function getCard() {
            vm.loading = true;

            var data = {
                'id': user !== null ? user.CardId : 0,
                'userId': 0
            };

            if (data.id === 0) {
                vm.errorDetails.push('Could not get card id for this user.');
            }
            if (user === null || user === undefined) {
                vm.errorDetails.push('Could not get user information.');
            }

            Card.get(data,
                function(model) {

                    //console.log('DEBUG: ' + angular.toJson(model));
                    

                    vm.loading = false;
                    card = model.Model;

                    if (card === undefined) {
                        vm.loading = false;
                        vm.ModelError = true;
                        card.FrontSrc = card.BackSrc = null;
                        card.FrontOrientation = card.BackOrientation = 'H';
                        vm.card = card;
                        return;

                    }
                    card.FrontSrc = card.FrontFileId === null ? card.FrontFileId : 'https://az381524.vo.msecnd.net/cards/' + card.FrontFileId + '.' + card.FrontType;
                    card.BackSrc = card.BackFileId === null ? card.BackFileId : 'https://az381524.vo.msecnd.net/cards/' + card.BackFileId + '.' + card.BackType;
                    // minor hack
                    if (card.BackFileId === 'b66ff0ee-e67a-4bbc-af3b-920cd0de56c6' || card.BackFileId === '00000000-0000-0000-0000-000000000000') {
                        card.BackSrc = null;
                    }
                    if (card.FrontFileId === 'b66ff0ee-e67a-4bbc-af3b-920cd0de56c6' || card.FrontFileId === '00000000-0000-0000-0000-000000000000') {
                        card.FrontSrc = null;
                    }
                    vm.card = card;

                    if ($location.path() === '/front') {
                        Analytics.trackEvent('CardUpdate', 'FrontImageUpdate', vm.card.CardId.toString());
                    } else {
                        Analytics.trackEvent('CardUpdate', 'BackImageUpdate', vm.card.CardId.toString());
                    }

                    Cache.put(CacheKeys.Card, angular.toJson(vm.card));
                    addWatches();
                    bind();
                },
                function (error) {
                    vm.errorDetails.push(angular.toJson(error));
                    if (error.status === 401) {
                        $location.path('/logout');
                        return;
                    } else {
                        vm.loading = false;
                        vm.ModelError = true;

                        card = card || {};
                        card.FrontSrc = card.BackSrc = null;
                        card.FrontOrientation = card.BackOrientation = 'H';
                        vm.card = card;
                    }
                });
        }

        if ($routeParams.token === undefined || $routeParams.token === null) {
            if (user === undefined || user === null || user.Token === null || user.Token === undefined) {
                if ($routeParams._u !== undefined) {
                    $http.defaults.headers.common['X-Authorization-Token'] = $routeParams._u;
                    vm.user = {};
                    vm.user.Token = $routeParams._u;

                    getCard();
                    Cache.put(CacheKeys.User, angular.toJson(vm.user));
                } else {
                    $location.path('/login');
                }
            } else {
                $http.defaults.headers.common['X-Authorization-Token'] = user.Token;
                getCard();
            }
        } else {

            Registration.activate({
                    token: $routeParams.token
                },
                function(buser) {

                    $http.defaults.headers.common['Set-Cookie'] = ';domain=.busidx.com';

                    Cache.put(CacheKeys.User, angular.toJson(buser));

                    user = Cache.get(CacheKeys.User);
                    user = angular.fromJson(user);
                    vm.user = user;

                    $http.defaults.headers.common['X-Authorization-Token'] = user.Token;

                    getCard();
                    bind();
                },
                function() {
                    $location.path('/login');
                });
            return;
        }
    }
]);