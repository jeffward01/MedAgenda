angular.module('app').factory('Specialty', function ($resource, apiUrl) {

    return $resource(apiUrl + 'api/specialties/:id', { id: '@SpecialtyId' }, {
		update: {
			method: 'PUT'
		}
	});
});