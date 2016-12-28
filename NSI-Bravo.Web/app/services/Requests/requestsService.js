'use strict';
app.factory('requestsService', ['$http', '$q', 'localStorageService', 'ngAuthSettings', 'Upload', function ($http, $q, localStorageService, ngAuthSettings, Upload) {

    var serviceBase = ngAuthSettings.apiServiceBaseUri;
    var requestsServiceFactory = {};

    var _GetCVTable = function () {
        return $http.get(serviceBase + 'api/CVtable/GetAll').then(function (response) {
            return response;
        });
    };

    var _GetUnconfirmedRequests = function (cv_id) {
        return $http.get(serviceBase + 'api/CVitem/GetAllUnconfirmedAndModified/' + cv_id).then(function (response) {
            return response;
        });
    };

    var _GetProcessedRequests = function (cv_id) {
        return $http.get(serviceBase + 'api/CVitem/GetProcessedRequests/' + cv_id).then(function (response) {
            return response;
        });
    };


    requestsServiceFactory.GetCVTable = _GetCVTable;
    requestsServiceFactory.GetProcessedRequests = _GetProcessedRequests;
    requestsServiceFactory.GetUnconfirmedRequests = _GetUnconfirmedRequests;
    
    return requestsServiceFactory;
}]);