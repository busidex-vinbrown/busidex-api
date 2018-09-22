/*SEARCH*/
function SearchCtrl($scope, $rootScope, $cookieStore, $location, $http, Busidex, Search, Activity, Analytics, $routeParams) {
    Analytics.trackPage($location.path());
    //var userId = $rootScope.User != null ? $rootScope.User.UserId : 0;
    $scope.IsLoggedIn = $rootScope.User !== null;
    $rootScope.IsLoggedIn = $rootScope.User !== null;
    $rootScope.ShowFilterControls = false;
    $rootScope.SetCurrentMenuItem('Search');
    $scope.SearchByTag = searchByTag;
    $scope.Reset = reset;
    $scope.ShowResultsMessage = showResultsMessage;
    $scope.ShowNoOwnerMessage = showNoOwnerMessage;

    $scope.model = {
        SearchText: '',
        Results: []
    };
    $scope.model.TagSearch = false;

    $scope.dynamicImg = '';
    $scope.popupCard = null;
    $scope.ShowAddLink = false;
    $scope.Waiting = false;
    if ($rootScope.SearchModel === null) {
        $scope.model.SearchModel = {};
        $scope.model.SearchModel.Results = [];
    } else {
        $scope.model = $rootScope.SearchModel;
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

    $scope.GoToDetails = function (id) {
        setTimeout('goToDetails(' + id + ')', 1000);
    };
    $scope.GoToMyBusidex = function () {
        $location.path('/busidex/mine');
    };

    if ($routeParams._t !== null) {
        searchByTag($routeParams._t);

        _gaq.push(['_trackEvent', 'Search', 'Event', $routeParams._t]);
    }

    function reset() {
        $scope.model.SearchText = '';
        $scope.model.SearchResultsMessage = '';
        $scope.model.SearchModel.Results = [];
    }

    function searchByTag(tag) {
        $scope.Waiting = true;

        _gaq.push(['_trackEvent', 'Search', 'Event', tag]);

        Search.searchByTag({ systag: tag },
            function (model) {
                $scope.Waiting = false;
                $scope.model = model;
                $scope.model.TagSearch = true;
                $scope.model.SearchText = '';
                showResultsMessage();
                $scope.UserId = $rootScope.User !== null ? $rootScope.User.UserId : null;

                if (model && model.SearchModel && model.SearchModel.Results) {

                    for (var i = 0; i < model.SearchModel.Results.length; i++) {

                        var thisCard = model.SearchModel.Results[i];
                        if (thisCard.OwnerId !== null && thisCard.Searchable === true) {
                            thisCard.HasOwner = true;
                            addActivity($rootScope.EventSources.SEARCH, thisCard.CardId);
                        } else {
                            thisCard.HasOwner = false;
                        }
                        thisCard.FrontOrientationClass = thisCard.FrontOrientation == 'V' ? 'v_preview' : 'h_preview';
                    }
                }
                $rootScope.SearchModel = $scope.model;
            },
            function (data) {
                $scope.resultData = data;
                alert("Getting search results failed.");
                $scope.Waiting = false;
            });
    }

    function showNoOwnerMessage() {
        alert('Sorry, this card has not been activated.');
    }

    function showResultsMessage() {
        $scope.model.SearchResultsMessage = '';
        if ($scope.model.TagSearch === true) {
            return $scope.model.SearchResultsMessage;
        }

        if ($scope.model.SearchModel.Results.length === 0) {
            $scope.model.SearchResultsMessage = "Your search for '" + $scope.model.SearchText.split(' ').join('+') + "' didn't return any results.";
        } else {
            $scope.model.SearchResultsMessage = "We searched for anything containing '" + $scope.model.SearchText.split(' ').join('\' or \'') + "' and found " + $scope.model.SearchModel.Results.length + " cards!";
        }

        return $scope.model.SearchResultsMessage;
    }
    $scope.doSearch = function () {

        $scope.model.NoResults = false;


        $scope.model.SearchModel.Results = [];
        if ($scope.model.SearchModel.Distance === 0) {
            $scope.model.SearchModel.Distance = 25;
        }
        $scope.Waiting = true;
        $scope.model.SearchModel.UserId = $rootScope.User !== null ? $rootScope.User.UserId : null;
        $scope.model.UserId = null;//userId;
        $scope.model.CardType = 1; // search for Cards only

        _gaq.push(['_trackEvent', 'Search', 'General', $scope.model.SearchText]);
        Search.post($scope.model,
            function (model) {

                $scope.Waiting = false;
                $scope.model = model;
                $scope.model.TagSearch = false;
                $scope.UserId = $rootScope.User !== null ? $rootScope.User.UserId : null;

                $scope.model.SearchText = model.SearchModel.SearchText;

                for (var i = 0; i < model.SearchModel.Results.length; i++) {

                    var thisCard = model.SearchModel.Results[i];
                    addActivity($rootScope.EventSources.SEARCH, thisCard.CardId);
                    if (thisCard.OwnerId !== null && thisCard.Searchable === true) {
                        thisCard.HasOwner = true;
                        addActivity($rootScope.EventSources.SEARCH, thisCard.CardId);
                    } else {
                        thisCard.HasOwner = false;
                    }
                    thisCard.FrontOrientationClass = thisCard.FrontOrientation == 'V' ? 'v_preview' : 'h_preview';
                }

                $rootScope.SearchModel = $scope.model;
                $scope.model.NoResults = $scope.model.SearchModel.Results.length === 0;
                showResultsMessage();
            },
            function (data) {
                $scope.resultData = data;
                alert("Getting search results failed.");
                $scope.Waiting = false;
            });
    };
    $scope.setPopupImage = function (card) {

        if (!card.HasOwner) {
            return false;
        }

        var img = 'https://az381524.vo.msecnd.net/cards/' + card.FrontFileId + '.' + card.FrontFileType;
        $scope.dynamicImg = img;
        $scope.popupCard = card;
        $scope.ShowAddLink = !card.ExistsInMyBusidex;
        $scope.AddLinkMessage = ($scope.ShowAddLink ? "Add to " : "View in ") + $rootScope.MyBusidexName;
        $scope.ViewBusidexLink = "";
    };
    $scope.AddToMyBusidex = function (card) {

        if ($rootScope.User === null) {
            setTimeout(goToLogin(), 1000);
            return;
        }

        Busidex.post({ userId: $rootScope.User.UserId, cId: card.CardId },
            function () {
                alert("This card is now in your Busidex!");
                card.ExistsInMyBusidex = true;
                $scope.ViewBusidexLink = "#/busidex/mine";
                $scope.ShowAddLink = false;
            },
            function () {
                alert('There was a problem adding this card to your Busidex');
                return false;
            });
    };
}
SearchCtrl.$inject = ['$scope', '$rootScope', '$cookieStore', '$location', '$http', 'Busidex', 'Search', 'Activity', 'Analytics', '$routeParams'];
