angular.module('app').controller('AdminSpecialtiesController', function ($rootScope, $scope, Specialty) {

    $rootScope.$broadcast('change-page-title', { title: 'Administration: Manage Specialties' });

    $scope.specialties = Specialty.query();

    $scope.deleteSpecialty = function (specialty) {
        if (confirm('Are you sure you want to delete specialty: ' +
                                                    specialty.SpecialtyName + '?')) {
            Specialty.delete({ id: specialty.SpecialtyId }, function (data) {
                var index = $scope.specialties.indexOf(specialty);
                $scope.specialties.splice(index, 1);
                toastr.success('The specialty ' +
                                specialty.SpecialtyName + ' was deleted successfully');
            });
        }
    }
});