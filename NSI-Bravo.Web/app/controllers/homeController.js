'use strict';
app.controller('homeController', ['$scope', '$http', 'authService', function ($scope, $http, authService) {


    $scope.loginModel = {
        username: "User123",
        password: "User123."
    }

 

    /*$http({
    url: 'http://localhost:48202/BusinessLogic/Account.svc/json/login',
    method: "POST",
    data: {
        loginModel: {
            "Username": "User123",
            "Password": "User123."
        }
    },
    withCredentials: true
    }).then(function (response) {

        console.log(response);

    
    })*/
    
    $http.get('http://localhost:48202/BusinessLogic/Account.svc/json/auth', { withCredentials: true })
         .then(function (response) {
             console.log(response.data);
         })
         .catch(function (response) {

             console.error(response.data)
         });
    $scope.authentication = authService.authentication;

   
}]);