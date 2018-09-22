'use strict';

function MainCtrl($scope, $rootScope, $navigate, Activity) {
    
    $scope.$navigate = $navigate;
    $scope.LoginText = 'Sign In';

    if (localStorage.User != null && localStorage.User != 'null') {
        $scope.LoginText = 'Switch Accounts';
        $rootScope.User = JSON.parse(localStorage.User);
    }
    if ($rootScope.EventSources == null) {
        Activity.query({},
            function (data) {
                $rootScope.EventSources = data.EventSources;
            },
            function () {

            });
    }
}
MainCtrl.$inject = ['$scope', '$rootScope', '$navigate', 'Activity'];

function AdminCtrl($scope, $rootScope, $cookieStore, $location, $http) {

    $rootScope.ShowFilterControls = false;
    $rootScope.User = $rootScope.User || $cookieStore.get('User');
    $rootScope.LoginModel = {
        LoginText: $rootScope.User == null ? 'Sign In' : 'LogOut',
        LoginRoute: $rootScope.User == null ? '#/account/login' : '#/account/logout'
    };

    $rootScope.MyBusidex = $rootScope.MyBusidex || [];
    $scope.Waiting = false;
    $rootScope.IsLoggedIn = $rootScope.User != null;

    if ($rootScope.User == null) {
        $location.path("/account/login");
    } else {
     
        $http({
            method: 'GET',
            cache: false,
            url: ROOT + "/admin/newcards"
        })
            .success(function(response) {

                $scope.Cards = response.Cards;
                for (var i = 0; i < $scope.Cards.length; i++) {
                    $scope.Cards[i].Created = new Date($scope.Cards[i].Created).toDateString();
                    $scope.Cards[i].Updated = new Date($scope.Cards[i].Updated).toDateString();
                }
            })
            .error(function() {

            });


    }
}
AdminCtrl.$inject = ['$scope', '$rootScope', '$cookieStore', '$location', '$http'];

function PasswordChangeCtrl($scope, $rootScope, $cookieStore, $location, Password) {
    $rootScope.ShowFilterControls = false;
    $rootScope.User = $rootScope.User || $cookieStore.get('User');
    $rootScope.LoginModel = {
        LoginText: $rootScope.User == null ? 'Sign In' : 'LogOut',
        LoginRoute: $rootScope.User == null ? '#/account/login' : '#/account/logout'
    };

    $rootScope.MyBusidex = $rootScope.MyBusidex || [];
    $scope.Waiting = false;
    $rootScope.IsLoggedIn = $rootScope.User != null;
    $scope.PasswordInfo = {};
    
    Password.get(
        function(response) {
            response.Model.UserId = $rootScope.User.UserId;
            $scope.PasswordInfo = response.Model;
        },
        function() {

        });

    $scope.save = function() {
        Password.update($scope.PasswordInfo,
            function() {
                $location.path("/account/index");
            },
            function() {
                alert('password not changed');
            });
        return true;
    };
}
PasswordChangeCtrl.$inject = ['$scope', '$rootScope', '$cookieStore', '$location', 'Password'];

function SettingsCtrl($scope, $rootScope, $cookieStore, $location, Settings) {
    $rootScope.ShowFilterControls = false;
    $rootScope.User = $rootScope.User || $cookieStore.get('User');
    $rootScope.LoginModel = {
        LoginText: $rootScope.User == null ? 'Sign In' : 'LogOut',
        LoginRoute: $rootScope.User == null ? '#/account/login' : '#/account/logout'
    };

    $rootScope.MyBusidex = $rootScope.MyBusidex || [];
    $scope.Waiting = false;
    $rootScope.IsLoggedIn = $rootScope.User != null;
    $scope.SettingsInfo = {};

    Settings.get({ id: $rootScope.User.UserId },
        function (response) {
            response.Model.UserId = $rootScope.User.UserId;
            $scope.SettingsInfo = response.Model;
        },
        function () {

        });

    $scope.save = function () {
        
        Settings.update($scope.SettingsInfo,
            function () {
                $location.path("/account/index");
            },
            function () {
                alert('settings not changed');
            });
        return true;
    };
}
SettingsCtrl.$inject = ['$scope', '$rootScope', '$cookieStore', '$location', 'Settings'];

