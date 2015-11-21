angular.module('app').controller('ApptUpcomingController', function ($rootScope, $scope, Upcoming) {
    
    $scope.load = function () {
        $scope.loading = true;
        $scope.appointments = Upcoming.query(function () {
            $scope.loading = false;
        });
    };

    $scope.load();
});