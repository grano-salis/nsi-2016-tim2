'use strict';
app.factory('fullLogService', ['$http', '$q', 'localStorageService', 'ngAuthSettings', function ($http, $q, localStorageService, ngAuthSettings) {

    var serviceBase = ngAuthSettings.apiServiceBaseUri;
    var logServiceFactory = {};

    var _GetLogs = function () {
        return $http.get(serviceBase + 'api/Log/GetFullLog/').then(function (response) {
            return response;
        });
    };

    var _GetCriteria = function (id) {
        return $http.get(serviceBase + 'api/Criteria/GetCriteria/' + id).then(function (response) {
            return response;
        });
    };

    var _ConfirmRequest = function (item_id) {
        console.log('poziva se post')
        return $http.post(serviceBase + 'api/CVitem/UpdateStatus/' + item_id + '/2').then(function (response) {
            console.log('radi post')
            return response;
        });
    };

    var _RejectRequest = function (item_id) {
        console.log('poziva se post')
        return $http.post(serviceBase + 'api/CVitem/UpdateStatus/' + item_id + '/4').then(function (response) {
            console.log('radi post')
            return response;
        });
    };

    var _RestoreRequest = function (item_id) {
        console.log('poziva se post')
        return $http.post(serviceBase + 'api/CVitem/UpdateStatus/' + item_id + '/3').then(function (response) {
            console.log('radi post')
            return response;
        });
    };

    logServiceFactory.GetLogs = _GetLogs;
    logServiceFactory.GetCriteria = _GetCriteria;
    logServiceFactory.ConfirmRequest = _ConfirmRequest;
    logServiceFactory.RejectRequest = _RejectRequest;
    logServiceFactory.RestoreRequest = _RestoreRequest;
    return logServiceFactory;
}]);