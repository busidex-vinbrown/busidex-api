'use strict';
var GlobalCardCount = 0;
var showMenu = false;
var getGroup;

function manualLogin(user, $scope, $rootScope, $cookieStore, $http, SharedCard, $location) {
    $scope.resultData = user;
    $rootScope.User = user;

    var acctType = $rootScope.User != null ? parseInt($rootScope.User.AccountTypeId) : 0;
    $rootScope.MyBusidexMenuName = parseInt($rootScope.User.AccountTypeId) === ACCT_TYPE_ORGANIZATION ? 'Edit Referrals' : 'My Busidex';
    $rootScope.MyBusidexName = $rootScope.User != null && acctType === ACCT_TYPE_ORGANIZATION ? 'Referrals' : 'My Busidex';

    $rootScope.SearchModel = null;

    $rootScope.LoginModel = {
        LoginText: $rootScope.User == null ? 'Login' : 'LogOut',
        LoginRoute: $rootScope.User == null ? '#/account/login' : '#/account/logout',
        HomeLink: $rootScope.User != null && $rootScope.User.StartPage == 'Organization' ? '#/groups/organization/' + $rootScope.User.Organizations[0].Item2 : '#/home'
    }

    $cookieStore.put('User', user);
    $rootScope.IsLoggedIn = $rootScope.User != null;

    var token = $rootScope.User != null ? $rootScope.User.Token : null;
    $http.defaults.headers.common['X-Authorization-Token'] = token;

    SharedCard.get({},
            function (data) {
                if (data.SharedCards && data.SharedCards.length > 0) {


                    $('#notificationPopup').modal('show');

                    $rootScope.HasSharedCards = true;
                    $rootScope.SharedCards = [];
                    for (var i = 0; i < data.SharedCards.length; i++) {
                        $rootScope.SharedCards.push(data.SharedCards[i]);
                    }
                    $scope.SendFromEmail = data.SharedCards[0].SendFromEmail;
                }
            },
            function () {

            });

    if (user.StartPage == 'Organization') {

        $location.path('/groups/organization/' + user.Organizations[0].Item2);

    } else {

        $location.path('/home');
        $location.url($location.path()); // remove any query string parameters

        $http({
            method: 'GET',
            url: ROOT + "/mycard/?id=" + user.UserId,
            headers: { 'Content-Type': 'application/json' }
        }).success(function (model) {

            if ($rootScope.User.AccountTypeId === ACCT_TYPE_PROFESSIONAL || $rootScope.User.AccountTypeId === ACCT_TYPE_BETA) {
                var card = model.MyCards[0];
                if (card != null &&
                (card.Name == null || card.Name.length == 0) &&
                (card.CompanyName == null || card.CompanyName.length == 0)) {
                    $location.path('/card/edit/' + card.CardId);
                } else if (card == null) {
                    $location.path('/card/add/mine');
                }
            }
        });
    }
}

var GetCardCount = function () {
    GlobalCardCount = GlobalCardCount + 1;

    //setTimeout(GetCardCount, 1000);
};

function goToLogin() {
    window.location.href = '#/account/login';
}

function goToDetails(id) {
    window.location.href = '#/card/details/' + id;
}

var Address = function (id, cardId, addr1, addr2, city, state, zip) {//, reg, country

    if (state == null) {
        state = {
            StateCodeId: 0,
            Code: '',
            Name: ''
        };
    }
    var self = this;
    self.CardAddressId = id;
    self.CardId = cardId;
    self.Address1 = addr1;
    self.Address2 = addr2;
    self.City = city;
    self.State = {
        StateCodeId: state.StateCodeId,
        Code: state.Code,
        Name: state.Name
    };
    self.ZipCode = zip;
    //self.Region = reg;
    //self.Country = country;
    self.Deleted = false;
    self.DeleteSrc = '../../img/delete.png';
    self.Display = function () {
        return self.Address1 + ' ' +
            self.Address2 + ' ' +
            self.City + ' ' +
            (self.State != null ? self.State.Code : '') +
            (self.ZipCode != null && self.ZipCode.length > 0 ? ', ' : ' ') +
            self.ZipCode;
        //self.Region + ' ' +
        //self.Country;
    };
    self.Selected = false;
};

/*SIGN IN CONTROLLER*/
function SignInController($scope) {
    // This flag we use to show or hide the button in our HTML.
    $scope.signedIn = false;

    // Here we do the authentication processing and error handling.
    // Note that authResult is a JSON object.
    $scope.processAuth = function (authResult) {
        // Do a check if authentication has been successful.
        if (authResult.access_token) {
            // Successful sign in.
            $scope.signedIn = true;

            //     ...
            // Do some work [1].
            //     ...
        } else if (authResult.error) {
            // Error while signing in.
            $scope.signedIn = false;

            // Report error.
        }
    };

    // When callback is received, we need to process authentication.
    $scope.signInCallback = function (authResult) {
        $scope.$apply(function () {
            $scope.processAuth(authResult);
        });
    };

    // Render the sign in button.
    $scope.renderSignInButton = function () {
        gapi.signin.render('signInButton',
            {
                'callback': $scope.signInCallback, // Function handling the callback.
                'clientid': CLIENT_ID, // CLIENT_ID from developer console which has been explained earlier.
                'requestvisibleactions': 'http://schemas.google.com/AddActivity', // Visible actions, scope and cookie policy wont be described now,
                // as their explanation is available in Google+ API Documentation.
                'scope': 'https://www.googleapis.com/auth/plus.login https://www.googleapis.com/auth/userinfo.email',
                'cookiepolicy': 'single_host_origin'
            }
        );
    };

    // Start function in this example only renders the sign in button.
    $scope.start = function () {
        $scope.renderSignInButton();
    };

    // Call start function on load.
    $scope.start();
}

/*GENERIC CONTROLLER*/
function GenericViewCtrl($http, $scope, $rootScope, $cookieStore, Activity) {

    $rootScope.ToggleMenu = function () {
        $rootScope.ShowMenu = showMenu = !showMenu;
    };
    $rootScope.ShowFilterControls = false;
    $rootScope.User = $rootScope.User || $cookieStore.get('User');
    $rootScope.SharedCards = [];

    var token = $rootScope.User != null ? $rootScope.User.Token : null;
    $http.defaults.headers.common['X-Authorization-Token'] = token;

    if ($rootScope.User && !$rootScope.User.Token) {
        $rootScope.User = null;
        $cookieStore.remove("User");
    }    

    $rootScope.MyBusidex = $rootScope.MyBusidex || [];
    $rootScope.LoginModel = {
        LoginText: $rootScope.User == null ? 'Login' : 'LogOut',
        LoginRoute: $rootScope.User == null ? '#/account/login' : '#/account/logout',
        HomeLink: $rootScope.User != null && $rootScope.User.StartPage == 'Organization' ? '#/groups/organization/' + $rootScope.User.Organizations[0].Item2 : '#/home'
    };
    $rootScope.IsLoggedIn = $rootScope.User != null;
    $scope.Waiting = false;
    if ($rootScope.EventSources == null) {
        Activity.query({},
            function (data) {
                $rootScope.EventSources = data.EventSources;

            },
            function () {

            });
    }
    $rootScope.NavItems = {};
    $rootScope.NavItems['Home'] = false;
    $rootScope.NavItems['Search'] = false;
    $rootScope.NavItems['Add'] = false;
    $rootScope.NavItems['Mine'] = false;
    $rootScope.NavItems['Groups'] = false;
    $rootScope.NavItems['Busidex'] = false;
    $rootScope.NavItems['Memberships'] = false;
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
            $rootScope.User != null &&
            !$rootScope.User.IsAdmin &&
            !$rootScope.User.HasCard &&
            (parseInt($rootScope.User.AccountTypeId) === ACCT_TYPE_PROFESSIONAL || parseInt($rootScope.User.AccountTypeId) === ACCT_TYPE_BETA);
    };

    var acctType = $rootScope.User != null ? parseInt($rootScope.User.AccountTypeId) : 0;
    $rootScope.MyBusidexMenuName = $rootScope.User != null && acctType === ACCT_TYPE_ORGANIZATION ? 'Edit Referrals' : 'My Busidex';
    $rootScope.MyBusidexName = $rootScope.User != null && acctType === ACCT_TYPE_ORGANIZATION ? 'Referrals' : 'My Busidex';

    $rootScope.ShowMemberships = function () {
        
        var hasOrgs = false;
        acctType = $rootScope.User != null ? parseInt($rootScope.User.AccountTypeId) : 0;
        if ($rootScope.IsLoggedIn && $rootScope.User != null && $rootScope.User.Organizations != null && acctType != ACCT_TYPE_ORGANIZATION) {
            var obj = $rootScope.User.Organizations;
            for (var prop in obj) {
                if (obj.hasOwnProperty(prop)) {
                    hasOrgs = true;
                    break;
                }
            }
        }
        return hasOrgs;
    }

    $rootScope.SEOCardNames = '';
    //Card.seoResults({},
    //    function(results) {
    //        $rootScope.SEOCardNames = results.CardList;
    //    },
    //    function() {
            
    //    });
    $rootScope.HomeHelpShare = "Send your card to others so they can have it in their collection and always be up to date.";
    $rootScope.HomeHelpDetails = "People could be trying to add your card to their collection. Be sure to include your Name and/or Company Name so your card can be found in a search.";
    $rootScope.HomeHelpTags = "Adding Tags helps your card get found in searches. Keep them relevant to what your business is about so people can find you more easily.";

    $rootScope.HomeShareLink = "#/busidex/mine?share=true";
    $rootScope.HomeEditLink = ($rootScope.User != null && $rootScope.User.HasCard) ? "#/card/edit/" + $rootScope.User.CardId + "?details=true" : "#/card/add";
    $rootScope.HomeTagLink = ($rootScope.User != null && $rootScope.User.HasCard) ? "#/card/edit/" + $rootScope.User.CardId + "?tags=true" : "#/card/add";
    
}
GenericViewCtrl.$inject = ['$http', '$scope', '$rootScope', '$cookieStore', 'Activity'];

/*ADMIN MENU*/
function AdminMenuCtrl($scope) {
    $scope.MenuItems = [];
    $scope.MenuItems.push({ Name: 'User List', Link: '#/admin/users' });
    $scope.MenuItems.push({ Name: 'New Cards', Link: '#/admin/newcards' });
    $scope.MenuItems.push({ Name: 'Card Owners', Link: '#/admin/owners' });
    $scope.MenuItems.push({ Name: 'Unowned Cards', Link: '#/admin/unownedcards' });
    $scope.MenuItems.push({ Name: 'Errors', Link: '#/admin/errors' });
    $scope.MenuItems.push({ Name: 'Popular Tags', Link: '#/admin/populartags' });
    $scope.MenuItems.push({ Name: 'System Tags', Link: '#/admin/systemtags' });
    $scope.MenuItems.push({ Name: 'Device Details', Link: '#/admin/devicedetails' });
    $scope.MenuItems.push({ Name: 'Back to Dashboard', Link: '#/admin/index' });

    $scope.IsAdmin = true;
}

/*ADMIN*/
function AdminCtrl($scope, $rootScope, $cookieStore, $location, $http, $sce, SharedCard) {

    $scope.filterOptions = {
        filterText: ''
    };
    $scope.ownerFilterOptions = {
        filterText: ''
    };
    $scope.Errors = {};
    $scope.errorGridOptions = {
        data: 'Errors',
        enableCellEdit: true,
        enableColumnResize: true
    };
    $scope.userGridOptions = {
        data: 'Users',
        enableCellEdit: true,
        enableColumnResize: true,
        filterOptions: $scope.filterOptions,
        useExternalFilter: true
    };
    $scope.ownerGridOptions = {
        data: 'Owners',
        enableCellEdit: true,
        enableColumnResize: true,
        filterOptions: $scope.ownerFilterOptions,
        useExternalFilter: true,
        columnDefs: [
            { field: 'UserId', displayName: 'User Id' },
            { field: 'UserName', displayName: 'User Name' },
            { field: 'Email', displayName: 'Email' },
            { field: 'LastActivityDate', displayName: 'Last Activity Date' },
            { field: 'CardFileId', displayName: 'Card', cellTemplate: '<div style="height: 80px;"><img src="https://az381524.vo.msecnd.net/cards/{{row.getProperty(col.field)}}" /></div>' }
        ],
        rowHeight: 140
    };
    $scope.deviceDetailsGridOptions = {
        data: 'DeviceDetails',
        enableCellEdit: false,
        enableColumnResize: true
    };

    $scope.NewTag = '';
    $scope.Totals = {};
    $scope.Totals.NewCards = {};
    $scope.Totals.Users = {};
    $scope.Totals.Errors = {};
    $scope.Totals.Tags = {};
    $scope.Totals.Owners = {};
    $scope.CurrentPage = 0;
    $scope.AllUsers = [];
    $scope.SelectAll = false;
    $scope.ToggleSelectAll = toggleSelectAll;
    $scope.GetHtml = getHtml;
    $scope.SendTestCommunication = sendTestCommunication;
    $scope.SendCommunications = sendToAll;
    $scope.Send30DaySharedCardReminders = send30DaySharedCardReminders;
    $scope.LoadDeviceSummary = loadDeviceSummary;
    $scope.LoadDeviceDetails = loadDeviceDetails;
    $scope.NewCardDays = [20, 30, 60, 120];
    $scope.ErrorsDays = [7, 14, 21, 30, 120];
    $scope.UnownedCards = [];
    $scope.DeviceSummary = [];
    $scope.DeviceDetails = [];

    $scope.NewCardDaysBack = $scope.NewCardDays[0];
    $scope.ErrorsDaysBack = $scope.ErrorsDays[0];

    $scope.AddSystemTag = function () {
        $http({
            method: 'POST',
            cache: false,
            url: ROOT + "/admin/AddSystemTag?text=" + $scope.NewTag
        })
            .success(function () {
                $scope.NewTag = '';
                loadSystemTags();
            })
            .error(function () {
                alert('There was a problem adding the new tag');

            });
    };

    

    function send30DaySharedCardReminders() {
        SharedCard.send30DaySharedCardReminders({},
            function() {

            },
            function() {

            }
        );
    }

    function loadErrors() {
        $scope.Errors = {};
        $http({
            method: 'GET',
            cache: false,
            url: ROOT + "/admin/ApplicationErrors?daysBack=" + $scope.ErrorsDaysBack
        })
            .success(function (response) {

                $scope.Errors = response.Errors;
                $scope.Totals.Errors = $scope.Errors.length;

                if ($scope.Errors.length == 0) {
                    $scope.Errors = [
                        {
                            Message: 'No Errors to report.',
                            InnerException: '',
                            StackTrace: '',
                            ErrorDate: '',
                            UserId: ''
                        }
                    ];
                }
                $scope.errorGridOptions = {
                    data: 'Errors',
                    enableCellEdit: true
                };
            })
            .error(function () {
                $scope.Errors = null;
                $scope.errorGridOptions = {
                    data: 'Errors'

                };
            });
    }

    function loadUsers() {

        $http({
            method: 'GET',
            cache: false,
            url: ROOT + "/admin/UserList"
        })
           .success(function (response) {

               $scope.Users = response.Users;
               $scope.Totals.Users = $scope.Users.length;

               $scope.userGridOptions = {
                   data: 'Users'
               };
           })
           .error(function () {
               $scope.Users = null;
               $scope.userGridOptions = {
                   data: 'Users'

               };
           });
    }

    function loadOwners() {
        $http({
            method: 'GET',
            cache: false,
            url: ROOT + "/admin/OwnerList"
        })
           .success(function (response) {

               $scope.Owners = response.Users;
               $scope.Totals.Owners = $scope.Owners.length;

               $scope.ownerGridOptions = {
                   data: 'Owners'
               };
           })
           .error(function () {
               $scope.Owners = null;
               $scope.ownerGridOptions = {
                   data: 'Owners'

               };
           });
    }

    function loadNewCards() {
        $http({
            method: 'GET',
            cache: false,
            url: ROOT + "/admin/newcards?daysBack=" + $scope.NewCardDaysBack
        })
            .success(function (response) {

                $scope.Cards = response.Cards;
                $scope.Totals.NewCards = $scope.Cards != null ? $scope.Cards.length : 0;
                for (var i = 0; i < $scope.Cards.length; i++) {
                    $scope.Cards[i].Created = new Date($scope.Cards[i].Created).toDateString();
                    $scope.Cards[i].Updated = new Date($scope.Cards[i].Updated).toDateString();
                }
            })
            .error(function () {

            });

    }

    function loadPopularTags() {
        $http({
            method: 'GET',
            cache: false,
            url: ROOT + '/admin/populartags'
        }).success(function (response) {
            $scope.Tags = [];
            $scope.SortFilter = 'Tag';
            $scope.Reverse = false;

            var tags = [];
            for (var i = 0; i < response.Tags.length; i++) {
                tags.push({ Tag: response.Tags[i].Tag, Count: response.Tags[i].Count });
            }
            $scope.Tags = tags;
            $scope.Totals.Tags = $scope.Tags.length;

        }).error(function () {

        });

    }

    function loadSystemTags() {
        $http({
            method: 'GET',
            cache: false,
            url: ROOT + '/admin/systemtags'
        }).success(function (response) {
            $scope.SystemTags = [];
            $scope.SortFilter = 'Tag';
            $scope.Reverse = false;

            var tags = [];
            for (var i = 0; i < response.Tags.length; i++) {
                tags.push(response.Tags[i]);
            }
            $scope.SystemTags = tags;

        }).error(function () {

        });

    }

    function loadUnownedCards() {
        $http({
            method: 'GET',
            cache: false,
            url: ROOT + '/admin/unownedcards'
        }).success(function (response) {

            //var pages = [];
            //var thisPage = new Array();
            for (var i = 0; i < response.Cards.length; i++) {
                response.Cards[i].Created = new Date(response.Cards[i].Created).toDateString();
                response.Cards[i].Updated = new Date(response.Cards[i].Updated).toDateString();
                response.Cards[i].DateSent = response.Cards[i].LastContactDate == null ? 'Never' : new Date(response.Cards[i].LastContactDate).toDateString();
                response.Cards[i].EmailSentTo = response.Cards[i].Email;
                //thisPage.push(response.Cards[i]);
                //if (thisPage.length == 20) {
                //    pages.push(thisPage);
                //    thisPage = new Array();
                //}
                $scope.UnownedCards.push(response.Cards[i]);
            }

            //$scope.UnownedCards = pages;

        }).error(function () {

        });

    }

    function loadCommunications() {
        
        $http({
                method: 'GET',
                cache: false,
                url: ROOT + "/admin/AdminCommunication?code=NewsLetter"
            })
            .success(function(response) {

                var contacts = [];
                for (var i = 0; i < response.Users.length; i++) {

                    var contact = {
                        UserName: response.Users[i].UserName,
                        Email: response.Users[i].Email,
                        Selected: false,
                        LastSharedDate: '(not available)'
                    }
                   
                    contacts.push(contact);
                }

                $scope.AllUsers = contacts;
                $scope.EmailTemplate = response.Template;
                $scope.EmailTemplate.Subject = $scope.EmailTemplate.Body = '';
                $sce.trustAs('css', $scope.EmailTemplate.Body);
            })
            .error(function() {
                $scope.AllUsers = null;
                
            });

    }

    function loadDeviceDetails() {
      
        $http({
            method: 'GET',
            cache: false,
            url: ROOT + "/admin/DeviceDetails"
        })
           .success(function (response) {

               $scope.DeviceDetails = response.Details;
            
               $scope.deviceDetailsGridOptions = {
                   data: 'DeviceDetails'
               };
           })
           .error(function () {
               $scope.DeviceDetails = null;
               $scope.deviceDetailsGridOptions = {
                   data: 'DeviceDetails'

               };
           });
    }

    function loadDeviceSummary() {
        $http({
            method: 'GET',
            cache: false,
            url: ROOT + "/admin/DeviceSummary"
        })
           .success(function (response) {
               $scope.DeviceSummary = response.Summary;
           })
           .error(function () {
               $scope.DeviceSummary = null;
           });
    }

    function sendTestCommunication() {
        var emailList = [];
        emailList.push('vinbrown2@gmail.com');
        emailList.push('vinbrown@cox.net');
        sendCommunications(emailList);
    }

    function sendToAll() {
        var emailList = [];
        for (var i = 0; i < $scope.AllUsers.length; i++) {

            if ($scope.AllUsers[i].Selected) {
                emailList.push($scope.AllUsers[i].Email);
            }
        }
        sendCommunications(emailList);
    }

    function sendCommunications(emailList) {

        $http({
            method: 'POST',
            cache: false,
            data: { Template: $scope.EmailTemplate, UserId: $rootScope.User.UserId, SendTo: emailList },
            url: ROOT + "/admin/SendCommunication"
        })
            .success(function () {

                
            })
            .error(function () {
                $scope.AllUsers = null;

            });
    }

    function getHtml() {
        if ($scope.EmailTemplate == null) return '';
        return $sce.trustAsHtml($scope.EmailTemplate.Body);
    }

    function toggleSelectAll() {
        $scope.SelectAll = !$scope.SelectAll;
        for (var i = 0; i < $scope.AllUsers.length; i++) {
            $scope.AllUsers[i].Selected = $scope.SelectAll;
        }
    }

    var interval = 1000 * 60 * 10;
    window.i_errors = setInterval(loadErrors, interval);
    window.i_users = setInterval(loadUsers, interval);
    window.i_owners = setInterval(loadOwners, interval);
    window.i_newcards = setInterval(loadNewCards, interval);
    window.i_popular = setInterval(loadPopularTags, interval);
    window.i_tags = setInterval(loadSystemTags, interval);
    window.i_summary = setInterval(loadDeviceSummary, interval);
   // window.i_unowned = setInterval(loadUnownedCards, 1000 * 60);

    function setCurrentPage(i) {
        $scope.CurrentPage = i;
    }

    $scope.SetCurrentPage = setCurrentPage;

    $scope.LoadErrors = function () {
        loadErrors();
    };

    $scope.LoadUsers = function () {
        loadUsers();
    };

    $scope.LoadOwners = function () {
        loadOwners();
    };

    $scope.LoadNewCards = function () {
        loadNewCards();
    };

    $scope.LoadTags = function () {
        loadPopularTags();
    };

    $scope.LoadDeviceSummary = function () {
        loadDeviceSummary();
    };

    $scope.SendOwnerEmail = function (card) {

        $http({
            method: 'POST',
            cache: false,
            url: ROOT + '/admin/SendOwnerEmails',
            params: { userId: $rootScope.User.UserId, cardId: card.CardId, email: card.EmailSentTo }
        }).success(function () {

            alert('Email Sent!');
            card.SentTo = '';
            card.DateSent = new Date().toDateString();

        }).error(function () {
            alert('There was a problem sending the email.');
        });
    };

    if ($rootScope.User == null) {
        $location.path("/account/login");
    } else {

        loadErrors();
        loadUsers();
        loadOwners();
        loadNewCards();
        loadPopularTags();
        loadUnownedCards();
        loadSystemTags();
        loadCommunications();
        loadDeviceSummary();
        loadDeviceDetails();
    }
}
AdminCtrl.$inject = ['$scope', '$rootScope', '$cookieStore', '$location', '$http', '$sce', 'SharedCard'];

