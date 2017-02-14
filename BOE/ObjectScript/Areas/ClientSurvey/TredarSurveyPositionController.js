
angular.module('app').controller('TredarSurveyPositionController', ['$scope', '$http', '$compile', '$location', '$routeParams', 'DTOptionsBuilder', 'DTColumnBuilder', 
    function ($scope, $http, $compile, $location, $routeParams, DTOptionsBuilder, DTColumnBuilder, RowSelect) {

        $scope.Reload = function () {
            var currentPageTemplate = $route.current.templateUrl;
            $templateCache.remove(currentPageTemplate);
            $route.reload();
        }

        //Dropdown's data Load in create
        $scope.LoadDropdowns = function () {
            $scope.LoadTypes();
        }

        //load location dropdown
        $scope.LoadTypes = function () {
            $http.get('/ClientSurvey/TraderDeposit/GetTrader').success(function (data) {
                if (data.success === true) {
                    $scope.TraderList = data.data;
                } else {
                    toastr.error(data.message);
                }
            });
        };


        $scope.Search = Search;
        $scope.isTableShown = false;
        $scope.status = false;

        //$scope.LoadUserID = function ()
        //{
        //    if (new Date($scope.survey.FromDate) > new Date($scope.survey.ToDate)) {
        //        toastr.warning("ToDate must be greater than FromDate");
        //        return;
        //    }
        //    if ($scope.TredarSurveyPositionForm.$valid) {
        //        $http({
        //            method: 'POST',
        //            url: '/ClientSurvey/SurvayPosition/GetTraderIDForossition',
        //            data: $scope.survey
        //        }).success(function (data) {
                     
        //            $scope.status = true;
        //        });
        //    }
        //}

        //$scope.LoadTraderInfo = function () {
        //    if (new Date($scope.survey.FromDate) > new Date($scope.survey.ToDate))
        //    {
        //        toastr.warning("ToDate must be greater than FromDate");
        //        return;
        //    }
        //    $http.get('/ClientSurvey/SurvayPosition/LoadTraderInfo').success(function (data) {
        //        if (data.success === true) {
        //            $scope.survey = data.data;
        //        } else {
        //            toastr.error(data.message);
        //        }
        //    });
        //}


        function Search()
        {
            debugger;
            var fromDate = $scope.survey.FromDate.split("-").reverse().join("-");
            var toDate = $scope.survey.ToDate.split("-").reverse().join("-");

            if (fromDate > toDate) {
                toastr.warning("ToDate must be greater than FromDate");
                return;
            }
            if ($scope.status == true) {
                $scope.isTableShown = false;
            }
            //$scope.LoadUserID();
            //if (new Date($scope.survey.FromDate) > new Date($scope.survey.ToDate)) {
            //    toastr.warning("ToDate must be greater than FromDate");
            //    return;
            //}
            if ($scope.TredarSurveyPositionForm.$valid) {
                $http({
                    method: 'POST',
                    url: '/ClientSurvey/TraderDeposit/GetTraderIDForossition',
                    data: $scope.survey
                }).success(function (data) {
                    $scope.status = true;
                    $http.get('/ClientSurvey/TraderDeposit/LoadTraderInfo').success(function (data) {
                        if (data.success === true) {
                            $scope.user = data.data;
                            $scope.isTableShown = true;
                            //$scope.user = {};
                            $scope.edit = edit;
                            $scope.deleteRow = deleteRow;
                            $scope.dtColumns = [
                                                    DTColumnBuilder.newColumn("Date").withOption('width', '8%'),
                                                    DTColumnBuilder.newColumn("DepositeNo").withOption('width', '30%'),
                                                    DTColumnBuilder.newColumn("Amount").withOption('width', '30%'),
                                                    DTColumnBuilder.newColumn("Type", "Client Type").withOption('width', '32%')
                                                    //DTColumnBuilder.newColumn(null).withTitle('Edit/Delete').notSortable().renderWith(actionsHtml).withOption('width', '12%')
                            ]

                            debugger;

                            $scope.dtOptions = DTOptionsBuilder
                                .newOptions().withOption('ajax', {
                                    url: '/ClientSurvey/TraderDeposit/LoadTrader',
                                    type: "POST"
                                }).withPaginationType('full_numbers')
                                  .withOption('responsive', true)
                                  .withDisplayLength(10)
                                  .withOption('createdRow', createdRow);


                            function createdRow(row, data, dataIndex) {
                                // Recompiling so we can bind Angular directive to the DT
                                $compile(angular.element(row).contents())($scope);
                            }

                            function actionsHtml(data, type, full, meta) {
                                $scope.user[data.User] = data;
                                return '<button class="btn btn-success" ng-click="edit(' + data.ID + ')">' +
                                                   '   <i class="fa fa-edit"></i>' +
                                                   '</button>&nbsp;' +
                                                   '<button class="btn btn-danger" ng-click="deleteRow(' + data.ID + ')">' +
                                                   '   <i class="fa fa-trash-o"></i>' +
                                                   '</button>';
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


                            function edit(id) {
                                $scope.EditUI(id);
                            }

                            function deleteRow(id) {
                                $http.get('/Security/TraderDeposit/Delete/' + id).success(function (data) {
                                    if (data.success === true) {
                                        toastr.success(data.message);
                                        $window.location.reload();
                                    } else {
                                        toastr.error(data.message);
                                        $window.location.reload();
                                    }
                                });
                            }
                        } else {
                            toastr.error(data.message);
                        }
                    });
                });
            }
            //$scope.LoadTraderInfo();
            if (new Date($scope.survey.FromDate) > new Date($scope.survey.ToDate)) {
                toastr.warning("ToDate must be greater than FromDate");
                return;
            }
        }

        $scope.destroy = function () {
            $scope.isTableShown = false;
       }
    }]);

    //app.directive('myDatepicker', function ($parse) {
    //    return function (scope, element, attrs, controller) {
    //        var ngModel = $parse(attrs.ngModel);
    //        $(function () {
    //            element.datepicker({
    //                changeYear: true,
    //                changeMonth: true,
    //                dateFormat: 'dd/mm/yy',
    //                onSelect: function (dateText, inst) {
    //                    scope.$apply(function (scope) {
    //                        // Change binded variable
    //                        ngModel.assign(scope, dateText);
    //                    });
    //                }
    //            });
    //        });
    //    }
    //});

