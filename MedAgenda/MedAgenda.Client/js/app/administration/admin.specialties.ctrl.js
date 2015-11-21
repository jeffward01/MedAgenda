angular.module('app').controller('AdminSpecialtiesController', function ($rootScope, $scope, Specialty) {

    $rootScope.$broadcast('change-page-title', { title: 'Administration: Manage Specialties' });

    $scope.specialty = new Specialty();

    // Load specialty table, setting loading indicator while loading
    $scope.load = function () {
        $scope.loading = true;
        $scope.specialties = Specialty.query(function () {
            $scope.loading = false;
        });
    };

    // <Save> clicked to save new specialty 
    $scope.saveSpecialty = function () {

        // Do not allow save if the specialty is not valid
        if ($scope.specialtyForm.$invalid) {
            toastr.warning('Please verify that you have filled in the specialty');
            return;
        }

        // Save the new specialty in the database
        $scope.specialty.$save(function () {
            toastr.success('Specialty: ' + $scope.specialty.SpecialtyName +
                ' was added successfully');

            // Add the new specialty in the displayed list,
            // by allocating a new object for the new specialty and pushing it to the list
            var specialtyToAdd = {
                SpecialtyID: $scope.specialty.SpecialtyID,
                SpecialtyName: $scope.specialty.SpecialtyName
            };
            $scope.specialties.push(specialtyToAdd);

            // Clear the input specialty name and the input form
            $scope.specialty.SpecialtyName = '';
            $scope.specialtyForm.$setPristine();
            $scope.specialtyForm.$setUntouched();

        },
        function (error) {            
            toastr.error(error.data.ExceptionMessage);
        });
    };

    $scope.deleteSpecialty = function (specialtyToDelete) {
        if (confirm('Are you sure you want to delete specialty: ' +
                                                    specialtyToDelete.SpecialtyName + '?')) {
            Specialty.delete({ id: specialtyToDelete.SpecialtyID }, function (data) {
                var index = $scope.specialties.indexOf(specialtyToDelete);
                $scope.specialties.splice(index, 1);
                toastr.success('Specialty: ' +
                                specialtyToDelete.SpecialtyName + ' was deleted successfully');
            },
            function (error) {
                toastr.error(error.data.ExceptionMessage);
            });
        }
    }

    // After all definitions, load the specialty table
    $scope.load();
    
});