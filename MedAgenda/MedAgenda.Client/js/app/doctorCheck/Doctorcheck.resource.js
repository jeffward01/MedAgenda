angular.module('app').factory('DoctorChecks', function($resource, apiUrl){
    
    return $resource(apiUrl + 'DoctorsCheck/:id', {id :'@DoctorCheckID'}, {
        update: {
            method: 'PUT'
        }
    });
});