var app = angular.module('AngularAuthApp', ['ngRoute', 'LocalStorageModule', 'angular-loading-bar', 'treeGrid', 'ngFileUpload', 'ui.bootstrap','ui-notification']);

app.config(function ($routeProvider) {

    $routeProvider.when("/home", {
        controller: "homeController",
        templateUrl: "/app/views/home.html"
    });

    $routeProvider.when("/login", {
        controller: "loginController",
        templateUrl: "/app/views/login.html"
    });

    $routeProvider.when("/signup", {
        controller: "signupController",
        templateUrl: "/app/views/signup.html"
    });

    $routeProvider.when("/orders", {
        controller: "ordersController",
        templateUrl: "/app/views/orders.html"
    });

    $routeProvider.when("/refresh", {
        controller: "refreshController",
        templateUrl: "/app/views/refresh.html"
    });

    $routeProvider.when("/tokens", {
        controller: "tokensManagerController",
        templateUrl: "/app/views/tokens.html"
    });

    $routeProvider.when("/associate", {
        controller: "associateController",
        templateUrl: "/app/views/associate.html"
    });


    $routeProvider.when("/criteria", {
        controller: "criteriaController",
        templateUrl: "/app/views/Criteria/criteria.html"
    });
    $routeProvider.when("/myCV", {
        controller: "myCVController",
        templateUrl: "/app/views/Criteria/myCV.html"
    });

    $routeProvider.when("/myHistory", {
        controller: "historyController",
        templateUrl: "/app/views/Criteria/myHistory.html"
    });

    $routeProvider.when("/history", {
        controller: "historyController",
        templateUrl: "/app/views/Criteria/history.html"
    });

    $routeProvider.when("/requests", {
        controller: "unconfirmedRequestsController",
        templateUrl: "/app/views/Requests/unconfirmedRequests.html"
    });

    $routeProvider.when("/processedRequests", {
        controller: "processedRequestsController",
        templateUrl: "/app/views/Requests/processedRequests.html"
    });
    
    $routeProvider.when("/log", {
        controller: "logController",
        templateUrl: "/app/views/Criteria/log.html"
    });

    $routeProvider.otherwise({ redirectTo: "/home" });

});

var serviceBase = 'http://localhost:26264/';
//var serviceBase = 'http://ngauthenticationapi.azurewebsites.net/';
app.constant('ngAuthSettings', {
    apiServiceBaseUri: serviceBase,
    clientId: 'ngAuthApp'
});
/*
app.config(function ($httpProvider) {
    $httpProvider.interceptors.push('authInterceptorService');
});*/

app.run(['authService', function (authService) {
    authService.fillAuthData();
}]);

app.config(function (NotificationProvider) {
    NotificationProvider.setOptions({
        
        startTop: 20,
        startRight: 70,
        delay: 5000,
        verticalSpacing: 20,
        horizontalSpacing: 20,
        positionX: 'center',
        positionY: 'bottom'
    });
});

app.config(['$httpProvider', function ($httpProvider) {
    $httpProvider.defaults.withCredentials = true;
}])