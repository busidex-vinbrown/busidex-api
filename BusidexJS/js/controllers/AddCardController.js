/*ADD CARD*/
function AddCardCtrl($scope, $rootScope, $cookieStore, $location, $http, $route, $routeParams, phoneNumberTypes, stateCodes, fileSizeInfoContent, Busidex, $timeout, Analytics) {
    Analytics.trackPage($location.path());
    $scope.Card = {};
    $scope.Tabs = {};
    $rootScope.ShowFilterControls = false;

    var DEFAULT_IMAGE = 'b66ff0ee-e67a-4bbc-af3b-920cd0de56c6';

    $scope.Tabs.details = false;
    $scope.Tabs.tags = false;
    $scope.Tabs.notes = false;
    $scope.Tabs.mycard = false;
    $scope.Tabs.address = false;
    $scope.Tabs.phone = false;
    $scope.Tabs.scorecard = false;

    $rootScope.IsLoggedIn = $rootScope.User !== null;

    $scope.files = [];
    $scope.files.push("");
    $scope.files.push("");

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

                if (args.idx === 0) {
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
        if ($scope.Model.FrontImage !== null || $scope.Model.HasFrontImage) {
            p += 10;
        }
        if ($scope.Model.BackImage !== null || $scope.Model.HasBackImage) {
            p += 10;
        }
        if ($scope.Model.Name !== null && $scope.Model.Name.length > 0) {
            p += 10;
        }
        if ($scope.Model.CompanyName !== null && $scope.Model.CompanyName.length > 0) {
            p += 10;
        }
        if ($scope.Model.Title !== null && $scope.Model.Title.length > 0) {
            p += 5;
        }
        if ($scope.Model.Email !== null && $scope.Model.Email.length > 0) {
            p += 10;
        }
        if ($scope.Model.Url !== null && $scope.Model.Url.length > 0) {
            p += 5;
        }
        if ($scope.Model.PhoneNumbers !== null && $scope.Model.PhoneNumbers.length > 0 && $scope.Model.PhoneNumbers[0].Number.length > 0) {
            p += 10;
        }
        if ($scope.Model.Addresses !== null && $scope.Model.Addresses.length > 0) {

            var addr = $scope.Model.Addresses[0];
            if (addr.Address1 !== null && addr.Address1.length > 0) {
                p += 5;
            }
            if (addr.Address2 !== null && addr.Address2.length > 0) {
                p += 3;
            }
            if (addr.City !== null && addr.City.length > 0) {
                p += 5;
            }
            if (addr.State !== null && addr.State.StateCodeId > 0) {
                p += 5;
            }
            if (addr.ZipCode !== null && addr.ZipCode.length > 0) {
                p += 5;
            }
        }
        if ($scope.Model.Tags !== null && $scope.Model.Tags.length > 0) {
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
                model.Model.FrontType = model.Model.BackType = 'png';
            }
            if (model.Model.FrontFileId !== null && model.Model.FrontFileId.length > 0) {
                model.Model.FrontFileId = 'https://az381524.vo.msecnd.net/cards/' + model.Model.FrontFileId + '.' + model.Model.FrontType.replace('.', '');
            }
            if (model.Model.BackFileId !== null && model.Model.BackFileId.length > 0) {
                model.Model.BackFileId = 'https://az381524.vo.msecnd.net/cards/' + model.Model.BackFileId + '.' + model.Model.BackType.replace('.', '');
            }

            var addresses = [];
            var i = 0;
            for (i = 0; i < model.Model.Addresses.length; i++) {

                var a = model.Model.Addresses[i];
                addresses.push(
                    new Address(a.CardAddressId, a.CardId, a.Address1, a.Address2, a.City, a.State, a.ZipCode)
                );
            }

            model.Model.Addresses = addresses;

            model.Model.Notes = unescape(model.Model.Notes);

            $scope.FrontOrientationClass = model.Model.FrontOrientation === 'V' ? 'v_preview' : 'h_preview';
            $scope.BackOrientationClass = model.Model.BackOrientation === 'V' ? 'v_preview' : 'h_preview';
            $scope.UserId = $rootScope.User.UserId;

            model.Model.Visibility = model.Model.Visibility || 1;
            if (model.Model.Visibility === 0) model.Model.Visibility = 1;

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
            $scope.Title = ((_data.id === 0 || _data.id == 1) ? 'ADD ' + a_or_your : 'EDIT ' + a_or_your) + ' BUSINESS CARD';

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
            for (i = 0; i < stateCodes.length; i++) {
                if (stateCodes[i].StateCodeId == $scope.Model.Addresses[0].State.StateCodeId) {
                    $scope.Model.Addresses[0].State = stateCodes[i];
                    break;
                }
            }

            $scope.Model.AddPhoneNumber = function () {

                for (i = 0; i < $scope.Model.PhoneNumbers.length; i++) {
                    if ($scope.Model.PhoneNumbers[i].PhoneNumberTypeId === null) {
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

                if ($scope.Model.Email === null || $scope.Model.Email.length === 0) {
                    $scope.Model.IsValid = false;
                    $scope.SetCurrentTab('details');
                } else if ($scope.Model.CardId <= 1 && ($scope.files.length === 0 || $scope.files[0].length === 0)) {
                    $scope.Model.IsValid = false;
                    $scope.SetCurrentTab('mycard');
                } else if ($scope.Model.PhoneNumbers.length === 0 || $scope.Model.PhoneNumbers[0].Number.length === 0) {
                    $scope.Model.IsValid = false;
                    $scope.SetCurrentTab('phone');
                }

                return $scope.Model.IsValid;
            };
            $scope.Model.NewCardSaved = false;

            $scope.Model.AddAddress = function () {
                var addr = $scope.TempAddress;
                var validation = addr.Address1 + addr.Address2 + addr.City + addr.State.Code + addr.ZipCode + addr.Region + addr.Country;
                if (validation.length === 0) {
                    return;
                }
                var selectedIdx = -1;
                for (i = 0; i < $scope.Model.Addresses.length; i++) {
                    if ($scope.Model.Addresses[i].Selected === true) {
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

                $scope.Model.UserId = $rootScope.User.UserId;

                if (!$scope.Model.Validate()) {
                    return false;
                }
                if ($scope.ResetBackImage === true) {
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
                        var savedAddresses = [];
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
                }).success(function (result) {
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
                        if ($scope.ExistingCards.length === 0) {
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

            if ($scope.Model !== null &&
            ($scope.Model.Name === null || $scope.Model.Name.length === 0) &&
            ($scope.Model.CompanyName === null || $scope.Model.CompanyName.length === 0)) {
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

        }).error(function (data) {
            $scope.resultData = data;
            alert("Getting card failed.");
        });
    };

    $scope.NoDetails = function () {
        return ($scope.Model.Name === null || $scope.Model.Name.length === 0) && ($scope.Model.CompanyName === null || $scope.Model.CompanyName.length === 0);
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
            transformRequest: function (data) {

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
            success(function (data) {
                if (idx === 0) {
                    $scope.Model.FrontFileId = "data:image/gif;base64," + JSON.parse(data);
                }
                if (idx == 1) {
                    $scope.Model.BackFileId = "data:image/gif;base64," + JSON.parse(data);
                }

            }).
            error(function () {
                alert("failed!");
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
        $scope.Model.BackFileId = 'https://az381524.vo.msecnd.net/cards/' + DEFAULT_IMAGE + '.png';
    };

    $scope.SetCurrentTab('details');

    if ($rootScope.User === null) {
        $location.path("/account/login");
    } else {
        getModel();
    }
}
AddCardCtrl.$inject = ['$scope', '$rootScope', '$cookieStore', '$location', '$http', '$route', '$routeParams', 'phoneNumberTypes', 'stateCodes', 'fileSizeInfoContent', 'Busidex', '$timeout', 'Analytics'];
