var directives = directives || angular.module('busidexstartApp.directives', []);

directives
    .directive('fileUpload', function () {
        'use strict';
        return {
            controller: 'CardImageController',
            //scope: true,        //create a new scope
            link: function(scope, el, attrs) {
                el.unbind('change').bind('change', function (event) {
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
    });