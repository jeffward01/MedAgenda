angular.module('app').factory('patientChecksService', function($http, $q, apiUrl){
    
    var _get = function(get){
        var deferred = $q.defer();
        
        $http.get(apiUrl + 'api/patients/checkedin')
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
        
        $http.get(apiUrl + 'api/patients/')
        .success(function(response){
            deferred.resolve(response);
        });
        return deferred.promise;
    };
    
    var _getAllSpecialties = function(get){
        var deferred = $q.defer();
        
        $http.get(apiUrl + 'api/specialties/')
        .success(function(response){
            deferred.resolve(response);
        });
        return deferred.promise;
    };
    
    var _getAllDoctors = function(get){
        var deferred = $q.defer();
        
        $http.get(apiUrl+ 'api/doctors/')
        .success(function(response){
            deferred.resolve(response);
        })
        return deferred.promise;
    };
    
    var _getAllExamRooms = function(get){
        var deferred = $q.defer();
        
        $http.get(apiUrl + 'api/examrooms/')
        .success(function(response){
            deferred.resolve(response);
        })
        return deferred.promise;
    }
    
    var _getAllCheckedInDoctors = function(get){
        var deferred = $q.defer();
        
        $http.get(apiUrl + 'api/doctors/checkedin/')
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