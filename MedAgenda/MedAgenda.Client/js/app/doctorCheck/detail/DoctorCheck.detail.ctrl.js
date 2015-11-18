angular.module('app').controller('DoctorCheckDetailController', function ($rootScope, $scope, $compile, patientChecksService, $http, apiUrl, $log, $state, DoctorChecks) {
    
        $rootScope.$broadcast('change-page-title', {
        title: 'Doctor Check-In'
    });

    $scope.data = {
        selectedDoctor: {}
    };

    $scope.newDoctorCheckIn = new DoctorChecks();

    $scope.clear = function () {
        $scope.data.selectedDoctor = {};
    }


    patientChecksService.getAllDoctors().then(function (data) {
        $scope.allDoctors = data;
    });


    patientChecksService.getAllSpecialties().then(function (data) {
        $scope.allSpecialties = data;
    });
    
    patientChecksService.getAllExamRooms().then(function(data){
        $scope.allExamRooms = data;
    })



    $scope.save = function () {
        //Save Current DateTime
        var currentDate = moment.valueOf();
        currentDate = moment(currentDate, "YYY-MM-DDTHH:mm:ssZ").toDate();
        $scope.newDoctorCheckIn.CheckinDateTime = currentDate;
        $scope.newDoctorCheckIn.DoctorID = $scope.data.selectedDoctor.DoctorID;
        $scope.newDoctorCheckIn.ExamRoomID = $scope.data.allExamRooms.ExamRoomID;
        if($scope.newDoctorCheckIn.$save()){

        //Success Message
        toastr.success($scope.data.selectedDoctor.FirstName + " " + $scope.data.selectedDoctor.LastName + " has been checked in!", 'Success!');    
                $state.go('app.dashboard');
    
        } else{
                  toastr.danger($scope.data.selectedDoctor.FirstName + " " + $scope.data.selectedDoctor.LastName + " was not checked in!", 'Failure!');    
  
        }

    }
    
    function getExamRoom(id){
        for(var i = 0; i < $scope.allSpecialties.length; i++){
            var specialty = allSpecialties[i];
            if(specialty == id){
                return specialty;
            }
            else {
                return "Error";
            }
        }
    }
    
});
