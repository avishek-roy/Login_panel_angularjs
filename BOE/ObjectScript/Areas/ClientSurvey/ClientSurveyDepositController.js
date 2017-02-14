angular.module('app').controller('ClientSurveyDepositController', ['$scope', '$http', '$compile', '$location', '$routeParams','$route', '$templateCache', '$filter', 'DTOptionsBuilder', 'DTColumnBuilder',
    function ($scope, $http, $compile, $location, $routeParams,$route, $templateCache, $filter, DTOptionsBuilder, DTColumnBuilder) {

        $scope.Reload = function () {
            var currentPageTemplate = $route.current.templateUrl;
            $templateCache.remove(currentPageTemplate);
            $route.reload();
        }


///////////////////////////  Index Code //////////////////////////////////
        $scope.isTableShown = true;
        $scope.edit = edit;
        $scope.deleteRow = deleteRow;
        $scope.createdRow = createdRow;
        $scope.user = {};
        $scope.dtColumns = [
                                DTColumnBuilder.newColumn("Date").withOption('width', '10%'),
                                DTColumnBuilder.newColumn("DepositeNo").withOption('width', '26%'),
                                DTColumnBuilder.newColumn("Amount").withOption('width', '24%'),
                                DTColumnBuilder.newColumn("Type").withOption('width', '30%'),
                                DTColumnBuilder.newColumn(null).withTitle('Edit/Delete').notSortable().renderWith(actionsHtml).withOption('width', '10%')]

        function actionsHtml(data, type, full, meta) {
            $scope.user[data.User] = data;
            return '<button class="btn btn-success" ng-click="edit(\'' + data.ID + '\')">' +
                    '<i class="fa fa-edit"></i>' +
                    '</button>&nbsp;' +
                    '<button class="btn btn-danger" ng-click="deleteRow(\'' + data.ID + '\')">' +
                    '<i class="fa fa-trash-o"></i>' +
                    '</button>';
        }

        $scope.dtOptions = DTOptionsBuilder
            .newOptions().withOption('ajax', {
                url: "/ClientSurvey/TraderDeposit/LoadAllDeposit",
                type: "POST"
            })
            .withPaginationType('full_numbers')
            .withOption('responsive', true)
            .withOption('createdRow', createdRow);

        function createdRow(row, data, dataIndex) {
            // Recompiling so we can bind Angular directive to the DT
            $compile(angular.element(row).contents())($scope);
        }


        //Footer sorting
        $scope.Sort = function (index, order) {
            $('#entry-grid').dataTable().fnSort([index, order]);
        };


        $scope.sortOrder = false;
        $scope.sortColumn = 0;
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

        function edit(id) {
            $scope.EditUI(id);
        }


        $scope.destroy = function () {
            $scope.isTableShown = false;
        }

///////////////  Code for Delete ////////////////////////////
        function deleteRow(id) {
            $scope.destroy();
            $http.get('/ClientSurvey/TraderDeposit/Delete/' + id).success(function (data) {
                if (data.success === true) {
                    toastr.success(data.message);
                    //$scope.Search();
                    $scope.refressGrid();
                    debugger;
                } else {
                    toastr.error(data.message);
                    $scope.refressGrid();
                }
            });
        }

        $scope.refressGrid = function () {
            $scope.isTableShown = true;
            $scope.survey = {};
            $scope.edit = edit;
            $scope.deleteRow = deleteRow;
            $scope.dtInstance = {};
            $scope.dtColumns = [
                                DTColumnBuilder.newColumn("Date").withOption('width', '8%'),
                                DTColumnBuilder.newColumn("DepositeNo").withOption('width', '30%'),
                                DTColumnBuilder.newColumn("Amount").withOption('width', '20%'),
                                DTColumnBuilder.newColumn("Type").withOption('width', '30%'),
                                DTColumnBuilder.newColumn(null).withTitle('Edit/Delete').notSortable().renderWith(actionsHtml).withOption('width', '12%')
            ]

            $scope.dtOptions = DTOptionsBuilder
            .newOptions().withOption('ajax', {
                url: "/ClientSurvey/TraderDeposit/LoadAllDeposit",
                type: "POST"
            })
            .withPaginationType('full_numbers')
            .withOption('responsive', true)
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



        /////////////////////// Code For Create ////////////////////////////////////////
        $scope.filterValueForNumberOnly = function ($event) {
            if (isNaN(String.fromCharCode($event.keyCode))) {
                $event.preventDefault();
            }
        };


        //Dropdown's data Load in create
        $scope.LoadDropdowns = function () {
            $scope.LoadTypes();
        }


        $scope.LoadTypes = function () {
            $http.get('/ClientSurvey/TraderDeposit/GetTypes').success(function (data) {
                if (data.success === true) {
                    $scope.TypeList = data.data;
                } else {
                    toastr.error(data.message);
                }
            });
        };


        $scope.DepositeSave = function () {
            if ($scope.ClientSurveyDepositeForm.$valid) {
                $http({
                    method: 'POST',
                    url: '/ClientSurvey/TraderDeposit/ClientSurveySave',
                    data: $scope.surveyDeposite
                }).success(function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        $scope.destroy();
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
        /////////////////////// Code For Edit ////////////////////////

        $scope.LoadEditDropdowns = function () {
            $scope.LoadTypes();
            $scope.LoadSurveyDepositeByID();
        }

        $scope.EditUI = function (value) {
            $location.path('/EditSurveyDeposit/' + value);
        }

        $scope.LoadSurveyDepositeByID = function () {
            $http.get('/ClientSurvey/TraderDeposit/LoadLocationById/' + $routeParams.id).success(function (data) {
                if (data.success === true) {
                    $scope.surveyDeposite = data.data;
                    $scope.surveyDeposite.DepositeDate = new Date($filter('date')(($scope.surveyDeposite.DepositeDate.split('/Date(')[1]).split(')/')[0], "MM/dd/yyyy"));
                } else {
                    toastr.error(data.message);
                }
            });
        }

        //Edit data save
        $scope.UpdateSurveyDeposite = function () {
            if ($scope.ClientSurveyDepositeEditForm.$valid) {
                $http({
                    method: 'POST',
                    url: '/ClientSurvey/TraderDeposit/EditDepositeSave',
                    data: $scope.surveyDeposite
                }).success(function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        $location.path('/SurveyDeposit');
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


    }]);





