angular.module('app').controller('PatientCheckGridController', function ($rootScope, $scope, $compile, patientChecksService, $http, apiUrl, $stateParams, PatientChecks) {
    $rootScope.$broadcast('change-page-title', {
        title: 'Patient Check'
    });

    $scope.PatientChecks = PatientChecks.query();
  
    $scope.PatientCheckOut = function (patient) {
        //Save Current DateTime
        var currentDate = moment.valueOf();
        currentDate =  moment(currentDate, "YYY-MM-DDTHH:mm:ssZ").toDate(); 
        patient.CheckOutDateTime = currentDate;

        console.log(patient);
        patient.$update(
            function () {
                alert("Patient logged out!");
         toastr.success(doctor.Doctor.FirstName + " " + doctor.Doctor.LastName + " has been Logged out!", 'Success!');
 $scope.PatientChecks.splice($scope.PatientChecks.indexOf(patient), 1);

            },
            function () {
                alert("Error logging out!");
            }
        );
    }
});


