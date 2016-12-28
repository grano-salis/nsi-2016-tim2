﻿'use strict';

app.controller('unconfirmedRequestsController', ['$scope', '$location', '$timeout', '$routeParams', '$log', 'myCVService', 'requestsService', '$route', function ($scope, $location, $timeout, $routeParams, $log, criteriaService, requestsService, $route) {
    // MY CV TABLE
    $scope.data = new Array();
    $scope.tree_data = new Array();
    var rawTreeData = new Array();
    var data = new Array();
    var tree;
    $scope.my_tree = tree = {};
    var myTreeData = new Array();
    $scope.editCriteriaFull = new Array();

    
    // Linkovi
    $scope.links = [{ DESCRIPTION: '', URL: '' }];
    
    // Pregled stavki Lista
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
            field: "Actions",
            displayName: "Actions",
            cellTemplate: "<button id='viewMe{{row.branch.id}}' ng-click='cellTemplateScope.clickView(row.branch)' class='btn btn-danger btn-xs' data-toggle='modal' data-target='#viewCrModal' >Edit</button>" ,
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
    // Clear Tree CV Edit
    function clearTable() {
        $scope.data = [];
        $scope.tree_data = [];
        rawTreeData = [];
        data = [];
        $scope.my_tree = tree = {};
        myTreeData = [];
    };
    // Testiranje Klika Na neku tree stavku
    $scope.my_tree_handlerCV = function (branch) {
        console.log('You clicked on', branch);
        var modal = document.getElementById("viewMe" + branch.id);
        modal.click();
    }

    $scope.my_tree_handler = function (branch) {
        console.log('you clicked on', branch);
    }

    function GetUnconfirmedRequests() {
        clearTable();
        requestsService.GetCVTable().then(function (response) {
            var CVTable = response.data;
            for (var i = 0; i < CVTable.length; i++) {
                requestsService.GetUnconfirmedRequests(CVTable[i].iD_CV).then(function (response) {
                    data = response.data;
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
                        cv_item.id = data[i].cv_item.iD_ITEM;
                        cv_item.title = data[i].cv_item.name;
                        cv_item.description = data[i].cv_item.description;
                        cv_item.link = data[i].cv_item.attachmenT_LINK;
                        cv_item.user_cv_id = data[i].cv_item.cV_TABLE_ID_CV;
                        cv_item.criteria_id = data[i].cv_item.criteriA_ID_CRITERIA;
                        cv_item.links = data[i].cv_item.attachment;


                        var date = moment(data[i].cv_item.starT_DATE).format("YYYY-MM-DD");
                        cv_item.start_date = date;
                        date = moment(data[i].cv_item.enD_DATE).format("YYYY-MM-DD");
                        cv_item.end_date = date;
                        /*
                            var date = moment(data[i].datE_CREATED).format("DD-MM-YYYY");
                            if (date !== null)
                                criterion.created = date;
                        */
                        if (cv_item.title != null) {
                            rawTreeData.push(cv_item);
                        }
                    }
                    myTreeData = rawTreeData;
                    //getTree(rawTreeData, 'id', 'parent_id');
                    $scope.tree_data = myTreeData;
                });
            }
            
        });
    };
    GetUnconfirmedRequests();


    //MYcv Edit Criteria Choose
    $scope.tree_dataCrit = new Array();
    var rawTreeDataCrit = new Array();
    var treeCrit;
    $scope.my_treeCrit = treeCrit = {};
    var myTreeDataCrit = new Array();

    // Tree for Criteria
    $scope.expanding_propertyCrit = {
        field: "title",
        displayName: "Name",
        sortable: true,
        filterable: true,
        cellTemplate: "<a ng-click = 'user_clicks_branch(row.branch)'>{{row.branch[expandingProperty.field]}}</a>"
    };

    $scope.col_defsCrit = [
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
            cellTemplate: "<button ng-click='cellTemplateScope.clickAdd(row.branch)' class='btn btn-primary btn-xs' data-toggle='modal' data-target='#criteriaChoiceModal' >Select</button>",
            cellTemplateScope: {
                clickAdd: function (branch) {
                    criteriaService.GetCriteria(branch.id).then(function (response) {
                        $scope.editCriteriaFull = response.data;
                        $scope.editCriteria = $scope.editCriteriaFull.name;
                        console.log($scope.editCriteriaFull.name);
                    });
                }
            }
        }
    ];
    // Ciscenje Criteria vjijednosti
    function clearTableCrit() {
        $scope.dataCrit = [];
        $scope.tree_dataCrit = [];
        rawTreeDataCrit = [];
        data = [];
        $scope.my_treeCrit = tree = {};
        myTreeDataCrit = [];
    };
   
    // Zajednicki getTree
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
   
    $scope.selectedCr;
    $scope.clearForm = function () {
        //clear form
        $scope.cr = {};
        $scope.selectedCr = null;
    };

    $scope.confirmRequest = function (item_id) {
        requestsService.ConfirmRequest(item_id).then(function (response) {
            $log.log('Confirm request');
            $log.log(response);
            GetUnconfirmedRequests();
        });
    };

    $scope.rejectRequest = function (item_id) {
        requestsService.RejectRequest(item_id).then(function (response) {
            $log.log('Reject request');
            $log.log(response);
            GetUnconfirmedRequests();
        });
    };

}]);