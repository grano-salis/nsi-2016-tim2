
var app = angular.module("app", ['ngRoute']);

app.factory("ShareData", function () {
    return { value: 0 }
});

//Defining Routing
app.config(['$routeProvider', '$locationProvider', function ($routeProvider, $locationProvider) {

    $routeProvider.when("/home", { controller: "homeController", templateUrl: "/app/views/home.html" });
    
    $routeProvider.otherwise({ redirectTo: "/home" });

}]);

app.constant('DEBUG_MODE', /*DEBUG_MODE*/true/*DEBUG_MODE*/)
   .constant('VERSION_TAG', /*VERSION_TAG_START*/new Date().getTime()/*VERSION_TAG_END*/)
  
//Defining Translations

var serviceBase = 'http://localhost:26264/'; //port na kojem je web api

//var serviceBase = 'http://nwt-rmk-api.azurewebsites.net/';

app.constant('ngAuthSettings', {
    apiServiceBaseUri: serviceBase,
    clientId: 'ngAuthApp'
});




app.config(['$httpProvider', function ($httpProvider) {
    $httpProvider.defaults.useXDomain = true;
    delete $httpProvider.defaults.headers.common['X-Requested-With'];
}
]);