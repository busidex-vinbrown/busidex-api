/*GROUP DETAILS*/
function GroupDetailCtrl($scope, $rootScope, $cookieStore, $location, $route, Busigroup, GroupNotes) {

    $scope.Waiting = false;
    $rootScope.SetCurrentMenuItem('Groups');
    $scope.GroupId = $route.current.params.id;

    if ($rootScope.User === null) {
        $location.path("/account/login");
    } else {
        Busigroup.get({ id: $route.current.params.id },
            function (response) {

                $scope.Group = response.Model.Busigroup;
                $scope.Cards = [];
                for (var i = 0; i < response.Model.BusigroupCards.length; i++) {
                    var card = response.Model.BusigroupCards[i];
                    card.Ready = true;

                    card.OrientationClass = card.FrontOrientation == 'V' ? 'v_preview' : 'h_preview';

                    $scope.Cards.push(card);
                    //$scope.Cards[$scope.Cards.length-1].Ready = true;
                }

                //setTimeout('showCards()', 2000);

            },
            function () {

            });

        $scope.delete = function () {

            if (confirm('Are you sure you want to delete this group?')) {

                Busigroup.remove({ id: $route.current.params.id },
                    function () {
                        $location.path("/groups/mine");
                    },
                    function () {

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

    $scope.showCards = function() {
        for (var i = 0; i < $scope.Cards.length; i++) {
            $scope.Cards[i].Ready = true;
        }
    };
}
GroupDetailCtrl.$inject = ['$scope', '$rootScope', '$cookieStore', '$location', '$route', 'Busigroup', 'GroupNotes'];