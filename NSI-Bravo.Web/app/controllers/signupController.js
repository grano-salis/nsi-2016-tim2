'use strict';
app.controller('signupController', ['$scope', '$location', '$timeout', 'authService', 'Notification', function ($scope, $location, $timeout, authService, Notification) {

    $scope.savedSuccessfully = false;
    $scope.message = "";

    $scope.registration = {
        Username: "",
        password: "",
        confirmPassword: ""
    };

    $scope.signUp = function () {
        if ($scope.registration.confirmPassword != $scope.registration.Password) {
            Notification.error({ message: 'Error: Passwords do not match.', title: 'Failed to register' });
            return;
        }
        authService.register($scope.registration).then(function (response) {

            $scope.savedSuccessfully = true;
            Notification.success({message:'User has been registered successfully, you will be redicted to login page in 2 seconds.',delay:2000});
            startTimer();

        },
         function (response) {
             if (response.data.Message == "undefined")
                 return;
             var error = response.data.Message;
             if (!(error == undefined || error == "Password weak.")) {
                 Notification.error({ message: 'Error: ' + error, title: 'Failed to register' });
             }
         });
    };

    var startTimer = function () {
        var timer = $timeout(function () {
            $timeout.cancel(timer);
            $location.path('/login');
        }, 2000);
    }

}]);