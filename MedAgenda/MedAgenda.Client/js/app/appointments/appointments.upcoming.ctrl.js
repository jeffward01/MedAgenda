angular.module('app').controller('ApptUpcomingController', function ($rootScope, $scope, Upcoming) {
    $scope.appointments = Upcoming.query();
});