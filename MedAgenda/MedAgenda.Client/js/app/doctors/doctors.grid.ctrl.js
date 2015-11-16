angular.module('app').controller('DoctorGridController', function($rootScope, Doctor) {
    $scope.doctor = Doctor.query();

    $scope.deleteDoctor= function(doctor) {
        if(confirm('Are you sure you want to delete this doctor?')) {
            Doctor.delete({ id: doctor.DoctorId }, function (data) {
                $scope.doctor = Doctor.query();
            }); 
        }   
    }
});