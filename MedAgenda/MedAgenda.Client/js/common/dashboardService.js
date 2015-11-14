angular.module('app').factory('dashboardService', function ($http, $q, apiUrl) {
    var _get = function get() {
        var deferred = $q.defer();

        $http.get(apiUrl + 'dashboard')
		.success(function (response) {
		    deferred.resolve(response);
		})
		.error(function () {
		    deferred.reject('Error getting information for dashboard');
		});

        return deferred.promise;
    };

    return {
        get: _get
    };
});