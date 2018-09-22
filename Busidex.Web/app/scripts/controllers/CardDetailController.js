/*CARD DETAIL*/
function CardDetailCtrl($scope, $rootScope, $cookieStore, $location, $http, $route, $routeParams, Analytics, Busidex, Activity) {
    'use strict';
    Analytics.trackPage($location.path());
    $scope.Card = null;
    $scope.ShowBack = false;
    $scope.ShowFront = false;
    $rootScope.ShowFilterControls = false;
    $scope.BackOrientationClass = '';
    $scope.FrontOrientationClass = '';
    $scope.AddToMyBusidex = addToMyBusidex;
    $scope.ExistsInMyBusidex = false;

    var params = {
        'id': $routeParams.CardId
    };

    if (isNaN(parseInt(params.id))) {
        $location.path('/home');
        return;
    }

    var addActivity = function (sourceId, cardId) {

        var activity =
        {
            CardId: cardId,
            UserId: $rootScope.User !== null ? $rootScope.User.UserId : null,
            EventSourceId: sourceId
        };
        Activity.post(activity,
            function () {
                console.log('event saved');
            },
            function (status) {
                console.log('event NOT saved: ' + status);
            });
    };

    if ($rootScope.EventSources === null) {
        Activity.query({},
            function (data) {
                $rootScope.EventSources = data.EventSources;
                addActivity($rootScope.EventSources.DETAILS, $routeParams.CardId);
            },
            function () {

            });
    } else {
        addActivity($rootScope.EventSources.DETAILS, $routeParams.CardId);
    }


    function addToMyBusidex(id) {

        if ($rootScope.User === null) {
            $location.path('/account/login');
            return;
        }

        Busidex.post({ userId: $rootScope.User.UserId, cId: id },
            function () {
                window.alert('This card is now in your Busidex!');
                $scope.ExistsInMyBusidex = true;
            },
            function () {
                window.alert('There was a problem adding this card to your Busidex');
                return false;
            });
    }

    $scope.Waiting = true;
    $http({
        method: 'GET',
        url: ROOT + '/card/details/?id=' + params.id,
        data: JSON.stringify(params),
        headers: { 'Content-Type': 'application/json', 'X-UserId': $rootScope.User !== null ? $rootScope.User.UserId : 0 }
    }).success(function (data) {

        $scope.FrontOrientationClass = data.Model.FrontOrientation === 'V' ? '=v_preview' : 'h_preview';
        $scope.BackOrientationClass = data.Model.BackOrientation === 'V' ? 'v_preview' : 'h_preview';
        $scope.UserId = $rootScope.User !== null ? $rootScope.User.UserId : null;
        $scope.Card = data.Model;
        $scope.ShowBack = data.Model.HasBackImage && data.Model.BackFileId !== 'b66ff0ee-e67a-4bbc-af3b-920cd0de56c6';
        $scope.ShowFront = data.Model.FrontFileId !== null;
        $scope.ExistsInMyBusidex = data.Model.ExistsInMyBusidex;
        $scope.Waiting = false;

        document.title = data.Model.Name + ' | Busidex';

    }).error(function (data, status) {
        if (status === 404) {
            $location.path('/home');
            return;
        }
        $scope.resultData = data;
        window.alert('Getting card detail failed.');
        $scope.Waiting = false;
    });
}
CardDetailCtrl.$inject = ['$scope', '$rootScope', '$cookieStore', '$location', '$http', '$route', '$routeParams', 'Analytics', 'Busidex', 'Activity'];
