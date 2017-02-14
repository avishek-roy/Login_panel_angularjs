angular.module('app').controller('SurveyMonitorController', ['$scope', '$http', '$compile', '$location', '$routeParams', '$templateCache', 'DTOptionsBuilder', 'DTColumnBuilder',
    function ($scope, $http, $compile, $location, $routeParams, $templateCache, DTOptionsBuilder, DTColumnBuilder) {

        $scope.survey = {};

        $scope.filterValueForNumberOnly = function ($event) {
            debugger;
            //var todate = $scope.survey.ToDate;
            //var a = moment($scope.survey.ToDate).format('dd-mm-yyyy');
            //alert(a);
            var key = window.$event ? $event.keyCode : $event.which;
            if (($event.charCode >= 48 && $event.charCode <= 57) || $event.charCode == 8 || $event.charCode == 46 || $event.keyCode == 8 || $event.keyCode == 46) {
             
            //    if (todate.length == 2) {
            //        if ($scope.survey.ToDate >31) {
            //            $event.preventDefault();
            //        }
            //        key = key + "-";
            //    }
            //    if (todate.length == 5) {
            //        if ($scope.survey.ToDate > 12) {
            //            $event.preventDefault();
            //        }
            //        $scope.survey.ToDate = $scope.survey.ToDate + "-";
            //    }
            //    if (todate.length > 10) {
            //        if ($scope.survey.ToDate > 2100) {
            //            $event.preventDefault();
            //        }
            //        $event.preventDefault();
            //    }
            }
            else{
                $event.preventDefault();
            }
        }



        $scope.Reload = function () {
            var currentPageTemplate = $route.current.templateUrl;
            $templateCache.remove(currentPageTemplate);
            $route.reload();
        }

        $scope.LoadSurveyHistoryDropdowns = function () {
            $scope.LoadDropdowns();
        }

        $scope.LoadDropdowns = function () {
            $http.get('/ClientSurvey/SurveyComnucation/GetTraders').success(function (data) {
                if (data.success === true) {
                    $scope.TraderList = data.data;
                } else {
                    return;
                }
            });
        }

        $scope.Search = Search;
        $scope.status = false;
        $scope.isTableShown = false;
     

        function Search() {
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
            //if (new Date($scope.survey.FromDate) > new Date($scope.survey.ToDate)) {
            //    toastr.warning("ToDate must be greater than FromDate");
            //    return;
            //}
            $http({
                method: 'POST',
                url: '/ClientSurvey/SurveyComnucation/GetTraderIDForSession',
                data: $scope.survey
            }).success(function (data) {
                $scope.status = true;
                $http.get('/ClientSurvey/SurveyComnucation/LoadTraderInfo').success(function (data) {
                    if (data.success === true) {
                        $scope.user = data.data;
                        $scope.isTableShown = true;
                        $scope.survey = {};
                        $scope.dtInstance = {};
                        $scope.dtColumns = [
                                             DTColumnBuilder.newColumn("DateShow").withOption('width', '8%'),
                                             DTColumnBuilder.newColumn("ContactPerson").withOption('width', '15%'),
                                             DTColumnBuilder.newColumn("ContactNo").withOption('width', '8%'),
                                             DTColumnBuilder.newColumn("Address").withOption('width', '18%'),
                                             DTColumnBuilder.newColumn("FeedBack").withOption('width', '22%'),
                                             DTColumnBuilder.newColumn("FollowUpDateShow").withOption('width', '10%'),
                                             DTColumnBuilder.newColumn("ComnucationType").withOption('width', '12%')
                        ]

                        $scope.dtOptions = DTOptionsBuilder.newOptions().withOption('ajax', {
                            url: "/ClientSurvey/SurveyComnucation/LoadSurveyHistoryById",
                            type: "POST"
                        }).withPaginationType('full_numbers')
                          .withOption('responsive', true)
                          .withDisplayLength(10);

                        function actionsHtml(data, type, full, meta) {
                            $scope.survey[data.ID] = data;
                            return '<button class="btn btn-success" ng-click="edit(' + data.ID + ')">' +
                               '   <i class="fa fa-edit"></i>' +
                               '</button>&nbsp;' +
                               '<button class="btn btn-danger" ng-click="deleteRow(' + data.ID + ')">' +
                               '   <i class="fa fa-trash-o"></i>' +
                               '</button>';
                        }
                    } else {
                        toastr.error(data.message);
                    }
                });

            });

            //Footer sorting
            $scope.Sort = function (index,order) {
                $('#entry-grid').dataTable().fnSort([index, order]);
            };


            $scope.sortOrder = false;
            $scope.sortData = function (column,status) {
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

            //$scope.LoadSessionData();
            //$scope.LoadTraderInfo();
        }

        $scope.destroy = function () {
            $scope.isTableShown = false;
        }

    }]);