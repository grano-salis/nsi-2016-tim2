'use strict';

app.controller('criteriaController', ['$scope', '$location', '$timeout', '$routeParams', '$log', 'criteriaService', '$route', 'Notification', function ($scope, $location, $timeout, $routeParams, $log, criteriaService, $route, Notification) {
    //START DeleteCriteria
    $scope.data = new Array();
    $scope.tree_data = new Array();
    var rawTreeData = new Array();
    var data = new Array();
    var tree;
    $scope.my_tree = tree = {};

    var myTreeData = new Array();
    // $scope.row.branch.expanded = true;
    $scope.expanding_property = {
        field: "title",
        displayName: "Criteria Name",
        sortable: true,
        filterable: true,
        cellTemplate: "<span ng-click = 'user_clicks_branch(row.branch)'>{{row.branch[expandingProperty.field]}}</span>",
        expanded: true

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
            field: "points",
            displayName: "Points",
            sortable: true,
            sortingType: "number"
        },
        {
            field: "Actions",
            displayName: "Actions",
            cellTemplate: "<button ng-click='cellTemplateScope.clickView(row.branch)' class='btn btn-primary btn-xs' data-toggle='modal' data-target='#viewCrModal' >View</button>" + " " + "<button ng-click='cellTemplateScope.clickAdd(row.branch)' class='btn btn-primary btn-xs' data-toggle='modal' data-target='#addCrModal' >Add Subcriteria</button>" + " " + "<button ng-click='cellTemplateScope.clickEdit(row.branch)' class='btn btn-warning btn-xs' data-toggle='modal' data-target='#editCrModal' >Edit</button>" + " " + "<button ng-click='cellTemplateScope.clickDel(row.branch)' class='btn btn-danger btn-xs' data-toggle='modal' data-target='#delCrModal'  >Delete</button>",
            cellTemplateScope: {
                clickEdit: function (branch) {
                    $log.log(branch);
                    $scope.editCr = { name: branch.title, points: branch.points, id: branch.id, parent_id: branch.parent_id, desc: branch.description };
                    console.log($scope.editCr.desc);
                },
                clickDel: function (branch) {
                    $scope.editCr = { name: branch.title, points: branch.points, id: branch.id, parent_id: branch.parent_id, desc: branch.description };
                    //$scope.deleteCriteria(branch.id);
                },
                clickAdd: function (branch) {
                    $scope.addCr = { name: branch.title, points: branch.points, id: branch.id, parent_id: branch.parent_id, desc: branch.description };
                },
                clickView: function (branch) {
                    $scope.viewCr = { title: branch.title, points: branch.points, id: branch.id, parent_id: branch.parent_id, desc: branch.description, created: branch.created };
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
                    description: "",
                    parent_id: "",
                    created: "",
                    points: ""
                }
                criterion.id = data[i].iD_CRITERIA;
                criterion.title = data[i].name;
                criterion.description = data[i].description;
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
            var timer = $timeout(function () {
                $timeout.cancel(timer);
                $scope.my_tree.expand_all();
            }, 0);

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
            Notification.success('Successfully deleted');
            GetAllCriteria();

        },
         function (response) {
             Notification.error({ message: response.data.message, title: 'Failed to delete' });

             //Notification.error('Failed to delete:'+response.data.message);

         });
    };

    var startTimerX = function () {
        var timer = $timeout(function () {
            $timeout.cancel(timer);
            $scope.deleteMessage = "";
        }, 1000);

    };

    $scope.editCriteria = function (cr) {
        $log.log(cr);
        var data = {};
        data.NAME = cr.name;
        console.log(cr.desc);
        data.DESCRIPTION = cr.desc;
        data.POINTS = cr.points;
        data.ID_CRITERIA = cr.id;
        data.PARENT_CRITERIA = cr.parent_id;

        criteriaService.UpdateCriteria(cr.id, data).then(function (response) {
            $log.log(response);
            Notification.success('Successfully edited');
            GetAllCriteria();
        },
        function (response) {
            Notification.error({ message: response.data.message, title: 'Failed to Edit' });

        }
        );
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

    $scope.listSubcriteria = function (cr) {

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
            if (subcriteria.length > 0) {
                $scope.secondCriteria = subcriteria;
                $log.log($scope.secondCriteria);
            }
        });

    };


    $scope.addCriteria = function (cr, modal) {
        var data = {};
        this.hide;
        data.NAME = cr.name;
        console.log(cr.desc);
        data.DESCRIPTION = cr.desc;
        data.POINTS = cr.points;
        if ($scope.addCr != null) {
            data.PARENT_CRITERIA = $scope.addCr.id;
        }
        criteriaService.AddCriteria(data).then(function (response) {
            $log.log(response);
            Notification.success('Successfully added');
            $scope.clearForm();
            GetAllCriteria();
        },
        function (response) {
            Notification.error({ message: response.data.message, title: 'Failed to Add Criteria' });
        });
    };

    $scope.selectSecondCr = function (cr) {
        $scope.second = cr.name;
        $scope.selectedCr = cr.id;
    };

    $scope.clearForm = function () {
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