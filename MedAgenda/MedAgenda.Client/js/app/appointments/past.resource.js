angular.module('app').factory('Past', function ($resource, apiUrl) {
    return $resource(apiUrl + 'api/appointments/past/:id', { id: '@AppointmentId' }, {
        update: {
            method: 'Put'
        }
    })
});