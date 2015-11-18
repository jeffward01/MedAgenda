angular.module('app').factory('Doctor', function($resource) {
    return $resource('http://localhost:7000/api/doctors/:id', { id: '@DoctorID' }, {
        update: {
            method: 'PUT'   
        }   
    });
});