/*ADMIN EDIT*/
function AdminEditCtrl($scope, $rootScope, $cookieStore, $location, $http) {


    $scope.Waiting = true;

    $scope.Cards = [];

    if ($rootScope.User == null) {
        $location.path("/account/login");
    } else {

        $http({
            method: 'GET',
            cache: false,
            url: ROOT + "/admin/UnownedCards"
        })
            .success(function (response) {

                var cards = [];

                for (var i = 0; i < response.Cards.length; i++) {

                    var c = response.Cards[i];
                    cards.push(
                        {
                            CardId: c.CardId,
                            Name: c.Name,
                            FrontFileId: c.FrontFileId,
                            CompanyName: c.CompanyName,
                            Email: c.Email,
                            PhoneNumber: c.PhoneNumbers[0] != null ? c.PhoneNumbers[0].Number : '',
                            Save: function () {

                                $http({
                                    method: 'PUT',
                                    cache: false,
                                    url: ROOT + "/admin/SaveCardInfo",
                                    data: this
                                }).success(function () {
                                }).error(function () {

                                });
                            }
                        });
                }

                $scope.Cards = cards;
                $scope.Waiting = false;
            })
            .error(function () {

            });

        $scope.Save = function () {

        };

    }
}
AdminEditCtrl.$inject = ['$scope', '$rootScope', '$cookieStore', '$location', '$http'];

/*GROUPS (LIST)*/
function GroupsCtrl($scope, $rootScope, $cookieStore, $location, Groups, Analytics, Busigroup) {
    Analytics.trackPage($location.path());
    $scope.Waiting = false;
    $scope.GetGroup = resetGroup;
    $scope.Tabs = {};
    $scope.GroupId = 0;
    $rootScope.SetCurrentMenuItem('Groups');
    $rootScope.ShowFilterControls = false;
    $scope.GroupHelp = 'BusiGroups and Memberships are ways you can organize your cards. Use a BusiGroup if the group is just for you to see. Choose from any card in your Busidex collection to add to the group. Memberships are groups that all members of the group can see. The person that creates the group has control over who is included.';

    function resetGroup(groupId) {
        //$scope.Group = {};
        $scope.Cards = [];

        //setTimeout('getGroup(' + groupId + ')',1000);
        getGroup(groupId);
    }
    getGroup = function (groupId) {        
        
        Busigroup.get({ id: groupId },
            function (response) {
                $scope.Group = {};
                $scope.Cards = [];
                $scope.Group = response.Model.Busigroup;
                $scope.Group.EmailLink = 'mailto:?bcc=';
                setCurrentTab($scope.Group.Description);
                for (var i = 0; i < response.Model.BusigroupCards.length; i++) {
                    var card = response.Model.BusigroupCards[i];
                    card.OrientationClass = card.Card.FrontOrientation == 'V' ? 'v_preview' : 'h_preview';
                    $scope.Group.EmailLink += ';' + card.Card.Email;
                    $scope.Cards.push(card);
                }
            },
            function () {

            });
    };

    var deleteGroup = function(groupId) {

        if (confirm('Are you sure you want to delete this group?')) {

            Groups.remove({ id: groupId },
                function () {

                    for (var i = 0; i < $scope.Groups.length; i++) {
                        if ($scope.Groups[i].GroupId == groupId) {
                            $scope.Groups.splice(i, 1);
                            break;
                        }
                    }
                },
                function () {

                });
        }
    }

    var setCurrentTab = function (currentTab) {

        for (var tab in $scope.Tabs) {
            if ($scope.Tabs.hasOwnProperty(tab)) {
                $scope.Tabs[tab] = false;
            }
        }
        $scope.Tabs[currentTab] = true;
    };

    if ($rootScope.User == null) {
        $location.path("/account/login");
    } else {
        Groups.get({ id: $rootScope.User.UserId },
            function (response) {
                $scope.Groups = response.Model;
                for (var i = 0; i < $scope.Groups.length; i++) {
                    $scope.Tabs[$scope.Groups[i].Name] = false;
                }
                
                if ($scope.Groups.length > 0) {
                    setCurrentTab($scope.Tabs[$scope.Groups[0].Description]);
                    getGroup($scope.Groups[0].GroupId);
                }
            },
            function () {

            }
        );

        

        $scope.DeleteGroup = deleteGroup;

    }
}
GroupsCtrl.$inject = ['$scope', '$rootScope', '$cookieStore', '$location', 'Groups', 'Analytics', 'Busigroup'];

/*GROUP DETAILS*/
function GroupDetailCtrl($scope, $rootScope, $cookieStore, $location, $route, Busigroup, GroupNotes) {

    $scope.Waiting = false;
    $rootScope.SetCurrentMenuItem('Groups');
    $scope.GroupId = $route.current.params.id;

    if ($rootScope.User == null) {
        $location.path("/account/login");
    } else {
        Busigroup.get({ id: $route.current.params.id },
            function (response) {

                $scope.Group = response.Model.Busigroup;
                
                $scope.Cards = [];
                for (var i = 0; i < response.Model.BusigroupCards.length; i++) {
                    var card = response.Model.BusigroupCards[i];
                    card.Ready = true;

                    card.OrientationClass = card.Card.FrontOrientation == 'V' ? 'v_preview' : 'h_preview';

                    $scope.Cards.push(card);
                }
            },
            function () {

            });

        //function deleteGroup() {
        //    if (confirm('Are you sure you want to delete this group?')) {

        //        Busigroup.remove({ id: $route.current.params.id },
        //            function () {
        //                $location.path("/groups/mine");
        //            },
        //            function () {

        //            });
        //    }
        //}
        //$scope.delete = deleteGroup;

        

        $(document).on("change", "textarea.groupNotes", function () {

            GroupNotes.update({ id: $(this).attr("ucId"), notes: escape($(this).val()) },
                function () {

                },
                function () {

                });
        });
    }

    $scope.showCards = function () {
        for (var i = 0; i < $scope.Cards.length; i++) {
            $scope.Cards[i].Ready = true;
        }
    }
}
GroupDetailCtrl.$inject = ['$scope', '$rootScope', '$cookieStore', '$location', '$route', 'Busigroup', 'GroupNotes'];

/*CREATE GROUP*/
function CreateGroupCtrl($scope, $rootScope, $cookieStore, $location, $route, Busidex, Groups) {

    $scope.Waiting = false;
    $rootScope.IsLoggedIn = $rootScope.User != null;
    $scope.ShowSelectedOnly = false;
    $rootScope.SetCurrentMenuItem('Groups');
    $scope.Group = {
        Description: '',
        CardIds: []
    };

    if ($rootScope.User == null) {
        $location.path("/account/login");
    } else {

        if (!$rootScope.MyBusidex || $rootScope.MyBusidex.length == 0) {
            Busidex.get({ },
                function (response) {

                    $rootScope.MyBusidex = $scope.MyBusidex = response.MyBusidex.Busidex;
                    for (var i = 0; i < $scope.MyBusidex.length; i++) {
                        $scope.MyBusidex[i].Selected = false;
                        $scope.MyBusidex[i].OrientationClass = $scope.MyBusidex[i].Card.FrontOrientation == 'V' ? 'v_preview' : 'h_preview';
                    }
                },
                function () {
                    alert('error');
                });
        } else {
            $scope.MyBusidex = $rootScope.MyBusidex;
            for (var i = 0; i < $scope.MyBusidex.length; i++) {
                $scope.MyBusidex[i].Selected = false;
                $scope.MyBusidex[i].OrientationClass = $scope.MyBusidex[i].Card.FrontOrientation == 'V' ? 'v_preview' : 'h_preview';
            }
        }

        $scope.ToggleSelected = function (card) {

            card.Selected = !card.Selected;
            if (card.Selected == true) {
                $scope.Group.CardIds.push(card.Card.CardId);
            } else {
                for (var j = 0; j < $scope.Group.CardIds.length; j++) {
                    if ($scope.Group.CardIds[j] == card.Card.CardId) {
                        $scope.Group.CardIds.splice(j, 1);
                        break;
                    }
                }
            }
        };
        $scope.save = function () {

            var data = { userId: $rootScope.User.UserId, groupTypeId: GROUPTYPE_PERSONAL, id: 0, cardIds: $scope.Group.CardIds.join(), description: $scope.Group.Description };

            Groups.post(data,
                function () {

                    $location.path("/groups/mine");

                },
                function () {
                    alert('error');
                });
        };
    }

}
CreateGroupCtrl.$inject = ['$scope', '$rootScope', '$cookieStore', '$location', '$route', 'Busidex', 'Groups'];

/*EDIT GROUP*/
function EditGroupCtrl($scope, $rootScope, $cookieStore, $location, $route, Busidex, Groups, Busigroup) {

    $scope.Waiting = false;
    $rootScope.IsLoggedIn = $rootScope.User != null;
    $scope.ShowSelectedOnly = false;
    $rootScope.SetCurrentMenuItem('Groups');
    $scope.Group = {
        GroupId: 0,
        Description: '',
        CardIds: []
    };

    if ($rootScope.User == null) {
        $location.path("/account/login");
    } else {

        Busigroup.get({ id: $route.current.params.id },
        function (response) {

            $scope.Group = response.Model.Busigroup;
            $scope.Group.CardIds = [];
            $scope.Cards = response.Model.BusigroupCards;

            Busidex.get({ },
            function (bdexResponse) {

                $rootScope.MyBusidex = $scope.MyBusidex = bdexResponse.MyBusidex.Busidex;

                for (var b = 0; b < $scope.MyBusidex.length; b++) {
                    var busidexCard = $scope.MyBusidex[b];

                    busidexCard.OrientationClass = busidexCard.Card.FrontOrientation == 'V' ? 'v_preview' : 'h_preview';

                    for (var c = 0; c < $scope.Cards.length; c++) {
                        var groupCard = $scope.Cards[c];
                        
                       
                        if (busidexCard.CardId == groupCard.CardId) {
                            busidexCard.Selected = true;
                            $scope.Group.CardIds.push(groupCard.CardId);
                            break;
                        }
                    }
                }
            },
            function () {
                alert('error');
            });

        },
        function () {

        });

        $scope.ToggleSelected = function (card) {

            card.Selected = !card.Selected;
            if (card.Selected == true) {
                $scope.Group.CardIds.push(card.Card.CardId);
            } else {
                for (var i = 0; i < $scope.Group.CardIds.length; i++) {
                    if ($scope.Group.CardIds[i] == card.Card.CardId) {
                        $scope.Group.CardIds.splice(i, 1);
                        break;
                    }
                }
            }
        };
        $scope.save = function () {

            var data = { userId: $rootScope.User.UserId, groupTypeId: $scope.Group.GroupTypeId, id: $scope.Group.GroupId, cardIds: $scope.Group.CardIds.join(), description: $scope.Group.Description };

            Groups.update(data,
                function () {
                    $location.path("/groups/mine");
                },
                function () {
                    alert('error');
                });
        };
    }
}
EditGroupCtrl.$inject = ['$scope', '$rootScope', '$cookieStore', '$location', '$route', 'Busidex', 'Groups', 'Busigroup'];

