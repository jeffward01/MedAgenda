angular.module('app').controller('PatientCheckDetailController', function ($rootScope, $scope, $compile, patientChecksService, $http, apiUrl, $state, PatientChecks) {
    $rootScope.$broadcast('change-page-title', {
        title: 'Patient Check In'
    });

    $scope.data = {
        selectedPatient: {}
    };

    $scope.newPatientCheckIn = new PatientChecks();

    $scope.clear = function () {
        $scope.data.selectedPatient = {};
    }


    patientChecksService.getAllPatients().then(function (data) {
        $scope.allPatients = data;
    });


    patientChecksService.getAllSpecialties().then(function (data) {
        $scope.allSpecialties = data;
    });



    $scope.save = function () {
        //Save Current DateTime
        var currentDate = moment.valueOf();
        currentDate = moment(currentDate, "YYY-MM-DDTHH:mm:ssZ").toDate();
        $scope.newPatientCheckIn.CheckinDateTime = currentDate;
        $scope.newPatientCheckIn.PatientID = $scope.data.selectedPatient.PatientID;

        $scope.newPatientCheckIn.$save(
            function (data) {
                toastr.success($scope.data.selectedPatient.FirstName + " " + $scope.data.selectedPatient.LastName + " has been checked in!", 'Success!');

                $http.post(apiUrl + 'api/appointments/schedule', JSON.stringify(data), {
                    headers: {
                        'Content-Type': 'application/json'
                    }
                }).then(
                    function (response) {
                        //Success Message
                        toastr.success("An appointment was successfully created for " + $scope.data.selectedPatient.FirstName + " " + $scope.data.selectedPatient.LastName + "!", 'Success!');
                        $state.go('app.dashboard');
                    },
                    function (err) {
                        toastr.error("Appointment could not be created for " + $scope.data.selectedPatient.FirstName + " " + $scope.data.selectedPatient.LastName + "!", 'Failure!');
                    }
                );
            },
            function (err) {
                toastr.error($scope.data.selectedPatient.FirstName + " " + $scope.data.selectedPatient.LastName + " was not checked in!", 'Failure!');
            }
        );
    };
});