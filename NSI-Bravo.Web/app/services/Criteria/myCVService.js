'use strict';
app.factory('myCVService', ['$http', '$q', 'localStorageService', 'ngAuthSettings', function ($http, $q, localStorageService, ngAuthSettings) {

    var serviceBase = ngAuthSettings.apiServiceBaseUri;
    var criteriaServiceFactory = {};

    var _GetMyCVs = function (cv_id) {
        return $http.get(serviceBase + 'api/CVitem/GetAll/'+cv_id).then(function (response) {
            return response;
        });
    };

    var _EditCVItem = function (id, data) {
        return $http.put(serviceBase + 'api/CVitem/Update/' + id, data).then(function (response) {
            console.log(response);
            return response;
        });
    };


    var _DeleteCVItem = function (id) {
        return $http.delete(serviceBase + 'api/CVItem/Delete/' + id).then(function (response) {
            return response;
        });
    };




    var _GetAllCriteria = function () {
        return $http.get(serviceBase + 'api/Criteria/GetAllCriteria/').then(function (response) {
            return response;
        });
    };


    var _GetCriteria = function (id) {
        return $http.get(serviceBase + 'api/Criteria/GetCriteria/' + id).then(function (response) {
            return response;
        });
    };


    criteriaServiceFactory.GetMyCVs = _GetMyCVs;
    criteriaServiceFactory.EditCVItem = _EditCVItem;
    criteriaServiceFactory.DeleteCVItem = _DeleteCVItem;
    criteriaServiceFactory.GetAllCriteria = _GetAllCriteria;
    criteriaServiceFactory.GetCriteria = _GetCriteria;
    return criteriaServiceFactory;
}]);