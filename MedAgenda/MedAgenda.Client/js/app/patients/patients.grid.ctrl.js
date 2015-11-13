angular.module('app').controller('PatientsGridController', function ($rootScope) {
    $rootScope.$broadcast('change-page-title', { title: 'Patients' });  
});