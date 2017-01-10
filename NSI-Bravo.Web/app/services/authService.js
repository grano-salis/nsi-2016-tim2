'use strict';
app.factory('authService', ['$http', '$q', 'localStorageService', 'ngAuthSettings', '$location', function ($http, $q, localStorageService, ngAuthSettings, $location) {

    var serviceBase = ngAuthSettings.apiServiceBaseUri;
    var authServiceFactory = {};
    
    var _authentication = {
        isAuth: false,
        userName: "",
        UserId :null,
        roles: new Array()
    };

    var _externalAuthData = {
        provider: "",
        userName: "",
        externalAccessToken: ""
    };



    var _login = function (data) {

        //console.log(data);

        var deferred = $q.defer();

        $http({
            url: 'http://localhost:48202/BusinessLogic/Account.svc/json/login',
            method: "POST",
            data: {
                loginModel: {
                    "Username": data.userName,
                    "Password": data.password
                }
            },
            withCredentials: true
        }).then(function (response) {
            //success
            console.log(response); 
            deferred.resolve(response);

        },
        function (response) {

            deferred.reject(response);
        });
       
        return deferred.promise;

    };

    var _logOut = function () {

          return $http({
              url: 'http://localhost:48202/BusinessLogic/Account.svc/json/logout',
              method:"POST",
              withCredentials:true
          })
          .then(function (response) {
              console.log(response.data);
              _authentication.isAuth = false;
              _authentication.userName = "";
          });

      
        

    };


    var _register= function (registerData) {

        console.log(registerData);

        var deferred = $q.defer();

        $http({
            url: 'http://localhost:48202/BusinessLogic/Account.svc/json/register',
            method: "POST",
            data: registerData,
            withCredentials: true
        }).then(function (response) {
            //success
            console.log(response);
            deferred.resolve(response);

        },
        function (response) {

            deferred.reject(response);
        });

        return deferred.promise;

    };

    var _fillAuthData = function () {
        if (_authentication.isAuth == true) {
            
            return;
        }
       
        var deferred = $q.defer();
        
        $http.get('http://localhost:48202/BusinessLogic/Account.svc/json/auth', { withCredentials: true })
         .then(function (response) {
             
             _authentication.isAuth = true;
             _authentication.userName = response.data.Username;
             _authentication.roles = new Array();
             _authentication.UserId = response.data.UserId;
             for (var i = 0; i < response.data.Roles.length; i++)
                 _authentication.roles.push(response.data.Roles[i]);
             console.log(_authentication);
             deferred.resolve(response);
             if ($location.url() == '/login') {
                 if (_authentication.roles.lastIndexOf("CV_ADMIN") != -1)
                     $location.path('/myCV');
                 if (_authentication.roles.lastIndexOf("ADMIN") != -1)
                     $location.path('/criteria');
                 if (_authentication.roles.lastIndexOf("STUDENTSKA") != -1)
                     $location.path('/requests');
             }

             
         }, function (response) {
             deferred.reject(response);
             console.log("Something went wrong");
         });
        return deferred.promise;
    };


    authServiceFactory.login = _login;
    authServiceFactory.logOut = _logOut;
    authServiceFactory.register = _register;
    authServiceFactory.fillAuthData = _fillAuthData;
    authServiceFactory.authentication = _authentication;
    return authServiceFactory;
}]);