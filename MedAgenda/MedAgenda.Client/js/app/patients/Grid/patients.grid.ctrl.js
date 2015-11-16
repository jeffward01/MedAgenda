angular.module('app').controller('PatientsGridController', function ($rootScope) {
    $rootScope.$broadcast('change-page-title', { title: 'Patients' });

    if ($stateParams.id) {
        $scope.patients = Patient.get({ id: $stateParams.id });
    } else {
        $scope.patients = new Patient();
    }

    $scope.savePatient = function () {
        if ($scope.patients.PatientId) {
            $scope.patients.$update(function () {
                $state.go('patients.grid');
            });
        } else {
            $scope.patients.$save(function () {
                $state.go('patients.grid');
            });
        }
    }
});