angular.module('app').factory('EmergencyContact', function ($resource, apiUrl) {
    
	return $resource(apiUrl + 'api/emergencycontacts/:id', { id: '@EmergencyContactId' }, {
		update: {
			method: 'PUT'
		}
	});
});