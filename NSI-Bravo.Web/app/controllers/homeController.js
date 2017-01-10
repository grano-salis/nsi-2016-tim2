'use strict';
app.controller('homeController', ['$scope', '$http', 'authService', function ($scope, $http, authService) {
 
   
    $scope.authentication = authService.authentication;

  
}]);