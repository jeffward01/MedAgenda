angular.module('app').controller('AdminController', function ($rootScope, $scope, $state) {
    $rootScope.$broadcast('change-page-title', { title: 'Administration' });

    $scope.tabs = [
        { heading: "Exam Rooms", route: "app.admin.exam", active: false },
        { heading: "Specialties", route: "app.admin.specialties", active: false }
    ];

    $scope.go = function (route) {
        $state.go(route);
    };

    $scope.active = function (route) {
        return $state.is(route);
    };

    $scope.$on("$stateChangeSuccess", function () {
        $scope.tabs.forEach(function (tab) {
            tab.active = $scope.active(tab.route);
        });
    });
});