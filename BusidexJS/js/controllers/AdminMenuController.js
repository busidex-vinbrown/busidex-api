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
    $scope.MenuItems.push({ Name: 'Back to Dashboard', Link: '#/admin/index' });

    $scope.IsAdmin = true;
}