'use strict';

app.controller('criteriaController', ['$scope', '$location', '$timeout', '$routeParams','criteriaService', function ($scope, $location, $timeout, $routeParams, criteriaService) {
//START DeleteCriteria
    $scope.masterCriteria = new Array();
    $scope.levelOneCriteria = new Array();
    $scope.levelTwoCriteria = new Array();
    $scope.showLevel1 = "";
    $scope.showLevel2 = "";


    $scope.deleteMasterShow = "Master Criteria ";
    $scope.deleteLevel1Show = "Sub Criteria 1";
    $scope.deleteLevel2Show = "Sub Criteria 2";

    $scope.currentCriteria = "";
    var data = new Array();

    $scope.GetAllMasterCriteria = function () {
        $scope.showLevel2 = "";
        criteriaService.GetAllMasterCriteria().then(function (response) {
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
        $scope.deleteLevel1Show = "Sub Criteria 1";
        $scope.deleteLevel2Show = "Sub Criteria 2";

        $scope.currentCriteria = a.id;
        $scope.levelOneCriteria = new Array();
        $scope.deleteMasterShow = a.name;
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
                    $scope.showLevel1 = "Set";
                    $scope.levelOneCriteria.push(criterion);
                }
            }
        });
    };

    $scope.getLevelTwoCriteria = function (a) {
        $scope.deleteLevel2Show = "Sub Criteria 2";
        $scope.currentCriteria = a.id;
        $scope.levelTwoCriteria = new Array();
        $scope.deleteLevel1Show = a.name;
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
                    $scope.showLevel2 = "Set";
                    $scope.levelTwoCriteria.push(criterion);
                }
            }
        });
    };

    $scope.setCriteria = function (a) {
        $scope.currentCriteria = a.id;
        $scope.deleteLevel2Show = a.name;
    };

    $scope.update = function () {

    };

    $scope.GetAllMasterCriteria();



    $scope.deletedSuccessfully = false;
    $scope.deleteMessage = "";

    $scope.deleteCriteria = function () {
        criteriaService.DeleteCriteria($scope.currentCriteria).then(function (response) {

            $scope.deletedSuccessfully = true;
            $scope.deleteMessage = "Uspjesno ste izbrisali dati kriterij.";
            startTimer();

        },
         function (response) {
             $scope.deleteMessage = "Failed to delete:" + response.data.message;
             startTimer();
         });
    }

    var startTimer = function () {
        var timer = $timeout(function () {
            $timeout.cancel(timer);
            $scope.deleteMessage = "";
            $scope.masterCriteria = new Array();
            $scope.levelOneCriteria = new Array();
            $scope.levelTwoCriteria = new Array();
            $scope.currentCriteria = "";
            $scope.showLevel1 = "";
            $scope.showLevel2 = "";
            $scope.deleteMasterShow = "Master Criteria ";
            $scope.deleteLevel1Show = "Sub Criteria 1";
            $scope.deleteLevel2Show = "Sub Criteria 2";
            var data = new Array();
            $scope.GetAllMasterCriteria();
        }, 1000);
    }
//END DeleteCriteria

  
}]);