function GroupsCtrl($scope, $rootScope, $cookieStore, $location, Groups) {
    $rootScope.ShowFilterControls = false;
    $rootScope.User = $rootScope.User || $cookieStore.get('User');
    $rootScope.LoginModel = {
        LoginText: $rootScope.User == null ? 'Sign In' : 'LogOut',
        LoginRoute: $rootScope.User == null ? '#/account/login' : '#/account/logout'
    };

    $rootScope.MyBusidex = $rootScope.MyBusidex || [];
    $scope.Waiting = false;
    $rootScope.IsLoggedIn = $rootScope.User != null;

    if ($rootScope.User == null) {
        $location.path("/account/login");
    } else {
        Groups.get({ id: $rootScope.User.UserId },
            function(response) {
                $scope.Groups = response.Model;
            },
            function() {

            });
        
        $scope.DeleteGroup = function (groupId) {

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
        };
    }
}
GroupsCtrl.$inject = ['$scope', '$rootScope', '$cookieStore', '$location', 'Groups'];

function GroupDetailCtrl($scope, $rootScope, $cookieStore, $location, $route, Busigroup, GroupNotes) {
    
    $rootScope.ShowFilterControls = false;
    $rootScope.User = $rootScope.User || $cookieStore.get('User');
    $rootScope.LoginModel = {
        LoginText: $rootScope.User == null ? 'Sign In' : 'LogOut',
        LoginRoute: $rootScope.User == null ? '#/account/login' : '#/account/logout'
    };
    
    $rootScope.MyBusidex = $rootScope.MyBusidex || [];
    $scope.Waiting = false;
    $rootScope.IsLoggedIn = $rootScope.User != null;
    $scope.GroupId = $route.current.params.id;

    if ($rootScope.User == null) {
        $location.path("/account/login");
    } else {
        Busigroup.get({ id: $route.current.params.id },
            function(response) {

                $scope.Group = response.Model.Busigroup;
                $scope.Cards = response.Model.BusigroupCards;
            },
            function() {

            });

        $scope.delete = function() {

            if (confirm('Are you sure you want to delete this group?')) {
                
                Busigroup.remove({ id: $route.current.params.id },
                    function() {
                        $location.path("/groups/mine");
                    },
                    function() {

                    });
            }
        };
        
        $(document).on("change", "textarea.groupNotes", function () {

            GroupNotes.update({ id: $(this).attr("ucId"), notes: escape($(this).val()) },
                function () {

                },
                function () {

                });
        });
    }
}
GroupDetailCtrl.$inject = ['$scope', '$rootScope', '$cookieStore', '$location', '$route', 'Busigroup', 'GroupNotes'];

function AccountCtrl($scope, $rootScope, $cookieStore, Account) {
    $rootScope.ShowFilterControls = false;
    $rootScope.User = $rootScope.User || $cookieStore.get('User');
    $rootScope.LoginModel = {
        LoginText: $rootScope.User == null ? 'Sign In' : 'LogOut',
        LoginRoute: $rootScope.User == null ? '#/account/login' : '#/account/logout'
    };

    Account.get({ id: $rootScope.User.UserId },
        function(busidexUser) {
            $scope.UserInfo = busidexUser;
        },
        function() {

        });
    $rootScope.MyBusidex = $rootScope.MyBusidex || [];
    $scope.Waiting = false;
    $rootScope.IsLoggedIn = $rootScope.User != null;

    $scope.save = function() {
        Account.update($scope.UserInfo,
        function () {
            
        },
        function () {

        });
    };
}
AccountCtrl.$inject = ['$scope', '$rootScope', '$cookieStore', 'Account'];