/*ORGANIZATION*/
function OrganizationCtrl($http, $scope, $rootScope, $cookieStore, $location, $route, $routeParams, Organizations, $sce, $timeout, Busigroup, Groups, Search, FileUpload, SharedCard, Busidex) {
    $scope.Tabs = {};
    $scope.Tabs['home'] = false;
    $scope.Tabs['details'] = false;
    $scope.Tabs['addmembers'] = false;
    $scope.Tabs['guests'] = false;
    $scope.Tabs['members'] = false;
    $scope.Tabs['addgroup'] = false;
    $scope.Tabs['referrals'] = false;
    $scope.Tabs['share'] = false;
    $scope.Tabs['groupdetails'] = false;
    $scope.Tabs["homepage"] = false;

    $scope.Organization = {};
    $scope.Organization.Groups = [];
    $scope.Organization.Referrals = [];
    $scope.GetHtml = getHtml;
    $scope.Role = '';
    $scope.EditDetails = editDetails;
    $scope.CancelEdit = cancelEdit;
    $scope.SaveDetails = saveDetails;
    $scope.ShowAddMembers = showAddMembers;
    $scope.HideAddMembers = hideAddMembers;
    $scope.AddMember = addMember;
    $scope.RemoveMember = removeMember;
    $scope.EditHomePage = false;
    $scope.EditHomePage = editHomePage;
    $scope.SaveHomePage = saveDetails;
    $scope.ToggleSelectedOrGoToDetails = toggleSelectedOrGoToDetails;
    $scope.ToggleReferral = toggleReferral;
    $scope.SaveGroup = addOrUpdateGroup;
    $scope.GetGroup = getGroup;
    $scope.AddGroup = addGroup;
    $scope.EditGroup = editGroup;
    $scope.DeleteGroup = deleteGroup;
    $scope.CurrentGroup = {};
    $scope.EnterSearch = enterSearch;
    $scope.DoSearch = doSearch;
    $scope.MemberSearch = {};
    $scope.MemberSearch.Cards = [];
    $scope.MemberSearch.Criteria = '';
    $scope.GetReferrals = getReferrals;
    $scope.UpdateGuestStatus = updateGuestStatus;
    $scope.SetShowControls = setShowControls;
    $scope.ShareReferrals = share;
    $scope.Waiting = false;
    $scope.Logo = null;
    $scope.SearchFilter = {};
    $scope.SearchFilter.SearchFilters = '';
    $scope.Errors = [];
    $scope.MemberReferrals = [];
    $scope.MyBusidex = [];
    $scope.EditingGroup = false;
    $scope.EditingDetails = false;
    $scope.IsOrganizationAdmin = isOrganizationAdmin;
    $scope.IsOrganizationMember = isOrganizationMember;

    $rootScope.ShowFilterControls = false;
    if ($rootScope.MyBusidex == null || $rootScope.MyBusidex.length == 0) {
        Busidex.get({},
            function (model) {
                $scope.MyBusidex = [];
                for (var j = 0; j < model.MyBusidex.Busidex.length; j++) {
                    
                    var busidexCard = model.MyBusidex.Busidex[j].Card;
                    model.MyBusidex.Busidex[j].OrientationClass = busidexCard.FrontOrientation == 'V' ? 'v_preview' : 'h_preview';
                    if (busidexCard.OwnerId != null) {
                        busidexCard.PersonalMessage = '';
                        $scope.MyBusidex.push(model.MyBusidex.Busidex[j]);
                    }
                }
            },
            function() {
                alert('error');
            });
    } else {
        $scope.MyBusidex = [];
        for (var i = 0; i < $rootScope.MyBusidex.length; i++) {

            var thisCard = $rootScope.MyBusidex[i];
            if (thisCard.OwnerId != null) {
                thisCard.PersonalMessage = '';
                $scope.MyBusidex.push(model.MyBusidex.Busidex[i]);
            }
        }
        
    }

    $rootScope.SetCurrentMenuItem('Memberships');

    Organizations.select({ id: $routeParams.id},
        function (data) {
            loadOrganization(data);
        },
        function() {
            
        });

    function isOrganizationAdmin() {
        return $scope.Organization.Role == 'Admin';
    }

    function isOrganizationMember() {
        return $scope.Organization.Role == 'Member';
    }

    function cancelEdit() {
        Organizations.select({ id: $scope.Organization.OrganizationId },
        function (data) {
            loadOrganization(data);
            setCurrentTab('home');
        },
        function () {

        });
    }

    function loadOrganization(data) {
        $scope.Organization = data.Model || {};
      
        $scope.Organization.LogoFilePath = $scope.Organization.LogoFilePath + $scope.Organization.LogoFileName + '.' + $scope.Organization.LogoType;

        if ($scope.Organization.LogoFilePath != null) {
            $scope.Organization.LogoFilePath = $scope.Organization.LogoFilePath.replace('http:', 'https:');
        }
      
        $scope.Organization.Role = data.OrgRole;

        if ($scope.Organization == null || $scope.Organization.OrganizationId == 0 || 
            ($scope.Organization.Name == null && $scope.Organization.Description == null && $scope.Organization.Url == null && $scope.Organization.Phone1 == null)) {
            setCurrentTab('details');
            return;
        }
        $scope.Organization.Cards = [];
        $scope.Organization.CardIds = [];
        $scope.Organization.Referrals = [];
        $scope.Organization.Groups = data.Groups;        

        for (var j = 0; j < $scope.Organization.Groups.length; j++) {
            $scope.Tabs['_' + $scope.Organization.Groups[j].GroupId] = false;
        }
        getMembers();
        getReferrals();
        getGuests();
    }

    function deleteGroup(groupId) {

        if (!confirm('Are you sure you want to delete this group?')) {
            return;
        }

        Groups.remove({ id: groupId },
                function () {

                    setCurrentTab('home');

                    Organizations.select({ id: $routeParams.id },
                        function (org) {
                            loadOrganization(org);
                        },
                        function () {

                        });

                },
                function () {
                    alert('error');
                });
    }

    function addOrUpdateGroup() {

        var groupId = $scope.CurrentGroup.GroupId;
        if (groupId == null) groupId = 0;

        var data = { userId: $scope.Organization.OrganizationId, groupTypeId: GROUPTYPE_ORGANIZATION, id: groupId, cardIds: $scope.Organization.CardIds.join(), description: $scope.CurrentGroup.Description };

        if ($scope.CurrentGroup.GroupId > 0) {
            Groups.update(data,
                function () {

                    $scope.Tabs['addgroup'] = false;
                    Organizations.select({ id: $routeParams.id },
                        function (org) {
                            loadOrganization(org);
                            $scope.Tabs['_' + $scope.CurrentGroup.GroupId] = true;
                            getGroup($scope.CurrentGroup.GroupId);
                            $scope.EditingGroup = false;
                        },
                        function () {

                        });
                    
                },
                function () {
                    alert('error');
                });
        } else {
            Groups.post(data,
                function() {

                    $scope.Tabs['addgroup'] = false;

                    Organizations.select({ id: $routeParams.id },
                        function(org) {
                            loadOrganization(org);
                            var newId = $scope.Organization.Groups[$scope.Organization.Groups.length - 1].GroupId;
                           // $scope.Tabs['_' + newId] = true;
                            getGroup(newId);
                            $scope.Tabs['groupdetails'] = true;
                        },
                        function() {

                        });
                },
                function() {
                    alert('error');
                });
        }
    }

    function addGroup() {
        setCurrentTab('addgroup');
        $scope.EditingGroup = false;
        $scope.CurrentGroup = {};
        $scope.Organization.CardIds = [];

        for (var b = 0; b < $scope.Organization.Cards.length; b++) {
            var busidexCard = $scope.Organization.Cards[b];
            busidexCard.Selected = false;
        }
    }

    function editDetails() {
        $scope.EditingDetails = true;
        setCurrentTab('details');
        $scope.Tabs['home'] = true;
    }

    function editGroup(id) {
        setCurrentTab('_' + id);
        $scope.Tabs['addgroup'] = true;
        $scope.EditingGroup = true;
        $scope.Organization.CardIds = [];
        $scope.CurrentGroup.GroupId = id;
        for (var b = 0; b < $scope.Organization.Cards.length; b++) {
            var busidexCard = $scope.Organization.Cards[b];
            for (var c = 0; c < $scope.CurrentGroup.Cards.length; c++) {
                var groupCard = $scope.CurrentGroup.Cards[c];
                busidexCard.Selected = false;
                if (busidexCard.CardId == groupCard.CardId) {
                    busidexCard.Selected = true;
                    $scope.Organization.CardIds.push(busidexCard.CardId);
                    break;
                }
            }
        }
    }

    function toggleReferral(card) {
        card.Selected = !card.Selected;
        if (card.Selected == true) {
            $scope.MemberReferrals.push(card.CardId);
        } else {
            for (var j = 0; j < $scope.Organization.CardIds.length; j++) {
                if ($scope.Organization.CardIds[j] == card.CardId) {
                    $scope.MemberReferrals.splice(j, 1);
                    break;
                }
            }
        }
    }

    function toggleSelectedOrGoToDetails(card) {

        if ($scope.Tabs['addmembers'] == true || $scope.Tabs['addgroup'] == true) {
            card.Selected = !card.Selected;
            if (card.Selected == true) {
                $scope.Organization.CardIds.push(card.CardId);
            } else {
                for (var j = 0; j < $scope.Organization.CardIds.length; j++) {
                    if ($scope.Organization.CardIds[j] == card.CardId) {
                        $scope.Organization.CardIds.splice(j, 1);
                        break;
                    }
                }
            }
        } else {
            $location.path('/card/details/' + card.CardId);
        }
    }

    function editHomePage() {
        //if ($scope.Organization.Role == 'Admin') {
        //    $scope.EditHomePage = !$scope.EditHomePage;
        //}
        setCurrentTab("homepage");
    }

    function getMembers() {

        $scope.Organization.Cards = [];
        Organizations.getMembers({ organizationId: $scope.Organization.OrganizationId },
        function (cardsData) {

            for (var j = 0; j < cardsData.Model.length; j++) {
                var card = cardsData.Model[j];

                card.OrientationClass = card.FrontOrientation == 'V' ? 'v_preview' : 'h_preview';
                card.Selected = false;
                $scope.Organization.Cards.push(card);
            }

        },
        function () {

        });
    }

    function getGuests() {

        Organizations.getGuests({ organizationId: $scope.Organization.OrganizationId },
            function(response) {

                $scope.Organization.Guests = response.Guests;
            },
            function() {

            });
    }

    function updateGuestStatus(guest, newStatus) {

        guest.AddStatus = newStatus;
        for (var g = 0; g < $scope.Organization.Guests.length; g++) {
            if ($scope.Organization.Guests[g].UserCardId == guest.UserCardId) {
                $scope.Organization.Guests.splice(g, 1);
            }        
        }

        Busidex.updateCardStatus(guest,
            function() {
                
            },
            function() {
                
            }
        );
    }

    function getHtml() {
        if ($scope.Organization == null) return '';
        return $sce.trustAsHtml($scope.Organization.HomePage);
    }

    function saveDetails() {

        $scope.Errors = [];

        if ($scope.Organization == null || $scope.Organization.OrganizationId == null || $scope.Organization.OrganizationId == 0) {
            Organizations.post($scope.Organization,
                function () {
                    $scope.DetailsSaved = true;
                    $scope.EditingDetails = false;
                    if ($scope.Logo != null) {
                        FileUpload.UploadFile($scope.Logo, ROOT + '/Organization/UpdateLogo?id=' + $scope.Organization.OrganizationId,
                            function() {
                                Organizations.select({ id: $routeParams.id },
                                    function(data) {
                                        loadOrganization(data);
                                        $timeout(function() {
                                            $scope.DetailsSaved = false;
                                            setCurrentTab('home');
                                        }, 3000);
                                    },
                                    function() {

                                    });
                            });
                    } else {
                        
                        Organizations.select({ id: $routeParams.id },
                                    function (data) {
                                        loadOrganization(data);
                                        $timeout(function () {
                                            $scope.DetailsSaved = false;
                                            setCurrentTab('home');
                                        }, 3000);
                                    },
                                    function () {

                                    });
                    }

                },
                function () {

                }
            );
        } else {
            Organizations.update($scope.Organization,
                function () {

                    $scope.DetailsSaved = true;
                    $scope.EditingDetails = false;
                    if ($scope.Logo != null) {
                        FileUpload.UploadFile($scope.Logo, ROOT + '/Organization/UpdateLogo?id=' + $scope.Organization.OrganizationId,
                            function() {
                                Organizations.select({ id: $routeParams.id },
                                    function(data) {
                                        loadOrganization(data);
                                        $timeout(function() {
                                            $scope.DetailsSaved = false;
                                            setCurrentTab('home');
                                        }, 3000);
                                    },
                                    function() {

                                    });
                            });
                    } else {
                        Organizations.select({ id: $routeParams.id },
                                    function (data) {
                                        loadOrganization(data);
                                        $timeout(function () {
                                            $scope.DetailsSaved = false;
                                            setCurrentTab('home');
                                        }, 3000);
                                    },
                                    function () {

                                    });
                    }

                },
                function(response) {
                    if (response.status == 400) {
                        $scope.Errors.push(response.data.Message);                        
                    }
                }
            );
        }
    }

    function removeMember(card) {

        var cardId = card.CardId;
        var message = card.IsMyCard
            ? 'Are you sure you want to leave this organization?'
            : 'Are you sure you want to remove this member?';

        if (confirm(message)) {
            for (var j = 0; j < $scope.Organization.Cards.length; j++) {
                var orgCard = $scope.Organization.Cards[j];
                if (orgCard.CardId == cardId) {
                    $scope.Organization.Cards.splice(j, 1);
                    Organizations.remove({ organizationId: $scope.Organization.OrganizationId, cardId: cardId },
                    function () {

                    },
                    function () {

                    }
                );
                    break;
                }
            }
        }
    }

    function showAddMembers() {
        $scope.SearchFilter.SearchFilters = '';
        $scope.MemberSearch.Cards = [];
        
        setCurrentTab('addmembers');
        $scope.Tabs['members'] = true;
    }

    function hideAddMembers() {
        $scope.Tabs['addmembers'] = false;
    }

    function addMember(card) {
        
        for (var j = 0; j < $scope.MemberSearch.Cards.length; j++) {
            var memberCard = $scope.MemberSearch.Cards[j];
            if (memberCard.CardId == card.CardId) {
                $scope.MemberSearch.Cards.splice(j, 1);
                card.FrontType = card.FrontFileType;
                card.OrientationClass = card.FrontOrientationClass;
                $scope.Organization.Cards.push(card);

                Organizations.addMember({ organizationId: $scope.Organization.OrganizationId, cardId: card.CardId },
                    function() {

                    },
                    function() {

                    }
                );
                break;
            }
        }
    }

    function enterSearch() {
        if (event.keyCode == 13) {
            doSearch();
        }
    }

    function doSearch() {
        $scope.Waiting = true;
        $scope.MemberSearch.Cards = [];
        var model = {
            UserId: $rootScope.User.UserId,
            Criteria: $scope.MemberSearch.Criteria,
            SearchText: $scope.MemberSearch.Criteria,
            CardType: 1
        }

        if (model.Distance == 0 || model.Distance == null) {
            model.Distance = 25;
        }

        Search.post(model,
            function (response) {

                $scope.Waiting = false;
                
                $scope.MemberSearch.Criteria = response.SearchModel.SearchText;

                for (var j = 0; j < response.SearchModel.Results.length; j++) {

                    var modelCard = response.SearchModel.Results[j];
                    modelCard.FrontOrientationClass = modelCard.FrontOrientation == 'V' ? 'v_preview' : 'h_preview';
                    //var memberExists = false;
                    for (var c = 0; c < $scope.Organization.Cards.length; c++) {

                        modelCard.IsMember = false;
                        if ($scope.Organization.Cards[c].CardId == modelCard.CardId) {
                            //memberExists = true;
                            modelCard.IsMember = true;
                            break;
                        }                        
                    }
                    //if (!memberExists) {
                        $scope.MemberSearch.Cards.push(modelCard);
                    //}

                }
               

            }, function (data) {
                $scope.resultData = data;
                alert("Getting search results failed.");
                $scope.Waiting = false;
            });
    }    

    function getGroup(groupId) {

        if (groupId == null) {
            getMembers();
            setCurrentTab('home');
            return;
        }

        Busigroup.get({ id: groupId },
            function (response) {
                //
                $scope.Group = {};
                $scope.Cards = [];
                $scope.CurrentGroup = response.Model.Busigroup;
                $scope.CurrentGroup.Cards = [];

                setCurrentTab('_' + $scope.CurrentGroup.GroupId);
                $scope.Tabs['groupdetails'] = true;
                for (var i = 0; i < response.Model.BusigroupCards.length; i++) {
                    var card = response.Model.BusigroupCards[i];

                    card.OrientationClass = card.FrontOrientation == 'V' ? 'v_preview' : 'h_preview';

                    $scope.CurrentGroup.Cards.push(card);
                }
            },
            function () {

            });
    }

    function getReferrals() {
        
        $scope.Organization.Referrals = [];
        $scope.SearchFilter.SearchFilters = '';
        Organizations.getReferrals({ organizationId: $scope.Organization.OrganizationId },
        function (cardsData) {

            for (var i = 0; i < cardsData.Model.length; i++) {
                var card = cardsData.Model[i];
                card.ShowControls = false;
                if (card.Notes != null) {
                    card.Notes = unescape(card.Notes);
                }
                card.OrientationClass = ((card.Card.FrontOrientation === 'V') ? 'v_preview' : 'h_preview');
                $scope.Organization.Referrals.push(card);
            }

        },
        function () {

        });
    }

    function setShowControls(thisCard, show) {
        
        for (var i = 0; i < $scope.Organization.Referrals.length; i++) {
            var card = $scope.Organization.Referrals[i];
            card.ShowControls = false;
        }
        thisCard.ShowControls = show;
        
    }

    function share(){

        $scope.MemberReferrals = [];
        var sharedCards = [];
        for (var i = 0; i < $scope.MyBusidex.length; i++) {
            var card = $scope.MyBusidex[i];
            if (card.Selected) {

                var sharedCard = {
                    CardId: card.CardId,
                    SendFrom: $rootScope.User.UserId,
                    Email: $scope.Organization.AdminEmail,
                    ShareWith: $scope.Organization.UserId,
                    SharedDate: new Date(),
                    Accepted: null,
                    Declined: null,
                    Recommendation: card.Recommendation
                }
                sharedCards.push(sharedCard);
                card.Selected = false;
            }
        }

        SharedCard.post(sharedCards,
            function () {
                
                $scope.MemberReferrals = [];
                
                alert('Your referrals have been submitted to the organization.');
            },
            function () {
                alert('There was a problem sharing your referrals.');
            });
    }

    var setCurrentTab = function (currentTab) {
        $scope.SearchFilter.SearchFilters = '';
        for (var tab in $scope.Tabs) {
            if ($scope.Tabs.hasOwnProperty(tab)) {
                $scope.Tabs[tab] = false;
            }
        }
        $scope.Tabs[currentTab] = true;
    };

    $scope.SetCurrentTab = setCurrentTab;

    setCurrentTab('home');
}
OrganizationCtrl.$inject = ['$http', '$scope', '$rootScope', '$cookieStore', '$location', '$route', '$routeParams', 'Organizations', '$sce', '$timeout', 'Busigroup', 'Groups', 'Search', 'FileUpload', 'SharedCard', 'Busidex'];

