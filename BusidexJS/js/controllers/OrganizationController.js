/*ORGANIZATION*/
function OrganizationCtrl($http, $scope, $rootScope, $cookieStore, $location, $route, $routeParams, Organizations, $sce, $timeout, Busigroup, Groups, Search, FileUpload, SharedCard, Busidex) {
    $scope.Tabs = {};
    $scope.Tabs.home = false;
    $scope.Tabs.details = false;
    $scope.Tabs.addmembers = false;
    $scope.Tabs.guests = false;
    $scope.Tabs.members = false;
    $scope.Tabs.addgroup = false;
    $scope.Tabs.referrals = false;
    $scope.Tabs.share = false;
    $scope.Tabs.groupdetails = false;
    $scope.Tabs.homepage = false;

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
    $rootScope.ShowFilterControls = false;
    $scope.Errors = [];
    $scope.MemberReferrals = [];
    $scope.MyBusidex = [];
    $scope.EditingGroup = false;
    $scope.EditingDetails = false;
    $scope.IsOrganizationAdmin = isOrganizationAdmin;
    $scope.IsOrganizationMember = isOrganizationMember;

    if ($rootScope.MyBusidex === null || $rootScope.MyBusidex.length === 0) {
        Busidex.get({},
            function (model) {
                $scope.MyBusidex = [];
                for (var j = 0; j < model.MyBusidex.Busidex.length; j++) {

                    var busidexCard = model.MyBusidex.Busidex[j].Card;
                    model.MyBusidex.Busidex[j].OrientationClass = busidexCard.FrontOrientation === 'V' ? 'v_preview' : 'h_preview';
                    if (busidexCard.OwnerId !== null) {
                        busidexCard.PersonalMessage = '';
                        $scope.MyBusidex.push(model.MyBusidex.Busidex[j]);
                    }
                }
            },
            function () {
                alert('error');
            });
    } else {
        $scope.MyBusidex = [];
        for (var i = 0; i < $rootScope.MyBusidex.length; i++) {

            var thisCard = $rootScope.MyBusidex[i];
            if (thisCard.OwnerId !== null) {
                thisCard.PersonalMessage = '';
                $scope.MyBusidex.push(model.MyBusidex.Busidex[i]);
            }
        }

    }

    $rootScope.SetCurrentMenuItem('Memberships');

    Organizations.select({ id: $routeParams.id },
        function (data) {
            loadOrganization(data);
        },
        function () {

        });

    function isOrganizationAdmin() {
        return $scope.Organization.Role === 'Admin';
    }

    function isOrganizationMember() {
        return $scope.Organization.Role === 'Member';
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

        if ($scope.Organization.LogoFilePath !== null) {
            $scope.Organization.LogoFilePath = $scope.Organization.LogoFilePath.replace('http:', 'https:');
        }

        $scope.Organization.Role = data.OrgRole;

        if ($scope.Organization === null || $scope.Organization.OrganizationId === 0 ||
            ($scope.Organization.Name === null && $scope.Organization.Description === null && $scope.Organization.Url === null && $scope.Organization.Phone1 === null)) {
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
        if (groupId === null) groupId = 0;

        var data = { userId: $scope.Organization.OrganizationId, groupTypeId: GROUPTYPE_ORGANIZATION, id: groupId, cardIds: $scope.Organization.CardIds.join(), description: $scope.CurrentGroup.Description };

        if ($scope.CurrentGroup.GroupId > 0) {
            Groups.update(data,
                function () {

                    $scope.Tabs.addgroup = false;
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
                function () {

                    $scope.Tabs.addgroup = false;

                    Organizations.select({ id: $routeParams.id },
                        function (org) {
                            loadOrganization(org);
                            var newId = $scope.Organization.Groups[$scope.Organization.Groups.length - 1].GroupId;
                            getGroup(newId);
                            $scope.Tabs.groupdetails = true;
                        },
                        function () {

                        });
                },
                function () {
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
        $scope.Tabs.home = true;
    }

    function editGroup(id) {
        setCurrentTab('_' + id);
        $scope.Tabs.addgroup = true;
        $scope.EditingGroup = true;
        $scope.Organization.CardIds = [];
        $scope.CurrentGroup.GroupId = id;
        for (var b = 0; b < $scope.Organization.Cards.length; b++) {
            var busidexCard = $scope.Organization.Cards[b];
            for (var c = 0; c < $scope.CurrentGroup.Cards.length; c++) {
                var groupCard = $scope.CurrentGroup.Cards[c];
                busidexCard.Selected = false;
                if (busidexCard.CardId === groupCard.CardId) {
                    busidexCard.Selected = true;
                    $scope.Organization.CardIds.push(busidexCard.CardId);
                    break;
                }
            }
        }
    }

    function toggleReferral(card) {
        card.Selected = !card.Selected;
        if (card.Selected === true) {
            $scope.MemberReferrals.push(card.CardId);
        } else {
            for (var j = 0; j < $scope.Organization.CardIds.length; j++) {
                if ($scope.Organization.CardIds[j] === card.CardId) {
                    $scope.MemberReferrals.splice(j, 1);
                    break;
                }
            }
        }
    }

    function toggleSelectedOrGoToDetails(card) {

        if ($scope.Tabs.addmembers === true || $scope.Tabs.addgroup === true) {
            card.Selected = !card.Selected;
            if (card.Selected === true) {
                $scope.Organization.CardIds.push(card.CardId);
            } else {
                for (var j = 0; j < $scope.Organization.CardIds.length; j++) {
                    if ($scope.Organization.CardIds[j] === card.CardId) {
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
       
        setCurrentTab("homepage");
    }

    function getMembers() {

        $scope.Organization.Cards = [];
        Organizations.getMembers({ organizationId: $scope.Organization.OrganizationId },
        function (cardsData) {

            for (var j = 0; j < cardsData.Model.length; j++) {
                var card = cardsData.Model[j];

                card.OrientationClass = card.FrontOrientation === 'V' ? 'v_preview' : 'h_preview';
                card.Selected = false;
                $scope.Organization.Cards.push(card);
            }

        },
        function () {

        });
    }

    function getGuests() {

        Organizations.getGuests({ organizationId: $scope.Organization.OrganizationId },
            function (response) {

                $scope.Organization.Guests = response.Guests;
            },
            function () {

            });
    }

    function updateGuestStatus(guest, newStatus) {

        guest.AddStatus = newStatus;
        for (var g = 0; g < $scope.Organization.Guests.length; g++) {
            if ($scope.Organization.Guests[g].UserCardId === guest.UserCardId) {
                $scope.Organization.Guests.splice(g, 1);
            }
        }

        Busidex.updateCardStatus(guest,
            function () {

            },
            function () {

            }
        );
    }

    function getHtml() {
        if ($scope.Organization === null) return '';
        return $sce.trustAsHtml($scope.Organization.HomePage);
    }

    function saveDetails() {

        $scope.Errors = [];

        if ($scope.Organization === null || $scope.Organization.OrganizationId === null || $scope.Organization.OrganizationId === 0) {
            Organizations.post($scope.Organization,
                function () {
                    $scope.DetailsSaved = true;
                    $scope.EditingDetails = false;
                    if ($scope.Logo !== null) {
                        FileUpload.UploadFile($scope.Logo, ROOT + '/Organization/UpdateLogo?id=' + $scope.Organization.OrganizationId,
                            function () {
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
                    if ($scope.Logo !== null) {
                        FileUpload.UploadFile($scope.Logo, ROOT + '/Organization/UpdateLogo?id=' + $scope.Organization.OrganizationId,
                            function () {
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
                function (response) {
                    if (response.status == 400) {
                        $scope.Errors.push(response.data.Message);
                    }
                }
            );
        }
    }

    function removeMember(card) {

        var cardId = card.CardId;
        var message = card.IsMyCard ? 'Are you sure you want to leave this organization?' : 'Are you sure you want to remove this member?';

        if (confirm(message)) {
            for (var j = 0; j < $scope.Organization.Cards.length; j++) {
                var orgCard = $scope.Organization.Cards[j];
                if (orgCard.CardId === cardId) {
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
        $scope.Tabs.members = true;
    }

    function hideAddMembers() {
        $scope.Tabs.addmembers = false;
    }

    function addMember(card) {

        for (var j = 0; j < $scope.MemberSearch.Cards.length; j++) {
            var memberCard = $scope.MemberSearch.Cards[j];
            if (memberCard.CardId === card.CardId) {
                $scope.MemberSearch.Cards.splice(j, 1);
                card.FrontType = card.FrontFileType;
                card.OrientationClass = card.FrontOrientationClass;
                $scope.Organization.Cards.push(card);

                Organizations.addMember({ organizationId: $scope.Organization.OrganizationId, cardId: card.CardId },
                    function () {

                    },
                    function () {

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
        };

        if (model.Distance === 0 || model.Distance === null) {
            model.Distance = 25;
        }

        Search.post(model,
            function (response) {

                $scope.Waiting = false;

                $scope.MemberSearch.Criteria = response.SearchModel.SearchText;

                for (var j = 0; j < response.SearchModel.Results.length; j++) {

                    var modelCard = response.SearchModel.Results[j];
                    modelCard.FrontOrientationClass = modelCard.FrontOrientation == 'V' ? 'v_preview' : 'h_preview';
                    var memberExists = false;
                    for (var c = 0; c < $scope.Organization.Cards.length; c++) {

                        if ($scope.Organization.Cards[c].CardId === modelCard.CardId) {
                            memberExists = true;
                            break;
                        }
                    }
                    if (!memberExists) {
                        $scope.MemberSearch.Cards.push(modelCard);
                    }

                }


            }, function (data) {
                $scope.resultData = data;
                alert("Getting search results failed.");
                $scope.Waiting = false;
            });
    }

    function getGroup(groupId) {

        if (groupId === null) {
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
                $scope.Tabs.groupdetails = true;
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
                if (card.Notes !== null) {
                    card.Notes = unescape(card.Notes);
                }
                card.OrientationClass = card.FrontOrientation === 'V' ? 'v_preview' : 'h_preview';
                //card.Selected = false;
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

    function share() {

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
                };
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
