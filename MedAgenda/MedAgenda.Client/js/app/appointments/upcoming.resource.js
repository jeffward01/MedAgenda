angular.module('app').factory('Upcoming', function ($resource, apiUrl) {
    return $resource(apiUrl + 'api/appointments/upcoming/:id', { id: '@AppointmentId' }, {
        update: {
            method: 'Put'
        }
    })
});