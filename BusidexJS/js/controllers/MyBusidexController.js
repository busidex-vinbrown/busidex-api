/*MY BUSIDEX*/
function MyBusidexCtrl($scope, $rootScope, $cookieStore, $location, Busidex, Notes, SharedCard, Activity, Analytics, $routeParams, $http) {
    Analytics.trackPage($location.path());
    $rootScope.IsSharing = false;
    $rootScope.ToggleSharing = function () {
        $rootScope.IsSharing = !$rootScope.IsSharing;
        if (!$rootScope.IsSharing) {
            $scope.ShareWithEmail = '';
            $scope.SharedCardIds = {};
        }
    };


    $rootScope.SetCurrentMenuItem('Busidex');

    $scope.data = {};
    $scope.data.TotalToShow = 20;
    $scope.data.ShowMore = function (i) {
        if ($scope.data.TotalToShow <= $rootScope.MyBusidex.length) {
            $scope.data.TotalToShow += i;
        }
    };

    $scope.SharedCardIds = {};

    $scope.Waiting = true;
    $scope.Error = false;

    $scope.AddActivity = function (sourceId, cardId) {

        var activity =
        {
            CardId: cardId,
            UserId: $rootScope.User.UserId,
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

    $scope.CardToShare = {};
    $scope.share = function () {

        var sharedCards = [];
        var s = {};
        for (var i = 0; i < $rootScope.MyBusidex.length; i++) {
            var card = $rootScope.MyBusidex[i];
            if (card.Share) {

                s = {
                    SharedCardId: 0,
                    CardId: card.CardId,
                    SendFrom: $rootScope.User.UserId,
                    SendFromEmail: '',
                    Email: $scope.ShareWithEmail,
                    ShareWith: 0,
                    SharedDate: new Date(),
                    Accepted: false,
                    Declined: false,
                    Recommendation: ''
                };
                sharedCards.push(s);

                card.Share = false;
            }
        }
        if (sharedCards.length === 0) {
            alert('Please select at least one card to share');
            return false;
        }

        SharedCard.post(sharedCards,
            function () {
                $scope.ShareWithEmail = '';
                $scope.SharedCardIds = {};
                $rootScope.IsSharing = false;
                alert('Your cards have been shared!');
            },
            function () {
                alert('There was a problem sharing your cards.');
            });
    };

    $scope.setShowControls = function (allCards, c, show) {
        for (var i = 0; i < allCards.length; i++) {
            if (allCards[i] == c) {
                c.ShowControls = !c.ShowControls;
            } else {
                allCards[i].ShowControls = false;
            }
        }
        return false;
    };

    $scope.toggleMobile = function (card) {

        card.MobileView = !card.MobileView;
        Busidex.update({ id: card.UserCardId, isMobileView: card.MobileView });
    };

    $scope.RemoveCard = function (busidex, card) {

        Busidex.remove({ id: card.CardId, userId: $scope.User.UserId },
            function () {
                for (var i = 0; i < busidex.length; i++) {
                    if (busidex[i].CardId == card.CardId) {
                        busidex.splice(i, 1);
                        break;
                    }
                }
            },
            function () {
                alert('There was a problem removing this card.');
            });
    };

    if ($rootScope.User === null) {
        if ($location.path() === '/busidex/mine') {
            $location.path("/account/login");
        }

    } else {

        $rootScope.ShowFilterControls = true;

        if ($rootScope.MyBusidex === null || $rootScope.MyBusidex.length === 0) {

            Busidex.get({ all: true },
                function (model) {

                    for (var i = 0; i < model.MyBusidex.Busidex.length; i++) {

                        var thisCard = model.MyBusidex.Busidex[i].Card;
                        model.MyBusidex.Busidex[i].OrientationClass = thisCard.FrontOrientation == 'V' ? 'v_preview' : 'h_preview';
                        if (thisCard.OwnerId == $rootScope.User.UserId ||
                            (!thisCard.OwnerId && (thisCard.CreatedBy == $rootScope.User.UserId || model.MyBusidex.Busidex[i].SharedById == $rootScope.User.UserId))) {
                            thisCard.ShowEdit = true;
                        } else {
                            model.MyBusidex.Busidex[i].Card.ShowEdit = false;
                        }
                        thisCard.IconPath = thisCard.OwnerId ? "../img/searchIcon.png" : "../img/phone.png";
                        thisCard.IconLink = thisCard.OwnerId ? "#" + thisCard.CardId : "#/invite/" + thisCard.CardId;
                        thisCard.IconClass = thisCard.OwnerId ? "HasOwner" : "InMyBusidex";
                        thisCard.IconTitle = thisCard.OwnerId ? "In My Busidex" : "Send an invite";

                        var addresses = [];
                        var j = 0;
                        for (j = 0; j < thisCard.Addresses.length; j++) {

                            var a = thisCard.Addresses[j];
                            addresses.push(
                                new Address(a.CardAddressId, a.CardId, a.Address1 || '', a.Address2 || '', a.City || '', a.State, a.ZipCode || '')//, a.Region, a.Country
                            );
                        }

                        thisCard.Addresses = addresses;
                        thisCard.MapInfo = 'http://maps.google.com/maps?daddr={' + (thisCard.Addresses.length > 0 ? thisCard.Addresses[0].Display() : '') + '}';
                        thisCard.Share = false;

                        model.MyBusidex.Busidex[i].ShowControls = false;
                    }
                    $rootScope.MyBusidex = model.MyBusidex.Busidex;

                    if ($routeParams.share) {
                        $rootScope.ToggleSharing();
                    }

                    $scope.Waiting = false;

                }, function (data) {
                    $scope.resultData = data;
                    $scope.Waiting = false;
                    $scope.Error = true;
                });

        } else {

            $scope.IsSharing = false;
            $scope.ToggleSharing = function () {
                $scope.IsSharing = !$scope.IsSharing;
            };

            if ($routeParams.share) {
                $rootScope.ToggleSharing();
            }

            $scope.Waiting = false;
        }

        $(document).on("change", "textarea.Notes", function () {

            Notes.update({ id: $(this).attr("ucId"), notes: escape($(this).val()) },
                function () {

                },
                function () {

                });
        });

    }
}
MyBusidexCtrl.$inject = ['$scope', '$rootScope', '$cookieStore', '$location', 'Busidex', 'Notes', 'SharedCard', 'Activity', 'Analytics', '$routeParams', '$http'];
