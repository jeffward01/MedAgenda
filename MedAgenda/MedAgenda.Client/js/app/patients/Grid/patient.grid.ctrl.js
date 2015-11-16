angular.module('app').controller('PatientsGridController', function ($rootScope) {
    $rootScope.$broadcast('change-page-title', { title: 'Patients' });

    if ($stateParams.id) {
        $scope.patient = Patient.get({ id: $stateParams.id });
    } else {
        $scope.patient = new Patient();
    }

    $scope.savePatient = function () {
        if ($scope.patient.PatientId) {
            $scope.patient.$update(function () {
                $state.go('patient.grid');
            });
        } else {
            $scope.patient.$save(function () {
                $state.go('patient.grid');
            });
        }
    }
});