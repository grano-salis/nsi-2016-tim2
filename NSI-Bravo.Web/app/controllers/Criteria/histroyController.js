﻿'use strict';

app.controller('historyController', ['$scope', '$location', '$timeout', '$routeParams', '$log', 'historyService', '$route','criteriaService', function ($scope, $location, $timeout, $routeParams, $log, historyService, $route,criteriaService) {
    // Linkovi
    $scope.links = [{ DESCRIPTION: '', URL: '' }];
    $scope.addNewLink = function () {
        $scope.links.push({ DESCRIPTION: '', URL: '' });
    };
    $scope.resetLinks = function () {
        $scope.links = [{ DESCRIPTION: '', URL: '' }];
    }
    $scope.removeLink = function (id) {
        if ($scope.links.length > 1) {
            $scope.links.splice(id, 1);
        }
    }

    // My History
    $scope.myHistory;
    $scope.tree_data = new Array();
    var rawTreeData = new Array();
    var tree;
    $scope.my_tree = tree = {};

    $scope.GetMyHistory = function() {
        clearTableMyHistory();

        var dateFrom = moment($scope.myHistory.from).format("MM-DD-YYYY");
        var dateTo = moment($scope.myHistory.to).format("MM-DD-YYYY");
        var data = {
            from: "",
            to: ""
        };
        data.from = dateFrom;
        data.to = dateTo;
        historyService.GetMyHistory(data).then(function (response) {
           
            var data = response.data;
            if (data.length == 0) {
                var cv = {
                    id: "0",
                    title: "NO ENTRIES",
                    description: "For range of dates you entered, there are no entries.",
                    link: "",
                    user_cv_id: "0",
                    criteria_id: "0",
                    start_date: data.from,
                    end_date: data.to,
                    links: []
                }
                rawTreeData.push(cv);

            }
            for (var i = 0; i < data.length; i++) {
                var cv_item = {
                    id: "",
                    title: "",
                    description: "",
                    link: "",
                    user_cv_id: "",
                    criteria_id: "",
                    start_date: "",
                    end_date: "",
                    status: "",
                    links: [],
                    date_created:""
                }
                var status_code = data[i].statuS_ID;
                var status = "UNKNOWN";
                switch (status_code) {
                    case 1:
                        status = "UNCONFIRMED NEW"
                        break;
                    case 2:
                        status = "CONFIRMED"
                        break;
                    case 3:
                        status = "UNCONFIRMED MODIFIED"
                        break;
                    case 4:
                        status = "REJECTED"
                        break;
                    case 5:
                        status = "DELETED"
                        break;
                    default:
                        break;
                }
                cv_item.status = status;
                cv_item.id = data[i].iD_ITEM;
                cv_item.title = data[i].name;
                cv_item.description = data[i].description;
                cv_item.link = data[i].cV_ITEM_LINK_LINK;
                cv_item.user_cv_id = data[i].cV_TABLE_ID_CV;
                cv_item.criteria_id = data[i].criteriA_ID_CRITERIA;
                cv_item.links = data[i].cV_ITEM_LINK;
                var date = moment(data[i].datE_CREATED).format("YYYY-MM-DD");
                cv_item.date_created = date;
                var date = moment(data[i].starT_DATE).format("YYYY-MM-DD");
                cv_item.start_date = date;
                date = moment(data[i].enD_DATE).format("YYYY-MM-DD");
                cv_item.end_date = date;
                if (cv_item.title != null) {
                    rawTreeData.push(cv_item);
                }
            }
                $scope.tree_data = rawTreeData;

        });
    };
    function clearTableMyHistory() {
        $scope.tree_data = [];
        rawTreeData = [];
        var tree;
        $scope.my_tree = tree = {};
    }

    $scope.expanding_property = {
        field: "title",
        displayName: "CV Name",
        sortable: true,
        filterable: true,
        cellTemplate: "<span ng-click = 'user_clicks_branch(row.branch)'>{{row.branch[expandingProperty.field]}}</span>",
    };


    $scope.col_defs = [
        {
            field: "link",
            displayName: "Attachment link",
            sortable: false,
            cellTemplate: "<span ng-switch='row.branch[col.field]'><a ng-switch-when='undefined'>No Attachment</a><a ng-switch-default ng-href='{{row.branch[col.field]}}'>Download</a></span>",
            filterable: false
        },
        {
            field: "date_created",
            displayName: "Date created",
            sortable: true,
            sortingType: "string"
        },
        {
            field: "status",
            displayName: "Status",
            sortable: true,
            sortingType: "string",
            filterable: true
        },
        {
            field: "Action",
            displayName: "Action",
            cellTemplate: "<button id='viewMe{{row.branch.UserId}}' ng-click='cellTemplateScope.clickView(row.branch)' class='btn btn-primary btn-xs' data-toggle='modal' data-target='#viewCrModal' >View</button>" ,
            cellTemplateScope: {
             clickView: function (branch) {
                    $scope.viewCr = branch;
                    if (branch.links.length > 0) {
                        $scope.links = [];
                    }
                    else {
                        $scope.links = [];
                    }
                    for (var i = 0; i < branch.links.length; i++) {
                        $scope.links.push({ DESCRIPTION: branch.links[i].description, URL: branch.links[i].url });
                    }
                    criteriaService.GetCriteria(branch.criteria_id).then(function (response) {
                        $scope.viewCriteriaFull = response.data;
                        $scope.viewCriteria = $scope.viewCriteriaFull.name;
                        console.log($scope.viewCriteriaFull.name);
                    });
                }
            }
        }
    ];
    // My History END

    //History
    //Prof Choice

    // My History
    $scope.tree_dataProf = new Array();
    var rawTreeDataProf = new Array();
    var treeProf;
    $scope.my_treeProf = treeProf = {};

    $scope.GetAllProfessors = function () {
        clearTableProf();

        historyService.GetAllProfessors().then(function (response) {
            console.log(response.data);
            var data = response.data;
            if (data.length == 0) {
                var prof = {
                    id: "0",
                    username: "No Professors",
                    
                }
                rawTreeDataProf.push(cv);
            }
            console.log(data);
            for (var i = 0; i < data.length; i++) {
                var prof = {
                    id: "",
                    username: ""
                }
                prof.id = data[i].id;
               
                prof.username = data[i].username;
               
                rawTreeDataProf.push(prof);
            }
            $scope.tree_dataProf = rawTreeDataProf;

        });
    };
    function clearTableProf() {
        $scope.tree_dataProf = [];
        rawTreeDataProf = [];
        var treeProf;
        $scope.my_treeProf = treeProf = {};
    }

    $scope.expanding_propertyProf = {
        field: "username",
        displayName: "Username",
        sortable: true,
        filterable: true,
        cellTemplate: "<span ng-click = 'user_clicks_branch(row.branch)'>{{row.branch[expandingProperty.field]}}</span>",
    };


    $scope.col_defsProf = [
           {
            field: "Action",
            displayName: "Action",
            cellTemplate: "<button id='viewMe{{row.branch.UserId}}' ng-click='cellTemplateScope.clickView(row.branch)' class='btn btn-primary btn-xs' data-toggle='modal' data-target='#selectModal' >Select</button>",
            cellTemplateScope: {
                clickView: function (branch) {
                    $scope.profID = 0;
                    $scope.profID = branch.id;
                    $scope.profName = "";
                    $scope.profName = branch.username;
                    $scope.filterString = "";
                }
            }
        }
    ];
    //Prof Choice END

    // Specific History Start

    $scope.history;
    $scope.tree_dataCrit = new Array();
    var rawTreeDataCrit = new Array();
    var treeCrit;
    $scope.my_treeCrit = treeCrit = {};

    $scope.GetHistory = function () {
        clearTablehistory();

        var dateFrom = moment($scope.history.from).format("MM-DD-YYYY");
        var dateTo = moment($scope.history.to).format("MM-DD-YYYY");
        var data = {
            from: "",
            to: ""
        };
        data.from = dateFrom;
        data.to = dateTo;
        historyService.GetHistory($scope.profID,data).then(function (response) {
            var data = response.data;
            
            if (data.length == 0) {
                var cv = {
                    id: "0",
                    title: "NO ENTRIES",
                    description: "For range of dates you entered, there are no entries.",
                    link: "",
                    user_cv_id: "0",
                    criteria_id: "0",
                    start_date: data.from,
                    end_date: data.to,
                    status: "NO ENTRY",
                    links: []
                }
                rawTreeDataCrit.push(cv);

            }
            for (var i = 0; i < data.length; i++) {
               
                var cv_item = {
                    id: "",
                    title: "",
                    description: "",
                    link: "",
                    user_cv_id: "",
                    criteria_id: "",
                    start_date: "",
                    end_date: "",
                    status:"",
                    links: [],
                    date_created:""
                }
                var status_code = data[i].statuS_ID;
                var status = "UNKNOWN";
                switch (status_code) {
                    case 1:
                        status = "UNCONFIRMED NEW"
                        break;
                    case 2:
                        status = "CONFIRMED"
                        break;
                    case 3:
                        status = "UNCONFIRMED MODIFIED"
                        break;
                    case 4:
                        status = "REJECTED"
                        break;
                    case 5:
                        status = "DELETED"
                        break;
                    default:
                        break;
                }
                cv_item.status = status;
                cv_item.id = data[i].iD_ITEM;
                cv_item.title = data[i].name;
                cv_item.description = data[i].description;
                cv_item.link = data[i].cV_ITEM_LINK_LINK;
                cv_item.user_cv_id = data[i].cV_TABLE_ID_CV;
                cv_item.criteria_id = data[i].criteriA_ID_CRITERIA;
                cv_item.links = data[i].cV_ITEM_LINK;
                var date = moment(data[i].datE_CREATED).format("YYYY-MM-DD");
                cv_item.date_created = date;
                var date = moment(data[i].starT_DATE).format("YYYY-MM-DD");
                cv_item.start_date = date;
                date = moment(data[i].enD_DATE).format("YYYY-MM-DD");
                cv_item.end_date = date;
                if (cv_item.title != null) {
                    rawTreeDataCrit.push(cv_item);
                }
            }
            $scope.tree_dataCrit = rawTreeDataCrit;

        });
    };
    function clearTablehistory() {
        $scope.tree_dataCrit = [];
        rawTreeDataCrit = [];
        var treeCrit;
        $scope.my_treeCrit = treeCrit = {};
    }

    $scope.expanding_propertyCrit = {
        field: "title",
        displayName: "CV Name",
        sortable: true,
        filterable: true,
        cellTemplate: "<span ng-click = 'user_clicks_branch(row.branch)'>{{row.branch[expandingProperty.field]}}</span>",
    };


    $scope.col_defsCrit = [
        {
            field: "link",
            displayName: "Attachment link",
            sortable: false,
            cellTemplate: "<span ng-switch='row.branch[col.field]'><a ng-switch-when='undefined'>No Attachment</a><a ng-switch-default ng-href='{{row.branch[col.field]}}'>Download</a></span>",
            filterable: false
        },
        {
            field: "date_created",
            displayName: "Date created",
            sortable: true,
            sortingType: "string"
        },
        {
            field: "status",
            displayName: "Status",
            sortable: true,
            sortingType: "string",
            filterable: true
        },
        {
            field: "Action",
            displayName: "Action",
            cellTemplate: "<button id='viewMe{{row.branch.}}' ng-click='cellTemplateScope.clickView(row.branch)' class='btn btn-primary btn-xs' data-toggle='modal' data-target='#viewCrModal' >View</button>",
            cellTemplateScope: {
                clickView: function (branch) {
                    $scope.viewCr = branch;
                    if (branch.links.length > 0) {
                        $scope.links = [];
                    }
                    else {
                        $scope.links = [];
                    }
                    for (var i = 0; i < branch.links.length; i++) {
                        $scope.links.push({ DESCRIPTION: branch.links[i].description, URL: branch.links[i].url });
                    }
                    criteriaService.GetCriteria(branch.criteria_id).then(function (response) {
                        $scope.viewCriteriaFull = response.data;
                        $scope.viewCriteria = $scope.viewCriteriaFull.name;
                        console.log($scope.viewCriteriaFull.name);
                    });
                }
            }
        }
    ];

    // Specific History END
    //History END




    $scope.clearMyHistory = function () {
        clearTableMyHistory();
    }

    $scope.clearHistory = function () {
        clearTablehistory();
    }

    $scope.clearProf = function () {
        clearTableProf();
    }


    function onload() {
        if (window.location.hash == '#/history') {
            $scope.GetAllProfessors();
        }
    }
    onload();

    $scope.resetSearch = function () {
        $scope.filterString = "";
    }

}]);