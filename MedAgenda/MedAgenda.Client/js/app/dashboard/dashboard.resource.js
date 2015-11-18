angular.module('app').factory('Dashboard', function ($resource, apiUrl) {

    return $resource(apiUrl + '/api/dashboard', {
        update: {
            method: 'GET'
        }
    });
});