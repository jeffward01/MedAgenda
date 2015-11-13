angular.module('app').controller('AdminController', function ($rootScope, $scope) {
    $rootScope.$broadcast('change-page-title', { title: 'Administration' });
});