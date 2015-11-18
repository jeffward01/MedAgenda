angular.module('app').factory('Patient', function ($resource, apiUrl) {

    return $resource(apiUrl + 'patients/:id', { id: '@PatientID' }, {
        update: {
            method: 'PUT'
        }
    });
});