//var app = angular.module('app');

//angular.module('app', ['ngTouch', 'ui.grid', 'ui.grid.pagination', 'ui.grid.edit', 'ngRoute', 'ngResource', 'datatables', 'datatables.fixedcolumns' ], function ($routeProvider) {

angular.module('app', ['ngTouch', 'ngRoute', 'ngResource', 'datatables', 'datatables.fixedcolumns', 'moment-picker'], function ($routeProvider) {
    //angular.module('app', ['ngTouch', 'ngRoute', 'datatables'], function ($routeProvider) {
    $routeProvider.when('/', {
        templateUrl: '/Home/Index'
    }).when('/Home', {
        templateUrl: '/Home/Index'
    }).when('/Locations', {
        templateUrl : '/Security/Location/Index'
    }).when('/CreateLocation', {
        templateUrl: '/Security/Location/CreateLocation'
    }).when('/EditLocation/:id', {
        templateUrl: '/Security/Location/EditLocation'
    }).when('/SelfChabgePassword', {
        templateUrl: '/Security/ChangePssword/ChangePassword'
    }).when('/AdminChangePassword', {
        templateUrl: '/Security/ChangePssword/PasswordChangeByAdmin'
    }).when('/UaerGroup', {
        templateUrl: '/Security/UserGroup/Index'
    }).when('/CreateUserGroup', {
        templateUrl: '/Security/UserGroup/CreateUserGroup'
    }).when('/EditUserGroup/:id', {
        templateUrl: '/Security/UserGroup/EditUserGroup'
    }).when('/User', {
        templateUrl: '/Security/User/Index'
    }).when('/CreateUser', {
        templateUrl: '/Security/User/CreateUser'
    }).when('/EditUser/:id', {
        templateUrl: '/Security/User/EditUser'
    }).when('/LogIn', {
        templateUrl: '/Account/Login'
    }).when('/LogOut', {
        templateUrl: '/Account/LogOff'
    }).when('/SurveyComnucation', {
        templateUrl: '/ClientSurvey/SurveyComnucation/Index'
    }).when('/CreateSurvey', {
        templateUrl: '/ClientSurvey/SurveyComnucation/CreateSurvey'
    }).when('/EditSurvay/:id', {
        templateUrl: '/ClientSurvey/SurveyComnucation/EditSurvay'
    }).when('/SurveyStatusMonitor', {
        templateUrl: '/ClientSurvey/SurveyComnucation/SurveyStatusMonitor'
    }).when('/SurveyDeposit', {
        templateUrl: '/ClientSurvey/TraderDeposit/Index'
    }).when('/CreateSurveyDeposite', {
        templateUrl: '/ClientSurvey/TraderDeposit/CreateSurveyDeposit'
    }).when('/EditSurveyDeposit/:id', {
        templateUrl: '/ClientSurvey/TraderDeposit/EditClientSurvey'
    }).when('/TraderPositionByDeposit', {
        templateUrl: '/ClientSurvey/TraderDeposit/TraderPositionByDeposit'
    })

    toastr.options = {
        "closeButton": true,
        "debug": true,
        "newestOnTop": true,
        "progressBar": true,
        "positionClass": "toast-bottom-right",
        "preventDuplicates": true,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": "5000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    }

}).directive('basicClick', function ($parse, $rootScope) {
    return {
        compile: function (elem, attr) {
            var fn = $parse(attr.basicClick);
            return function (scope, elem) {
                elem.on('click', function (e) {
                    fn(scope, { $event: e });
                    scope.$apply();
                });
            };
        }
    };
}).directive('number', function () {
    return {
        link: function (scope, el, attr) {
            el.bind("keydown keypress", function (event) {
                //ignore all characters that are not numbers, except backspace, delete, left arrow and right arrow
                if ((event.keyCode < 48 || event.keyCode > 57) && event.keyCode != 8 && event.keyCode != 46 && event.keyCode != 37 && event.keyCode != 39) {
                    event.preventDefault();
                }
            });
        }
    };
//}).directive('number', function () {
//    debugger;
//        return {
//            link: function (scope, el, attr) {
//                el.bind("keydown keypress", function (event) {
//                    //ignore all characters that are not numbers, except backspace, delete, left arrow and right arrow
//                    if ((event.charCode >= 48 && event.charCode <= 57) || event.charCode == 8 || event.charCode == 46 || event.keyCode == 8 || event.keyCode == 46) {

//                    }
//                    else {
//                        event.preventDefault();
//                    }
//                });
//            }
//        };
}).config(['momentPickerProvider', function (momentPickerProvider) {
    momentPickerProvider.options({
        /* Picker properties */
        locale: 'en',
        format: 'L LTS',
        minView: 'decade',
        maxView: 'minute',
        startView: 'month',
        autoclose: true,
        today: true,
        keyboard: true,

        /* Extra: Views properties */
        leftArrow: '&larr;',
        rightArrow: '&rarr;',
        yearsFormat: 'YYYY',
        monthsFormat: 'MMM',
        daysFormat: 'D',
        hoursFormat: 'HH:[00]',
        minutesFormat: moment.localeData().longDateFormat('LT').replace(/[aA]/, ''),
        secondsFormat: 'ss',
        minutesStep: 5,
        secondsStep: 1
    });
}]);

