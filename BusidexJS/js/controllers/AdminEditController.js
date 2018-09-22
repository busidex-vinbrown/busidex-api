/*ADMIN EDIT*/
function AdminEditCtrl($scope, $rootScope, $cookieStore, $location, $http) {


    $scope.Waiting = true;

    $scope.Cards = [];

    if ($rootScope.User === null) {
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
                            PhoneNumber: c.PhoneNumbers[0] !== null ? c.PhoneNumbers[0].Number : ''//,
                            //Save: function () {

                            //    $http({
                            //        method: 'PUT',
                            //        cache: false,
                            //        url: ROOT + "/admin/SaveCardInfo",
                            //        data: this
                            //    }).success(function () {
                            //    }).error(function () {

                            //    });
                            //}
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