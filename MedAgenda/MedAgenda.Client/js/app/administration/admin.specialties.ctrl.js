angular.module('app').controller('AdminSpecialtiesController', function ($rootScope, $scope) {

    $rootScope.$broadcast('change-page-title', { title: 'Administration: Specialties' });


});