angular.module('app').controller('AdminExamController', function ($rootScope, $scope, ExamRoom) {

    $rootScope.$broadcast('change-page-title', { title: 'Administration: Exam Rooms' });

    $scope.examRooms = ExamRoom.query();

});