angular.module('app').controller('AdminExamController', function ($rootScope, $scope, ExamRoom) {   

    $rootScope.$broadcast('change-page-title', { title: 'Administration: Manage Exam Rooms' });   

    $scope.examRoom = new ExamRoom();

    // Load exam room table, setting loading indicator while loading
    $scope.load = function () {
        $scope.loading = true;
        $scope.examRooms = ExamRoom.query(function () {
            $scope.loading = false;
        });
    };

    // <Save> clicked to save new exam room 
    $scope.saveExamRoom = function () {

        // Do not allow save if the exam room is not valid
        if ($scope.examRoomForm.$invalid) {
            toastr.warning('Please verify that you have filled in the exam room');
            return;
        }       

        // Save the new exam room in the database
        $scope.examRoom.$save(function () {
            toastr.success('Exam room: ' + $scope.examRoom.ExamRoomName +
                ' was added successfully');

        // Add the new exam room in the displayed list,
        // by allocating a new object for the new exam room and pushing it to the list
        var examRoomToAdd = {ExamRoomID: $scope.examRoom.ExamRoomID, 
                             ExamRoomName: $scope.examRoom.ExamRoomName};
        $scope.examRooms.push(examRoomToAdd);       

        // Clear the input exam room name and the input form
        $scope.examRoom.ExamRoomName = '';
        $scope.examRoomForm.$setPristine();
        $scope.examRoomForm.$setUntouched();
            
        },
        function (error) {           
            toastr.error(error.data.ExceptionMessage);
        });
    };

    $scope.deleteExamRoom = function (roomToDelete) {
        if (confirm('Are you sure you want to delete exam room: ' +
                                                    roomToDelete.ExamRoomName + '?')) {
            ExamRoom.delete({ id: roomToDelete.ExamRoomID }, function (data) {
                var index = $scope.examRooms.indexOf(roomToDelete);
                $scope.examRooms.splice(index, 1);
                toastr.success('Exam room: ' +
                                roomToDelete.ExamRoomName + ' was deleted successfully');
            },
            function (error) {               
                toastr.error(error.data.ExceptionMessage);
            });
        }
    }

    // After all definitions, load the exam room table
    $scope.load();

});