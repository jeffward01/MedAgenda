angular.module('app').controller('PatientsGridController', function ($rootScope, $scope, Patient, patientService) {
    $rootScope.$broadcast('change-page-title', { title: 'Patients' });

    $scope.patients = Patient.query();

    $scope.deletePatient = function (patient) {
        if (confirm('Are you sure you want to delete this Patient?')) {
            Patient.delete({ id: patient.PatientID }, function (data) {
                var index = $scope.patients.indexOf(patient)
                $scope.patients.splice(index, 1);

            });
        }
        toastr.error('Tenant entry was erased!', 'Tenant Erased!');
        }
    
});