'use strict';
app.factory('authService', ['$http', '$q', 'localStorageService', 'ngAuthSettings', '$location', function ($http, $q, localStorageService, ngAuthSettings, $location) {

    var serviceBase = ngAuthSettings.apiServiceBaseUri;
    var authServiceFactory = {};
    
    var _authentication = {
        isAuth: false,
        userName: "",
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

    var _fillAuthData = function () {
        if (_authentication.isAuth == true) {
            console.log("tru je");
            return;
        }
        console.log(_authentication);
        var deferred = $q.defer();
        
        $http.get('http://localhost:48202/BusinessLogic/Account.svc/json/auth', { withCredentials: true })
         .then(function (response) {
             
             _authentication.isAuth = true;
             _authentication.userName = response.data.Username;
             _authentication.roles = new Array();
             for (var i = 0; i < response.data.Roles.length; i++)
                 _authentication.roles.push(response.data.Roles[i]);
             deferred.resolve(response);
             if($location.url()=='/login')
                 $location.path('/myCV');

             
         }, function (response) {
             deferred.reject(response);
             console.log("Something went wrong");
         });
        return deferred.promise;
    };


    authServiceFactory.login = _login;
    authServiceFactory.logOut = _logOut;
    authServiceFactory.fillAuthData = _fillAuthData;
    authServiceFactory.authentication = _authentication;
    return authServiceFactory;
}]);