angular.module('app').controller('DoctorDetailController', function ($rootScope, $state, $stateParams, Doctor) {
     $rootScope.$broadcast('change-page-title', { title: 'Doctors' })

    if($stateParams.id) {
        $scope.doctor = Doctor.get({ id: $stateParams.id });       
    } else {
        $scope.doctor = new Doctor();   
    }

    $scope.saveDoctor = function() {
        if($scope.doctor.DoctorId) {            
            $scope.doctor.$update(function() {
                $state.go('doctor.grid'); 
            }); 
        } else {
            $scope.doctor.$save(function() {
                $state.go('doctor.grid');
            });
        }   
    };
});