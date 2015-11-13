angular.module('app').controller('DashboardController', function ($rootScope, $scope) {
    $rootScope.$broadcast('change-page-title', { title: 'Dashboard' });
});