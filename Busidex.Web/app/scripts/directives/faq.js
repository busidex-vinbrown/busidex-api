var directives = directives || angular.module('Busidex.directives', []);

directives
    .directive('faq', ['$sce', function ($sce) {
        'use strict';
        return {
            restrict: 'A',
            replace: true,
            scope: { question: '=', answer: '=' },
            link: function (scope, element, attributes) {
                scope.showAnswer = false;
                
                var expression = $sce.parseAsHtml(attributes.answer);                

                scope.toggle = function() {
                    scope.showAnswer = !scope.showAnswer;
                    if (scope.showAnswer) {
                        element.find('#answer').html(expression);
                    } else {
                        element.find('#answer').html('');
                    }
                };
            },
            transclude: true,
            templateUrl: '/views/fragments/faq.html',
        };
    }]);