'use strict';
app.factory('criteriaService', ['$http', '$q', 'localStorageService', 'ngAuthSettings', function ($http, $q, localStorageService, ngAuthSettings) {

    var serviceBase = ngAuthSettings.apiServiceBaseUri;
    var criteriaServiceFactory = {};


    var _GetAllCriteria = function () {
        return $http.get(serviceBase + 'api/Criteria/GetAllCriteria/').then(function (response) {
            return response;
        });
    };

    var _GetCriteria = function (id) {
        return $http.get(serviceBase + 'api/Criteria/GetCriteria/'+id).then(function (response) {
            return response;
        });
    };

    var _DeleteCriteria = function (id) {
        return $http.delete(serviceBase + 'api/Criteria/DeleteCriteria/' + id).then(function (response) {
            return response;
        });
    };
    
    criteriaServiceFactory.GetAllCriteria = _GetAllCriteria;
    criteriaServiceFactory.GetCriteria = _GetCriteria;
    criteriaServiceFactory.DeleteCriteria = _DeleteCriteria;
    return criteriaServiceFactory;
}]);