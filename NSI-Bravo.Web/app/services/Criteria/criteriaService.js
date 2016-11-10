'use strict';
app.factory('criteriaService', ['$http', '$q', 'localStorageService', 'ngAuthSettings', function ($http, $q, localStorageService, ngAuthSettings) {

    var serviceBase = ngAuthSettings.apiServiceBaseUri;
    var criteriaServiceFactory = {};


    var _GetAllMasterCriteria = function () {
        return $http.get(serviceBase + 'api/Criteria/GetAllMasterCriteria/').then(function (response) {
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

    var _AddCriteria = function(data) {
            return $http.post(serviceBase + 'api/Criteria/PostCriteria', user_data).then(function (response) {
                console.log(response);
                return response;
            });
    };
    
    criteriaServiceFactory.GetAllMasterCriteria = _GetAllMasterCriteria;
    criteriaServiceFactory.GetCriteria = _GetCriteria;
    criteriaServiceFactory.DeleteCriteria = _DeleteCriteria;
    criteriaServiceFactory.AddCriteria = _AddCriteria;
    return criteriaServiceFactory;
}]);