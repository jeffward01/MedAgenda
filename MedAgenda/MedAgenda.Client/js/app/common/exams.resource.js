angular.module('app').factory('ExamRoom', function($resource, apiUrl) {
	return $resource(apiUrl + 'api/examrooms/:id', { id: '@ExamRoomId' }, {
		update: {
			method: 'PUT'
		}
	});
});