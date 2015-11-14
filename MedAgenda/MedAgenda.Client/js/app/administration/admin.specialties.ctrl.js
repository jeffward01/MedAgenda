angular.module('app').controller('AdminSpecialtiesController', function ($rootScope, $scope, Specialty) {

    $rootScope.$broadcast('change-page-title', { title: 'Administration: Specialties' });

    $scope.specialties = Specialty.query();
});