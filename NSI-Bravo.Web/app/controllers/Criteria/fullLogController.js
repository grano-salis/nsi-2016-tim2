'use strict';

app.controller('fullLogController', ['$scope', '$location', '$timeout', '$routeParams', '$log', 'fullLogService', '$route', function ($scope, $location, $timeout, $routeParams, $log, fullLogService, $route) {
    
    $scope.LogData = new Array();
    $scope.data = new Array();
    $scope.tree_data = new Array();
    $scope.tree_data_cv = new Array();
    var rawTreeData = new Array();
    var data = new Array();
    var dataMainArray = new Array();
    var tree;
    $scope.my_tree = tree = {};
    var myTreeData = new Array();

    // Linkovi
    $scope.links = [{ DESCRIPTION: '', URL: '' }];

    $scope.expanding_property = {
        field: "cvItemName",
        displayName: "CV Name",
        sortable: true,
        filterable: true,
        cellTemplate: "<span ng-click = 'user_clicks_branch(row.branch)'>{{row.branch[expandingProperty.field]}}</span>"
    };

    $scope.col_defs = [
        {
            field: "cvUserName",
            displayName: "CV Owner",
            sortable: true,
            sortingType: "string",
            filterable: true
        },
        {
            field: "eventStatus",
            displayName: "Event Status",
            sortable: true,
            sortingType: "string",
            filterable: true
        },
        {
            field: "eventCreated",
            displayName: "Event Created",
            sortable: true,
            sortingType: "string",
            filterable: false
        },
        {
            field: "eventUserName",
            displayName: "Done By",
            sortable: true,
            sortingType: "string",
            filterable: true
        }
        ,
        {
            field: "Action",
            displayName: "Action",
            cellTemplate: "<button id='viewMe{{row.branch.id}}' ng-click='cellTemplateScope.clickView(row.branch)' class='btn btn-danger btn-xs' data-toggle='modal' data-target='#viewCrModal' >Review</button>",
            cellTemplateScope: {
                clickView: function (branch) {
                    $scope.viewCr = branch;
                    $scope.links = [];
                    $scope.filterString = "";
                    for (var i = 0; i < branch.cvItem.cV_ITEM_LINK.length; i++) {
                        $scope.links.push({ DESCRIPTION: branch.cvItem.cV_ITEM_LINK[i].description, URL: branch.cvItem.cV_ITEM_LINK[i].url });
                    }
                    for (var j = 0; j < $scope.LogData.length; j++) {
                        if ($scope.LogData[j].cvItemNameGlobal == branch.cvItemName) {
                            $scope.tree_data_cv = $scope.LogData[j].cvLogs;
                        }
                    }
                    fullLogService.GetCriteria(branch.cvItem.criteriA_ID_CRITERIA).then(function (response) {
                        $scope.viewCriteriaFull = response.data;
                        $scope.viewCriteria = $scope.viewCriteriaFull.name;
                        console.log($scope.viewCriteriaFull.name);
                    });
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
        $scope.LogData = [];
        $scope.dataMainArray = [];
    };
    function LoadLog() {
        clearTable();

        fullLogService.GetLogs().then(function (response) {

            dataMainArray = response.data;
            for (var j = 0; j < dataMainArray.length; j++) {
                if (dataMainArray[j][0].log.description != null) {
                    var logdata = {
                        cvItemNameGlobal: "",
                        cvLogs: []
                    }
                    logdata.cvItemNameGlobal = dataMainArray[j][0].log.description.name;
                    $scope.LogData.push(logdata);
                    for (var i = 0; i < dataMainArray[j].length; i++) {
                        data = dataMainArray[j][i].log;
                        var log = {
                            logID: "",
                            eventCreated: "",
                            eventStatus: "",
                            eventUser: "",
                            eventUserName: "",
                            cvItem: "",
                            cvItemName: "",
                            cvUser: "",
                            cvUserName: ""
                        }
                        log.logID = data.loG_ID;
                        var date = moment(data.evenT_CREATED).format("YYYY-MM-DD HH:mm:ss");
                        if (date !== null)
                            log.eventCreated = date;
                        log.eventStatus = data.evenT_TYPE;
                        log.eventUser = data.user;
                        log.eventUserName = data.user.username;
                        log.cvItem = data.description;
                        log.cvItemName = data.description.name;
                        log.cvUser = data.description.cV_USER;
                        log.cvUserName = data.description.cV_USER.username;
                        
                        //DATE FIX
                        log.cvItem.starT_DATE = moment(log.cvItem.starT_DATE).format("YYYY-MM-DD");
                        log.cvItem.enD_DATE = moment(log.cvItem.enD_DATE).format("YYYY-MM-DD");
                        if (i == 0) {
                            rawTreeData.push(log);
                        }
                        $scope.LogData[$scope.LogData.length - 1].cvLogs.push(log);
                    }
                }
            }
            myTreeData = rawTreeData;
            $scope.tree_data = myTreeData;
        });
    };

    LoadLog();


    //END DeleteCriteria



    $scope.expanding_property_cv = {
        field: "cvItemName",
        displayName: "CV Name",
        sortable: true,
        filterable: true,
        cellTemplate: "<span ng-click = 'user_clicks_branch(row.branch)'>{{row.branch[expandingProperty.field]}}</span>"
    };

    $scope.col_defs_cv = [
        {
            field: "cvUserName",
            displayName: "CV Owner",
            sortable: true,
            sortingType: "string",
            filterable: true
        },
        {
            field: "eventStatus",
            displayName: "Event Status",
            sortable: true,
            sortingType: "string",
            filterable: true
        },
        {
            field: "eventCreated",
            displayName: "Event Created",
            sortable: true,
            sortingType: "string",
            filterable: false
        },
        {
            field: "eventUserName",
            displayName: "Done By",
            sortable: true,
            sortingType: "string",
            filterable: true
        }
    ];


    $scope.resetSearch = function () {
        $scope.filterString = "";
    }
    $scope.confirmRequest = function (item_id) {
        fullLogService.ConfirmRequest(item_id).then(function (response) {
            $log.log('Confirm request');
            $log.log(response);
            LoadLog();
            Notification.success('Request approved.');
        });
    };

    $scope.rejectRequest = function (item_id) {
        fullLogService.RejectRequest(item_id).then(function (response) {
            $log.log('Reject request');
            $log.log(response);
            LoadLog();
            Notification.success('Request rejected.');
        });
    };

    $scope.restoreRequest = function (item_id) {
        fullLogService.RestoreRequest(item_id).then(function (response) {
            $log.log('Restore request');
            $log.log(response);
            LoadLog();
            Notification.success('Request restored.');
        });
    };
}]);