/*MY ACCOUNT*/
function AccountCtrl($scope, $rootScope, $cookieStore, Account, Users, $location, $routeParams, Analytics, AccountType, Settings, Password, UserName, $timeout) {

    Analytics.trackPage($location.path());
    $rootScope.IsLoggedIn = $rootScope.User != null;
    $rootScope.ShowFilterControls = false;
    $scope.DeleteAccount = deleteAccount;

    $rootScope.SetCurrentMenuItem('Account');

    $scope.User = $rootScope.User;

    $scope.Tabs = {};
    $scope.Tabs['info'] = false;
    $scope.Tabs['settings'] = false;
    $scope.Tabs['username'] = false;
    $scope.Tabs['password'] = false;
    $scope.Tabs['accounttype'] = false;
    $scope.Tabs['abuse'] = false;
  
    $scope.SetCurrentTab = function (currentTab) {

        for (var tab in $scope.Tabs) {
            if ($scope.Tabs.hasOwnProperty(tab)) {
                $scope.Tabs[tab] = false;
            }
        }
        $scope.Tabs[currentTab] = true;
    };

    if ($routeParams.tab == 'accounttype') {
        $scope.SetCurrentTab('accounttype');
    } else {
        $scope.SetCurrentTab('info');
    }
    $scope.Waiting = false;

    //ACCOUNT INFO
    $scope.AccountInfoModel = {};
    $scope.AccountInfoModel.UserInfo = {};
    $scope.AccountInfoModel.AccountTypeId = 0;
    $scope.AccountInfoModel.Saved = $scope.AccountInfoModel.Error = false;
    $scope.AccountInfoModel.EmailOkToUse = '';
    $scope.AccountInfoModel.OriginalEmail = "";    

    Account.get({ id: $rootScope.User.UserId },
        function (busidexUser) {
            $scope.AccountInfoModel.UserInfo = busidexUser;
            $scope.AccountInfoModel.OriginalEmail = busidexUser.Email;
        },
        function () {
            alert('error');
        });

    $scope.CheckEmailAvailability = function () {

        if ($scope.AccountInfoModel.UserInfo.Email == $scope.OriginalEmail) {
            $scope.AccountInfoModel.EmailOkToUse = '';
            return;
        }

        if ($scope.AccountInfoModel.UserInfo.Email == undefined ||
            $scope.AccountInfoModel.UserInfo.Email.length < 5 ||
            $scope.AccountInfoModel.UserInfo.Email.length > 20) {
            return;
        }

        Users.get({ name: $scope.AccountInfoModel.UserInfo.Email },
            function (data) {
                $scope.AccountInfoModel.EmailOkToUse = data.User == null ? 'OK' : 'USED';
            },
            function () {
                alert('There was a problem validating your username');
            });
    };
    $scope.SaveAccountInfo = function () {
        $scope.AccountInfoModel.Saved = false;
        Account.update($scope.AccountInfoModel.UserInfo,
        function () {
            $scope.AccountInfoModel.Saved = true;
            $timeout(function () {
                $scope.AccountInfoModel.EmailOkToUse = '';
                $scope.AccountInfoModel.Saved = false;
                
            }, 5000);
        },
        function () {
            $scope.AccountInfoModel.Saved = false;
        });
    };

    function deleteAccount() {

        $location.path('/account/delete');
    }

    // ACCOUNT TYPE
    $scope.AccountTypeModel = {};
    AccountType.get({ id: $rootScope.User.UserId },
        function (results) {

            $scope.Plans = results.Plans;
            for (var i = 0; i < $scope.Plans.length; i++) {
                $scope.Plans[i].Selected = $scope.Plans[i].AccountTypeId == results.SelectedPlanId;
                if ($scope.Plans[i].Selected) {
                    $scope.AccountTypeModel.AccountTypeId = $scope.Plans[i].AccountTypeId;
                }
            }

        },
        function () {
        });

    $scope.SaveAccountType = function () {

        $scope.AccountTypeModel.Saved = $scope.AccountTypeModel.Error = false;
        AccountType.update({ userAccountId: $rootScope.User.UserAccountId, accountTypeId: $scope.AccountTypeModel.AccountTypeId },
        function () {
            $scope.AccountTypeModel.Saved = true;

            $rootScope.User.AccountTypeId = $scope.AccountTypeModel.AccountTypeId;
            $cookieStore.put('User', $rootScope.User);
        },
        function (response) {
            $scope.AccountTypeModel.Saved = false;
            $scope.AccountTypeModel.Error = true;
            //console.log(response.status);
        });
    };

    // SETTINGS
    $scope.SettingsInfo = {};
    Settings.get({ id: $rootScope.User.UserId },
        function (response) {
            response.Model.UserId = $rootScope.User.UserId;
            $scope.SettingsInfo = response.Model;
        },
        function () {

        });

    $scope.SaveSettings = function () {

        Settings.update($scope.SettingsInfo,
            function () {
                $location.path("/account/index");
            },
            function () {
                alert('settings not changed');
            });
        return true;
    };

    // PASSWORD CHANGE
    $scope.PasswordInfo = {};
    $scope.PasswordModel = {};
    $scope.PasswordModel.SavingPassword = false;
    $scope.PasswordModel.PasswordError = false;

    $scope.PasswordModel.Validate = function () {

        return ($scope.PasswordInfo.OldPassword != null && $scope.PasswordInfo.OldPassword.length > 0) &&
            ($scope.PasswordInfo.NewPassword != null && $scope.PasswordInfo.NewPassword.length > 0) &&
            $scope.PasswordInfo.OldPassword != $scope.PasswordInfo.NewPassword &&
            $scope.PasswordInfo.NewPassword == $scope.PasswordInfo.ConfirmPassword;
    };
    Password.get(
        function (response) {
            response.Model.UserId = $rootScope.User.UserId;
            $scope.PasswordInfo = response.Model;
        },
        function () {

        });

    $scope.SavePassword = function () {
       
        Password.update($scope.PasswordInfo,
            function () {
                $scope.PasswordModel.PasswordError = false;
                $scope.PasswordModel.SavingPassword = true;
                $timeout(function () {
                   
                    $scope.PasswordModel.SavingPassword = false;
                    
                }, 5000);
                
            },
            function () {
                $scope.PasswordModel.PasswordError = true;
                $timeout(function () {

                    $scope.PasswordModel.PasswordError = false;

                }, 5000);
            });
        return true;
    };

    // USER NAME CHANGE
    $scope.UserNameModel = {};
    $scope.UserNameModel.UserName = $rootScope.User.UserName;
    $scope.UserNameModel.NewUserName = '';
    $scope.UserNameModel.UserNameOkToUse = false;
    $scope.UserNameChanged = false;
    $scope.BadUserName = false;
    $scope.Error = false;

    $scope.CheckUserNameAvailability = function () {

        if ($scope.UserNameModel.UserName == undefined || $scope.UserNameModel.UserName.length < 5 || $scope.UserNameModel.UserName.length > 20) {
            return;
        }

        Users.get({ name: $scope.UserNameModel.UserName },
            function (data) {
                $scope.UserNameModel.UserNameOkToUse = data.User == null ? 'OK' : 'USED';
            },
            function () {
                alert('There was a problem validating your username');
            });
    };
    $scope.SaveUserName = function () {

        $scope.UserNameModel.UserNameChanged = false;
        $scope.UserNameModel.BadUserName = false;
        $scope.UserNameModel.Error = false;

        UserName.update({ userId: $rootScope.User.UserId, name: $scope.UserNameModel.NewUserName },
            function () {
                $scope.UserNameModel.UserNameChanged = true;
                $rootScope.User.UserName = $scope.UserNameModel.NewUserName;
                $scope.UserNameModel.UserName = $scope.UserNameModel.NewUserName;

                $cookieStore.put('User', $rootScope.User);
            },
            function (response) {
                if (response.status == 400 || response.status == 404) {
                    $scope.UserNameModel.BadUserName = true;
                } else {
                    $scope.UserNameModel.Error = true;
                }
            });
    };
}
AccountCtrl.$inject = ['$scope', '$rootScope', '$cookieStore', 'Account', 'Users', '$location', '$routeParams', 'Analytics', 'AccountType', 'Settings', 'Password', 'UserName', '$timeout'];

/*DELETE ACCOUNT*/
function DeleteAccountCtrl($scope, $rootScope, $cookieStore, Account, $location, Analytics) {

    Analytics.trackPage($location.path());

    $scope.User = $rootScope.User;
    $scope.Model = {};
    $scope.Model.Errors = [];

    $scope.DeleteAccountModel = {
        UserName: '',
        Password: ''
    };
    $scope.DeleteAccount = deleteAccount;

    function deleteAccount() {
        var _model = $scope.DeleteAccountModel;
        Account.checkUser(_model,
            function(data) {

                Account.remove({ token: data.Token },
                    function() {
                        $location.path('/account/logout');
                    },
                    function() {
                        
                    });
            },
            function(response) {
                if (response.status == 401) {
                    $scope.Model.Errors.push(response.data.Message);
                }
            });
    }
}
DeleteAccountCtrl.$inject = ['$scope', '$rootScope', '$cookieStore', 'Account', '$location', 'Analytics'];

/*REGISTRATION COMPLETE*/
function RegistrationCompleteCtrl($scope, $rootScope, $cookieStore, $route, $routeParams, Registration, $location, Analytics, $http) {
    Analytics.trackPage($location.path());
    $scope.Waiting = false;
    $rootScope.IsLoggedIn = $rootScope.User !== null;

    $scope.RegistrationErrors = [];
    var _token = $routeParams.token;
   
    Registration.update({ token: _token },
        function (user) {
            _gaq.push(['_trackEvent', 'Registration', 'Complete', _token]);

            $rootScope.User = user;
            $rootScope.User.RememberMe = false;

            var acctType = $rootScope.User != null ? parseInt($rootScope.User.AccountTypeId) : 0;
            $rootScope.MyBusidexMenuName = parseInt($rootScope.User.AccountTypeId) === ACCT_TYPE_ORGANIZATION ? 'Edit Referrals' : 'My Busidex';
            $rootScope.MyBusidexName = $rootScope.User != null && acctType === ACCT_TYPE_ORGANIZATION ? 'Referrals' : 'My Busidex';

            $rootScope.SearchModel = null;

            var token = $rootScope.User != null ? $rootScope.User.Token : null;
            $http.defaults.headers.common['X-Authorization-Token'] = token;

            $rootScope.LoginModel = {
                LoginText: $rootScope.User == null ? 'Login' : 'LogOut',
                LoginRoute: $rootScope.User == null ? '#/account/login' : '#/account/logout',
                HomeLink: $rootScope.User != null && $rootScope.User.StartPage == 'Organization' ? '#/groups/organization/' + $rootScope.User.Organizations[0].Item2 : '#/home'
            }

            $cookieStore.put('User', user);
            $rootScope.IsLoggedIn = $rootScope.User != null;

            // redirect the user to the card page if they have a beta or professional account
            if ($rootScope.ShowAddCard()) {
                $location.path(user.CardId > 0 ? '/card/edit/' + card.CardId : '/card/add/mine');
            } else {
                $location.path('/home');
            }
        },
        function (error) {
            if (error == null || error.Message == null) {
                error = { Message: 'There was a problem completing your registration.' };
            }
            $scope.RegistrationErrors.push(error.Message);
        });
}
RegistrationCompleteCtrl.$inject = ['$scope', '$rootScope', '$cookieStore', '$route', '$routeParams', 'Registration', '$location', 'Analytics', '$http'];

/*REGISTRATION*/
function RegistrationViewCtrl($scope, $http, $rootScope, $cookieStore, Registration, $route, $routeParams, Users, $location, Analytics) {

    /*
    registration scenarios:
    1. http://local.busidex.com/#/account/register?token=<guid> send owner token
    2. http://local.busidex.com/#/account/register?sId=<guid> organization invitation
    3. http://local.busidex.com/#/account/register?promo_code=<code> invite with promo code (system tag)
    */

    Analytics.trackPage($location.path());
    $scope.Waiting = true;
    $rootScope.IsLoggedIn = $rootScope.User != null;

    var token = $route.current.params.token;
    var inviteCardToken = $route.current.params.sId;

    $scope.ShowOwnerCard = false;
    $scope.CurrentStep = 1;
    $scope.RegistrationErrors = [];
    $scope.Model = {};

    $http.defaults.useXDomain = true;

    $scope.EmailOkToUse = '';

    Registration.get({ id: token },
        function (model) {            
            model.DisplayName = '';
            model.Email = '';
            model.ConfirmEmail = '';
            model.Password = '';
            model.ConfirmPassword = '';
            model.HumanQuestion = "How many letters are there in the word Busidex? (Hint: use a number, don't spell out the word.)";
            model.AccountTypeId = 6;
            model.ShowOwnerCard = model.Card !== null;
            model.HasBackImage = model.Card !== null && model.Card.BackFileId.toUpperCase() != 'B66FF0EE-E67A-4BBC-AF3B-920CD0DE56C6';
            model.CardOwnerToken = token;
            model.PromoCode = $routeParams.promo_code;
            model.PromoCodeValid = true;
            model.IsMobile = false;
            model.Agree = false;
            model.InviteCardToken = inviteCardToken;
            model.ReferredBy = '';
            model.ReferredByPerson = '';
            model.ReferredByOther = '';

            model.ReferralTypes = {
                Email: 'Email',
                Shared: 'Shared',
                Personal: 'Personal',
                Other: 'Other'
            };

            if (model.Card !== null) {
                model.FrontOrientationClass = model.Card.FrontOrientation == "H" ? "h_preview" : "v_preview";
                model.BackOrientationClass = model.Card.BackOrientation == "H" ? "h_preview" : "v_preview";

                model.Card.CompanyName = encodeURIComponent(model.Card.CompanyName);
                model.Card.Name = encodeURIComponent(model.Card.Name);
                model.Card.Title = encodeURIComponent(model.Card.Title);
            }
            $scope.Model = model;

            $scope.Waiting = false;
        },
        function () {
            $scope.Waiting = false;
        });

    function checkReferredTypeValid(model) {

        var retVal = '';
        model.MissingPersonalReference = false;
        model.MissingOtherReference = false;

        return retVal;

        switch (model.ReferredBy) {
        case model.ReferralTypes.Personal:
        {
            if (model.ReferredByPerson.trim() === '') {
                model.MissingPersonalReference = true;
                retVal = 'Please fill in who referred you to us';
            }
            break;
        }
        case model.ReferralTypes.Other:
        {
            if (model.ReferredByOther.trim() === '') {
                model.MissingOtherReference = true;
                retVal = 'Please fill in who referred you to us';
            }
            break;
        }
        case model.ReferralTypes.Email:
        {
            break;
        }
        case model.ReferralTypes.Shared:
        {
            break;
        }

        default:
        {
            retVal = 'Please indicate how you heard about us';
            break;
        }
        }
        return retVal;
    };

    function formatReferredBy(model) {
        switch (model.ReferredBy) {
        case model.ReferralTypes.Email: // all good
        case model.ReferralTypes.Shared: // all good
        case model.ReferralTypes.Personal:
        {
            model.ReferredBy += ': ' + model.ReferredByPerson;
            break;
        }
        case model.ReferralTypes.Other:
        {
            model.ReferredBy += ': ' + model.ReferredByOther;
        }
        }
    }

    $scope.CheckPromoCode = function () {


        $scope.Model.PromoCodeValid = false;

    };
   
    $scope.CheckEmailAvailability = function () {

        if ($scope.Model.Email == undefined || $scope.Model.Email.length < 5) {
            return;
        }

        Users.get({ name: $scope.Model.Email },
            function (data) {
                $scope.EmailOkToUse = data.User == null ? 'OK' : 'USED';
            },
            function () {
                alert('There was a problem validating your email address');
            });
    };

   

    $scope.Regsister = function () {

        //var referredTypeError = checkReferredTypeValid($scope.Model);
        //if (referredTypeError !== '') {
        //    alert(referredTypeError);
        //    return;
        //}

        formatReferredBy($scope.Model);

        $scope.RegistrationErrors = [];

        $http.defaults.headers.post['Content-Type'] = '' + 'application/x-www-form-urlencoded; charset=UTF-8';
        $http.defaults.headers.post['Access-Control-Allow-Origin'] = '' + 'local.busidex.com';
        $http.defaults.transformRequest = function (model) {
            return "model=" + JSON.stringify(model);
        };

        Registration.post($scope.Model,
            function (response) {
                //$scope.SetStep(4);
                window.location.href = 'https://start.busidex.com/#/front?token=' + response.Token;
            },
            function (error) {
                $scope.RegistrationErrors.push(error.data.Message);
            });
    };

    _gaq.push(['_trackEvent', 'Registration', 'Step', 'Page Load']);

    $scope.SetStep = function (step) {
        $scope.CurrentStep = step;
        _gaq.push(['_trackEvent', 'Registration', 'Step', step.toString()]);
    };
    
    //INSPECTLET CODE
    if (window.location.href.indexOf('local.') < 0) {
        window.__insp = window.__insp || [];
        __insp.push(['wid', 128335322]);
        (function() {
            function __ldinsp() {
                var insp = document.createElement('script');
                insp.type = 'text/javascript';
                insp.async = true;
                insp.id = "inspsync";
                insp.src = ('https:' == document.location.protocol ? 'https' : 'http') + '://cdn.inspectlet.com/inspectlet.js';
                var x = document.getElementsByTagName('script')[0];
                x.parentNode.insertBefore(insp, x);
            }

            if (window.attachEvent) {
                window.attachEvent('onload', __ldinsp);
            } else {
                window.addEventListener('load', __ldinsp, false);
            }
        })();
    }
    // END INSPECTLET CODE
}
RegistrationViewCtrl.$inject = ['$scope', '$http', '$rootScope', '$cookieStore', 'Registration', '$route', '$routeParams', 'Users', '$location', 'Analytics'];

