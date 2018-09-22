var directives = directives || angular.module('busidexstartApp.directives', []);

directives
    .directive('filepicker', function () {
        'use strict';
        //return {
        //    controller: 'CardImageController',
        //    //scope: true,        //create a new scope
        //    //scope: {
        //    //    'idx': '=',
        //    //    'cardsrc': '='
        //    //},
        //    link: function(scope, el) {
        //        el.bind('change', function (e) {
        //            scope.IsBound = null;
        //            scope.file = (e.srcElement || e.target).files[0];
        //            scope.ImagePreview(scope.idx);
        //        });
        //    }
        //};
        return {
            transclude: true,
            restrict: 'E',
            template: '<input type="filepicker">',
            replace: 'true',
            link: function (scope, element) {
                filepicker.
                    constructWidget(element)
                    .pick(
                     {
                         services: ['COMPUTER', 'CONVERT'],
                         mimetype: 'image/*',
                         container: 'modal',
                         imageDim: [450, 450],
                         cropDim: [400, 400],
                         cropRatio: [1 / 1],
                         cropForce: true
                     },
                      function (Blob) {
                          console.log(JSON.stringify(Blob));
                          var img = document.createElement('img');
                          img.src = Blob.url;
                          var tag = document.getElementById('crop_profile_results');
                          tag.innerHTML = '';
                          tag.appendChild(img);
                      },
                      function (FPError) {
                          console.log(FPError.toString());
                      }
                    );
            }
        };
    });