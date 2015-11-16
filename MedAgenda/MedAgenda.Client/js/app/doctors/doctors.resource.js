angular.module('app').factory('DoctorController', function($resource) {
    return $resource('http://localhost:7000/api/doctors/:id', { id: '@DoctorId' }, {
        update: {
            method: 'PUT'   
        }   
    });
});