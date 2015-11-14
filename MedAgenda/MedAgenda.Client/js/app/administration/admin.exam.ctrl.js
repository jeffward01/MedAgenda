angular.module('app').controller('AdminExamController', function ($rootScope, $scope) {

    $rootScope.$broadcast('change-page-title', { title: 'Administration: Exam Rooms' });


});