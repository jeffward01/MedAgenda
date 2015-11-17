angular.module('app').controller('DashboardController', function ($rootScope, $scope, $compile, dashboardService, $http, apiUrl, $log) {
    $rootScope.$broadcast('change-page-title', {
        title: 'Dashboard'
    });

    //Dashboard Code
    dashboardService.get().then(
        function (data) {
            // callback from deferred.resolve
            // bind data now!

            $scope.dashboard = data;
            //Onsite Doctors Page Settings
           // $scope.totalItems = $scope.dashboard.CheckedinDoctors.length;
             $scope.maxSize = 5;
            $scope.itemsPerPage = 5;
            $scope.currentPage = 1;         
        },
        function (error) {
            // callback from deferred.reject
            // show sad faces :(
        }
    );

    //Accordian Code
    $scope.oneAtATime = false;
    $scope.status = {
        isFirstOpen: true,
        isFirstDisabled: false
    };

});