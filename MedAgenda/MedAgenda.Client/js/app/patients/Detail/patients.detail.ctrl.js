angular.module('app').controller('PatientsDetailController', function ($scope, $stateParams, Patient, $state) {

    if ($stateParams.id) {
        $scope.patient = Patient.get({ id: $stateParams.id });
    } else {
        $scope.patient = new Patient();
    }

    $scope.saveTenant = function () {
        if ($scope.patient.PatientId) {
            $scope.patient.$update(function () {
                $state.go('patients.grid');
            });
        } else {
            $scope.patient.$save(function () {
                $state.go('patients.grid');
            });
        }
    }
});