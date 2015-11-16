angular.module('app').controller('PatientsDetailController', function () {

    if ($stateParams.id) {
        $scope.patient = Patient.get({ id: $stateParams.id });
    } else {
        $scope.patient = new Patient();
    }

    $scope.saveTenant = function () {
        if ($scope.patient.PatientId) {
            $scope.patient.$update(function () {
                $state.go('patient.detail');
            });
        } else {
            $scope.patient.$save(function () {
                $state.go('patient.detail');
            });
        }
    }
});