/*HOME*/
function HomeViewCtrl($scope, $rootScope, $cookieStore, $http, $location, Feature, Busidex, $window) {
    $rootScope.ShowFilterControls = false;
    $scope.model = {};
    $scope.model.FeaturedCard = null;
    $scope.AddToMyBusidex = addToMyBusidex;
    $scope.ExistsInMyBusidex = false;

    $scope.TrackEvent = function(e, url) {

        
        _gaq.push(['_trackEvent', 'Home', 'Feature', e]);
        $window.open(url);
    };

    Feature.get({},
        function (resp) {

            $scope.model.FeaturedCard = resp.FeaturedCard;
        },
        function () {

        });

    function addToMyBusidex(id) {

        if ($rootScope.User == null) {
            $location.path('/account/login');
            return;
        }

        Busidex.post({ userId: $rootScope.User.UserId, cId: id },
            function () {
                alert("This card is now in your Busidex!");
                $scope.ExistsInMyBusidex = true;
            },
            function () {
                alert('There was a problem adding this card to your Busidex');
                return false;
            });
    }

    $rootScope.SetCurrentMenuItem('Home');
}
HomeViewCtrl.$inject = ['$scope', '$rootScope', '$cookieStore', '$http', '$location', 'Feature', 'Busidex', '$window'];

/*LOG OUT*/
function LogoutViewCtrl($scope, $rootScope, $cookieStore, $location) {

    $cookieStore.remove("User");
    $rootScope.User = $scope.User = null;
    $rootScope.MyBusidex = null;
    $rootScope.HasSharedCards = false;
    $rootScope.SharedCards = [];
    $rootScope.IsLoggedIn = false;

    $rootScope.SearchModel = null;
    
    gapi.auth.signOut();

    $rootScope.LoginModel = {
        LoginText: 'Login',
        LoginRoute: '#/account/login',
        HomeLink: '#/home'
    };
    $location.path("/home");
}
LogoutViewCtrl.$inject = ['$scope', '$rootScope', '$cookieStore', '$location'];

/*LOG IN*/
function LoginPartialViewCtrl($scope, $rootScope, $cookieStore, $routeParams) {

    $rootScope.MyBusidex = $rootScope.MyBusidex || [];
    $scope.Waiting = false;
    $rootScope.IsLoggedIn = $rootScope.User != null;
    
}
LoginPartialViewCtrl.$inject = ['$scope', '$rootScope', '$cookieStore', '$routeParams'];

/*LOG IN PAGE*/
function LoginViewCtrl($scope, $rootScope, $http, $cookieStore, $location, Login, SharedCard, $routeParams) {

    $scope.lastForm = {};
    $scope.form = {
        UserName: '',
        Password: '',
        Token: $routeParams._t,
        EventTag: $routeParams._e,
        AcceptSharedCards: $routeParams._a || false
    };
    
    $rootScope.User = null;
    $scope.sendForm = function (form) {

        $scope.Waiting = true;
        $scope.lastForm = angular.copy(form);
        //var data = {
        //    'UserName': $scope.form.UserName,
        //    'Password': $scope.form.Password,
        //    'Token:': $scope.form.Token,
        //    'RememberMe': false

        //};
        $scope.LoginErrors = [];
        Login.post($scope.form,
            function (user) {
                $scope.Waiting = false;
                $scope.resultData = user;
                $rootScope.User = user;
                $rootScope.User.RememberMe = $scope.form.RememberMe;

                var acctType = $rootScope.User != null ? parseInt($rootScope.User.AccountTypeId) : 0;
                $rootScope.MyBusidexMenuName = parseInt($rootScope.User.AccountTypeId) === ACCT_TYPE_ORGANIZATION ? 'Edit Referrals' : 'My Busidex';
                $rootScope.MyBusidexName = $rootScope.User != null && acctType === ACCT_TYPE_ORGANIZATION ? 'Referrals' : 'My Busidex';

                $rootScope.HomeShareLink = "#/busidex/mine?share=true";
                $rootScope.HomeEditLink = ($rootScope.User != null && $rootScope.User.HasCard) ? "#/card/edit/" + $rootScope.User.CardId + "?details=true" : "#/card/add";
                $rootScope.HomeTagLink = ($rootScope.User != null && $rootScope.User.HasCard) ? "#/card/edit/" + $rootScope.User.CardId + "?tags=true" : "#/card/add";


                $rootScope.SearchModel = null;

                $rootScope.LoginModel = {
                    LoginText: $rootScope.User == null ? 'Login' : 'LogOut',
                    LoginRoute: $rootScope.User == null ? '#/account/login' : '#/account/logout',
                    HomeLink: $rootScope.User != null && $rootScope.User.StartPage == 'Organization' ? '#/groups/organization/' + $rootScope.User.Organizations[0].Item2 : '#/home'
                }
            
                var cookieOptions = { domain: 'busidex.com', path: 'busidex.com', secure: true };

                $cookieStore.put('User', user, cookieOptions);
                $rootScope.IsLoggedIn = $rootScope.User != null;

                var token = $rootScope.User != null ? $rootScope.User.Token : null;
                $http.defaults.headers.common['X-Authorization-Token'] = token;

                SharedCard.get({},
                        function (data) {
                            if (data.SharedCards && data.SharedCards.length > 0) {

                               
                               $('#notificationPopup').modal('show');
                               
                                $rootScope.HasSharedCards = true;
                                $rootScope.SharedCards = [];
                                for (var i = 0; i < data.SharedCards.length; i++) {
                                    $rootScope.SharedCards.push(data.SharedCards[i]);
                                }
                                $scope.SendFromEmail = data.SharedCards[0].SendFromEmail;
                            }
                        },
                        function () {

                        });

                if (user.StartPage == 'Organization') {
                    
                    $location.path('/groups/organization/' + user.Organizations[0].Item2);
                    
                } else {

                    $location.path('/home');
                    $location.url($location.path()); // remove any query string parameters

                    $http({
                        method: 'GET',
                        url: ROOT + "/mycard/?id=" + user.UserId,
                        headers: { 'Content-Type': 'application/json' }
                    }).success(function(model) {

                        if ($rootScope.User.AccountTypeId === ACCT_TYPE_PROFESSIONAL || $rootScope.User.AccountTypeId === ACCT_TYPE_BETA) {
                            var card = model.MyCards[0];
                            if (card != null &&
                            (card.Name == null || card.Name.length == 0) &&
                            (card.CompanyName == null || card.CompanyName.length == 0)) {
                                $location.path('/card/edit/' + card.CardId);
                                //window.location.href = 'https://start.busidex.com/#/front?token=' + user.ActivationToken;
                            } else if (card == null) {
                                $location.path('/card/add/mine');
                                //window.location.href = 'https://start.busidex.com/#/front?token=' + user.ActivationToken;
                            }
                        }
                    });
                }
            },
            function (error) {

                if (error.status == 404) {
                    $scope.LoginErrors.push('Username or password is incorrect');
                } else {
                    $scope.LoginErrors.push(error.data.Message);
                }
                $scope.Waiting = false;
            });
    };

    $scope.resetForm = function () {
        $scope.form = angular.copy($scope.lastForm);
    };
}
LoginViewCtrl.$inject = ['$scope', '$rootScope', '$http', '$cookieStore', '$location', 'Login', 'SharedCard', '$routeParams'];

/*CONFIRM MY CARD*/
function ConfirmCardCtrl($scope, $route) {

    $scope.Token = $route.current.params.token;
}
ConfirmCardCtrl.$inject = ['$scope', '$route'];

