angular.module('app').controller('ApptPastController', function ($rootScope, $scope, Past) {
   
    $scope.load = function () {
        $scope.loading = true;
        $scope.appointments = Past.query(function () {
            $scope.loading = false;
        });
    };

    $scope.load();

});