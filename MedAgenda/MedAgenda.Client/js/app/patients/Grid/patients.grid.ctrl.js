angular.module('app').controller('PatientsGridController', function ($rootScope, $scope, Patient) {
    $rootScope.$broadcast('change-page-title', { title: 'Patients' });

    $scope.patients = Patient.query();

    $scope.deletePatient = function (patient) {
        if (confirm('Are you sure you want to delete this Patient?')) {
            Patient.delete({ id: patient.PatientId }, function (data) {
                var index = $scope.patients.indexOf(patient);
                $scope.patientss.splice(index, 1);
            });
        }
    }
});