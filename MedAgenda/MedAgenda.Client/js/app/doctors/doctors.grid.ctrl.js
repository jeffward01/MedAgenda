angular.module('app').controller('DoctorsGridController', function($rootScope, Doctor, $scope) {


    $scope.load = function () {
        $scope.loading = true;
        $scope.doctors = Doctor.query(function () {
            $scope.loading = false;
        });
    };
    

    $scope.deleteDoctor = function (doctor) {
        if (confirm('Are you sure you want to delete this doctor?')) {
            Doctor.delete({ id: doctor.DoctorId }, function (data) {
                $scope.doctor = Doctor.query();
            });
        }
    };

    $scope.load();
});