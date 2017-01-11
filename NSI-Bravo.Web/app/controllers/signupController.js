'use strict';
app.controller('signupController', ['$scope', '$location', '$timeout', 'authService', 'Notification', function ($scope, $location, $timeout, authService, Notification) {

    $scope.savedSuccessfully = false;
    $scope.message = "";

    $scope.registration = {
        userName: "",
        password: "",
        confirmPassword: ""
    };

    $scope.signUp = function () {
        //validation needed: password must have minimum 8 characters ,at least one lowercase & uppercase,
        //username must be unique
        //email must be unique
        authService.register($scope.registration).then(function (response) {

            $scope.savedSuccessfully = true;
            Notification.success({message:'User has been registered successfully, you will be redicted to login page in 2 seconds.',delay:2000});
            startTimer();

        },
         function (response) {
             var errors = [];
             for (var key in response.data.modelState) {
                 for (var i = 0; i < response.data.modelState[key].length; i++) {
                     errors.push(response.data.modelState[key][i]);
                 }
             }
             Notification.error({ message: 'Error', title: 'Failed to register' });
         });
    };

    var startTimer = function () {
        var timer = $timeout(function () {
            $timeout.cancel(timer);
            $location.path('/login');
        }, 2000);
    }

}]);