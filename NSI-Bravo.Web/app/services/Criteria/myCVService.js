'use strict';
app.factory('myCVService', ['$http', '$q', 'localStorageService', 'ngAuthSettings','Upload', function ($http, $q, localStorageService, ngAuthSettings,Upload) {

    var serviceBase = ngAuthSettings.apiServiceBaseUri;
    var criteriaServiceFactory = {};

    var _GetMyCVItems = function () {
        return $http.get(serviceBase + 'api/CVitem/GetMy/',{ withCredentials: true }).then(function (response) {
            return response;
        });
    };

    var _EditCVItem = function (id, data) {
        console.log(data);
        return Upload.upload({
            url: serviceBase + 'api/CVitem/Update/'+id,
            data: data,
            method: 'put',
            withCredentials: true
        });
    };

    //with credentials-> send cookie to api
    var _DeleteCVItem = function (id) {
        return $http.delete(serviceBase + 'api/CVItem/Delete/' + id, { withCredentials: true }).then(function (response) {
            return response;
        });
    };




    var _GetAllCriteria = function () {
        return $http.get(serviceBase + 'api/Criteria/GetAllCriteria/',{ withCredentials: true }).then(function (response) {
            return response;
        });
    };


    var _GetCriteria = function (id) {
        return $http.get(serviceBase + 'api/Criteria/GetCriteria/' + id).then(function (response) {
            return response;
        });
    };


    var _AddCV = function (data) {
        return Upload.upload({
            url: serviceBase + 'api/CVitem/Create',
            data: data,
            withCredentials: true
        }).then(function (resp) {
            console.log('Success ' + resp.config.data.file.name + 'uploaded. Response: ' + resp.status);
        }, function (resp) {
            console.log('Error status: ' + resp.status);
        }, function (evt) {
            var progressPercentage = parseInt(100.0 * evt.loaded / evt.total);
            console.log('progress: ' + progressPercentage + '% ' + evt.config.data.file.name);
        });


        /*
        return $http.post(serviceBase + 'api/CVitem/Create', data).then(function (response) {
            console.log(response);
            return response;
        });*/
    };


    var _GetScore = function (id) {
        return $http.get(serviceBase + 'api/CVtable/Score/' + id, { withCredentials: true }).then(function (response) {
            return response;
        });
    };

    criteriaServiceFactory.GetMyCVItems = _GetMyCVItems;
    criteriaServiceFactory.EditCVItem = _EditCVItem;
    criteriaServiceFactory.DeleteCVItem = _DeleteCVItem;
    criteriaServiceFactory.GetAllCriteria = _GetAllCriteria;
    criteriaServiceFactory.GetCriteria = _GetCriteria;
    criteriaServiceFactory.AddCV = _AddCV;
    criteriaServiceFactory.GetScore = _GetScore;

    return criteriaServiceFactory;
}]);