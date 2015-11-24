angular.module('app').controller('DoctorCheckGridController', function ($rootScope, $scope, $compile, patientChecksService, $http, apiUrl, $log, DoctorChecks) {

    $rootScope.$broadcast('change-page-title', {
        title: 'Doctor Check Out'
    });

    $scope.DoctorChecks = DoctorChecks.query();

    patientChecksService.getAllCheckedInDoctors().then(
        function (data) {
            $scope.CheckedInDoctors = data;
        },
        function (err) {

        }
    );

    $scope.DoctorCheckOut = function (doctor) {
        //Save Current DateTime
        var currentDate = moment.valueOf();
        currentDate = moment(currentDate, "YYY-MM-DDTHH:mm:ssZ").toDate();
        doctor.CheckOutDateTime = currentDate;

        console.log(doctor);
        doctor.$update(
            function () {
                toastr.success(doctor.Doctor.FirstName + " " + doctor.Doctor.LastName + " has been checked out!", 'Success!');
                $scope.DoctorChecks.splice($scope.DoctorChecks.indexOf(doctor), 1);

            },
            function () {
              toastr.error("Error checking " + doctor.Doctor.FirstName + " " + doctor.Doctor.LastName + " out. Please try again."  , 'Failure!');

            }
        );
    }

});