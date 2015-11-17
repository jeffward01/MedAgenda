angular.module('app').factory('patientChecksService', function($http, $q, apiUrl){
    
    var _get = function(get){
        var deferred = $q.defer();
        
        $http.get(apiUrl + 'PatientChecks')
        .success(function(response){
            deferred.resolve(response);
        })
        .error(function(){
            deferred.reject('Error getting information for Patient Check');
        });
        
        return deferred.promise;
    };
    
    return {
        get: _get
    };   
});