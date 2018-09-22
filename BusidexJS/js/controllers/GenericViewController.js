function GenericViewCtrl($http, $scope, $rootScope, $cookieStore, Activity) {

    $rootScope.ToggleMenu = function () {
        $rootScope.ShowMenu = showMenu = !showMenu;
    };
    $rootScope.ShowFilterControls = false;
    $rootScope.User = $rootScope.User || $cookieStore.get('User');
    $rootScope.SharedCards = [];

    var token = $rootScope.User !== null ? $rootScope.User.Token : null;
    $http.defaults.headers.common['X-Authorization-Token'] = token;

    if ($rootScope.User && !$rootScope.User.Token) {
        $rootScope.User = null;
        $cookieStore.remove("User");
    }

    $rootScope.MyBusidex = $rootScope.MyBusidex || [];
    $rootScope.LoginModel = {
        LoginText: $rootScope.User === null ? 'Login' : 'LogOut',
        LoginRoute: $rootScope.User === null ? '#/account/login' : '#/account/logout',
        HomeLink: $rootScope.User !== null && $rootScope.User.StartPage == 'Organization' ? '#/groups/organization/' + $rootScope.User.Organizations[0].Item2 : '#/home'
    };
    $rootScope.IsLoggedIn = $rootScope.User !== null;
    $scope.Waiting = false;
    if ($rootScope.EventSources === null) {
        Activity.query({},
            function (data) {
                $rootScope.EventSources = data.EventSources;

            },
            function () {

            });
    }
    $rootScope.NavItems = {};
    $rootScope.NavItems.Home = false;
    $rootScope.NavItems.Search = false;
    $rootScope.NavItems.Add = false;
    $rootScope.NavItems.Mine = false;
    $rootScope.NavItems.Groups = false;
    $rootScope.NavItems.Busidex = false;
    $rootScope.NavItems.Memberships = false;
    $rootScope.SetCurrentMenuItem = function (currentItem) {

        for (var item in $rootScope.NavItems) {
            if ($rootScope.NavItems.hasOwnProperty(item)) {
                $rootScope.NavItems[item] = false;
            }
        }

        $rootScope.NavItems[currentItem] = true;
    };

    $rootScope.ShowAddCard = function () {

        return $rootScope.IsLoggedIn &&
            $rootScope.User !== null &&
            !$rootScope.User.IsAdmin &&
            !$rootScope.User.HasCard &&
            (parseInt($rootScope.User.AccountTypeId) === ACCT_TYPE_PROFESSIONAL || parseInt($rootScope.User.AccountTypeId) === ACCT_TYPE_BETA);
    };

    var acctType = $rootScope.User !== null ? parseInt($rootScope.User.AccountTypeId) : 0;
    $rootScope.MyBusidexMenuName = $rootScope.User !== null && acctType === ACCT_TYPE_ORGANIZATION ? 'Edit Referrals' : 'My Busidex';
    $rootScope.MyBusidexName = $rootScope.User !== null && acctType === ACCT_TYPE_ORGANIZATION ? 'Referrals' : 'My Busidex';

    $rootScope.ShowMemberships = function() {

        var hasOrgs = false;
        acctType = $rootScope.User !== null ? parseInt($rootScope.User.AccountTypeId) : 0;
        if ($rootScope.IsLoggedIn && $rootScope.User !== null && $rootScope.User.Organizations !== null && acctType != ACCT_TYPE_ORGANIZATION) {
            var obj = $rootScope.User.Organizations;
            for (var prop in obj) {
                if (obj.hasOwnProperty(prop)) {
                    hasOrgs = true;
                    break;
                }
            }
        }
        return hasOrgs;
    };

    $rootScope.SEOCardNames = '';
    $rootScope.HomeHelpShare = "Send your card to others so they can have it in their collection and always be up to date.";
    $rootScope.HomeHelpDetails = "People could be trying to add your card to their collection. Be sure to include your Name and/or Company Name so your card can be found in a search.";
    $rootScope.HomeHelpTags = "Adding Tags helps your card get found in searches. Keep them relevant to what your business is about so people can find you more easily.";
    $rootScope.HomeShareLink = "#/busidex/mine?share=true";
    $rootScope.HomeEditLink = ($rootScope.User !== null && $rootScope.User.HasCard) ? "#/card/edit/" + $rootScope.User.CardId + "?details=true" : "#/card/add";
    $rootScope.HomeTagLink = ($rootScope.User !== null && $rootScope.User.HasCard) ? "#/card/edit/" + $rootScope.User.CardId + "?tags=true" : "#/card/add";

}
GenericViewCtrl.$inject = ['$http', '$scope', '$rootScope', '$cookieStore', 'Activity'];