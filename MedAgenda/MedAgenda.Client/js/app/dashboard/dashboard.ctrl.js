angular.module('app').controller('DashboardController', function ($rootScope, $scope, $compile, dashboardService, patientChecksService, $http, apiUrl, $log) {
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

            //Angular Count-To
            $scope.countFrom = 0;
            $scope.countToPatientTotal = $scope.dashboard.PatientTotalCount;
            $scope.countToDoctorTotal = $scope.dashboard.DoctorTotalCount;
            $scope.countToExamRoomTotal = $scope.dashboard.ExamRoomTotalCount;
            $scope.countToOpenExamRoomTotal = $scope.dashboard.OpenExamRoomsCount;
            $scope.countToCurrentApptLength = $scope.dashboard.CurrentAppointments.length;
            $scope.countToDoctorsCheckedinCount = $scope.dashboard.DoctorsCheckedinCount;
            $scope.countToPatientsCheckedinCount = $scope.dashboard.PatientsCheckedinCount;
            $scope.countToExamRoomPercent = $scope.dashboard.ExamRoomsFilledPercentage
            $scope.countToDoctorsOnSitePercercent = $scope.dashboard.DoctorsOnsitePercentage

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



    //Average Patient Age Graph
    patientChecksService.getAllPatients().then(function (data) {
        nv.addGraph(function () {
            $scope.AllPatients = data;

            function countAge(min, max) {
                var count = 0;
                // look through all patients
                for (var i = 0; i < $scope.AllPatients.length; i++) {
                    var patient = $scope.AllPatients[i];
                    if (patient.Age > min && patient.Age < max) {
                        count++
                    }
                }
                return count;
                // if age > min && age < max

                // add to a count

                // return count;
            }

            CumulativeAges = [
                {
                    key: "Cumulative Ages",
                    values: [
                        {
                            "label": "0-16",
                            "value": countAge(0, 16)
                },
                        {
                            "label": "17-25",
                            "value": countAge(17, 25)
                },
                        {
                            "label": "26-35",
                            "value": countAge(26, 35)
                },
                        {
                            "label": "36-45",
                            "value": countAge(36, 45)
                },
                        {
                            "label": "46-55",
                            "value": countAge(46, 55)
                },
                        {
                            "label": "56-65",
                            "value": countAge(56, 65)
                },
                        {
                            "label": "66-75",
                            "value": countAge(66, 75)
                },
                        {
                            "label": "76+",
                            "value": countAge(76, 200)
                }
            ]
        }
    ];

            var chart = nv.models.discreteBarChart()
                .x(function (d) {
                    return d.label
                })
                .y(function (d) {
                    return d.value
                })
                .staggerLabels(true)
                //.staggerLabels(historicalBarChart[0].values.length > 8)
                .showValues(true)
                .duration(250);
            chart.yAxis
                .axisLabel('Number of Patients in Age Group');

            d3.select('#chart1 svg')
                .datum(CumulativeAges)
                .call(chart);
            nv.utils.windowResize(chart.update);
            return chart;
        });
    });

    //Graphics
   






});