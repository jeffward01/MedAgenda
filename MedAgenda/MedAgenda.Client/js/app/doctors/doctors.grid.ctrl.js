angular.module('app').controller('DoctorsGridController', function($rootScope, Doctor, $scope) {
    $rootScope.$broadcast('change-page-title', { title: 'Doctors' });

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
                toastr.error('Doctor entry was erased!', 'Doctor Erased!');
            });
        }
    };

    $scope.load();
});