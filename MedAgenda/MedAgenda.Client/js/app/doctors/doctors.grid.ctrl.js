angular.module('app').controller('DoctorsGridController', function ($rootScope) {
    $rootScope.$broadcast('change-page-title', { title: 'Doctors' });
});