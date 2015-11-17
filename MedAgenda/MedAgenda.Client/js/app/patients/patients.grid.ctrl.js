angular.module('app').controller('PatientsGridController', function ($rootScope, $scope, Patient, apiUrl) {
    $rootScope.$broadcast('change-page-title', { title: 'Patients' });

    $scope.patients = Patient.query();

    $scope.deletePatient = function (patient) {
        if (confirm('Are you sure you want to delete this Patient?')) {
            Patient.delete({ id: patient.PatientID }, function (data) {
                $scope.patient = Patient.query();

            });
        }
    }
});