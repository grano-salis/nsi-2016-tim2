'use strict';
app.factory('historyService', ['$http', '$q', 'localStorageService', 'ngAuthSettings', function ($http, $q, localStorageService, ngAuthSettings) {

    var serviceBase = ngAuthSettings.apiServiceBaseUri;
    var historyServiceFactory = {};

    var _GetMyHistory = function (data) {
        return $http.post(serviceBase + 'api/CVtable/GetMyHistory',data).then(function (response) {
            return response;
        });
    };

    var _GetAllProfessors = function () {
        return $http.get(serviceBase + 'api/CVtable/GetAllProfessors/').then(function (response) {
            return response;
        });
    };

    var _GetHistory = function (id,data) {
        return $http.post(serviceBase + 'api/CVtable/GetByDateRange/'+id, data).then(function (response) {
            return response;
        });
    };

    historyServiceFactory.GetMyHistory = _GetMyHistory;
    historyServiceFactory.GetAllProfessors = _GetAllProfessors;
    historyServiceFactory.GetHistory = _GetHistory;
    return historyServiceFactory;
}]);