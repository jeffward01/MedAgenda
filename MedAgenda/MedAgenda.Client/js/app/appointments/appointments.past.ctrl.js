angular.module('app').controller('ApptPastController', function ($rootScope, $scope, Past) {
    $scope.appointments = Past.query();
});