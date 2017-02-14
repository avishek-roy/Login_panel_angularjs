
angular.module('app').controller('menuController', function ($scope, $http, $rootScope) {


    //Hide all menu
    $scope.Basic = false;
    $scope.BrokerRelated = false;
    $scope.Location = false;
    $scope.User = false;
    $scope.Administration = false;
    $scope.ChangePassword = false;
    $scope.ChangePasswordByAdmin = false;
    $scope.UserGroup = false;
    $scope.TraderSurveyByAdmin = false;
    $scope.ClientDeposite = false;
    $scope.ClientServey = false;
    $scope.TraderPositionByDeposite = false;
    $scope.ClientSurvey = false;
    $scope.SurveyStatusMonitor = false;

    var showApplication = "";
    var showModule = "";
    var showName = "";

    $http.get('/Home/GetSiteMenu').then(function (data) {
        $scope.menu = data.data;

        $scope.colors = [{ colorName: "pink", val: true }, { colorName: "green", val: false }, { colorName: "Red", val: true }, { colorName: "Yellow", val: true }]

        for (var i = 0; i < $scope.menu.data.length; i++)
        {
            if ($scope.menu.data[i].ApplicationID == "Basic")
                $scope.Basic = true;
            if ($scope.menu.data[i].ModuleID == "Administration")
                $scope.Administration = true;
            if ($scope.menu.data[i].ModuleID == "BrokerRelated")
                $scope.BrokerRelated = true;
            if ($scope.menu.data[i].ModuleName == "Location")
                $scope.Location = true;
            if ($scope.menu.data[i].ModuleName == "User")
                $scope.User = true;
            if ($scope.menu.data[i].ModuleName == "ChangePassword")
                $scope.ChangePassword = true;
            if ($scope.menu.data[i].ModuleName == "ChangePasswordByAdmin")
                $scope.ChangePasswordByAdmin = true;
            if ($scope.menu.data[i].ModuleName == "UserGroup")
                $scope.UserGroup = true;
            if ($scope.menu.data[i].ApplicationID == "ClientServey")
                $scope.ClientServey = true;
            if ($scope.menu.data[i].ModuleName == "ClientDeposite")
                $scope.ClientDeposite = true;
            if ($scope.menu.data[i].ModuleName == "TraderPositionByDeposite")
                $scope.TraderPositionByDeposite = true;
            if ($scope.menu.data[i].ModuleName == "ClientSurvey")
                $scope.ClientSurvey = true;
            if ($scope.menu.data[i].ModuleName == "SurveyStatusMonitor")
                $scope.SurveyStatusMonitor = true;
        }
    });
});


