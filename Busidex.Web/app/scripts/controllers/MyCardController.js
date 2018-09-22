/*MY CARD*/
angular.module('Busidex').controller('MyCardController', [
    '$cookieStore', '$location', '$http', 'Card', 'Activity', 'Analytics', 'SharedCard', 'Communications', 'SharedCardPreview', 'OutlookContacts', 'Cache', 'CacheKeys', 'SharedCardCart', '$window',
    function ($cookieStore, $location, $http, Card, Activity, Analytics, SharedCard, Communications, SharedCardPreview, OutlookContacts, Cache, CacheKeys, SharedCardCart, $window) {
        'use strict';

        var monthNames = [
            'January', 'February', 'March', 'April', 'May', 'June',
            'July', 'August', 'September', 'October', 'November', 'December'
        ];

        var vm = this;
        var user = Cache.get(CacheKeys.User);
        user = angular.fromJson(user);

        vm.User = user;

        if (vm.User === null) {
            $location.path('/login');
            return;
        } else {

            $http.defaults.headers.common['X-Authorization-Token'] = user.Token;

            vm.Waiting = true;
            getMyCard();
        }

        vm.ShowPreviousMonth = showPreviousMonth;
        vm.ShowNextMonth = showNextMonth;
        vm.ShowBack = false;
        vm.ShowFront = false;
        vm.Sharing = false;
        vm.SelectAll = false;
        vm.AdvancedSharing = false;
        vm.ShowSelectedOnly = false;
        vm.Model = {};
        vm.Model.ShareWith = '';
        vm.ShareMyCard = shareMyCard;
        vm.ToggleSharing = toggleSharing;
        vm.ToggleAdvancedSharing = toggleAdvancedSharing;
        vm.ToggleShowSelected = toggleShowSelected;
        vm.CancelSharing = cancelSharing;
        vm.ToggleSelectAll = toggleSelectAll;
        vm.ShowPreview = showPreview;
        vm.ShareWithAll = shareWithAll;
        vm.SendTestEmail = sendTestEmail;
        vm.SwitchAccounts = switchAccounts;
        vm.EncodeStr = encode;
        vm.ConnectedAccount = '';
        vm.PersonalMessage = '';
        vm.PreviewBody = null;
        vm.ClosePreview = closePreview;
        vm.AdvancedSharingOptions = {
            Gmail: 'Gmail',
            Yahoo: 'Yahoo',
            Outlook: 'Outlook'
        };
        vm.Contacts = [];
        vm.Model.OutlookFile = '';
        vm.LoadOutlookContacts = loadOutlookContacts;
        vm.Model.SelectedSharingOption = '';
        vm.EmailListString = '';
        vm.Filter = '';
        vm.ReadContacts = readContacts;
        vm.NoCardClick = true;
        vm.MonthlyActivities = {};
        var acctType = vm.User !== null ? parseInt(vm.User.AccountTypeId) : 0;
        vm.ShowActions = acctType !== ACCT_TYPE_BASIC;
        vm.CurrentMonthOffest = 0;
        vm.CurrentMonth = getCurrentMonth();

       function closePreview() {
            vm.PreviewBody = null;
        }

        function showPreview() {
            var shareWith = vm.Model.ShareWith;
            var list = [];
            list.push(vm.card.CardId);

            var data = {
                SharedCardId: 0,
                CardId: vm.card.CardId,
                SendFrom: vm.User.UserId,
                SendFromEmail: '',
                Email: shareWith,
                ShareWith: 0,
                SharedDate: new Date(),
                Accepted: false,
                Declined: false,
                Recommendation: encodeURIComponent(vm.PersonalMessage)
            };

            SharedCardPreview.post(data,
                function(response) {
                    vm.PreviewBody = response.Template.Body;
                }, function() {

                });
        }

        function sendTestEmail() {

            var message = encodeURIComponent(vm.PersonalMessage);

            var sharedCard = {
                CardId: vm.card.CardId,
                SendFrom: vm.User.UserId,
                Email: '',
                SharedDate: new Date(),
                Accepted: null,
                Declined: null,
                Recommendation: message
            };

            SharedCard.sendTest(sharedCard, function() {

                    vm.Model.ShareWith = '';

                    getLastCommunicationDates(vm.EmailListString);
                    window.alert('A sample email has been sent to your account email.');
                },
                function() {

                });
        }

        function shareWithAll() {

            if (!window.confirm('Send your card to all selected email addresses now?')) {
                return;
            }

            var selected = 0;
            var sharedCards = [];
            var sharedDate = new Date();
            var message = encodeURIComponent(vm.PersonalMessage);
            for (var i = 0; i < vm.Contacts.length; i++) {
                if (vm.Contacts[i].Selected) {
                    selected++;

                    var sharedCard = {
                        CardId: vm.card.CardId,
                        SendFrom: vm.User.UserId,
                        Email: vm.Contacts[i].Email,
                        SharedDate: sharedDate,
                        Accepted: null,
                        Declined: null,
                        Recommendation: message
                    };
                    sharedCards.push(sharedCard);

                    vm.Contacts[i].LastSharedDate = new Date().toLocaleDateString();
                }
            }

            if (sharedCards.length > 0) {
                SharedCard.post(sharedCards, function() {

                        vm.Model.ShareWith = '';

                        getLastCommunicationDates(vm.EmailListString);
                        window.alert('Your Card Has Been Shared!');
                    },
                    function() {

                    });
            }
        }

        function getLastCommunicationDates(emailList) {

            Communications.post({ EmailList: emailList, UserId: vm.User.UserId },
                function(data) {

                    if (data.Communications !== null) {

                        var communications = data.Communications;
                        for (var i = 0; i < vm.Contacts.length; i++) {

                            if (communications[vm.Contacts[i].Email] !== null) {
                                vm.Contacts[i].LastSharedDate = new Date(communications[vm.Contacts[i].Email].DateSent).toLocaleDateString();
                            }
                        }
                    }
                }, function() {

                });
        }

        function toggleSelectAll() {
            vm.SelectAll = !vm.SelectAll;
            for (var i = 0; i < vm.Contacts.length; i++) {
                vm.Contacts[i].Selected = vm.SelectAll;
            }
        }

        function switchAccounts() {
            var logout = '<iframe id="logoutframe" src="https://accounts.google.com/logout" style="display: none"></iframe>';

            $(logout).appendTo('body');
            setTimeout(readContacts, 2000);

        }

        function loadOutlookContacts() {

            var fd = new FormData();
            fd.append('file', vm.Model.OutlookFile);
            OutlookContacts.post(fd,
                function(data) {
                    var contacts = [];
                    var emailList = [];

                    for (var i = 0; i < data.Contacts.length; i++) {

                        var name = data.Contacts[i].FirstName + ' ' + data.Contacts[i].LastName;
                        if (name === ' ') {
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

                    vm.Contacts = contacts;
                    vm.EmailListString = emailList.join(',');
                    toggleSharing();
                    getLastCommunicationDates(emailList);
                    vm.AdvancedSharing = !vm.AdvancedSharing;
                },
                function() {

                });
        }

        function readContacts() {

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
                    $.get('https://www.google.com/m8/feeds/contacts/default/full?alt=json&access_token=' + authResult.access_token + '&max-results=700&v=3.0',
                        function(response) {

                            vm.ConnectedAccount = response.feed.author[0].email.$t;

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

                            vm.$apply(function() {
                                vm.Contacts = contacts;
                                vm.EmailListString = emailList.join(',');
                                toggleSharing();
                                getLastCommunicationDates(emailList);
                            });

                        });
                }
            }
        }

        function toggleShowSelected() {
            vm.ShowSelectedOnly = !vm.ShowSelectedOnly;
        }

        function toggleSharing() {
            if (!vm.Sharing) {
                var list = [];
                list.push(vm.card);
                var noName = !list[0].Name || list[0].Name.length === 0;
                var noCompanyName = !list[0].CompanyName || list[0].CompanyName.length === 0;
                if (noName && noCompanyName) {
                    window.alert('You have to fill in your card name or company before sharing.');
                    return;
                }
            }
            vm.Sharing = !vm.Sharing;
        }

        function cancelSharing() {
            vm.Sharing = false;
        }

        function shareMyCard() {

            SharedCardCart.put(vm.card);
            //var shareWith = vm.Model.ShareWith;
            //var sharedCards = [];
            //var sharedCard = {
            //    CardId: vm.MyCards[0].CardId,
            //    SendFrom: vm.User.UserId,
            //    Email: shareWith,
            //    SharedDate: new Date(),
            //    Accepted: null,
            //    Declined: null,
            //    Recommendation: encodeURIComponent(vm.PersonalMessage)
            //};
            //sharedCards.push(sharedCard);

            //SharedCard.post(sharedCards, function() {

            //        vm.Model.ShareWith = '';
            //        window.alert('Your Card Has Been Shared!');
            //    },
            //    function() {

            //    });
            //vm.Sharing = false;
        }

        function toggleAdvancedSharing(option) {

            switch (option) {
            case vm.AdvancedSharingOptions.Gmail:
            {
                vm.AdvancedSharing = !vm.AdvancedSharing;
                vm.Model.SelectedSharingOption = vm.AdvancedSharingOptions.Gmail;
                readContacts();
                break;
            }
            case vm.AdvancedSharingOptions.Outlook:
            {
                vm.Model.SelectedSharingOption = vm.AdvancedSharingOptions.Outlook;
                break;
            }
            case vm.AdvancedSharingOptions.Yahoo:
            {
                vm.Model.SelectedSharingOption = vm.AdvancedSharingOptions.Yahoo;
                break;
            }
            default:
            {
                vm.AdvancedSharing = !vm.AdvancedSharing;
                vm.Model.SelectedSharingOption = {};
                break;
            }
            }
        }

        function getMyCard() {

            if(vm.User.CardId <= 0){
                $window.location.href = 'https://start.busidex.com/#/front/' + encode(vm.User.Token);
                return;
            }

            Card.get({ id: vm.User.CardId },
                function(response) {
                    //console.log('Front file: ' + response.Model.FrontFileId);
                    if (response.Model.FrontFileId === null) {
                        $window.location.href = 'https://start.busidex.com/#/front/' + encode(vm.User.Token);
                    } else {


                        response.Model.FrontOrientationClass = response.Model.FrontOrientation === 'V' ? 'v_thumbnail' : 'h_thumbnail';
                        response.Model.BackOrientationClass = response.Model.BackOrientation === 'V' ? 'v_thumbnail' : 'h_thumbnail';
                        response.Model.ShowFront = response.Model.FrontFileId !== null;
                        response.Model.ShowBack = response.Model.HasBackImage;

                        loadActivities(response.Model.CardId);

                        vm.UserId = vm.User.UserId;
                        vm.card = response.Model;
                        vm.Waiting = false;
                        vm.NoCards = response.Model === null;

                        document.title = response.Model.Name + ' | Busidex';
                    }
                },
                function(data) {
                    vm.resultData = data;
                    window.alert('Getting My Card failed.');
                    vm.Waiting = false;
                });
           
        }       

        function showPreviousMonth() {
            vm.CurrentMonthOffest--;
            vm.CurrentMonth = getCurrentMonth();
            loadActivities(vm.card.CardId);
        }

        function showNextMonth() {
            vm.CurrentMonthOffest++;
            vm.CurrentMonth = getCurrentMonth();
            loadActivities(vm.card.CardId);
        }

        function getCurrentMonth() {

            var date = new Date();
            date.setDate(1);
            date.setMonth(date.getMonth() + vm.CurrentMonthOffest);
            var m = monthNames[date.getMonth()];

            return { Name: m, Month: date.getMonth() + 1, Year: date.getFullYear() };
        }

        function loadActivities(id) {

            Activity.get({ cardId: id, month: vm.CurrentMonth.Month },
                function(results) {

                    vm.Sources = results.Sources;
                    vm.MonthlyActivities = [];
                    if (results.Activities[0] !== undefined && results.Activities[0].Data !== null) {
                        for (var i = 0; i < results.Sources.length > 0; i++) {
                            var item = results.Activities[0].Data[results.Sources[i].EventCode];
                            if (item !== undefined) {
                                vm.MonthlyActivities.push(
                                    {
                                        SourceCode: results.Sources[i].EventCode,
                                        Description: results.Sources[i].Description,
                                        Value: item
                                    }
                                );
                            }
                        }

                        vm.Chart = {};
                        vm.Chart.labels = [];
                        vm.Chart.data = [];
                        vm.colors = ['#5993a9', '#01262f', '#479A1B', '#004b5c', '#73a4b7', '#FF6600', '#777', '#49311c', '#f46066', '#a460f4'];
                        if (vm.MonthlyActivities.length > 0) {
                            for (var j = 0; j < vm.MonthlyActivities.length; j++) {
                                vm.Chart.labels.push(vm.MonthlyActivities[j].Description);
                                vm.Chart.data.push(vm.MonthlyActivities[j].Value);
                            }
                            vm.Chart.colours = vm.colors;
                        } else {
                            vm.MonthlyActivities.push(
                                    {
                                        SourceCode: '',
                                        Description: 'No Data To Display',
                                        Value: null
                                    }
                                );
                            vm.Chart.labels.push(vm.MonthlyActivities[0].Description);
                            vm.Chart.data.push(vm.MonthlyActivities[0].Value);
                            vm.Chart.colours = ['#ffffff'];
                        }
                    }

                },
                function() {

                });
        }

        function encode(str) {
            return escape(str);
        }

        vm.getGraphValueBackground = function(idx) {
            var style = { backgroundColor: vm.colors[idx] };

            return style;
        };

        vm.DeleteCard = function(id) {

            if (!window.confirm('Are you sure you want to delete your card? Everyone with whom you have shared your card will no longer be able to see it.')) {
                return;
            }

            Card.remove({ id: id, userId: vm.User.UserId },
                function() {
                    getMyCard();
                },
                function() {

                });
        };
    }
]);