/*ADD CARD*/
function AddCardCtrl($scope, $rootScope, $cookieStore, $location, $http, $route, $routeParams, phoneNumberTypes, stateCodes,
        fileSizeInfoContent, Busidex, $timeout, Analytics, Account) {

    Analytics.trackPage($location.path());

    if ($rootScope.User == null) {
        $location.path("/account/login");
    }

    $scope.Card = {};
    $scope.Tabs = {};
    $rootScope.ShowFilterControls = false;

    var DEFAULT_IMAGE = null; //'b66ff0ee-e67a-4bbc-af3b-920cd0de56c6';

    $scope.Tabs['terms'] = false;
    $scope.Tabs['details'] = false;
    $scope.Tabs['tags'] = false;
    $scope.Tabs['notes'] = false;
    $scope.Tabs['mycard'] = false;
    $scope.Tabs['address'] = false;
    $scope.Tabs['phone'] = false;
    $scope.Tabs['scorecard'] = false;

    $rootScope.IsLoggedIn = $rootScope.User != null;

    $scope.files = [];
    $scope.files.push("");
    $scope.files.push("");
    $scope.FileError = false;
    $scope.acceptingTerms = false;

    $scope.ModelError = false;
    $scope.Title = '';

    $scope.$on("fileSelected", function (event, args) {

        if (!$scope.IsBound) {
            $scope.$apply(function () {

                $scope.IsBound = true;
                var fileType = args.file.type;
                var validFileTypes = [];
                validFileTypes.push('jpg');
                validFileTypes.push('gif');
                validFileTypes.push('jpeg');
                validFileTypes.push('png');
                validFileTypes.push('bmp');

                var valid = false;
                for (var i = 0; i < validFileTypes.length; i++) {
                    if (fileType.indexOf(validFileTypes[i]) >= 0) {
                        valid = true;
                        break;
                    }
                }
                if (!valid) {
                    alert('Sorry, only jpg, gif, png and bmp files are supported');
                    return false;
                }
                //add the file object to the scope's files collection
                $scope.files[args.idx] = args.file;
                $scope.ImagePreview(args.idx);

                if (args.idx == 0) {
                    $scope.Model.HasFrontImage = true;
                }
                if (args.idx == 1) {
                    $scope.Model.HasBackImage = true;
                }
            });
        }
    });
    $scope.Model = {};

    $scope.TagHelp = "Adding Tags helps your card get found in searches. Keep them relevant to what your business is about so people can find you more easily.";
    $scope.DetailsHelp = "Be sure to include your Name and/or Company Name so your card can be found in a search.";
    $scope.Model.IsValid = true;

    $scope.Model.Errors = [];
    $scope.ExistingCards = [];

    if ($rootScope.User.AccountTypeId != 5 && $rootScope.User.AccountTypeId != 6) {
        $location.path('/');
        return;
    }    

    $scope.CalculateProgress = function () {
        var p = 0;
        if ($scope.Model.FrontImage != null || $scope.Model.HasFrontImage) {
            p += 10;
        }
        if ($scope.Model.BackImage != null || $scope.Model.HasBackImage) {
            p += 10;
        }
        if ($scope.Model.Name != null && $scope.Model.Name.length > 0) {
            p += 10;
        }
        if ($scope.Model.CompanyName != null && $scope.Model.CompanyName.length > 0) {
            p += 10;
        }
        if ($scope.Model.Title != null && $scope.Model.Title.length > 0) {
            p += 5;
        }
        if ($scope.Model.Email != null && $scope.Model.Email.length > 0) {
            p += 10;
        }
        if ($scope.Model.Url != null && $scope.Model.Url.length > 0) {
            p += 5;
        }
        if ($scope.Model.PhoneNumbers != null && $scope.Model.PhoneNumbers.length > 0 &&
            $scope.Model.PhoneNumbers[0].Number != null && $scope.Model.PhoneNumbers[0].Number.length > 0) {
            p += 10;
        }
        if ($scope.Model.Addresses != null && $scope.Model.Addresses.length > 0) {

            var addr = $scope.Model.Addresses[0];
            if (addr.Address1 != null && addr.Address1.length > 0) {
                p += 5;
            }
            if (addr.Address2 != null && addr.Address2.length > 0) {
                p += 3;
            }
            if (addr.City != null && addr.City.length > 0) {
                p += 5;
            }
            if (addr.State != null && addr.State.StateCodeId > 0) {
                p += 5;
            }
            if (addr.ZipCode != null && addr.ZipCode.length > 0) {
                p += 5;
            }
        }
        if ($scope.Model.Tags != null && $scope.Model.Tags.length > 0) {
            p += $scope.Model.Tags.length;
        }
        if (p > 100) p = 100;
        return p;
    };
    var getModel = function () {

        var _data = {
            'id': $routeParams.id || 1,
            'userId': $rootScope.User.UserId
        };
        $scope.Waiting = true;
        $http({
            method: 'GET',
            url: ROOT + "/card/?id=" + _data.id + '&userId=' + _data.userId,
            data: JSON.stringify(_data),
            headers: { 'Content-Type': 'application/json' }
        }).success(function (model) {

            if (model.Model.CardId === 0) {
                model.Model.FrontFileId = model.Model.BackFileId = DEFAULT_IMAGE;
                model.Model.FrontType = model.Model.BackType = 'jpg';
            }
            if (model.Model.FrontFileId != null && model.Model.FrontFileId.length > 0) {
                model.Model.FrontFileId = 'https://az381524.vo.msecnd.net/cards/' + model.Model.FrontFileId + '.' + model.Model.FrontType.replace('.', '');
            }
            if (model.Model.BackFileId != null && model.Model.BackFileId.length > 0) {
                model.Model.BackFileId = 'https://az381524.vo.msecnd.net/cards/' + model.Model.BackFileId + '.' + model.Model.BackType.replace('.', '');
            }

            var addresses = new Array();
            var i = 0;
            for (i = 0; i < model.Model.Addresses.length; i++) {

                var a = model.Model.Addresses[i];
                addresses.push(
                    new Address(a.CardAddressId, a.CardId, a.Address1, a.Address2, a.City, a.State, a.ZipCode)//, a.Region, a.Country
                );
            }
            if (addresses.length === 0) {
                addresses.push(new Address(0, model.Model.CardId, null, null, null, null, null));
            }

            model.Model.Addresses = addresses;

            model.Model.Notes = unescape(model.Model.Notes);

            $scope.FrontOrientationClass = model.Model.FrontOrientation == 'V' ? 'v_preview' : 'h_preview';
            $scope.BackOrientationClass = model.Model.BackOrientation == 'V' ? 'v_preview' : 'h_preview';
            $scope.UserId = $rootScope.User.UserId;

            model.Model.Visibility = model.Model.Visibility || 1;
            if (model.Model.Visibility == 0) model.Model.Visibility = 1;

            $scope.Model = model.Model;

            $scope.Model.TagTypes = [];
            $scope.Model.TagTypes.push('');
            $scope.Model.TagTypes.push('User');
            $scope.Model.TagTypes.push('System');

            $scope.Model.Errors = [];
            $scope.Model.Progress = 55;
            if (_data.id <= 1) {
                $scope.Model.IsMyCard = $location.$$path == '/card/add/mine';
            }

            var a_or_your = $scope.Model.IsMyCard ? 'MY' : "A";
            $scope.Title = ((_data.id == 0 || _data.id == 1)
                ? 'ADD ' + a_or_your
                : 'EDIT ' + a_or_your) + ' BUSINESS CARD';

            $scope.Waiting = false;

            $scope.Model.PhoneNumberTypes = phoneNumberTypes;
            $scope.Model.StateCodes = stateCodes;
            $scope.Model.FileSizeInfoContent = fileSizeInfoContent;
            $scope.Model.PhoneValid = true;
            $scope.Model.IsValid = true;
            $scope.Model.ResetBackImage = false;

            if ($scope.Model.Notes.toLowerCase() == 'null') {
                $scope.Model.Notes = '';
            }
            $scope.ModelReset = model.Model;

            // select the current address in the option list
            if ($scope.Model.Addresses[0] != undefined) {
                for (i = 0; i < stateCodes.length; i++) {
                    if (stateCodes[i].StateCodeId == $scope.Model.Addresses[0].State.StateCodeId) {
                        $scope.Model.Addresses[0].State = stateCodes[i];
                        break;
                    }
                }
            }
            $scope.Model.AddPhoneNumber = function () {

                for (i = 0; i < $scope.Model.PhoneNumbers.length; i++) {
                    if ($scope.Model.PhoneNumbers[i].PhoneNumberTypeId == null) {
                        $scope.Model.PhoneValid = false;
                        return false;
                    }
                }
                $scope.Model.PhoneValid = true;

                $scope.Model.PhoneNumbers.push({
                    Number: '',
                    PhoneNumberId: 0,
                    PhoneNumberTypeId: 0,
                    PhoneNumberType: {
                        PhoneNumberTypeId: 0,
                        Name: '',
                        Deleted: false
                    },
                    Extension: '',
                    Deleted: false,
                    Selected: false
                });
                return true;
            };
            $scope.Model.RemovePhoneNumber = function (number) {

                if ($scope.Model.PhoneNumbers.length == 1) {
                    return;
                }
                for (i = 0; i < $scope.Model.PhoneNumbers.length; i++) {
                    if ($scope.Model.PhoneNumbers[i].Number == number) {
                        $scope.Model.PhoneNumbers[i].Deleted = true;
                        break;
                    }
                }
            };
            $scope.Model.Validate = function () {

                $scope.Model.IsValid = true;

                if ($scope.Model.Email == null || $scope.Model.Email.length == 0) {
                    $scope.Model.IsValid = false;
                    $scope.SetCurrentTab('details');
                } else if ($scope.Model.CardId <= 1 && ($scope.files.length == 0 || $scope.files[0].length == 0)) {
                    $scope.Model.IsValid = false;
                    $scope.SetCurrentTab('mycard');
                } else if ($scope.Model.PhoneNumbers.length == 0 || $scope.Model.PhoneNumbers[0].Number.length == 0) {
                    $scope.Model.IsValid = false;
                    $scope.SetCurrentTab('phone');
                }

                return $scope.Model.IsValid;
            };
            $scope.Model.NewCardSaved = false;

            $scope.Model.AddAddress = function () {
                var addr = $scope.TempAddress;
                var validation = addr.Address1 + addr.Address2 + addr.City + addr.State.Code + addr.ZipCode + addr.Region + addr.Country;
                if (validation.length == 0) {
                    return;
                }
                var selectedIdx = -1;
                for (i = 0; i < $scope.Model.Addresses.length; i++) {
                    if ($scope.Model.Addresses[i].Selected == true) {
                        selectedIdx = i;
                        break;
                    }
                }

                if (selectedIdx >= 0) { // update address
                    $scope.Model.Addresses[selectedIdx].CardAddressId = addr.CardAddressId;
                    $scope.Model.Addresses[selectedIdx].CardId = addr.CardId;
                    $scope.Model.Addresses[selectedIdx].Address1 = addr.Address1;
                    $scope.Model.Addresses[selectedIdx].Address2 = addr.Address2;
                    $scope.Model.Addresses[selectedIdx].City = addr.City;
                    $scope.Model.Addresses[selectedIdx].State = addr.State;
                    $scope.Model.Addresses[selectedIdx].ZipCode = addr.ZipCode;
                    $scope.Model.Addresses[selectedIdx].Region = addr.Region;
                    $scope.Model.Addresses[selectedIdx].Country = addr.Country;
                    $scope.Model.Addresses[selectedIdx].Selected = false;

                } else { // add new address
                    $scope.Model.Addresses.push(
                        new Address(addr.CardAddressId, addr.CardId, addr.Address1, addr.Address2, addr.City, addr.State, addr.ZipCode, addr.Region, addr.Country)
                    );

                }
                $scope.TempAddress = new Address($scope.Model.Addresses.length, 0, '', '', '', { StateCodeId: -1, Code: '', Name: '' }, '', '', '');

            };
            $scope.Save = function () {
                $scope.FileError = false;

                $scope.Model.UserId = $rootScope.User.UserId;

                if (!$scope.Model.Validate()) {
                    return false;
                }
                if ($scope.Model.ResetBackImage == true) {
                    $scope.Model.BackFileId = null;
                }

                var _data = {
                    'id': $routeParams.id || 1,
                    'userId': $rootScope.User.UserId
                };
                var edit = $scope.Model.CardId > 1;
                $http({
                    method: edit ? 'PUT' : 'POST',
                    url: ROOT + "/card/?id=" + _data.id + '&userId=' + _data.userId,
                    headers: {
                        'Content-Type': undefined,
                        'Access-Control-Allow-Headers': 'Content-Type'
                    },
                    transformRequest: function (data) {

                        var formData = new FormData();
                        //need to convert our json object to a string version of json otherwise
                        // the browser will do a 'toString()' on the object which will result 
                        // in the value '[Object object]' on the server.
                        var o = $scope.Model;
                        o.FrontFileId = o.BackFileId = o.FrontImage = o.BackImage = null;
                        var isMyCard = o.IsMyCard;
                        o.IsMyCard = isMyCard == "1" ? true : false;
                        var savedAddresses = new Array();
                        for (i = 0; i < o.Addresses.length; i++) {
                            var addr = o.Addresses[i];
                            savedAddresses.push(
                                new Address(addr.CardAddressId, addr.CardId, addr.Address1, addr.Address2, addr.City, addr.State, addr.ZipCode)//, addr.Region, addr.Country
                            );
                            addr.State = { Code: addr.State.Code, StateCodeId: addr.State.StateCodeId, Name: addr.State.Name };
                        }
                        formData.append("model", angular.toJson(o));
                        //now add all of the assigned files
                        formData.append("file0", $scope.files[0] || "");
                        formData.append("file1", $scope.files[1] || "");

                        o.Addresses = savedAddresses;
                        o.IsMyCard = isMyCard;

                        return formData;
                    },
                    data: { model: $scope.model, files: $scope.files }
                }).success(function () {
                    $rootScope.MyBusidex = []; // clear cache
                    $rootScope.User.HasCard = true;

                    $cookieStore.put('User', $rootScope.User);

                    $scope.Model.Progress = $scope.CalculateProgress();
                    for (i = 0; i < stateCodes.length; i++) {
                        if (stateCodes[i].StateCodeId == $scope.Model.Addresses[0].State.StateCodeId) {
                            $scope.Model.Addresses[0].State = stateCodes[i];
                            break;
                        }
                    }

                    $scope.Model.NewCardSaved = true;
                    $timeout(function () {
                        if (edit) {
                            $scope.Model.NewCardSaved = false;
                        } else {
                            $location.path("/busidex/mine");
                        }
                    }, 5000);

                }).error(function (data, status) {

                    $scope.resultData = data;
                    $scope.Model.SelectedExistingCardId = 0;
                    if (status == 400) {
                        //$scope.ExistingCards = data.Model.ExistingCards;
                        if ($scope.ExistingCards.length == 0) {
                            for (var e in data.Model.ErrorCollection) {
                                $scope.Model.Errors.push(data.Model.ErrorCollection[e]);
                            }
                        }
                    } else {
                        alert("There was a problem saving your card.");
                    }
                });
                return false;
            };
            $scope.Model.EditAddress = function (id) {

                for (i = 0; i < $scope.Model.Addresses.length; i++) {
                    if ($scope.Model.Addresses[i].CardAddressId == id) {

                        var addr = $scope.Model.Addresses[i];
                        $scope.TempAddress = new Address(addr.CardAddressId, addr.CardId, addr.Address1, addr.Address2, addr.City, addr.State, addr.ZipCode, addr.Region, addr.Country);
                        $scope.Model.Addresses[i].Selected = true;

                        break;
                    }
                }
            };
            $scope.Model.ToggleAddress = function (id) {
                for (i = 0; i < $scope.Model.Addresses.length; i++) {
                    if ($scope.Model.Addresses[i].CardAddressId == id) {
                        $scope.Model.Addresses[i].Deleted = !$scope.Model.Addresses[i].Deleted;
                        $scope.Model.Addresses[i].DeleteSrc = $scope.Model.Addresses[i].Deleted ? '../../img/undoDelete.png' : '../../img/delete.png';
                        break;
                    }
                }
            };
            $scope.Model.AddTag = function () {

                var found = false;
                var text = $scope.Model.NewTag;
                for (i = 0; i < $scope.Model.Tags.length; i++) {
                    if ($scope.Model.Tags[i].Text == text) {
                        found = true;
                        break;
                    }
                }
                if (!found) {
                    $scope.Model.Tags.push({
                        Text: text,
                        TagType: 1
                    });
                }
                $scope.Model.NewTag = '';
            };
            $scope.Model.RemoveTag = function (text) {
                for (i = 0; i < $scope.Model.Tags.length; i++) {
                    if ($scope.Model.Tags[i].Text == text) {
                        $scope.Model.Tags.splice(i, 1);
                        break;
                    }
                }
            };

            if ($scope.Model != null &&
            ($scope.Model.Name == null || $scope.Model.Name.length == 0) &&
            ($scope.Model.CompanyName == null || $scope.Model.CompanyName.length == 0)) {
                $scope.SetCurrentTab('details');
            } else {
                if ($routeParams.tags) {
                    $scope.SetCurrentTab('tags');
                }
                if ($routeParams.details) {
                    $scope.SetCurrentTab('details');
                }
                if ($routeParams.mycard) {
                    $scope.SetCurrentTab('mycard');
                }
            }

            Account.getTerms(
                function (terms) {
                    if (terms !== null && terms != undefined && terms.length > 0) {
                        $scope.SetCurrentTab('details');
                    } else {
                        $scope.SetCurrentTab('terms');
                    }
                },
                function () {

                });

        }).error(function (data) {
            $scope.resultData = data;
            alert("Getting card failed.");
        });
    };

    $scope.NoDetails = function () {
        return ($scope.Model.Name == null || $scope.Model.Name.length == 0) && ($scope.Model.CompanyName == null || $scope.Model.CompanyName.length == 0);
    };
    $scope.ClearExisting = function () {
        $scope.Model.SelectedExistingCardId = 0;
        $scope.ExistingCards = [];
    };
    $scope.SetSelectedExistingCardId = function (id) {
        $scope.Model.SelectedExistingCardId = id;
    };
    $scope.AddExistingCard = function () {
        if ($scope.Model.SelectedExistingCardId > 0) {
            Busidex.post({ userId: $rootScope.User.UserId, cId: $scope.Model.SelectedExistingCardId },
                function () {
                    alert("This card is now in your Busidex!");
                    $scope.ClearExisting();
                    $location.path('busidex/mine');
                },
                function () {
                    alert('There was a problem adding this card to your Busidex');
                    return false;
                });
        }
    };
    $scope.ImagePreview = function (idx) {

        $scope.FileError = false;
        $http({
                method: 'POST',
                cache: false,
                url: ROOT + "/card/ImagePreview/?idx=" + idx,
                //IMPORTANT!!! You might think this should be set to 'multipart/form-data' 
                // but this is not true because when we are sending up files the request 
                // needs to include a 'boundary' parameter which identifies the boundary 
                // name between parts in this multi-part request and setting the Content-type 
                // manually will not set this boundary parameter. For whatever reason, 
                // setting the Content-type to 'false' will force the request to automatically
                // populate the headers properly including the boundary parameter.
                headers: { 'Content-Type': undefined },
                //This method will allow us to change how the data is sent up to the server
                // for which we'll need to encapsulate the model data in 'FormData'
                transformRequest: function(data) {

                    var formData = new FormData();
                    //need to convert our json object to a string version of json otherwise
                    // the browser will do a 'toString()' on the object which will result 
                    // in the value '[Object object]' on the server.
                    //formData.append("model", angular.toJson(data.model));
                    //now add all of the assigned files
                    formData.append("file0", data.files[0]);
                    if (data.files.length == 2) {
                        formData.append("file1", data.files[1]);
                    }
                    return formData;
                },
                //Create an object that contains the model and files which will be transformed
                // in the above transformRequest method
                data: { files: $scope.files }
            }).
            success(function(data) {
                if (idx == 0) {
                    $scope.Model.FrontFileId = "data:image/gif;base64," + JSON.parse(data);
                }
                if (idx == 1) {
                    $scope.Model.BackFileId = "data:image/gif;base64," + JSON.parse(data);
                }

            }).
            error(function() {
                $scope.FileError = true;
                $scope.files[idx] = null;
                if (parseInt(idx) === 0) {
                    $scope.Model.FrontImage = null;
                    $("#getFrontImgHdn").val(null);

                } else {
                    $scope.Model.BackImage = null;
                    $("#getBackImgHdn").val(null);
                }
                return false;
            });
    };
    $scope.SetCurrentTab = function (currentTab) {

        for (var tab in $scope.Tabs) {
            if ($scope.Tabs.hasOwnProperty(tab)) {
                $scope.Tabs[tab] = false;
            }
        }
        $scope.Tabs[currentTab] = true;
    };
    $scope.Reset = function () {
        getModel();
    };

    $scope.ResetBackImage = function() {
        $scope.Model.ResetBackImage = true;
        $scope.Model.BackFileId = 'https://az381524.vo.msecnd.net/cards/' +  DEFAULT_IMAGE + '.png';
    }

    $scope.accept_terms = function () {
        $scope.acceptingTerms = true;
        Account.acceptTerms({},
            function () {
                $scope.acceptingTerms = false;
                $scope.SetCurrentTab('details');
            },
            function() {

            });
    }

    if ($rootScope.User == null) {
        $location.path("/account/login");
    } else {
        getModel();
    }
}
AddCardCtrl.$inject = ['$scope', '$rootScope', '$cookieStore', '$location', '$http', '$route', '$routeParams', 'phoneNumberTypes', 'stateCodes', 'fileSizeInfoContent', 'Busidex', '$timeout', 'Analytics', 'Account'];

