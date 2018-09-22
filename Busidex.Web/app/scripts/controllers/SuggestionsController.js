/*SUGGESTIONS*/
function SuggestionsCtrl($scope, $rootScope, $cookieStore, $location, $http, Suggestions, Analytics) {
    'use strict';
    Analytics.trackPage($location.path());
    $scope.SuggestionInfo = [];
    $scope.Waiting = true;

    Suggestions.get(
        function (response) {
            $scope.Waiting = false;
            $scope.SuggestionInfo = response.Suggestions;
            $scope.Suggestion = response.Model;
        },
        function () {

        });

    $scope.save = function () {

        $scope.Suggestion.CreatedBy = $rootScope.User !== null ? $rootScope.User.UserId : 0;

        //var data = { suggestion: $scope.Suggestion };
        Suggestions.post($scope.Suggestion,
            function (response) {
                $scope.SuggestionInfo = response.Suggestions;
                $scope.Suggestion = response.Model;
            },
            function () {
                window.alert('Sorry, we had a problem saving your idea.');
            });
        return true;
    };

    $scope.vote = function (id) {

        Suggestions.update({ id: id },
            function (response) {
                $scope.SuggestionInfo = response.Suggestions;
                $scope.Suggestion = response.Model;
            },
            function () {
                window.alert('Sorry, Vote not saved');
            });
        return true;
    };
}
SuggestionsCtrl.$inject = ['$scope', '$rootScope', '$cookieStore', '$location', '$http', 'Suggestions', 'Analytics'];
