﻿'use strict';
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

    var _ConfirmRequest = function (item_id) {
        console.log('poziva se post')
        return $http.post(serviceBase + 'api/CVitem/Update/' + item_id + '/2').then(function (response) {
            console.log('radi post')
            return response;
        });
    };

    var _RejectRequest = function (item_id) {
        console.log('poziva se post')
        return $http.post(serviceBase + 'api/CVitem/Update/' + item_id + '/4').then(function (response) {
            console.log('radi post')
            return response;
        });
    };

    requestsServiceFactory.GetCVTable = _GetCVTable;
    requestsServiceFactory.GetProcessedRequests = _GetProcessedRequests;
    requestsServiceFactory.GetUnconfirmedRequests = _GetUnconfirmedRequests;
    requestsServiceFactory.ConfirmRequest = _ConfirmRequest;
    requestsServiceFactory.RejectRequest = _RejectRequest;
    
    return requestsServiceFactory;
}]);