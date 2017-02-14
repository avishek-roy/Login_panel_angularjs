angular.module('app').controller('SurveyController', ['$scope','$rootScope', '$http', '$compile', '$location', '$routeParams', '$templateCache', '$filter', '$route', 'DTOptionsBuilder', 'DTColumnBuilder',
    function ($scope,$rootScope, $http, $compile, $location, $routeParams, $templateCache,$filter,$route, DTOptionsBuilder, DTColumnBuilder) {

        $scope.Reload = function () {
            var currentPageTemplate = $route.current.templateUrl;
            $templateCache.remove(currentPageTemplate);
            $route.reload();
        }

        //$scope.dateMask = function ($event) {
        //    debugger;
        //    var key = window.$event ? $event.keyCode : $event.which;
        //    if (($event.charCode >= 48 && $event.charCode <= 57) || $event.charCode == 8 || $event.charCode == 46 || $event.keyCode == 8 || $event.keyCode == 46 || $event.keyCode == 173) {
        //        return true;
        //    } else {
        //        return false;
        //    }
        //}

///////////////////////// Code for Index Page /////
        $scope.isTableShown = true;
        $scope.survey = {};
        $scope.edit = edit;
        $scope.deleteRow = deleteRow;
        $scope.dtInstance = {};
        $scope.dtColumns = [
                             DTColumnBuilder.newColumn("DateShow").withOption('width', '6%').withOption('Center'),
                             DTColumnBuilder.newColumn("ContactPerson").withOption('width', '12%'),
                             DTColumnBuilder.newColumn("ContactNo").withOption('width', '10%'),
                             DTColumnBuilder.newColumn("Address").withOption('width', '15%'),
                             DTColumnBuilder.newColumn("FeedBack").withOption('width', '20%'),
                             DTColumnBuilder.newColumn("FollowUpDateShow").withOption('width', '10%'),
                             DTColumnBuilder.newColumn("Type").withOption('width', '10%'),
                             DTColumnBuilder.newColumn(null).withTitle('Edit/Delete').notSortable().renderWith(actionsHtml).withOption('width', '10%')
                           ]

        $scope.dtOptions = DTOptionsBuilder.newOptions().withOption('ajax', {
            url: "/ClientSurvey/SurveyComnucation/LoadPersonByComnucation",
            type: "POST"
        }).withPaginationType('full_numbers')
          .withOption('responsive', true)
          .withDisplayLength(10)
          .withOption('createdRow', createdRow);


        function actionsHtml(data, type, full, meta) {
            $scope.survey[data.ID] = data;
            return '<button class="btn btn-success" ng-click="edit(' + data.ID + ')">' +
               '   <i class="fa fa-edit"></i>' +
               '</button>&nbsp;' +
               '<button class="btn btn-danger" ng-click="deleteRow(' + data.ID + ')">' +
               '   <i class="fa fa-trash-o"></i>' +
               '</button>';
        }

        function createdRow(row, data, dataIndex) {
            // Recompiling so we can bind Angular directive to the DT
            $compile(angular.element(row).contents())($scope);
        }


        $scope.destroy = function () {
            $scope.isTableShown = false;
        }

 ////////////////////////// Code For Delete  //////////////////////////////
        //Delete data save
        function deleteRow(id) {
            $scope.destroy();
            $http.get('/ClientSurvey/SurveyComnucation/DeleteSurvey/' + id).success(function (data) {
                if (data.success === true) {
                    toastr.success(data.message);
                    $scope.refressGrid();
                } else {
                    toastr.error(data.message);
                }
            });
        };


        $scope.refressGrid = function () {
            $scope.isTableShown = true;
            $scope.survey = {};
            $scope.edit = edit;
            $scope.deleteRow = deleteRow;
            $scope.dtInstance = {};
            $scope.dtColumns = [
                             DTColumnBuilder.newColumn("DateShow").withOption('width', '6%').withOption('Center'),
                             DTColumnBuilder.newColumn("ContactPerson").withOption('width', '12%'),
                             DTColumnBuilder.newColumn("ContactNo").withOption('width', '10%'),
                             DTColumnBuilder.newColumn("Address").withOption('width', '15%'),
                             DTColumnBuilder.newColumn("FeedBack").withOption('width', '20%'),
                             DTColumnBuilder.newColumn("FollowUpDateShow").withOption('width', '10%'),
                             DTColumnBuilder.newColumn("Type").withOption('width', '10%'),
                             DTColumnBuilder.newColumn(null).withTitle('Edit/Delete').notSortable().renderWith(actionsHtml).withOption('width', '10%')
            ]

            $scope.dtOptions = DTOptionsBuilder.newOptions().withOption('ajax', {
                url: "/ClientSurvey/SurveyComnucation/LoadPersonByComnucation",
                type: "POST"
            }).withPaginationType('full_numbers')
              .withOption('responsive', true)
              .withDisplayLength(10)
              .withOption('createdRow', createdRow);


            function actionsHtml(data, type, full, meta) {
                $scope.survey[data.ID] = data;
                return '<button class="btn btn-success" ng-click="edit(' + data.ID + ')">' +
                   '   <i class="fa fa-edit"></i>' +
                   '</button>&nbsp;' +
                   '<button class="btn btn-danger" ng-click="deleteRow(' + data.ID + ')">' +
                   '   <i class="fa fa-trash-o"></i>' +
                   '</button>';
            }

            function createdRow(row, data, dataIndex) {
                // Recompiling so we can bind Angular directive to the DT
                $compile(angular.element(row).contents())($scope);
            }
        }

        //Footer sorting
        $scope.Sort = function (index, order) {
            $('#entry-grid').dataTable().fnSort([index, order]);
        };


        $scope.sortOrder = false;
        $scope.sortData = function (column, status) {
            $scope.sortOrder = ($scope.sortColumn == column) ? !$scope.sortOrder : false
            $scope.sortColumn = column;
            $scope.Sort(column, $scope.GetSortOrder(column, status));
            //$scope.getSortClass(column);
        }
        $scope.sortColumn = 0;
        $scope.GetSortOrder = function (column, status) {
            if ($scope.sortColumn == column) {
                if (status === 1) {
                    return $scope.sortOrder ? 'asc' : 'desc'
                }
                else {
                    return $scope.sortOrder ? 'desc' : 'asc'
                }
            } else {
                if (status === 1) {
                    return 'asc';
                }
                else {
                    return 'desc';
                }
            }
        }

        $scope.getSortClass = function (column) {
            if ($scope.sortColumn == column)
                return $scope.sortOrder ? 'glyphicon glyphicon-sort-by-attributes' : 'glyphicon glyphicon-sort-by-attributes-alt'
            return 'glyphicon glyphicon-sort';
        }
        //Footer sorting end


//////////////////////// Code  for Edit /////////////////////////////////////////////
        function edit(id) {
            $scope.EditUI(id);
        }

        $scope.EditUI = function (value) {
            $location.path('/EditSurvay/' + value);
        };

        //Edit data Load
        $scope.LoadDropdownsAndLocation = function () {
            $scope.LoadDropdowns();
            $scope.LoadSurveyDataById();
        }


        //Edit data load sub function
        $scope.LoadSurveyDataById = function () {
            $http.get('/ClientSurvey/SurveyComnucation/LoadSurveyDataById/' + $routeParams.id).success(function (data) {
                if (data.success === true) {
                    $scope.survey = data.data;
                    $scope.survey.FollowUpDate = new Date($filter('date')(($scope.survey.FollowUpDate.split('/Date(')[1]).split(')/')[0], "MM/dd/yyyy"));
                    $scope.survey.CommunicationDate = new Date($filter('date')(($scope.survey.CommunicationDate.split('/Date(')[1]).split(')/')[0], "MM/dd/yyyy"));
                } else {
                    toastr.error(data.message);
                }
            });
        }


        //Edit data save
        $scope.UpdateSurvey = function () {
            if ($scope.SurveyEditForm.$valid) {
                $http({
                    method: 'POST',
                    url: '/ClientSurvey/SurveyComnucation/EditSurveySave',
                    data: $scope.survey
                }).success(function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        $location.path('/SurveyComnucation');
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


////////////////////////////////////////////////////////////////////// Code For Save Data /////////// 
        $scope.filterValueForNumberOnly = function ($event) {
            if (isNaN(String.fromCharCode($event.keyCode))) {
                $event.preventDefault();
            }
        };

        //Dropdown's data Load in create
        $scope.LoadDropdowns = function () {
            $scope.LoadTypes();
        }

        //load location dropdown
        $scope.LoadTypes = function () {
            $http.get('/ClientSurvey/SurveyComnucation/GetTypes').success(function (data) {
                if (data.success === true) {
                    $scope.ComnucationTypeList = data.data;
                } else {
                    return;
                }
            });
        };


        $scope.SaveServayEntry = function () {
            if ($scope.SurveyEntryForm.$valid) {
                $http({
                    method: 'POST',
                    url: '/ClientSurvey/SurveyComnucation/ClientSurveySave', 
                    data: $scope.survey
                }).success(function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                    } else {
                        if (data.message === "LogOut") {
                            $location.path('/LogIn');
                        }
                        toastr.error(data.message);
                    }
                }).error(function (XMLHttpRequest, textStatus, errorThrown) {
                    toastr.error(XMLHttpRequest + ": " + textStatus + ": " + errorThrown, 'Error!!!');
                });
            }
        };

    }]);