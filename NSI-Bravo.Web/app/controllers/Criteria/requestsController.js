'use strict';

app.controller('requestsController', ['$scope', '$location', '$timeout', '$routeParams', '$log', 'requestsService', '$route', function ($scope, $location, $timeout, $routeParams, $log, requestService, $route) {
    //Initial Variables :
    $scope.data = new Array();
    $scope.tree_data = new Array();
    var rawTreeData = new Array();
    var data = new Array();
    var tree;
    $scope.my_tree = tree = {};
    var myTreeData = new Array();

    // Tree for Criteria
    $scope.expanding_property = {
        field: "title",
        displayName: "Name",
        sortable: true,
        filterable: true,
        cellTemplate: "<a ng-click = 'user_clicks_branch(row.branch)'>{{row.branch[expandingProperty.field]}}</a>"
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
            cellTemplate: "<button ng-click='cellTemplateScope.clickAdd(row.branch)' class='btn btn-primary btn-xs' data-toggle='modal' data-target='#addCrModal' >Select</button>",
            cellTemplateScope: {
                clickAdd: function (branch) {
                    $log.log(branch);
                    $scope.addCr = { name: branch.title, points: branch.points, id: branch.id, parent_id: branch.parent_id };
                }
            }
        }
    ];

    $scope.my_tree_handler = function (branch) {
        console.log('you clicked on', branch);
    }

    function clearTable() {
        $scope.data = [];
        $scope.tree_data = [];
        rawTreeData = [];
        data = [];
        $scope.my_tree = tree = {};
        myTreeData = [];
    };

    function GetAllCriteria() {

        clearTable();

        requestService.GetAllCriteria().then(function (response) {

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
                if (date !== null)
                    criterion.created = date;
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
    // Tree for CriteriaEnd

    //Add New CV Item

    $scope.addCVItem = function (cr,file) {
        var data = {};
        data.NAME = cr.name;
        data.DESCRIPTION = cr.desc;
        data.START_DATE = cr.startDate;
        data.END_DATE = cr.endDate;
        data.CRITERIA_ID_CRITERIA = $scope.addCr.id;
        // FORCED FOR THE MOMENT
        data.CV_TABLE_ID_CV = 2;
        data.STATUS_ID = 2;
        // END OF FORCED DATA
        data.file = file;
        requestService.AddCV(data).then(function (response) {
            $log.log(response);
            $scope.clearForm();
            GetAllCriteria();
        });
    };

    $scope.filetest = function (test) {
        alert(test);
    };

    $scope.clearForm = function () {
        //clear form
        $scope.cr = {};
        $scope.addCr = {};
        $scope.selectedCr = null;
    };

    //END DeleteCriteria

}]);