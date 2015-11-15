angular.module('app').controller('AdminExamController', function ($rootScope, $scope, ExamRoom) {   

    $rootScope.$broadcast('change-page-title', { title: 'Administration: Manage Exam Rooms' });

    $scope.examRooms = ExamRoom.query();

    $scope.examRoom = new ExamRoom();

    // Save new exam room
    $scope.saveExamRoom = function () {
        if ($scope.examRoomForm.$invalid) {
            toastr.warning('Please verify that you have filled in the exam room');
            return;
        }

        $scope.examRoom.$save(function () {
            toastr.success($scope.examRoom.ExamRoomName + ' was added successfully');
            
        });
    };

    $scope.deleteExamRoom = function (examRoom) {
        if (confirm('Are you sure you want to delete exam room: ' +
                                                    examRoom.ExamRoomName + '?')) {
            ExamRoom.delete({ id: examRoom.ExamRoomId }, function (data) {
                var index = $scope.examRooms.indexOf(examRoom);
                $scope.examRooms.splice(index, 1);
                toastr.success('The exam room ' +
                                examRoom.ExamRoomName + ' was deleted successfully');
            },
            function (error) {               
                toastr.error(error.data.Message);
            });
        }
    }

});