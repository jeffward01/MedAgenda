angular.module('app').controller('PatientCheckDetailController', function ($rootScope, $scope, $compile, patientChecksService, $http, apiUrl, $state, PatientChecks) {
    $rootScope.$broadcast('change-page-title', {
        title: 'Patient Check'
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
        if($scope.newPatientCheckIn.$save()){

        //Success Message
        toastr.success($scope.data.selectedPatient.FirstName + " " + $scope.data.selectedPatient.LastName +  " has been checked in!", 'Success!');    
                $state.go('app.dashboard');
    
        } else{
                  toastr.danger($scope.data.selectedPatient.FirstName + " " + $scope.data.selectedPatient.LastName + " was not checked in!", 'Failure!');    
  
        }

    }
});