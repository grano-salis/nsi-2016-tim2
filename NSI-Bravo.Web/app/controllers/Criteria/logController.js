﻿'use strict';

app.controller('logController', ['$scope', '$location', '$timeout', '$routeParams', '$log', 'logService', '$route', function ($scope, $location, $timeout, $routeParams, $log, logService, $route) {
    //START DeleteCriteria
    $scope.data = new Array();
    $scope.tree_data = new Array();
    var rawTreeData = new Array();
    var data = new Array();
    var tree;
    $scope.my_tree = tree = {};
    var myTreeData = new Array();

    $scope.expanding_property = {
        field: "type",
        displayName: "Event Type",
        sortable: true,
        filterable: true,
        cellTemplate: "<a ng-click = 'user_clicks_branch(row.branch)'>{{row.branch[expandingProperty.field]}}</a>"
    };

    $scope.col_defs = [
        {
            field: "created",
            displayName: "Created",
            sortable: true,
            sortingType: "string",
            filterable: true
        },
        {
            field: "description",
            displayName: "Description",
            sortable: true,
            sortingType: "number"
        },
        {
            field: "fullName",
            displayName: "User",
            sortable: true,
            sortingType: "number"
        }/*,
        {
            field: "Action",
            displayName: "Actions",
            cellTemplate: "<button ng-click='cellTemplateScope.clickView(row.branch)' class='btn btn-primary btn-xs' data-toggle='modal' data-target='#viewCrModal' >View</button>",
            cellTemplateScope: {
                clickView: function (branch) {
                    $scope.viewCr = { title: branch.title, points: branch.points, id: branch.id, parent_id: branch.parent_id, desc: branch.description, created: branch.created };
                }
            }
        }*/
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
    $scope.selectedCr;
    function LoadLog() {

        clearTable();

        logService.GetLogs().then(function (response) {

            data = response.data;
            for (var i = 0; i < data.length; i++) {

                var log = {
                    id: "",
                    type: "",
                    description: "",
                    user: "",
                    created: "",
                    fullName: ""
                }
                log.id = data[i].loG_ID;
                log.type = data[i].evenT_TYPE;
                log.description = data[i].description;
                log.user = data[i].user;
                log.fullName = log.user.firstname + " " + log.user.lastname;
                var date = moment(data[i].evenT_CREATED).format("YYYY-MM-DD");
                if (date !== null)
                    log.created = date;
                if (log.type != null) {
                    rawTreeData.push(log);
                }
                console.log(log);   
            }
            myTreeData = rawTreeData;
            $scope.tree_data = myTreeData;
        });
    };

    LoadLog();


    //END DeleteCriteria

}]);