angular.module('app').factory('patientChecksService', function($http, $q, apiUrl){
    
    var _get = function(get){
        var deferred = $q.defer();
        
        $http.get(apiUrl + 'patients/checkedin')
        .success(function(response){
            deferred.resolve(response);
        })
        .error(function(){
            deferred.reject('Error getting information for Patients Checked in');
        });
        
        return deferred.promise;
    };
    
    var _getAllPatients = function(get){
        var deferred = $q.defer();
        
        $http.get(apiUrl + '/patients/')
        .success(function(response){
            deferred.resolve(response);
        });
        return deferred.promise;
    };
    
    var _getAllSpecialties = function(get){
        var deferred = $q.defer();
        
        $http.get(apiUrl + '/specialties/')
        .success(function(response){
            deferred.resolve(response);
        });
        return deferred.promise;
    };
    
    var _getAllDoctors = function(get){
        var deferred = $q.defer();
        
        $http.get(apiUrl+ '/doctors/')
        .success(function(response){
            deferred.resolve(response);
        })
        return deferred.promise;
    };
    
    var _getAllExamRooms = function(get){
        var deferred = $q.defer();
        
        $http.get(apiUrl + '/examrooms/')
        .success(function(response){
            deferred.resolve(response);
        })
        return deferred.promise;
    }
    
    var _getAllCheckedInDoctors = function(get){
        var deferred = $q.defer();
        
        $http.get(apiUrl + '/doctors/checkedin/')
        .success(function(response){
            deferred.resolve(response);
        })
        return deferred.promise;
    }
    
    return {
        get: _get,
        getAllPatients : _getAllPatients,
        getAllSpecialties : _getAllSpecialties,
        getAllDoctors: _getAllDoctors,
        getAllExamRooms : _getAllExamRooms,
        getAllCheckedInDoctors : _getAllCheckedInDoctors
    };   
});