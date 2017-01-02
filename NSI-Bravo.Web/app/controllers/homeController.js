'use strict';
app.controller('homeController', ['$scope', '$http', function ($scope, $http) {


    $scope.loginModel = {
        username: "User123",
        password: "User123."
    }

 

  /*  $http({
    url: 'http://do.mac.ba:8888/BusinessLogic/Account.svc/json/login',
    method: "POST",
    data: {
        loginModel: {
            "Username": "User123",
            "Password": "User123."
        }
    }
    }).then(function (response) {

        console.log(response);

    
    })*/

   $http.get('http://do.mac.ba:8888/BusinessLogic/Account.svc/json/auth',{withCredentials:true})
         .then(function (response) {
             console.log(response.data);
         })
         .catch(function (response) {

             console.error(response.data)
         });


   
}]);