﻿'use strict';
app.controller('indexController', ['$scope', '$location', 'authService', '$route', '$window', 'Notification', function ($scope, $location, authService, $route, $window, Notification) {

    $scope.logOut = function () {
        authService.logOut();
        $window.location.reload();
        $location.path('/login');
       
    }
    $scope.authentication = authService.authentication;

    $scope.isActive = function (viewLocation) {
        return viewLocation === $location.path();
    };
    $scope.isStudentskaOrAdmin = function () {
        if ($scope.authentication.roles.lastIndexOf("STUDENTSKA") != -1 || $scope.authentication.roles.lastIndexOf("ADMIN") != -1)
            return true;
        return false;
    };

    $scope.isAdminOrCVAdmin = function () {
        if ($scope.authentication.roles.lastIndexOf("ADMIN") != -1 || $scope.authentication.roles.lastIndexOf("CV_ADMIN") != -1)
            return true;
        return false;
    };

    $scope.isAdmin = function () {
        if ($scope.authentication.roles.lastIndexOf("ADMIN") != -1)
            return true;
        return false;
    };
    $scope.isStudentskaOrAdmin();
    $scope.isAdminOrCVAdmin();
    $scope.isAdmin();
}]);