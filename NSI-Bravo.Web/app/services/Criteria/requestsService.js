'use strict';
app.factory('requestsService', ['$http', '$q', 'localStorageService', 'ngAuthSettings', function ($http, $q, localStorageService, ngAuthSettings) {

    var serviceBase = ngAuthSettings.apiServiceBaseUri;
    var cvItemServiceFactory = {};

    var _GetAllCriteria = function () {
        return $http.get(serviceBase + 'api/Criteria/GetAllCriteria/').then(function (response) {
            return response;
        });
    };

    var _AddCV = function (data) {
        return $http.post(serviceBase + 'api/CVitem/Create', data).then(function (response) {
            console.log(response);
            return response;
        });
    };

    cvItemServiceFactory.GetAllCriteria = _GetAllCriteria;
    cvItemServiceFactory.AddCV = _AddCV;
    return cvItemServiceFactory;
}]);