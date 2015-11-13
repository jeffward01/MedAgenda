angular.module('app').controller('AppointmentsController', function ($rootScope) {
    $rootScope.$broadcast('change-page-title', { title: 'Appointments' });
});