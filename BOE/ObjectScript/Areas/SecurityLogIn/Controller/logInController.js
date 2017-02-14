
angular.module('app').controller('logInController', function ($scope, $http, $location, $route) {

    $scope.divShowError = false;
    $scope.divUserNameValidation = false;
    $scope.divUserPasswordValidation = false;

    $scope.errorMess = false;

    $scope.LogIn = function () {
        var dt = $scope.logIn;
        if ($scope.LoginForm.$valid) {
            $http({
                method: 'POST',
                url: '/Account/Login',
                dataType: "json",
                params: dt,
                //params: { model:$scope.logIn },
                //data: { model: $scope.logIn },
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                }
                //headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
            }).success(function (data) {
                debugger;
                if (data.success) {
                    debugger;
                    $route.reload();
                    $scope.LogInPage();
                } else {
                    debugger;
                    $scope.errorMessage(data);
                    if (data.message === "Success") {
                        $route.reload();
                        $scope.LogInPage();
                    }
                    //$scope.divShowError = true;
                }
            }).error(function (data, status, headers, config) {
                $scope.status = status + ' ' + headers;
                console.log($scope.status);
            });
            //.
            //error(function (XMLHttpRequest, textStatus, errorThrown) {
            //    toastr.error(XMLHttpRequest + ": " + textStatus + ": " + errorThrown, 'Error!!!');
            //});
        }
    };

    $scope.LogInPage = function () {
        debugger;
        //$location.path('/Home');
        window.location = "/Home#";
    };

    $scope.errorMessage = function (data) {
        $scope.errorMess = true;
        $scope.logIn.Error = data.message;
    };


    $scope.LogOut = function () {
        debugger;
        $http.get('/Account/LogOff').success(function (data) {
            if (data.success === true) {
                window.location = "/Home#";
            } else {
                window.location = "/";
            }
        });
    };

}).directive('onKeydown', function () {
    return {
        restrict: 'A',
        link: function (scope, elem, attrs) {
            // this next line will convert the string
            // function name into an actual function
            var functionToCall = scope.$eval(attrs.ngKeydown);
            elem.on('keydown', function (e) {
                // on the keydown event, call my function
                // and pass it the keycode of the key
                // that was pressed
                // ex: if ENTER was pressed, e.which == 13
                alert("OK");
                functionToCall(e.which);
            });
        }
    };
});




