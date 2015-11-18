angular.module('app').controller('AppointmentsController', function ($rootScope, $scope, $state) {
    $rootScope.$broadcast('change-page-title', { title: 'Appointments' });

    $scope.go = function (route) {
        $state.go(route);
    };

    $scope.active = function (route) {
        return $state.is(route);
    };

    $scope.tabs = [
        { heading: "Upcoming Appointments", route: "app.appointments.upcoming", active: false },
        { heading: "Past Appointments", route: "app.appointments.past", active: false }
    ];

    $scope.$on("$stateChangeSuccess", function () {
        $scope.tabs.forEach(function (tab) {
            tab.active = $scope.active(tab.route);
        });
    });
});