angular.module('app').factory('Patient', function ($resource, apiUrl) {

    return $resource(apiUrl + 'api/patients/:id', { id: '@PatientID' }, {
        update: {
            method: 'PUT'
        }
    });
});