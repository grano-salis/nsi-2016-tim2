'use strict';
app.factory('requestsService', ['$http', '$q', 'localStorageService', 'ngAuthSettings','Upload', function ($http, $q, localStorageService, ngAuthSettings,Upload) {

    var serviceBase = ngAuthSettings.apiServiceBaseUri;
    var cvItemServiceFactory = {};

    var _GetAllCriteria = function () {
        return $http.get(serviceBase + 'api/Criteria/GetAllCriteria/').then(function (response) {
            return response;
        });
    };

    var _AddCV = function (data) {
         return Upload.upload({
            url: serviceBase + 'api/CVitem/Create',
            data: data
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

    cvItemServiceFactory.GetAllCriteria = _GetAllCriteria;
    cvItemServiceFactory.AddCV = _AddCV;
    return cvItemServiceFactory;
}]);