/*MY BUSIDEX*/
function MyBusidexCtrl($scope, $rootScope, $cookieStore, $location, Busidex, Notes, SharedCard, Activity, Analytics, $routeParams) {
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
                //console.log('event saved');
            },
            function (status) {
                //console.log('event NOT saved: ' + status);
            });
    };

    $scope.CardToShare = {};
    $scope.share = function () {

        var sharedCards = new Array();
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
        if (sharedCards.length == 0) {
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

    if ($rootScope.User == null) {
        if ($location.path() == '/busidex/mine') {
            $location.path("/account/login");
        }

    } else {

        $rootScope.ShowFilterControls = true;

        if ($rootScope.MyBusidex == null || $rootScope.MyBusidex.length == 0) {

            Busidex.get({ all: true },
                function (model) {

                    for (var i = 0; i < model.MyBusidex.Busidex.length; i++) {

                        var thisCard = model.MyBusidex.Busidex[i].Card;
                        model.MyBusidex.Busidex[i].OrientationClass = thisCard.FrontOrientation == 'V' ? 'v_preview' : 'h_preview';
                        if ($rootScope.User.IsAdmin || thisCard.OwnerId == $rootScope.User.UserId ||
                            (!thisCard.OwnerId && (thisCard.CreatedBy == $rootScope.User.UserId || model.MyBusidex.Busidex[i].SharedById == $rootScope.User.UserId))) {
                            thisCard.ShowEdit = true;
                        } else {
                            model.MyBusidex.Busidex[i].Card.ShowEdit = false;
                        }
                        thisCard.IconPath = thisCard.OwnerId ? "../img/searchIcon.png" : "../img/phone.png";
                        thisCard.IconLink = thisCard.OwnerId ? "#" + thisCard.CardId : "#/invite/" + thisCard.CardId;
                        thisCard.IconClass = thisCard.OwnerId ? "HasOwner" : "InMyBusidex";
                        thisCard.IconTitle = thisCard.OwnerId ? "In My Busidex" : "Send an invite";

                        var addresses = new Array();
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

        //$(document).on("change", "textarea.Notes", function () {

            //Notes.update({ id: $(this).attr("ucId"), notes: escape($(this).val()) },
            //    function () {

            //    },
            //    function () {

            //    });
        //});

    }
}
MyBusidexCtrl.$inject = ['$scope', '$rootScope', '$cookieStore', '$location', 'Busidex', 'Notes', 'SharedCard', 'Activity', 'Analytics', '$routeParams'];

/*CARD DETAIL*/
function CardDetailCtrl($scope, $rootScope, $cookieStore, $location, $http, $route, $routeParams, Analytics, Busidex, Activity) {
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
        $location.path("/home");
        return;
    }

    var addActivity = function (sourceId, cardId) {

        var activity =
        {
            CardId: cardId,
            UserId: $rootScope.User != null ? $rootScope.User.UserId : null,
            EventSourceId: sourceId
        };
        Activity.post(activity,
            function () {
                //console.log('event saved');
            },
            function (status) {
                //console.log('event NOT saved: ' + status);
            });
    };

    if ($rootScope.EventSources == null) {
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

        if ($rootScope.User == null) {
            $location.path('/account/login');
            return;
        }

        Busidex.post({ userId: $rootScope.User.UserId, cId: id },
            function () {
                alert("This card is now in your Busidex!");
                $scope.ExistsInMyBusidex = true;
            },
            function () {
                alert('There was a problem adding this card to your Busidex');
                return false;
            });
    }

    $scope.Waiting = true;
    $http({
        method: 'GET',
        url: ROOT + "/card/details/?id=" + params.id,
        data: JSON.stringify(params),
        headers: { 'Content-Type': 'application/json', 'X-UserId': $rootScope.User != null ? $rootScope.User.UserId : 0 }
    }).success(function (data) {

        $scope.FrontOrientationClass = data.Model.FrontOrientation == 'V' ? 'v_preview' : 'h_preview';
        $scope.BackOrientationClass = data.Model.BackOrientation == 'V' ? 'v_preview' : 'h_preview';
        $scope.UserId = $rootScope.User != null ? $rootScope.User.UserId : null;
        $scope.Card = data.Model;
        $scope.ShowBack = data.Model.HasBackImage && data.Model.BackFileId != 'b66ff0ee-e67a-4bbc-af3b-920cd0de56c6';
        $scope.ShowFront = data.Model.FrontFileId != null;
        $scope.ExistsInMyBusidex = data.Model.ExistsInMyBusidex;
        $scope.Waiting = false;

        //document.title = data.Model.Name + ' | Busidex';

    }).error(function (data, status) {
        if (status == 404) {
            $location.path("/home");
            return;
        }
        $scope.resultData = data;
        alert("Getting card detail failed.");
        $scope.Waiting = false;
    });
}
CardDetailCtrl.$inject = ['$scope', '$rootScope', '$cookieStore', '$location', '$http', '$route', '$routeParams', 'Analytics', 'Busidex', 'Activity'];

/*MY CARD*/
function MyCardsCtrl($scope, $rootScope, $cookieStore, $location, $http, Card, Activity, Analytics, SharedCard, Communications, SharedCardPreview, OutlookContacts) {
    Analytics.trackPage($location.path());

    $scope.ShowPreviousMonth = showPreviousMonth;
    $scope.ShowNextMonth = showNextMonth;
    $scope.ShowBack = false;
    $scope.ShowFront = false;
    $scope.Sharing = false;
    $scope.SelectAll = false;
    $scope.AdvancedSharing = false;
    $scope.ShowSelectedOnly = false;
    $scope.Model = {};
    $scope.Model.ShareWith = '';
    $scope.ShareMyCard = shareMyCard;
    $scope.ToggleSharing = toggleSharing;
    $scope.ToggleAdvancedSharing = toggleAdvancedSharing;
    $scope.ToggleShowSelected = toggleShowSelected;
    $scope.CancelSharing = cancelSharing;
    $scope.ToggleSelectAll = toggleSelectAll;
    $scope.ShowPreview = showPreview;
    $scope.ShareWithAll = shareWithAll;
    $scope.SendTestEmail = sendTestEmail;
    $scope.SwitchAccounts = switchAccounts;
    $scope.ConnectedAccount = '';
    $scope.PersonalMessage = '';
    $scope.PreviewBody = null;
    $scope.ClosePreview = closePreview;
    $scope.AdvancedSharingOptions = {
        Gmail: "Gmail",
        Yahoo: "Yahoo",
        Outlook: "Outlook"
    }
    $scope.Contacts = [];
    $scope.Model.OutlookFile = '';
    $scope.LoadOutlookContacts = loadOutlookContacts;
    $scope.Model.SelectedSharingOption = '';
    $scope.EmailListString = '';
    $scope.Filter = '';
    $scope.ReadContacts = readContacts;
    $rootScope.ShowFilterControls = false;
    $rootScope.SetCurrentMenuItem('Mine');

    $scope.MonthlyActivities = {};

    var acctType = $rootScope.User != null ? parseInt($rootScope.User.AccountTypeId) : 0;
    $scope.ShowActions = acctType != ACCT_TYPE_BASIC;
    
    function closePreview() {
        $scope.PreviewBody = null;
    }
    function showPreview() {
        var shareWith = $scope.Model.ShareWith;
        var list = [];
        list.push($scope.MyCards[0].CardId);

        var data = {
            SharedCardId: 0,
            CardId: $scope.MyCards[0].CardId,
            SendFrom: $rootScope.User.UserId,
            SendFromEmail: '',
            Email: shareWith,
            ShareWith: 0,
            SharedDate: new Date(),
            Accepted: false,
            Declined: false,
            Recommendation: encodeURIComponent($scope.PersonalMessage)
        };
      
        SharedCardPreview.post(data,
            function (response) {
                $scope.PreviewBody = response.Template.Body;
            }, function () {

            });
    }
    function sendTestEmail() {
        
        var message = encodeURIComponent($scope.PersonalMessage);

        var sharedCard = {
            CardId: $scope.MyCards[0].CardId,
            SendFrom: $rootScope.User.UserId,
            Email: '',
            SharedDate: new Date(),
            Accepted: null,
            Declined: null,
            Recommendation: message
        };

        SharedCard.sendTest(sharedCard, function () {

            $scope.Model.ShareWith = '';

            getLastCommunicationDates($scope.EmailListString);
            alert('A sample email has been sent to your account email.');
        },
                function () {

                });
    }
    function shareWithAll() {

        if (!confirm('Send your card to all selected email addresses now?')) {
            return;
        }
        
        var selected = 0;
        var sharedCards = [];
        var sharedDate = new Date();
        var message = encodeURIComponent($scope.PersonalMessage);
        for (var i = 0; i < $scope.Contacts.length; i++) {
            if ($scope.Contacts[i].Selected) {
                selected++;

                var sharedCard = {
                    CardId: $scope.MyCards[0].CardId,
                    SendFrom: $rootScope.User.UserId,
                    Email: $scope.Contacts[i].Email,
                    SharedDate: sharedDate,
                    Accepted: null,
                    Declined: null,
                    Recommendation: message
                }
                sharedCards.push(sharedCard);

                $scope.Contacts[i].LastSharedDate = new Date().toLocaleDateString();
            }
        }

        if (sharedCards.length > 0) {
            SharedCard.post(sharedCards, function() {

                    $scope.Model.ShareWith = '';

                    getLastCommunicationDates($scope.EmailListString);
                    alert('Your Card Has Been Shared!');
                },
                function() {

                });
        }
    }
    function getLastCommunicationDates(emailList) {

        Communications.post({ EmailList: emailList, UserId: $rootScope.User.UserId },
            function (data) {

                if (data.Communications != null) {

                    var communications = data.Communications;
                    for (var i = 0; i < $scope.Contacts.length; i++) {

                        if (communications[$scope.Contacts[i].Email] != null) {
                            $scope.Contacts[i].LastSharedDate = new Date(communications[$scope.Contacts[i].Email].DateSent).toLocaleDateString();
                        }
                    }
                }
            }, function () {

            });
    }
    function toggleSelectAll() {
        $scope.SelectAll = !$scope.SelectAll;
        for (var i = 0; i < $scope.Contacts.length; i++) {
            $scope.Contacts[i].Selected = $scope.SelectAll;
        }
    }
    function switchAccounts() {
        var logout = '<iframe id="logoutframe" src="https://accounts.google.com/logout" style="display: none"></iframe>';

        $(logout).appendTo('body');
        setTimeout(readContacts, 2000);

    }
    function loadOutlookContacts(file) {

        var fd = new FormData();
        //fd.append('file', $scope.Model.OutlookFile);
        fd.append('file', file);
        OutlookContacts.post(fd,
            function (data) {
                var contacts = [];
                var emailList = [];

                for (var i = 0; i < data.Contacts.length; i++) {

                    var name = data.Contacts[i].FirstName + ' ' + data.Contacts[i].LastName;
                    if (name == ' ') {
                        name = data.Contacts[i].CompanyName;
                    }
                    if (name.trim() == '') {
                        name = '(No Name)';
                    }
                    var contact = {
                        Name: name,
                        Email: data.Contacts[i].Email,
                        Selected: false,
                        LastSharedDate: '(not available)'
                    }
                    emailList.push(contact.Email);
                    contacts.push(contact);
                }

                //$scope.$apply(function () {
                    $scope.Contacts = contacts;
                    $scope.EmailListString = emailList.join(',');
                    toggleSharing();
                    getLastCommunicationDates(emailList);
                //});
                $scope.AdvancedSharing = !$scope.AdvancedSharing;
            },
            function() {
                
            });
    }

    function readContacts() {
        //var promise = GoogleLogin.login();
        //promise.then(function (data) {
        //    console.log(data.email);
        //}, function (reason) {
        //    console.log('Failed: ' + reason);
        //});

        var clientId = API_ID;
        var apiKey = API_KEY;
        var scopes = 'https://www.google.com/m8/feeds';

        gapi.client.setApiKey(apiKey);
        window.setTimeout(checkAuth, 3);

        function checkAuth() {
            gapi.auth.authorize({ client_id: clientId, scope: scopes, immediate: false }, handleAuthResult);
        }

        function handleAuthResult(authResult) {
            if (authResult && !authResult.error) {
                $.get("https://www.google.com/m8/feeds/contacts/default/full?alt=json&access_token=" + authResult.access_token + "&max-results=700&v=3.0",
                  function (response) {

                      $scope.ConnectedAccount = response.feed.author[0].email.$t;

                      var data = response.feed.entry;
                      var contacts = [];
                      var emailList = [];
                      for (var i = 0; i < data.length; i++) {

                          if (data[i].gd$email == undefined || data[i].gd$email[0] == undefined) {
                              continue;
                          }

                          var contact = {
                              Name: data[i].title.$t,
                              Email: data[i].gd$email[0].address.trim(),
                              Selected: false,
                              LastSharedDate: '(not available)'
                          }
                          emailList.push(contact.Email);
                          contacts.push(contact);
                      }

                      $scope.$apply(function () {
                          $scope.Contacts = contacts;
                          $scope.EmailListString = emailList.join(',');
                          toggleSharing();
                          getLastCommunicationDates(emailList);
                      });

                  });
            }
        }
    }
    function toggleShowSelected() {
        $scope.ShowSelectedOnly = !$scope.ShowSelectedOnly;
    }
    function toggleSharing() {
        if (!$scope.Sharing) {
            var list = [];
            list.push($scope.MyCards[0]);
            var noName = !list[0].Name || list[0].Name.length == 0;
            var noCompanyName = !list[0].CompanyName || list[0].CompanyName.length == 0;
            if (noName && noCompanyName) {
                alert('You have to fill in your card name or company before sharing.');
                return;
            }
        }
        $scope.Sharing = !$scope.Sharing;
    }
    function cancelSharing() {
        $scope.Sharing = false;
    }
    function shareMyCard() {
        var shareWith = $scope.Model.ShareWith;
        var sharedCards = [];
        var sharedCard = {
            CardId: $scope.MyCards[0].CardId,
            SendFrom: $rootScope.User.UserId,
            Email: shareWith,
            SharedDate: new Date(),
            Accepted: null,
            Declined: null,
            Recommendation: encodeURIComponent($scope.PersonalMessage)
        }
        sharedCards.push(sharedCard);

        SharedCard.post(sharedCards, function() {

                $scope.Model.ShareWith = '';
                alert('Your Card Has Been Shared!');
            },
            function() {

            });
        $scope.Sharing = false;
    }
    function toggleAdvancedSharing(option) {
        
        switch (option) {
        case $scope.AdvancedSharingOptions.Gmail:
        {
            $scope.AdvancedSharing = !$scope.AdvancedSharing;
            $scope.Model.SelectedSharingOption = $scope.AdvancedSharingOptions.Gmail;
            readContacts();
            break;
        }
        case $scope.AdvancedSharingOptions.Outlook:
        {
            $scope.Model.SelectedSharingOption = $scope.AdvancedSharingOptions.Outlook;
            break;
        }
        case $scope.AdvancedSharingOptions.Yahoo:
        {
            $scope.Model.SelectedSharingOption = $scope.AdvancedSharingOptions.Yahoo;
            break;
        }
        default:
        {
            $scope.AdvancedSharing = !$scope.AdvancedSharing;
            $scope.Model.SelectedSharingOption = {};
            break;
        }
        }
    }
    function getMyCard() {
        $http({
            method: 'GET',
            url: ROOT + "/mycard/?id=" + $rootScope.User.UserId,
            headers: { 'Content-Type': 'application/json' }
        }).success(function (model) {

            

            for (var i = 0; i < model.MyCards.length; i++) {
                model.MyCards[i].FrontOrientationClass = model.MyCards[i].FrontOrientation == 'V' ? 'v_preview' : 'h_preview';
                model.MyCards[i].BackOrientationClass = model.MyCards[i].BackOrientation == 'V' ? 'v_preview' : 'h_preview';
                model.MyCards[i].ShowFront = model.MyCards[i].FrontFileId != null;
                model.MyCards[i].ShowBack = model.MyCards[i].HasBackImage;

                loadActivities(model.MyCards[i].CardId);
            }
            $scope.UserId = $rootScope.User.UserId;
            $scope.MyCards = model.MyCards;
            $scope.Waiting = false;
            $scope.NoCards = model.MyCards.length == 0;

            document.title = model.MyCards[0].Name + ' | Busidex';

        }).error(function (data) {
            $scope.resultData = data;
            alert("Getting My Cards failed.");
            $scope.Waiting = false;
        });
    }
    
    var monthNames = ["January", "February", "March", "April", "May", "June",
    "July", "August", "September", "October", "November", "December"];

    function showPreviousMonth() {
        $scope.CurrentMonthOffest--;
        $scope.CurrentMonth = getCurrentMonth();
        loadActivities($scope.MyCards[0].CardId);
    }
    function showNextMonth() {
        $scope.CurrentMonthOffest++;
        $scope.CurrentMonth = getCurrentMonth();
        loadActivities($scope.MyCards[0].CardId);
    }
    function getCurrentMonth() {

        var date = new Date();
        date.setDate(1);
        date.setMonth(date.getMonth() + $scope.CurrentMonthOffest);
        var m = monthNames[date.getMonth()];

        return { Name: m, Month: date.getMonth() + 1, Year: date.getFullYear() };
    }
    $scope.CurrentMonthOffest = 0;
    $scope.CurrentMonth = getCurrentMonth();
    

    function loadActivities(id) {

        //var m = new Date().getMonth();

        Activity.get({ cardId: id, month: $scope.CurrentMonth.Month },
            function (results) {

                $scope.Sources = results.Sources;
                $scope.MonthlyActivities = results.Activities.length > 0 ? results.Activities[0].Data : {};

            },
            function () {
                //alert(error.status);
            });
    }

    var user = $rootScope.User;
    if (user === null || user === undefined) {
        user = angular.fromJson($cookieStore.get('user'));
        $rootScope.User = user;

        manualLogin(user, $scope, $rootScope, $cookieStore, $http, SharedCard, $location);

    }
    if ($rootScope.User === null || $rootScope.User === undefined) {
        $location.path("/account/login");
        return;
    } else {

        $scope.Waiting = true;
        getMyCard();
    }

    $scope.DeleteCard = function (id) {

        if (!confirm('Are you sure you want to delete your card? Everyone with whom you have shared your card will no longer be able to see it.')) {
            return;
        }

        Card.remove({ id: id, userId: $rootScope.User.UserId },
            function () {
                getMyCard();
            },
            function () {

            });
    };

}
MyCardsCtrl.$inject = ['$scope', '$rootScope', '$cookieStore', '$location', '$http', 'Card', 'Activity', 'Analytics', 'SharedCard', 'Communications', 'SharedCardPreview', 'OutlookContacts'];

/*SUGGESTIONS*/
function SuggestionsCtrl($scope, $rootScope, $cookieStore, $location, $http, Suggestions, Analytics) {

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

        $scope.Suggestion.CreatedBy = $rootScope.User != null ? $rootScope.User.UserId : 0;

        //var data = { suggestion: $scope.Suggestion };
        Suggestions.post($scope.Suggestion,
            function (response) {
                $scope.SuggestionInfo = response.Suggestions;
                $scope.Suggestion = response.Model;
            },
            function () {
                alert('Sorry, we had a problem saving your idea.');
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
                alert('Sorry, Vote not saved');
            });
        return true;
    };
}
SuggestionsCtrl.$inject = ['$scope', '$rootScope', '$cookieStore', '$location', '$http', 'Suggestions', 'Analytics'];

/*SEARCH*/
function SearchCtrl($scope, $rootScope, $cookieStore, $location, $http, Busidex, Search, Activity, Analytics, $routeParams) {
    Analytics.trackPage($location.path());
    //var userId = $rootScope.User != null ? $rootScope.User.UserId : 0;
    $scope.IsLoggedIn = $rootScope.User != null;
    $rootScope.IsLoggedIn = $rootScope.User != null;
    $rootScope.ShowFilterControls = false;
    $rootScope.SetCurrentMenuItem('Search');
    $scope.SearchByTag = searchByTag;
    $scope.SearchByOrganization = searchByOrganization;
    $scope.SearchByGroupName = searchByGroupName;
    $scope.Reset = reset;
    $scope.ShowResultsMessage = showResultsMessage;
    $scope.ShowNoOwnerMessage = showNoOwnerMessage;

    $scope.model = {
        SearchText: '',
        Results: []
    };
    $scope.model.TagSearch = false;

    $scope.ShowLogo = function(tag) {
        reset();
        if (tag === 'riarexpo') {
            $scope.model.ShowRIAR = true;
        }
    }
    

    $scope.dynamicImg = '';
    $scope.popupCard = null;
    $scope.ShowAddLink = false;
    $scope.Waiting = false;
    if ($rootScope.SearchModel == null) {
        $scope.model.SearchModel = {};
        $scope.model.SearchModel.Results = [];
    } else {
        $scope.model = $rootScope.SearchModel;
    }

    var addActivity = function (sourceId, cardId) {

        var activity =
        {
            CardId: cardId,
            UserId: $rootScope.User != null ? $rootScope.User.UserId : null,
            EventSourceId: sourceId
        };
        Activity.post(activity,
            function () {
                //console.log('event saved');
            },
            function (status) {
                //console.log('event NOT saved: ' + status);
            });
    };

    $scope.GoToDetails = function (id) {
        setTimeout('goToDetails(' + id + ')', 1000);
    };
    $scope.GoToMyBusidex = function () {
        $location.path('/busidex/mine');
    };

    if ($routeParams._t != null) {
        searchByTag($routeParams._t);
        _gaq.push(['_trackEvent', 'Search', 'Event', $routeParams._t]);
    }else if ($routeParams._o != null) {
        searchByOrganization($routeParams._o);
        _gaq.push(['_trackEvent', 'Search', 'Event', $routeParams._o]);
    } else if ($routeParams._g != null) {
        searchByGroupName($routeParams._g);
        _gaq.push(['_trackEvent', 'Search', 'Event', $routeParams._g]);
    }

    function reset() {
        $scope.model.PrivateMessage = '';
        $scope.model.SearchText = '';
        $scope.model.SearchResultsMessage = '';
        $scope.model.SearchModel.Results = [];
        $scope.model.ShowRIAR = false;
    }

    function searchByTag(tag) {
        $scope.Waiting = true;

        _gaq.push(['_trackEvent', 'Search', 'Event', tag]);

        //if (tag.toLowerCase() === 'riarexpo2015') {
        //    $scope.model.ShowRIAR = true;
        //    $scope.Waiting = false;
        //    return;
        //}
        Search.searchByTag({ systag: tag },
            function (model) {
                $scope.Waiting = false;
                $scope.model = model;
                $scope.model.TagSearch = true;
                $scope.model.SearchText = '';
                showResultsMessage();
                $scope.UserId = $rootScope.User != null ? $rootScope.User.UserId : null;
                $scope.model.PrivateMessage = '';

                var privateMessage = 'The event for which you are searching is private. Only attendees may view this. If you did attend and are seeing this message, please email us at "busidex.help@gmail.com".';
                if (model && model.SearchModel && model.SearchModel.Results && model.SearchModel.Results.length > 0) {

                    for (var i = 0; i < model.SearchModel.Results.length; i++) {

                        var thisCard = model.SearchModel.Results[i];
                        if (thisCard.OwnerId != null && thisCard.Searchable == true) {
                            thisCard.HasOwner = true;
                            addActivity($rootScope.EventSources.SEARCH, thisCard.CardId);
                        } else {
                            thisCard.HasOwner = false;
                        }
                        thisCard.FrontOrientationClass = thisCard.FrontOrientation == 'V' ? 'v_preview' : 'h_preview';
                    }
                } else {
                    $scope.model.PrivateMessage = privateMessage;
                }
                $rootScope.SearchModel = $scope.model;
            },
            function (data) {
                $scope.resultData = data;
                alert("Getting search results failed.");
                $scope.Waiting = false;
            });
    }

    function searchByOrganization(orgId) {
   
        $scope.Waiting = true;

        Search.searchByOrganization({ orgId: orgId },
        function (model) {

            $scope.Waiting = false;
            $scope.model = model;
            $scope.model.TagSearch = true;
            $scope.model.SearchText = '';
            showResultsMessage();

            if (model && model.SearchModel && model.SearchModel.Results) {

                for (var i = 0; i < model.SearchModel.Results.length; i++) {

                    var thisCard = model.SearchModel.Results[i];
                    //if (thisCard.OwnerId != null && thisCard.Searchable == true) {
                        thisCard.HasOwner = true;
                        addActivity($rootScope.EventSources.SEARCH, thisCard.CardId);
                    //} else {
                    //    thisCard.HasOwner = false;
                    //}
                    thisCard.FrontOrientationClass = thisCard.FrontOrientation == 'V' ? 'v_preview' : 'h_preview';
                }
            }

            //for (var j = 0; j < cardsData.Model.length; j++) {
            //    var card = cardsData.Model[j];

            //    card.OrientationClass = card.FrontOrientation == 'V' ? 'v_preview' : 'h_preview';
            //    card.Selected = false;
            //    $scope.Organization.Cards.push(card);
            //}

        },
        function (data) {
            $scope.resultData = data;
            alert("Getting search results failed.");
            $scope.Waiting = false;
        });
    }

    function searchByGroupName(groupName) {
        $scope.Waiting = true;

        Search.searchByGroupName({ groupName: groupName },
        function (model) {

            $scope.Waiting = false;
            $scope.model = model;
            $scope.model.TagSearch = false;
            $scope.model.SearchText = '';
            //showResultsMessage();

            if (model && model.SearchModel && model.SearchModel.Results) {

                for (var i = 0; i < model.SearchModel.Results.length; i++) {

                    var thisCard = model.SearchModel.Results[i];
                    thisCard.HasOwner = true;
                    addActivity($rootScope.EventSources.SEARCH, thisCard.CardId);
                    thisCard.FrontOrientationClass = thisCard.FrontOrientation == 'V' ? 'v_preview' : 'h_preview';
                }
            }
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
        if ($scope.model.TagSearch == true) {
            return $scope.model.SearchResultsMessage;
        }

        if ($scope.model.SearchModel.Results.length == 0) {
            $scope.model.SearchResultsMessage = "Your search for '" + $scope.model.SearchText.split(' ').join('+') + "' didn't return any results.";
        } else {
            $scope.model.SearchResultsMessage = "We searched for anything containing '" + $scope.model.SearchText.split(' ').join('\' or \'') + "' and found " + $scope.model.SearchModel.Results.length + " cards!";
        }
       
        return $scope.model.SearchResultsMessage;
    }

    $scope.doSearch = function () {

        $scope.model.NoResults = false;
       

        $scope.model.SearchModel.Results = [];
        if ($scope.model.SearchModel.Distance == 0) {
            $scope.model.SearchModel.Distance = 25;
        }
        $scope.Waiting = true;
        $scope.model.SearchModel.UserId = $rootScope.User != null ? $rootScope.User.UserId : null;
        $scope.model.UserId = null;//userId;
        $scope.model.CardType = 1; // search for Cards only

        _gaq.push(['_trackEvent', 'Search', 'General', $scope.model.SearchText]);
        Search.post($scope.model,
            function (model) {

                $scope.Waiting = false;
                $scope.model = model;
                $scope.model.TagSearch = false;
                $scope.UserId = $rootScope.User != null ? $rootScope.User.UserId : null;

                $scope.model.SearchText = model.SearchModel.SearchText;

                for (var i = 0; i < model.SearchModel.Results.length; i++) {

                    var thisCard = model.SearchModel.Results[i];
                    addActivity($rootScope.EventSources.SEARCH, thisCard.CardId);
                    if (thisCard.OwnerId != null && thisCard.Searchable == true) {
                        thisCard.HasOwner = true;
                        addActivity($rootScope.EventSources.SEARCH, thisCard.CardId);
                    } else {
                        thisCard.HasOwner = false;
                    }
                    thisCard.FrontOrientationClass = thisCard.FrontOrientation == 'V' ? 'v_preview' : 'h_preview';
                }

                $rootScope.SearchModel = $scope.model;
                $scope.model.NoResults = $scope.model.SearchModel.Results.length == 0;
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
        };

        var img = 'https://az381524.vo.msecnd.net/cards/' + card.FrontFileId + '.' + card.FrontFileType;
        $scope.dynamicImg = img;
        $scope.popupCard = card;
        $scope.ShowAddLink = !card.ExistsInMyBusidex;
        $scope.AddLinkMessage = ($scope.ShowAddLink ? "Add to " : "View in ") + $rootScope.MyBusidexName;
        $scope.ViewBusidexLink = "";
    };
    $scope.AddToMyBusidex = function (card) {

        if ($rootScope.User == null) {
            setTimeout('goToLogin()', 1000);
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

/*SHARED CARDS*/
function SharedCardCtrl($scope, $rootScope, $cookieStore, $http, SharedCard, $location, $route, $timeout) {

    $rootScope.User = $rootScope.User || $cookieStore.get('User');
    
    var userId = $rootScope.User != null ? $rootScope.User.UserId : 0;
    
    $scope.HasSharedCards = false;
    

    if ($rootScope.User != null) {
        SharedCard.get({},
            function(data) {
                if (data.SharedCards && data.SharedCards.length > 0) {
                    
                    $rootScope.HasSharedCards = true;

                    $timeout(function () {
                        if($location.path().indexOf('home') >= 0) {
                            $('#notificationPopup').modal('show');
                        }
                    }, 2000);

                    
                    $rootScope.SharedCards = [];
                    for (var i = 0; i < data.SharedCards.length; i++) {
                        $rootScope.SharedCards.push(data.SharedCards[i]);
                    }
                    
                    $scope.SendFromEmail = data.SharedCards[0].SendFromEmail;
                }
            },
            function() {

            });
    }

    $scope.AcceptSharedCards = function () {

        var accepted = [];
        var declined = [];

        for (var i = $scope.$parent.SharedCards.length - 1; i >= 0 ; i--) {
            var sharedCard = $scope.$parent.SharedCards[i];
            if (sharedCard.Accepted == "true") {
                accepted.push(sharedCard.CardId);
                $scope.$parent.SharedCards.splice(i, 1);
                $scope.SharedCards.splice(i, 1);
            }
            if (sharedCard.Accepted == "false") {
                declined.push(sharedCard.CardId);
                $scope.$parent.SharedCards.splice(i, 1);
                $scope.SharedCards.splice(i, 1);
            }
        }
        var model = { AcceptedCardIdList: accepted, DeclinedCardIdList: declined, UserId: userId, SharedWith: userId };

        SharedCard.update({ id: userId }, JSON.stringify(model),
            function () {

                if ($scope.$parent.SharedCards.length == 0) {
                    $rootScope.HasSharedCards = $scope.HasSharedCards = false;
                }
                $rootScope.MyBusidex = []; // force a refresh
                if ($location.path() == '/busidex/mine') {
                    $route.reload();
                } else {
                    $location.path('/busidex/mine');
                }
            },
            function () {

            });


    };
}
SharedCardCtrl.$inject = ['$scope', '$rootScope', '$cookieStore', '$http', 'SharedCard', '$location', '$route', '$timeout'];

/*APPLICATION VERSION*/
function ApplicationVersionCtrl($scope, ApplicationVersion) {
    $scope.Version = ApplicationVersion.Version;
}
ApplicationVersionCtrl.$inject = ['$scope', 'ApplicationVersion'];

/*FILE UPLOAD*/
function FileUploadCtrl($scope, $http) {

    //a simple model to bind to and send to the server
    $scope.model = {
        name: "",
        comments: ""
    };

    //an array of files selected
    $scope.files = [];

    //listen for the file selected event
    $scope.$on("fileSelected", function (event, args) {
        $scope.$apply(function () {
            //add the file object to the scope's files collection
            $scope.files.push(args.file);
        });
    });

    //the save method
    $scope.save = function () {
        $http({
            method: 'POST',
            url: ROOT + "/ImagePreview",
            //IMPORTANT!!! You might think this should be set to 'multipart/form-data' 
            // but this is not true because when we are sending up files the request 
            // needs to include a 'boundary' parameter which identifies the boundary 
            // name between parts in this multi-part request and setting the Content-type 
            // manually will not set this boundary parameter. For whatever reason, 
            // setting the Content-type to 'false' will force the request to automatically
            // populate the headers properly including the boundary parameter.
            headers: { 'Content-Type': false },
            //This method will allow us to change how the data is sent up to the server
            // for which we'll need to encapsulate the model data in 'FormData'
            transformRequest: function (data) {
                var formData = new FormData();
                //need to convert our json object to a string version of json otherwise
                // the browser will do a 'toString()' on the object which will result 
                // in the value '[Object object]' on the server.
                formData.append("model", angular.toJson(data.model));
                //now add all of the assigned files
                for (var i = 0; i < data.files; i++) {
                    //add each file to the form data and iteratively name them
                    formData.append("file" + i, data.files[i]);
                }
                return formData;
            },
            //Create an object that contains the model and files which will be transformed
            // in the above transformRequest method
            data: { model: $scope.model, files: $scope.files }
        }).
        success(function () {
            alert("success!");
        }).
        error(function () {
            alert("failed!");
        });
    };
};

/*PASSWORD RECOVERY*/
function PasswordRecoverCtrl($scope, $rootScope, $location, Users, Analytics) {
    Analytics.trackPage($location.path());
    $scope.Model = {};
    $scope.Model.Email = '';
    $scope.Model.Error = false;
    $scope.Model.BadEmail = false;

    if (!$scope.EmailSent) {
        $scope.Model.SendEmail = function () {

            $scope.EmailSent = true;
            Users.update({ email: $scope.Model.Email },
                function () {
                    $location.path('/account/recoversent');
                },
                function (response) {
                    if (response.status == 400 || response.status == 404) {
                        $scope.Model.BadEmail = true;
                    } else {
                        $scope.Model.Error = true;
                    }

                });
        };
    }
}
PasswordRecoverCtrl.$inject = ['$scope', '$rootScope', '$location', 'Users', 'Analytics'];

/*PASSWORD RESET*/
function PasswordResetCtrl($scope, $rootScope, $location, Login, $routeParams, $cookieStore, SharedCard, Analytics, $http) {
    Analytics.trackPage($location.path());
    $scope.Model = {};
    $scope.Model.Email = '';
    $scope.Model.Error = false;
    $scope.Model.Password = '';
    $scope.Model.ConfirmPassword = '';
    $scope.Model.BadEmail = false;

    $scope.Model.LoginData = {
        UserName: '',
        Password: '',
        Token: '',
        AcceptSharedCards: false,
        EventTag: '',
        TempData: $routeParams.data,
        RememberMe: false
    };

    $scope.passwordsMatch = function() {

        return $scope.Model.Password === $scope.Model.ConfirmPassword &&
            $scope.Model.Password !== null &&
            $scope.Model.Password.length > 0 &&
            $scope.Model.ConfirmPassword !== null &&
            $scope.Model.ConfirmPassword.length > 0;
    };

    if (!$scope.LoggedIn) {
        $scope.Model.Login = function () {
            $scope.LoggedIn = true;
            if ($scope.Model.Password == $scope.Model.ConfirmPassword) {
                $scope.Model.LoginData.Password = $scope.Model.Password;
                $scope.Model.LoginData.UserName = $scope.Model.UserName;

                Login.update($scope.Model.LoginData,
                    function (user) {

                        $scope.Waiting = false;
                        $scope.resultData = user;
                        $rootScope.User = user;
                        $rootScope.LoginModel = {
                            LoginText: $rootScope.User == null ? 'Login' : 'LogOut',
                            LoginRoute: $rootScope.User == null ? '#/account/login' : '#/account/logout',
                            HomeLink: $rootScope.User != null && $rootScope.User.StartPage == 'Organization' ? '#/groups/organization/' + $rootScope.User.Organizations[0].Item2 : '#/home'
                        }

                        $cookieStore.put('User', user);

                        var token = $rootScope.User != null ? $rootScope.User.Token : null;
                        $http.defaults.headers.common['X-Authorization-Token'] = token;

                        $location.path("/home");
                        $rootScope.IsLoggedIn = $rootScope.User != null;

                        SharedCard.get({},
                            function (data) {
                                if (data.SharedCards && data.SharedCards.length > 0) {
                                    $rootScope.HasSharedCards = true;
                                    SharedCardList = data.SharedCards;
                                    $scope.SendFromEmail = data.SharedCards[0].SendFromEmail;
                                }
                            },
                            function () {

                            });
                    },
                    function () {
                        $scope.Model.Error = true;
                        $scope.LoggedIn = false;
                        $scope.Model.BadEmail = true;
                    });
            } else {
                $scope.Model.Error = true;
                $scope.LoggedIn = false;
            }
        };
    }
}
PasswordResetCtrl.$inject = ['$scope', '$rootScope', '$location', 'Login', '$routeParams', '$cookieStore', 'SharedCard', 'Analytics', '$http'];

/*USERNAME RECOVERY*/
function UserNameRecoverCtrl($scope, $rootScope, $location, Users, Analytics) {
    Analytics.trackPage($location.path());
    $scope.Model = {};
    $scope.Model.Email = '';
    $scope.Model.BadEmail = false;
    $scope.Model.Error = false;

    $scope.Model.SendEmail = function () {
        Users.post({ email: $scope.Model.Email },
            function () {
                $location.path('/account/recoversent');
            },
            function (response) {
                if (response.status == 400 || response.status == 404) {
                    $scope.Model.BadEmail = true;
                } else {
                    $scope.Model.Error = true;
                }
            });
    };
}
UserNameRecoverCtrl.$inject = ['$scope', '$rootScope', '$location', 'Users', 'Analytics'];