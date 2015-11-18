angular.module('app').factory('Doctor', function($resource, apiUrl) {
    return $resource(apiUrl + 'api/doctors/:id', { id: '@DoctorID' }, {
        update: {
            method: 'PUT'   
        }   
    });
});