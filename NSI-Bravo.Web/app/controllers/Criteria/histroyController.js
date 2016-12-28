'use strict';

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
                    links: []
                }
                cv_item.id = data[i].iD_ITEM;
                cv_item.title = data[i].name;
                cv_item.description = data[i].description;
                cv_item.link = data[i].attachmenT_LINK;
                cv_item.user_cv_id = data[i].cV_TABLE_ID_CV;
                cv_item.criteria_id = data[i].criteriA_ID_CRITERIA;
                cv_item.links = data[i].attachment;
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
        displayName: "Name",
        sortable: true,
        filterable: true,
        cellTemplate: "<a ng-click = 'user_clicks_branch(row.branch)'>{{row.branch[expandingProperty.field]}}</a>",
    };


    $scope.col_defs = [
        {
            field: "link",
            displayName: "Attachment link",
            sortable: true,
            cellTemplate: "<span ng-switch='row.branch[col.field]'><a ng-switch-when='undefined'>No Attachment</a><a ng-switch-default ng-href='{{row.branch[col.field]}}'>Download</a></span>",
            sortingType: "number",
            filterable: true
        },
        {
            field: "Action",
            displayName: "Action",
            cellTemplate: "<button id='viewMe{{row.branch.id}}' ng-click='cellTemplateScope.clickView(row.branch)' class='btn btn-primary btn-xs' data-toggle='modal' data-target='#viewCrModal' >View</button>" ,
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
            var data = response.data;
            if (data.length == 0) {
                var prof = {
                    id: "0",
                    firstname: "No Professors",
                    lastname: "No Professors"
                }
                rawTreeDataProf.push(cv);
            }
            for (var i = 0; i < data.length; i++) {
                var prof = {
                    id: "",
                    firstname: "",
                    lastname: ""
                }
                prof.id = data[i].iD_CV;
                prof.firstname = data[i].firstname;
                prof.lastname = data[i].lastname;
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
        field: "firstname",
        displayName: "First Name",
        sortable: true,
        filterable: true,
        cellTemplate: "<a ng-click = 'user_clicks_branch(row.branch)'>{{row.branch[expandingProperty.field]}}</a>",
    };


    $scope.col_defsProf = [
        {
            field: "lastname",
            displayName: "Last Name",
            sortable: true,
            sortingType: "number",
            filterable: true
        },
        {
            field: "Action",
            displayName: "Action",
            cellTemplate: "<button id='viewMe{{row.branch.id}}' ng-click='cellTemplateScope.clickView(row.branch)' class='btn btn-primary btn-xs' data-toggle='modal' data-target='#selectModal' >Select</button>",
            cellTemplateScope: {
                clickView: function (branch) {
                    $scope.profID = 0;
                    $scope.profID = branch.id;
                    $scope.profName = "";
                    $scope.profName = branch.firstname + " "+ branch.lastname;
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
                    links: []
                }
                cv_item.id = data[i].iD_ITEM;
                cv_item.title = data[i].name;
                cv_item.description = data[i].description;
                cv_item.link = data[i].attachmenT_LINK;
                cv_item.user_cv_id = data[i].cV_TABLE_ID_CV;
                cv_item.criteria_id = data[i].criteriA_ID_CRITERIA;
                cv_item.links = data[i].attachment;
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
        displayName: "Name",
        sortable: true,
        filterable: true,
        cellTemplate: "<a ng-click = 'user_clicks_branch(row.branch)'>{{row.branch[expandingProperty.field]}}</a>",
    };


    $scope.col_defsCrit = [
        {
            field: "link",
            displayName: "Attachment link",
            sortable: true,
            cellTemplate: "<span ng-switch='row.branch[col.field]'><a ng-switch-when='undefined'>No Attachment</a><a ng-switch-default ng-href='{{row.branch[col.field]}}'>Download</a></span>",
            sortingType: "number",
            filterable: true
        },
        {
            field: "Action",
            displayName: "Action",
            cellTemplate: "<button id='viewMe{{row.branch.id}}' ng-click='cellTemplateScope.clickView(row.branch)' class='btn btn-primary btn-xs' data-toggle='modal' data-target='#viewCrModal' >View</button>",
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



}]);