function LoginViewCtrl($scope, $rootScope, $http, $cookieStore, LoginSvc, $navigate) {

    $scope.lastForm = {};
    $scope.form = {};
    $scope.form.UserName = '';
    $scope.form.Password = '';
    $scope.form.RememberMe = true;
    $rootScope.User = null;
    $scope.sendForm = function (form) {

        $scope.Waiting = true;
        $scope.lastForm = angular.copy(form);
        var _data = {
            'UserName': $scope.form.UserName,
            'Password': $scope.form.Password,
            'RememberMe': $scope.form.RememberMe
        };
        $scope.LoginErrors = [];

        LoginSvc.post(_data,
            function (user) {
                $scope.Waiting = false;
                $scope.resultData = user;
                $rootScope.User = user;

                localStorage.User = JSON.stringify(user);
                $rootScope.IsLoggedIn = $rootScope.User != null;

                if ($scope.LoginText == 'Switch Accounts') {
                    localStorage.MyBusidex = $rootScope.MyBusidex = JSON.stringify(null);
                }
                $navigate.go("/mine/");


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

    $scope.LoginText = 'Sign In';
    if (localStorage.User != null) {
        $scope.LoginText = 'Switch Accounts';
    }
    $scope.resetForm = function () {
        $scope.form = angular.copy($scope.lastForm);
    };
}
LoginViewCtrl.$inject = ['$scope', '$rootScope', '$http', '$cookieStore', 'LoginSvc', '$navigate'];

function LoginPartialViewCtrl($scope, $rootScope) {
    
    $rootScope.ShowFilterControls = false;
    $rootScope.User = $rootScope.User || localStorage.User;
    
    $rootScope.MyBusidex = $rootScope.MyBusidex || localStorage.Mybusidex || [];
    $scope.Waiting = false;
    $rootScope.IsLoggedIn = $rootScope.User != null;
}
LoginPartialViewCtrl.$inject = ['$scope', '$rootScope', '$cookieStore'];

function PopupCtrl($scope, $rootScope, $route, $routeParams) {
    
    $scope.image = {
        path: 'https://az381524.vo.msecnd.net/cards/' + $routeParams.img
    };
}
LoginPartialViewCtrl.$inject = ['$scope', '$rootScope', '$route', '$routeParams'];

function LogoutViewCtrl($scope, $rootScope, ApplicationImages) {

    $rootScope.LoginModel = {
        LoginText: $rootScope.User == null ? 'Sign In' : 'LogOut',
        LoginRoute: $rootScope.User == null ? '#/account/login' : '#/account/logout'
    };
    $scope.LoginIcon = ApplicationImages.LoginIcon;
}
LogoutViewCtrl.$inject = ['$scope', '$rootScope', 'ApplicationImages'];

function HomeViewCtrl($scope, $rootScope, $cookieStore) {
    $rootScope.ShowFilterControls = false;
    $rootScope.User = $rootScope.User || $cookieStore.get('User');
    $rootScope.MyBusidex = $rootScope.MyBusidex || [];
    $scope.model = {};
    $rootScope.IsLoggedIn = $rootScope.User != null;
    $rootScope.ShowAddCard = function () {

        return !$rootScope.User.HasCard && parseInt($rootScope.User.AccountTypeId) === ACCT_TYPE_PROFESSIONAL;
    };
}
HomeViewCtrl.$inject = ['$scope', '$rootScope', '$cookieStore'];

var Address = function (id, cardId, addr1, addr2, city, state, zip) {

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
    self.Deleted = false;
    self.DeleteSrc = '../../img/delete.png';
    self.Display = function () {
        return (self.Address1 != null ? self.Address1 : '') + ' ' +
            (self.Address2 != null ? self.Address2 : '') + ' ' +
            (self.City != null ? self.City : '') + ' ' +
            (self.State != null ? self.State.Code : '') +
            (self.ZipCode.length > 0 ? ', ' : ' ') +
            self.ZipCode;
    };
    self.Selected = false;
};

function MyBusidexCtrl($scope, $rootScope, $cookieStore, $location, Busidex, $navigate, $route, $routeParams, ApplicationImages, Activity) {
    
    $scope.SharedCardIds = {};
    $rootScope.User = $rootScope.User || $cookieStore.get('User');
    $rootScope.LoginModel = {
        LoginText: $rootScope.User == null ? 'Sign In' : 'LogOut',
        LoginRoute: $rootScope.User == null ? '#/account/login' : '#/account/logout'
    };
    $rootScope.IsLoggedIn = $rootScope.User != null;
    $scope.Waiting = true;
    $scope.GearIcon = ApplicationImages.GearIcon;
    $scope.NotesIcon = ApplicationImages.NotesIcon;
       
    $scope.data = {};
    $scope.data.TotalToShow = 20;
    $scope.data.ShowMore = function (_i) {
        if ($scope.data.TotalToShow <= $rootScope.MyBusidex.length) {
            $scope.data.TotalToShow += _i;
        }
    };
    
    $scope.ImageRoot = 'https://az381524.vo.msecnd.net/mobile-images/';

    $scope.CurrentGroup = $routeParams.gId;
    
    var useFavorites = $routeParams.gId == '1';
    $scope.Title = useFavorites ? 'Favorites' : 'My Busidex';
        
    $scope.setShowControls = function (c, show) {
        c.ShowControls = show;
        return false;
    };
    $scope.ShowImagePopup = false;
    
    $scope.ImagePopup = function (card) {
        if (card == null) {
            $scope.PopupImage = '';
            $scope.ShowImagePopup = false;
        } else {
            addActivity($rootScope.EventSources.DETAILS, card.CardId);
            $scope.PopupImage = 'https://az381524.vo.msecnd.net/cards/' + card.FrontFileId + '.' + card.FrontType.replace('.', '');
            $scope.ShowImagePopup = true;
        }
        return false;
    };
    $scope.GoToSettings = function () {
        $navigate.go('/', 'slide');
    };

    $scope.ToggleNotes = function (card) {
        card.ShowingNotes = !card.ShowingNotes;
        card.Card.NotesIcon = card.ShowingNotes ? 'images/contact.png' : 'images/notes.png';
        
    };
   
    $scope.AddActivity = function (sourceId, cardId) {
        addActivity(sourceId, cardId);
    };

    var addActivity = function(sourceId, cardId) {
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
    $scope.openUrl = function (card) {
        try {
            addActivity($rootScope.EventSources.WEBSITE, card.CardId);

            var urlToOpen = 'http://' + card.Url;
            window.open(urlToOpen, '_system');
        } catch (e) {
            
        }
        return false;
    };
    $scope.openMap = function (card) {
        try {
            var address = card.Addresses[0];
            addActivity($rootScope.EventSources.MAP, card.CardId);
            
            var urlToOpen = encodeURI('http://maps.google.com/maps?daddr={' + address.Display() + '}');
            window.open(urlToOpen, '_system');
        } catch(e) {
            
        }
        return false;
    };

    if (localStorage.User == null || localStorage.User == 'null') {
        $navigate.go('/login', 'slide');
    } else {
        $rootScope.User = JSON.parse(localStorage.User);
        
        $rootScope.ShowFilterControls = true;
        
        if (localStorage.MyBusidex == 'null' || localStorage.MyBusidex == undefined || localStorage.MyBusidex == null || $routeParams.sync == 'sync') {

            Busidex.get({ id: $rootScope.User.UserId },
                function(_model) {
                    var len = _model.MyBusidex.Busidex.length;
                    for (var i = 0; i < len; i++) {
                        var _thisCard = _model.MyBusidex.Busidex[i].Card;
                        _model.MyBusidex.Busidex[i].OrientationClass = _thisCard.FrontOrientation == 'V' ? 'v_preview' : 'h_preview';
                        _thisCard.VClass = _thisCard.FrontOrientation == 'V' ? 'v_notesIcon' : '';
                        _thisCard.ShowingNotes = false;
                        _thisCard.NotesIcon = 'images/notes.png';
                        _thisCard.MapIcon = 'images/map.png';
                        _thisCard.Addresses = loadAddresses(_thisCard.Addresses);
                    }
                    
                    $scope.MyBusidex = _model.MyBusidex.Busidex;
                    $rootScope.MyBusidex = $scope.MyBusidex || [];
                    $scope.Waiting = false;
                    localStorage.MyBusidex = JSON.stringify($scope.MyBusidex);
                }, function(data) {
                    $scope.resultData = data;
                    alert("Getting MyBusidex failed.");
                });
            
        } else {
            var model = JSON.parse(localStorage.MyBusidex);
            var len = model.length;
            for (var i = 0; i < len; i++) {
                var thisCard = model[i].Card;
                model[i].OrientationClass = thisCard.FrontOrientation == 'V' ? 'v_preview' : 'h_preview';
                thisCard.VClass = thisCard.FrontOrientation == 'V' ? 'v_notesIcon' : '';
                thisCard.ShowingNotes = false;
                thisCard.NotesIcon = 'images/notes.png';
                thisCard.MapIcon = 'images/map.png';
                thisCard.Addresses = loadAddresses(thisCard.Addresses);
            }
            $scope.MyBusidex = model;
            $rootScope.MyBusidex = $scope.MyBusidex || [];
            $scope.Waiting = false;
        }
    }
    function loadAddresses(addressInfo) {
        var addresses = new Array();
        for (var j = 0; j < addressInfo.length; j++) {
            var a = addressInfo[j];
            addresses.push(
                new Address(a.CardAddressId, a.CardId, a.Address1, a.Address2, a.City, a.State, a.ZipCode)
            );
        }
        return addresses;
    }
}
MyBusidexCtrl.$inject = ['$scope', '$rootScope', '$cookieStore', '$location', 'Busidex', '$navigate', '$route', '$routeParams', 'ApplicationImages', 'Activity'];

function CardDetailCtrl($scope, $rootScope, $cookieStore, $location, $http, $route, $routeParams) {
    
    $rootScope.ShowFilterControls = false;
    $rootScope.User = $rootScope.User || $cookieStore.get('User');
    $rootScope.LoginModel = {
        LoginText: $rootScope.User == null ? 'Sign In' : 'LogOut',
        LoginRoute: $rootScope.User == null ? '#/account/login' : '#/account/logout'
    };
    $rootScope.IsLoggedIn = $rootScope.User != null;
    $scope.Card = null;
    $scope.ShowBack = false;
    $scope.ShowFront = false;
    $scope.BackOrientationClass = '';
    $scope.FrontOrientationClass = '';
    
    if ($rootScope.User == null) {
        $location.path("/account/login");
    } else {
        var _data = {
            'id': $routeParams.CardId
        };
        $scope.Waiting = true;
        $http({
            method: 'GET',
            url: ROOT + "/card/details/?id=" + _data.id,
            data: JSON.stringify(_data),
            headers: { 'Content-Type': 'application/json' }
        }).success(function (data) {
            
            $scope.FrontOrientationClass = data.Model.FrontOrientation == 'V' ? 'v_preview' : 'h_preview';
            $scope.BackOrientationClass = data.Model.BackOrientation == 'V' ? 'v_preview' : 'h_preview';
            $scope.UserId = $rootScope.User.UserId;
            $scope.Card = data.Model;
            $scope.ShowBack = data.Model.HasBackImage;
            $scope.ShowFront = data.Model.FrontFileId != null;
            $scope.Waiting = false;
        }).error(function (data) {
            $scope.resultData = data;
            alert("Getting card detail failed.");
        });
    }
}
CardDetailCtrl.$inject = ['$scope', '$rootScope', '$cookieStore', '$location', '$http', '$route', '$routeParams'];

function SearchCtrl($scope, $rootScope, $cookieStore, $location, $http, $navigate, ApplicationImages, Activity) {

    $rootScope.ShowFilterControls = false;
    $rootScope.User = $rootScope.User || $cookieStore.get('User');
    $rootScope.LoginModel = {
        LoginText: $rootScope.User == null ? 'Sign In' : 'LogOut',
        LoginRoute: $rootScope.User == null ? '#/account/login' : '#/account/logout'
    };
    var userId = $rootScope.User != null ? $rootScope.User.UserId : 0;
    $scope.IsLoggedIn = $rootScope.User != null;
    $rootScope.IsLoggedIn = $rootScope.User != null;
    $scope.model = {
        SearchText: '',
        Results: []
    };
    $scope.GearIcon = 'images/gear.png';
    $scope.dynamicImg = '';
    $scope.popupId = 0;
    $scope.ShowAddLink = false; 
    $scope.Waiting = false;
    $scope.CheckImage = ApplicationImages.Check;

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

    $scope.GoToSettings = function () {
        $navigate.go('/', 'slide');
    };
    $scope.openMap = function (card) {
        addActivity($rootScope.EventSources.MAP, card.CardId);
        var urlToOpen = encodeURI('http://maps.google.com/maps?daddr={' + card.Addresses[0].Display() + '}');
        window.open(urlToOpen, '_system');
        return false;
    };        
    $scope.openUrl = function (card) {
        try {
            addActivity($rootScope.EventSources.WEBSITE, card.CardId);

            var urlToOpen = 'http://' + card.Url;
            window.open(urlToOpen, '_system');
        } catch (e) {

        }
        return false;
    };

    $scope.ImageRoot = 'https://az381524.vo.msecnd.net/mobile-images/';
    $scope.ImagePopup = function (card) {
        if (card == null) {
            $scope.PopupImage = '';
            $scope.ShowImagePopup = false;
        } else {
            addActivity($rootScope.EventSources.DETAILS, card.CardId);
            $scope.PopupImage = 'https://az381524.vo.msecnd.net/cards/' + card.FrontFileId + '.' + card.FrontFileType.replace('.', '');
            $scope.ShowImagePopup = true;
        }
    };
    $rootScope.ShowFilterControls = false;
    $scope.doSearch = function () {

        $scope.model.SearchModel.Results = [];
        $scope.model.UserId = userId;

        $scope.Waiting = true;
        $http({
            method: 'POST',
            url: ROOT + "/search/?userId=" + userId,
            data: JSON.stringify($scope.model),
            cache: false,
            headers: { 'Content-Type': 'application/json' }
        }).success(function(model) {

            $scope.Waiting = false;
            $scope.model = model;

            $scope.model.SearchText = model.SearchModel.SearchText;
            
            var len = model.SearchModel.Results.length;
            for (var i = 0; i < len; i++) {

                var thisCard = model.SearchModel.Results[i];

                addActivity($rootScope.EventSources.SEARCH, thisCard.CardId);

                model.SearchModel.Results[i].FrontOrientationClass = model.SearchModel.Results[i].FrontOrientation == 'V' ? 'v_preview' : 'h_preview';
                thisCard.IconClass = thisCard.OwnerId ? "HasOwner" : "InMyBusidex";
                thisCard.VClass = thisCard.FrontOrientation == 'V' ? 'v_notesIcon' : '';
                thisCard.MapIcon = 'images/map.png';

                var addresses = new Array();
                for (var j = 0; j < thisCard.Addresses.length; j++) {

                    var a = thisCard.Addresses[j];
                    addresses.push(
                        new Address(a.CardAddressId, a.CardId, a.Address1, a.Address2, a.City, a.State, a.ZipCode)
                    );
                }

                thisCard.Addresses = addresses;
            }

            $rootScope.SearchModel = $scope.model;            

        }).error(function(data) {
            $scope.resultData = data;
            alert("Getting search results failed.");
            $scope.Waiting = false;
        });
    };
    $scope.setPopupImage = function (id, img, inMyBusidex) {
        $scope.dynamicImg = img;
        $scope.popupId = id;
        $scope.ShowAddLink = !inMyBusidex;        
    };
    $scope.AddToMyBusidex = function (card) {

        var thisCard = card;
        addActivity($rootScope.EventSources.ADD, thisCard.CardId);
        $http({
            url: ROOT + '/busidex?userId=' + $rootScope.User.UserId + '&cId=' + thisCard.CardId,
            method: 'POST'
        })
            .success(function () {

                thisCard.ExistsInMyBusidex = true;
                localStorage.MyBusidex = null;
                $scope.ViewBusidexLink = "#/busidex/mine";
                $scope.ShowAddLink = false;
            })
            .error(function () {
                alert('There was a problem adding this card to your Busidex');
                return false;
            });
    };
}
SearchCtrl.$inject = ['$scope', '$rootScope', '$cookieStore', '$location', '$http', '$navigate', 'ApplicationImages', 'Activity'];

function SharedCardCtrl($scope, $rootScope, $cookieStore, $http, SharedCard) {
    
    $rootScope.User = $rootScope.User || $cookieStore.get('User');
    var userId = $rootScope.User != null ? $rootScope.User.UserId : 0;
    $scope.SharedCards = [];
    $scope.HasSharedCards = false;

    SharedCard.get({ id: userId },
        function(data) {
            if (data.SharedCards && data.SharedCards.length > 0) {
                $scope.HasSharedCards = true;
                $scope.SharedCards = data.SharedCards;
                $scope.SendFromEmail = data.SharedCards[0].SendFromEmail;
            }
        },
        function() {

        });

    $scope.SaveSharedCards = function () {
        $http({
            method: 'POST',
            url: ROOT + "/SharedCard/?userId=" + userId,
            data: JSON.stringify($scope.SharedCards),
            headers: { 'Content-Type': 'application/json' }
        }).success(function () {

        }).error(function() {

        });
    };
    
    $scope.AcceptSharedCards = function() {

        var accepted = [];
        var declined = [];
        
        for (var i = $scope.$parent.SharedCards.length - 1; i >=0 ; i--) {
            var sharedCard = $scope.$parent.SharedCards[i];
            if (sharedCard.Accepted == "true") {
                accepted.push(sharedCard.CardId);
                $scope.$parent.SharedCards.splice(i, 1);
                $scope.SharedCards.splice(i, 1);
            }
            if (sharedCard.Accepted == "false"){
                declined.push(sharedCard.CardId);
                $scope.$parent.SharedCards.splice(i, 1);
                $scope.SharedCards.splice(i, 1);
            }
        }
        var model = { AcceptedCardIdList: accepted, DeclinedCardIdList: declined, UserId: userId, SharedWith: userId };

        SharedCard.update({ id: userId }, JSON.stringify(model),
            function(data) {

                if ($scope.$parent.SharedCards.length == 0) {
                    $rootScope.HasSharedCards = $scope.HasSharedCards = false;
                }
            },
            function(error) {

            });
        
        
    };
}
SharedCardCtrl.$inject = ['$scope', '$rootScope', '$cookieStore', '$http', 'SharedCard'];

function AddCardCtrl($scope, $rootScope, $cookieStore, $http, MobileUpload, PhoneGap, Camera, $window, $location, $navigate) {

    $rootScope.User = $rootScope.User || $cookieStore.get('User');
    if (localStorage.User == null || localStorage.User == 'null') {
        $navigate.go('/login', 'slide');
        return;
    }

    $scope.Model = {};
    $scope.ImageSet = false;

    var ImageDTO = function() {
        var self = this;
        self.src = '';
        self.name = '';
        self.phone = '';
        self.email = '';
        self.mId = $rootScope.User.UserId;
    };
    $scope.Model.Image = new ImageDTO();

    $scope.Loading = false;
    $scope.Model.WaitImage = 'images/spinner.gif';
    $scope.CameraIcon = 'images/camera.png';
    $scope.getPicture = function() {
        $scope.Loading = true;
        Camera.getPicture(
            function (img) { // success
                $scope.Model.Image.src = "data:image/jpeg;base64," + img;
                $window.document.getElementById('imgPreview').src = $scope.Model.Image.src;
                $scope.Loading = false;
                $scope.ImageSet = true;
            },
            function(message) { // error
                setTimeout(alert('get picture failed: ' + message), 10);
                $scope.Loading = false;
                $scope.ImageSet = false;
            },
            { // options
                quality: 50,
                destinationType: Camera.DestinationType.DATA_URL, 
                sourceType: Camera.PictureSourceType.CAMERA
            });
    };

    $scope.save = function () {
        
        //if ($scope.Model.Image.src == null) {
        //    return false;
        //}
        
        $scope.Loading = true;

        var root = 'https://www.busidexapi.com/api';
        //var root = 'http://local.busidexapi.com/api';

        $http({
            method: 'POST',
            url: root + '/MobileCard',
            cache: false,
            data: JSON.stringify($scope.Model.Image)
        }).success(function () {
            $scope.Loading = false;
            $location.path('/mine/sync');
            
        }).error(function() {
            alert('There was a problem uploading your card');
            $scope.Loading = false;
        });
        
        //var ft = new FileTransfer();
        //ft.upload($scope.Model.Image.src, encodeURI(root + "/MobileCard/?mId=" + $rootScope.User.UserId + "&phone=" + $scope.Model.Image.phone + "&name=" + $scope.Model.Image.name),
        //    function (meta) { setTimeout(alert("Win: " + JSON.stringify(meta)), 10); },
        //    function (meta) { setTimeout(alert("Fail:" + JSON.stringify(meta)), 10); });
        return true;
    };
}
AddCardCtrl.$inject = ['$scope', '$rootScope', '$cookieStore', '$http', 'MobileUpload', 'PhoneGap', 'Camera', '$window', '$location', '$navigate'];

function RegistrationViewCtrl($scope, $http, $rootScope, $cookieStore, Registration, $navigate) {

    $rootScope.ShowFilterControls = false;
    $rootScope.User = $rootScope.User || $cookieStore.get('User');
    $rootScope.LoginModel = {
        LoginText: $rootScope.User == null ? 'Sign In' : 'LogOut',
        LoginRoute: $rootScope.User == null ? '#/account/login' : '#/account/logout'
    };
    $rootScope.MyBusidex = $rootScope.MyBusidex || [];
    $scope.Waiting = false;
    $rootScope.IsLoggedIn = $rootScope.User != null;

    $scope.ShowOwnerCard = false;
    $scope.CurrentStep = 1;
    $scope.RegistrationErrors = [];
    $http.defaults.useXDomain = true;
    Registration.get({},
        function (model) {
            model.UserName = '';
            model.Email = '';
            model.ConfirmEmail = '';
            model.Password = '';
            model.ConfirmPassword = '';
            model.HumanQuestion = "How many letters are there in the word Busidex? (Hint: use a number, don't spell out the word.)";
            model.AccountTypeId = 6;
            model.IsMobile = true;
            model.IAgree = 'No';
            model.MobileNumber = '';
            $scope.Model = model;
        },
        function () {

        });

    $scope.openUrl = function (url) {
        try {
            
            window.open(url, '_system', 'menubar=no,location=no');
        } catch (e) {

        }
        return false;
    };

    $scope.Regsister = function () {
        
        $scope.RegistrationErrors = [];

        $http.defaults.headers.post['Content-Type'] = ''
           + 'application/x-www-form-urlencoded; charset=UTF-8';
        $http.defaults.transformRequest = function (model) {
            return "model=" + JSON.stringify(model);
        };

        $scope.Model.IAgree = ($scope.Model.IAgree == 'Yes') ? true : false;

        Registration.post($scope.Model, function () {
            $scope.SetStep(4);
        },
        function (error) {
            $scope.RegistrationErrors.push(error.data.Message);
        });
    };

    $scope.CheckVerification = function() {
        Registration.update({ token: $scope.Model.ConfirmationCode },
            function (user) {
                
                $rootScope.User = user;

                localStorage.User = JSON.stringify(user);

                $rootScope.IsLoggedIn = $rootScope.User != null;

                localStorage.MyBusidex = $rootScope.MyBusidex = JSON.stringify(null);

                $navigate.go("/mine/");
            },
            function(error) {
                $scope.RegistrationErrors.push(error.data.Message);
            });
    };

    $scope.SetStep = function (step) {
        $scope.CurrentStep = step;
    };
}
RegistrationViewCtrl.$inject = ['$scope', '$http', '$rootScope', '$cookieStore', 'Registration', '$navigate'];

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

