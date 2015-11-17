angular.module('app').controller('PatientCheckDetailController', function ($rootScope, $scope, $compile, patientChecksService, $http, apiUrl, PatientChecks) {

    $rootScope.$broadcast('change-page-title', {
        title: 'Patient Check'
    });
$scope.itemArray = [
                {
                    id: 1,
                    name: 'first'
                },
                {
                    id: 2,
                    name: 'second'
                },
                {
                    id: 3,
                    name: 'third'
                },
                {
                    id: 4,
                    name: 'fourth'
                },
                {
                    id: 5,
                    name: 'fifth'
                },
    ];

            $scope.selectedItem = $scope.itemArray[0];



});