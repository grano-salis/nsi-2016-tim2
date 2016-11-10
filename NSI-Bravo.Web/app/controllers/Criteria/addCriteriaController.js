'use strict';

app.controller('addCriteriaController', ['$scope', '$location', '$timeout', '$routeParams','criteriaService', function ($scope, $location, $timeout, $routeParams, criteriaService) {

    $scope.criterionText = "Criteria";
    $scope.subcriterionText = "Subcriteria";

    $scope.masterCriteria = new Array();
    $scope.subcriteria = new Array();
    $scope.currentCriteria;

    $scope.getCriteria = function () {
        criteriaService.GetAllMasterCriteria().then(function (response) {
            var data = response.data;
            for (var i = 0; i < data.length; i++) {
                var criterion = {};
                criterion.id = data[i].iD_CRITERIA;
                criterion.name = data[i].name;
                if (criterion.name != null) {
                    $scope.masterCriteria.push(criterion);
                }
            }
        });
    };

    $scope.getSubcriteria = function (cr) {
        
        $scope.currentCriteria = cr.id;
        $scope.subcriteria = new Array();
        $scope.criterionText = cr.name;
        criteriaService.GetCriteria(cr.id).then(function (response) {
            var data = response.data;
            for (var i = 0; i < data.criteriA1.length; i++) {
                var criterion = {};
                criterion.id = data.criteriA1[i].iD_CRITERIA;
                criterion.name = data.criteriA1[i].name;
                if (criterion.name != null) {
                    console.log(criterion.name);
                    $scope.subcriteria.push(criterion);
                }
            }
        });
    };

    $scope.getCriteria();

    $scope.addCriteria = function (cr) {
        var data = {};
        data.NAME = cr.name;
        data.DESCRIPTION = cr.desc;
        data.POINTS = cr.points;
        data.PARENT_CRITERIA = $scope.currentCriteria;

        criteriaService.addCriteria(data).then(function(response) { console.log(response); });
    };

    $scope.selectCriterion = function(cr){
        $scope.currentCriteria = cr.id;
    };

}]);