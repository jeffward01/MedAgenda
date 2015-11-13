angular.module('app').controller('AppController', function ($rootScope, $scope, $state) {
    $scope.pageTitle = 'MedAgenda';

    $rootScope.$on('change-page-title', function (event, args) {
        $scope.pageTitle = args.title;
    });
});