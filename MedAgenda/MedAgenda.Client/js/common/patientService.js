angular.module('app').factory('patientService', function ($http, $q, apiUrl) {

    var _get = function get() {
        var deferred = $q.defer();

        $http.get(apiUrl + 'api/patients')
        .success(function (response) {
            deferred.resolve(response);
        })
        .error(function () {
            deferred.reject('Error getting information for patients');
        });

        return deferred.promise;
    };

    return {
        get: _get
    };
}); 