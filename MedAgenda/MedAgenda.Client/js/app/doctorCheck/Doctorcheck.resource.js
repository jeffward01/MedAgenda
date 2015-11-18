angular.module('app').factory('DoctorChecks', function($resource, apiUrl){
    
    return $resource(apiUrl + 'DoctorChecks/:id', {id :'@DoctorCheckID'}, {
        update: {
            method: 'PUT'
        }
    });
});