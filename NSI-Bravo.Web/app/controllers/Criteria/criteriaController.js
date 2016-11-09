'use strict';

app.controller('criteriaController', ['$scope', '$location', '$timeout', '$routeParams','criteriaService', function ($scope, $location, $timeout, $routeParams, criteriaService) {

    $scope.masterCriteria = new Array();
    $scope.levelOneCriteria = new Array();
    $scope.levelTwoCriteria = new Array();

    $scope.currentCriteria = "";

    $scope.criteria = new Array();
    var data = new Array();

    $scope.getAllCriteria = function () {
        criteriaService.GetAllCriteria().then(function (response) {
            data = response.data;

            for (var i = 0; i < data.length; i++) {

                var criterion = {
                    id: "",
                    name: ""
                }
                criterion.id = data[i].iD_CRITERIA;
                criterion.name = data[i].name;
                if (criterion.name != null) {
                    $scope.masterCriteria.push(criterion);
                }
            }
        });
    };



    $scope.getLevelOneCriteria = function (a) {
        $scope.currentCriteria = a.id;
        $scope.levelOneCriteria = new Array();
        criteriaService.GetCriteria(a.id).then(function (response) {
            data = response.data;
            for (var i = 0; i < data.criteriA1.length; i++) {

                var criterion = {
                    id: "",
                    name: ""
                }
                criterion.id = data.criteriA1[i].iD_CRITERIA;
                criterion.name = data.criteriA1[i].name;
                if (criterion.name != null) {
                    $scope.levelOneCriteria.push(criterion);
                }
            }
        });
    };

    $scope.getLevelTwoCriteria = function (a) {
        $scope.currentCriteria = a.id;
        criteriaService.GetCriteria(a.id).then(function (response) {
            data = response.data;

            for (var i = 0; i < data.length; i++) {

                var criterion = {
                    id: "",
                    name: ""
                }
                criterion.id = data[i].iD_CRITERIA;
                criterion.name = data[i].name;
                if (criterion.name != null) {
                    $scope.levelTwoCriteria.push(criterion);
                }
            }
        });
    };

    $scope.setCriteria = function (a) {
        $scope.currentCriteria = a.id;
    };

    $scope.update = function () {

    };

    $scope.getAllCriteria();

  
}]);