angular.module('app').factory('Dashboard', function ($resource, apiUrl) {

    return $resource(apiUrl + 'dashboard', {
        update: {
            method: 'GET'
        }
    });
});