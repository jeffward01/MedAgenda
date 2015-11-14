angular.module('app').controller('DashboardController', function ($rootScope, $scope, dashboardService, $http, apiUrl) {
    $rootScope.$broadcast('change-page-title', {
        title: 'Dashboard'
    });

    //Dashboard Code

    dashboardService.get().then(
        function (data) {
            // callback from deferred.resolve
            // bind data now!

            $scope.dashboard = data;


        },
        function (error) {
            // callback from deferred.reject
            // show sad faces :(
        }
    );

    //Accordian Code
    $scope.oneAtATime = false;


    $scope.addItem = function () {
        var newItemNo = $scope.items.length + 1;
        $scope.items.push('Item ' + newItemNo);
    };

    $scope.status = {
        isFirstOpen: true,
        isFirstDisabled: false
    };


    //Liquid Fill Gauge





});