angular.module('app', ['ui.router', 'ui.bootstrap', 'ngResource']).config(function ($stateProvider, $urlRouterProvider) {
    $urlRouterProvider.otherwise('/app/dashboard');

    $stateProvider
        .state('app', { url: '/app', templateUrl: '/templates/app/app.html', controller: 'AppController' })
            
        .state('app.dashboard', { url: '/dashboard', templateUrl: '/templates/app/dashboard/dashboard.html', controller: 'DashboardController'})

        .state('app.doctors', {abstract: true, url: '/doctors', template: '<ui-view/>' })
            .state('app.doctors.grid', { url: '/grid', templateUrl: '/templates/app/doctors/grid.html', controller: 'DoctorsGridController' })
            .state('app.doctors.detail', { url: '/detail/:id', templateUrl: '/templates/app/doctors/detail.html', controller: 'DoctorsGridController' })

         .state('app.patients', { abstract: true, url: '/patients', template: '<ui-view/>' })
            .state('app.patients.grid', { url: '/grid', templateUrl: '/templates/app/patients/grid.html', controller: 'PatientsGridController' })
            .state('app.patients.detail', { url: '/detail/:id', templateUrl: '/templates/app/patients/detail.html', controller: 'PatientsGridController' })
            
         .state('app.appointments', { url: '/appointments', templateUrl: '/templates/app/appointments/tabs.html', controller: 'AppointmentsController' })
                .state('app.appointments.upcoming', { url: '/upcoming', templateUrl: '/templates/app/appointments/upcoming.html', controller: 'AppointmentsController' })
                .state('app.appointments.past', { url: '/past', templateUrl: '/templates/app/appointments/past.html', controller: 'AppointmentsController' })

            .state('app.admin', { url: '/admin', templateUrl: '/templates/app/admin/tabs.html', controller: 'AdminController' })
                .state('app.admin.exam', { url: '/exam', templateUrl: '/templates/app/admin/exam.html', controller: 'AdminExamController' })
                .state('app.admin.specialties', { url: '/specialties', templateUrl: '/templates/app/admin/specialties.html', controller: 'AdminSpecialtiesController' });
});

angular.module('app').value('apiUrl', 'http://localhost:7000/');