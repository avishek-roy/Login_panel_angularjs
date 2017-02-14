
angular.module('app').controller('changePasswordController', function ($scope, $http, $rootScope, $route, $templateCache, $location) {

    $scope.Reload = function () {
        var currentPageTemplate = $route.current.templateUrl;
        $templateCache.remove(currentPageTemplate);
        $route.reload();
    }

    //Change Password Save
    $scope.ChangePassword = function () {
        var newPassword = $scope.changePassword.NewPassword;
        var confirmPassword = $scope.changePassword.ConfirmPassword;
        if (newPassword.length < 6 || confirmPassword < 6) {
            toastr.error("Your password will be minimum 6 character");
            return;
        }
        if ($scope.changePassword.NewPassword != $scope.changePassword.ConfirmPassword) {
            toastr.error("New and Confirm Password are not matched");
            return;
        }
        if ($scope.SelfChangePasswordForm.$valid) {
            $http({
                method: 'POST',
                url: '/Security/ChangePssword/SelfPasswordChange',
                data: $scope.changePassword
            }).success(function (data) {
                if (data.success) {
                    debugger;
                    //toastr.success(data.message);
                    //$location.path('/LogOut');
                    //$scope.changePassword = {};
                    $scope.Reload();
                } else {
                    if (data.message === "LogOut") {
                        $location.path('/LogIn');
                    }
                    toastr.error(data.message);
                }
            }).
            error(function (XMLHttpRequest, textStatus, errorThrown) {
                toastr.error(XMLHttpRequest + ": " + textStatus + ": " + errorThrown, 'Error!!!');
            });
        }
    };


    $scope.Reload = function () {
        debugger;
        $http.get('/Account/LogOff').success(function (data) {
            if (data.success === true) {
                window.location = "/Home#";
            } else {
                window.location = "/";
            }
        });
    }
    
//Auto complete for User Full Name
$http.get("/Security/ChangePssword/LoadAllFullName")
    .then(function (response) {
        $rootScope.searchItems = [];
        for (var i = 0; i < response.data.data.length; i++) {
            $rootScope.searchItems.push(response.data.data[i].UserFullName);
        }
});


//Define Suggestions List
$rootScope.suggestions = [];
//Define Selected Suggestion Item
$rootScope.selectedIndex = -1;

//Function To Call On ng-change
$rootScope.searchFullName = function () {
    $rootScope.suggestions = [];
    var myMaxSuggestionListLength = 0;
    for(var i=0; i<$rootScope.searchItems.length; i++){
        var searchItemsSmallLetters = angular.lowercase($rootScope.searchItems[i]);
        var searchTextSmallLetters = angular.lowercase($scope.changePassword.FullName);
        if( searchItemsSmallLetters.indexOf(searchTextSmallLetters) !== -1){
            $rootScope.suggestions.push(searchItemsSmallLetters);
            myMaxSuggestionListLength += 1;
            if(myMaxSuggestionListLength == 5){
                break;
            }
        }
    }
}

//Keep Track Of Search Text Value During The Selection From The Suggestions List  
$rootScope.$watch('selectedIndex',function(val){
    if(val !== -1) {
        $scope.changePassword.FullName = $rootScope.suggestions[$rootScope.selectedIndex];
    }
});


//Text Field Events
//Function To Call on ng-keydown
$rootScope.checkKeyDown = function (event) {
    if(event.keyCode === 40){//down key, increment selectedIndex
        event.preventDefault();
        if($rootScope.selectedIndex+1 !== $rootScope.suggestions.length){
            $rootScope.selectedIndex++;
        }
    }else if(event.keyCode === 38){ //up key, decrement selectedIndex
        event.preventDefault();
        if($rootScope.selectedIndex-1 !== -1){
            $rootScope.selectedIndex--;
        }
    }else if(event.keyCode === 13){ //enter key, empty suggestions array
        event.preventDefault();
        $rootScope.suggestions = [];
    }
}

//Function To Call on ng-keyup
$rootScope.checkKeyUp = function(event){ 
    if(event.keyCode !== 8 || event.keyCode !== 46){//delete or backspace
        if ($scope.changePassword.FullName == "") {
            $rootScope.suggestions = [];
        }
    }
}


//List Item Events
//Function To Call on ng-click
$rootScope.AssignValueAndHide = function(index){
    $scope.changePassword.FullName = $rootScope.suggestions[index];
    $rootScope.suggestions=[];
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//UserName Auto Complete
$http.get("/Security/ChangePssword/LoadAllUserName")
.then(function (response) {
    $rootScope.searchItems1 = [];
    for (var i = 0; i < response.data.data.length; i++) {
        $rootScope.searchItems1.push(response.data.data[i].UserName);
    }
});

//Define Suggestions List
$rootScope.suggestions1 = [];
//Define Selected Suggestion Item
$rootScope.selectedIndex1 = -1;

//Function To Call On ng-change
$rootScope.searchUserName = function () {
$rootScope.suggestions1 = [];
var myMaxSuggestionListLength = 0;
for (var i = 0; i < $rootScope.searchItems1.length; i++) {
    var searchItemsSmallLetters = angular.lowercase($rootScope.searchItems1[i]);
    var searchTextSmallLetters = angular.lowercase($scope.changePassword.UserName);
    if (searchItemsSmallLetters.indexOf(searchTextSmallLetters) !== -1) {
        $rootScope.suggestions1.push(searchItemsSmallLetters);
        myMaxSuggestionListLength += 1;
        if (myMaxSuggestionListLength == 5) {
            break;
        }
    }
}
}

//Keep Track Of Search Text Value During The Selection From The Suggestions List  
$rootScope.$watch('selectedIndex1', function (val) {
if (val !== -1) {
    $scope.changePassword.UserName = $rootScope.suggestions1[$rootScope.selectedIndex1];
}
});


//Text Field Events
//Function To Call on ng-keydown
$rootScope.checkKeyDown = function (event) {
if (event.keyCode === 40) {//down key, increment selectedIndex
    event.preventDefault();
    if ($rootScope.selectedIndex1 + 1 !== $rootScope.suggestions1.length) {
        $rootScope.selectedIndex1++;
    }
} else if (event.keyCode === 38) { //up key, decrement selectedIndex
    event.preventDefault();
    if ($rootScope.selectedIndex1 - 1 !== -1) {
        $rootScope.selectedIndex1--;
    }
} else if (event.keyCode === 13) { //enter key, empty suggestions array
    event.preventDefault();
    $rootScope.suggestions1 = [];
}
}

//Function To Call on ng-keyup
$rootScope.checkKeyUp = function (event) {
if (event.keyCode !== 8 || event.keyCode !== 46) {//delete or backspace
    if ($scope.changePassword.UserName == "") {
        $rootScope.suggestions1 = [];
    }
}
}


//List Item Events
//Function To Call on ng-click
$rootScope.AssignValueAndHide1 = function (index) {
    $scope.changePassword.UserName = $rootScope.suggestions1[index];
    $rootScope.suggestions1 = [];
}


//Change Password By Admin Save
$scope.ChangePasswordByAdmin = function () {
    debugger;
    var userFulName = $scope.changePassword.FullName;
    var userName = $scope.changePassword.UserName;
    if (userFulName === "" || userName === "") {
        toastr.error("Please atleast one field between User or User Full Name");
        return;
    }
    if ($scope.changePassword.NewPassword.length < 6 || $scope.changePassword.NewPassword == "undefined") {
        toastr.error("Password would be minimum 6 character");
        return;
    }
    if ($scope.AdminChangePasswordForm.$valid) {
        $http({
            method: 'POST',
            url: '/Security/ChangePssword/PasswordChangeByAdminSave',
            data: $scope.changePassword
        }).success(function (data) {
            if (data.success) {
                toastr.success(data.message);
                $scope.changePassword = {};
            } else {
                if (data.message === "LogOut") {
                    $location.path('/LogIn');
                }
                toastr.error(data.message);
            }
        }).
        error(function (XMLHttpRequest, textStatus, errorThrown) {
            toastr.error(XMLHttpRequest + ": " + textStatus + ": " + errorThrown, 'Error!!!');
        });
    }
};



});

