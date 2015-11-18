angular.module('app').factory('PatientChecks', function($resource, apiUrl){
    
    return $resource(apiUrl + 'PatientChecks/:id', {id :'@PatientCheckID'}, {
        update: {
            method: 'PUT'
        }
    });
});