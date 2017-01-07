'use strict';
app.controller('loginController', ['$scope', '$location', 'authService', 'ngAuthSettings', 'Notification', '$route', function ($scope, $location, authService, ngAuthSettings, Notification, $route) {

   

    $scope.message = "";
    $scope.authentication = authService.authentication;
    $scope.login = function () {
        if ($scope.loginData.userName == "" || $scope.loginData.password == "") {
            return;
        }
           
        authService.login($scope.loginData).then(function (response) {
            Notification.success('Successfully logged in');
            
             //fill authService.authentification user data from server
        authService.fillAuthData();

        },
         function (response) {
             console.log(response);
             Notification.error({ message: 'Wrong username or password', title: 'Failed to log in' });
             
         });
  
    };

   
   
   
}]);
