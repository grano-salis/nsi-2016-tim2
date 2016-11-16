'use strict';

app.controller('criteriaController', ['$scope', '$location', '$timeout', '$routeParams','criteriaService','$route', function ($scope, $location, $timeout, $routeParams, criteriaService,$route) {
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
            cellTemplate: "<div class='container-fluid'><button ng-click='cellTemplateScope.clickEdit()' class='btn btn-warning btn-sm' >Edit</button>" +" "+"<button ng-click='cellTemplateScope.clickDel(row.branch)' class='btn btn-danger btn-sm' >Del</button></div>",
            cellTemplateScope: {
                clickEdit: function () {
                    alert(1);
                },
                clickDel: function (branch) {
                    $scope.deleteCriteria(branch.id);
                }
            }
        }
    ];
    $scope.my_tree_handler = function (branch) {
        console.log('you clicked on', branch)
    }
   

    function GetAllCriteria() {
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
                criterion.created = data[i].datE_CREATED;
                criterion.points = data[i].points;
                if (criterion.title != null) {
                    rawTreeData.push(criterion);
                }
            }
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
            parent,
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
            $scope.deleteMessage = "Uspjesno ste izbrisali dati kriterij.";
            startTimer();

        },
         function (response) {
             $scope.deleteMessage = "Failed to delete:" + response.data.message;
             startTimerX();
         });
    };

    var startTimer = function () {
        var timer = $timeout(function () {
            $timeout.cancel(timer);
            $route.reload();
        }, 1000);
    };

    var startTimerX = function () {
        var timer = $timeout(function () {
            $timeout.cancel(timer);
            $scope.deleteMessage = "";
        }, 1000);
    };

//END DeleteCriteria

}]);