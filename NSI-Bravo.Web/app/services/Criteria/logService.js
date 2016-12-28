'use strict';
app.factory('logService', ['$http', '$q', 'localStorageService', 'ngAuthSettings', function ($http, $q, localStorageService, ngAuthSettings) {

    var serviceBase = ngAuthSettings.apiServiceBaseUri;
    var logServiceFactory = {};

    var _GetLogs = function () {
        return $http.get(serviceBase + 'api/Log/GetAllLogs/').then(function (response) {
            return response;
        });
    };
    logServiceFactory.GetLogs = _GetLogs;
    return logServiceFactory;
}]);