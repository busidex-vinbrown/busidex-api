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
    };
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
            }, function (status) {

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
                };
                sharedCards.push(sharedCard);

                $scope.Contacts[i].LastSharedDate = new Date().toLocaleDateString();
            }
        }

        if (sharedCards.length > 0) {
            SharedCard.post(sharedCards, function () {

                $scope.Model.ShareWith = '';

                getLastCommunicationDates($scope.EmailListString);
                alert('Your Card Has Been Shared!');
            },
                function () {

                });
        }
    }
    function getLastCommunicationDates(emailList) {

        Communications.post({ EmailList: emailList, UserId: $rootScope.User.UserId },
            function (data) {

                if (data.Communications !== null) {

                    var communications = data.Communications;
                    for (var i = 0; i < $scope.Contacts.length; i++) {

                        if (communications[$scope.Contacts[i].Email] !== null) {
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
    function loadOutlookContacts() {

        var fd = new FormData();
        fd.append('file', $scope.Model.OutlookFile);
        OutlookContacts.post(fd,
            function (data) {
                var contacts = [];
                var emailList = [];

                for (var i = 0; i < data.Contacts.length; i++) {

                    var name = data.Contacts[i].FirstName + ' ' + data.Contacts[i].LastName;
                    if (name == ' ') {
                        name = data.Contacts[i].CompanyName;
                    }
                    if (name.trim() === '') {
                        name = '(No Name)';
                    }
                    var contact = {
                        Name: name,
                        Email: data.Contacts[i].Email,
                        Selected: false,
                        LastSharedDate: '(not available)'
                    };
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
            function () {

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

                          if (data[i].gd$email === undefined || data[i].gd$email[0] === undefined) {
                              continue;
                          }

                          var contact = {
                              Name: data[i].title.$t,
                              Email: data[i].gd$email[0].address.trim(),
                              Selected: false,
                              LastSharedDate: '(not available)'
                          };
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
            var noName = !list[0].Name || list[0].Name.length === 0;
            var noCompanyName = !list[0].CompanyName || list[0].CompanyName.length === 0;
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
        };
        sharedCards.push(sharedCard);

        SharedCard.post(sharedCards, function () {

            $scope.Model.ShareWith = '';
            alert('Your Card Has Been Shared!');
        },
            function () {

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
                model.MyCards[i].ShowFront = model.MyCards[i].FrontFileId !== null;
                model.MyCards[i].ShowBack = model.MyCards[i].HasBackImage;

                loadActivities(model.MyCards[i].CardId);
            }
            $scope.UserId = $rootScope.User.UserId;
            $scope.MyCards = model.MyCards;
            $scope.Waiting = false;
            $scope.NoCards = model.MyCards.length === 0;

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

        var m = new Date().getMonth();

        Activity.get({ cardId: id, month: $scope.CurrentMonth.Month },
            function (results) {

                $scope.Sources = results.Sources;
                $scope.MonthlyActivities = results.Activities.length > 0 ? results.Activities[0].Data : {};

            },
            function (error) {
                //alert(error.status);
            });
    }

    if ($rootScope.User === null) {
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
