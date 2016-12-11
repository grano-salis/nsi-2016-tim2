'use strict';

app.controller('criteriaController', ['$scope', '$location', '$timeout', '$routeParams', '$log', 'criteriaService','$route', function ($scope, $location, $timeout, $routeParams, $log, criteriaService,$route) {
//START DeleteCriteria
    $scope.data = new Array();
    $scope.tree_data = new Array();
    var rawTreeData = new Array();
    var data = new Array();
    var tree;
    $scope.my_tree = tree = {};
    var myTreeData = new Array();

    $scope.expanding_property = {
        field: "title",
        displayName: "Name",
        sortable: true,
        filterable: true,
        cellTemplate: "<i>{{row.branch[expandingProperty.field]}}</i>"
    };

    $scope.col_defs = [
        {
            field: "created",
            displayName: "Created",
            sortable: true,
            sortingType: "number",
            filterable: true
        },
        {
            field: "points",
            displayName: "Points",
            sortable: true,
            sortingType: "number"
        },
        {
            field: "Actions",
            displayName: "Actions",
            cellTemplate: "<button ng-click='cellTemplateScope.clickAdd(row.branch)' class='btn btn-primary btn-xs' data-toggle='modal' data-target='#addCrModal' >Add Subcriteria</button>"+" "+"<button ng-click='cellTemplateScope.clickEdit(row.branch)' class='btn btn-warning btn-xs' data-toggle='modal' data-target='#editCrModal' >Edit</button>" +" "+"<button ng-click='cellTemplateScope.clickDel(row.branch)' class='btn btn-danger btn-xs' data-toggle='modal' data-target='#delCrModal'  >Delete</button>",
            cellTemplateScope: {
                clickEdit: function (branch) {
                    $log.log(branch);
                    $scope.editCr = { name: branch.title, points: branch.points, id: branch.id, parent_id : branch.parent_id };
                },
                clickDel: function (branch) {
                    $scope.editCr = { name: branch.title, points: branch.points, id: branch.id, parent_id: branch.parent_id };
                    //$scope.deleteCriteria(branch.id);
                },
                clickAdd: function (branch) {
                    $scope.addCr = { name: branch.title, points: branch.points, id: branch.id, parent_id: branch.parent_id };
                }
            }
        }
    ];

    $scope.my_tree_handler = function (branch) {
        console.log('you clicked on', branch);
        $scope.getMasterCriteria();
    }

    function clearTable(){
        $scope.data = [];
        $scope.tree_data = [];
        rawTreeData = [];
        data = [];
        $scope.my_tree = tree = {};
        myTreeData = [];
    };
    $scope.selectedCr;
    function GetAllCriteria() {

        clearTable();
    
        criteriaService.GetAllCriteria().then(function (response) {

            data = response.data;
            for (var i = 0; i < data.length; i++) {
                
                var criterion = {
                    id: "",
                    title: "",
                    parent_id: "",
                    created: "",
                    points: ""
                }
                criterion.id = data[i].iD_CRITERIA;
                criterion.title = data[i].name;
                criterion.parent_id = data[i].parenT_CRITERIA;
                var date = moment(data[i].datE_CREATED).format("YYYY-MM-DD");
                if(date !== null)
                    criterion.created = date;
                criterion.points = data[i].points;
                if (criterion.title != null) {
                    rawTreeData.push(criterion);
                }
            }
            console.log(rawTreeData);
            myTreeData = getTree(rawTreeData, 'id', 'parent_id');
            $scope.tree_data = myTreeData;
        });
    };

    GetAllCriteria();

    $scope.alarmAll = function () {
        alert(1);
    }

    function getTree(data, primaryIdName, parentIdName) {

        if (!data || data.length == 0 || !primaryIdName || !parentIdName)
            return [];

        var tree = [],
            rootIds = [],
            item = data[0],
            primaryKey = item[primaryIdName],
            treeObjs = {},
            parentId,
            parent = {},
            len = data.length,
            i = 0;

        while (i < len) {
            item = data[i++];
            primaryKey = item[primaryIdName];
            treeObjs[primaryKey] = item;
            parentId = item[parentIdName];

            if (parentId) {
                parent = treeObjs[parentId];
                if (parent.children) {
                    parent.children.push(item);
                } else {
                    parent.children = [item];
                }
            } else {
                rootIds.push(primaryKey);
            }
        }

        for (var i = 0; i < rootIds.length; i++) {
            tree.push(treeObjs[rootIds[i]]);
        };
        return tree;
    }


    $scope.deletedSuccessfully = false;
    $scope.deleteMessage = "";

    $scope.deleteCriteria = function (currentCriteria) {
        criteriaService.DeleteCriteria(currentCriteria).then(function (response) {

            $scope.deletedSuccessfully = true;
            //$scope.deleteMessage = "Uspjesno ste izbrisali dati kriterij.";
            GetAllCriteria();
            
        },
         function (response) {
             $scope.deleteMessage = "Failed to delete:" + response.data.message;
             startTimerX();
         });
    };

    var startTimerX = function () {
        var timer = $timeout(function () {
            $timeout.cancel(timer);
            $scope.deleteMessage = "";
        }, 1000);

    };

    $scope.editCriteria = function(cr) {
        $log.log(cr);
        var data = {};
        data.NAME = cr.name;
        data.DESCRIPTION = cr.desc;
        data.POINTS = cr.points;
        data.ID_CRITERIA =  cr.id;
        data.PARENT_CRITERIA = cr.parent_id;

        criteriaService.UpdateCriteria(cr.id,data).then(function(response) { 
            $log.log(response); 
            GetAllCriteria();
        });
    };

    $scope.first = "Criteria";
    $scope.second = "Subcategory";

    $scope.firstCriteria = [];
    $scope.secondCriteria = [];

    $scope.getMasterCriteria = function () {

        $scope.firstCriteria = [];

        criteriaService.GetAllMasterCriteria().then(function (response) {
            var data = response.data;
            for (var i = 0; i < data.length; i++) {
                var criterion = {};
                criterion.id = data[i].iD_CRITERIA;
                criterion.name = data[i].name;
                if (criterion.name != null) 
                    $scope.firstCriteria.push(criterion);
                    $log.log(criterion);
                }
        
        });
    };
    $scope.getMasterCriteria();

    $scope.listSubcriteria = function(cr) {
        
        $scope.first = cr.name;
        $scope.second = 'Subcategory';
        $scope.selectedCr = cr.id;
        $log.log(cr.id);
        var subcriteria = [];
        criteriaService.GetCriteria(cr.id).then(function (response) {
            var data = response.data;
            for (var i = 0; i < data.criteriA1.length; i++) {
                var criterion = {};
                criterion.id = data.criteriA1[i].iD_CRITERIA;
                criterion.name = data.criteriA1[i].name;
                if (criterion.name != null) {
                    subcriteria.push(criterion);
                }
            }
            $log.log('sub' + subcriteria);
            if(subcriteria.length > 0){
                $scope.secondCriteria = subcriteria;
                $log.log($scope.secondCriteria);
            }
        });

    };

    
    $scope.addCriteria = function (cr) {
        var data = {};
        data.NAME = cr.name;
        data.DESCRIPTION = cr.desc;
        data.POINTS = cr.points;
        data.PARENT_CRITERIA =  $scope.addCr.id;
        criteriaService.AddCriteria(data).then(function(response) { 
            $log.log(response);
            $scope.clearForm();
            GetAllCriteria();
        });
    };

    $scope.selectSecondCr = function(cr){
        $scope.second = cr.name;
        $scope.selectedCr = cr.id;
    };

    $scope.clearForm = function(){
        //clear form
        $scope.cr = {};
        $scope.first = "Criteria";
        $scope.second = "Subcategory";
        $scope.selectedCr = null;
        $scope.addCr = [];
        $scope.secondCriteria = [];
        $scope.firstCriteria = [];
        $scope.getMasterCriteria();
    };

//END DeleteCriteria

}]);