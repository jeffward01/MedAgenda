angular.module('app').controller('PatientsDetailController', function ($scope, $rootScope, $stateParams, Patient, $state, patientService) {
    $rootScope.$broadcast('change-page-title', { title: 'Patients' });

    $('.dropdown-toggle').dropdown();


        // If an ID was passed to state, then a tenant is being edited: get the tenant to update
        //  otherwise a tenant is being added: create a new tenant
        if ($stateParams.id) {
            $scope.patient = Patient.get({
                id: $stateParams.id
            });
        } else {
            $scope.patient = new Patient();
        }

        // Save tenant:
        // If an ID was passed to state, then a tenant is being edited: update the tenant
        //  otherwise a tenant is being added: save the tenant
        // After updating or saving, change state to tenants.list
        $scope.savePatient = function () {


            //Validation to ensure no fields are empty
            if (($('#patientFirstName').val() === "") || ($('#patientLastName').val() === "") || ($('#patientEmail').val() === "") || ($('#patientTelephone').val() === "") || ($('#patientBirthdate').val() === "") || ($('#patientBloodType').val() === "") || ($('#patientAddress1').val() === "") || ($('#patientCity').val() === "") || ($('#patientState').val() === "") || ($('#patientZip').val() === "")) {
                alert("Please input all fields");
                return false;
            }

            //Success Call back and Change of State
            var successCallback = function () {
                $state.go('app.patients.grid');
            };

            if ($scope.patient.PatientID) {
                $scope.patient.$update(successCallback);
            } else {
                $scope.patient.$save(successCallback);
            }
            toastr.success('Patient was added!', 'New patient was saved successfuly!');

        };

        $scope.saveEmergencyContact = function (ec) {
            // call the $save method, passing in ec.
        };

    
        $scope.addEmergencyContact = function () {
            $scope.patient.EmergencyContacts.push({});
        }

});
