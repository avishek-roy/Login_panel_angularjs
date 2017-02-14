
angular.module('app').controller('locationController', ['$scope', '$http', '$compile', '$location', '$routeParams', '$route', '$templateCache', 'DTOptionsBuilder', 'DTColumnBuilder',
    function ($scope, $http, $compile, $location, $routeParams, $route,$templateCache, DTOptionsBuilder, DTColumnBuilder) {


        $scope.Reload = function () {
            var currentPageTemplate = $route.current.templateUrl;
            $templateCache.remove(currentPageTemplate);
            $route.reload();
        }


        $scope.isTableShown = true;
        $scope.location = {};
        $scope.edit = edit;
        $scope.deleteRow = deleteRow;
        $scope.dtInstance = {};
        $scope.dtColumns = [
                             DTColumnBuilder.newColumn("Code").withOption('width', '10%'),
                             DTColumnBuilder.newColumn("Name").withOption('width', '30%'),
                             DTColumnBuilder.newColumn("PhoneNo").withOption('width', '10%'),
                             DTColumnBuilder.newColumn("Email").withOption('width', '20%'),
                             DTColumnBuilder.newColumn("ParentLocation").withOption('width', '20%'),
                             DTColumnBuilder.newColumn(null).withTitle('Edit/Delete').notSortable().renderWith(actionsHtml).withOption('width', '10%')
        ]


        $scope.dtOptions = DTOptionsBuilder.newOptions().withOption('ajax', {
            url: "/Security/Location/LoadAllLocation",
            type: "POST"
        }).withPaginationType('full_numbers').withOption('responsive', true)
          .withDisplayLength(10)
          .withOption('createdRow', createdRow);


        function actionsHtml(data, type, full, meta) {
            $scope.location[data.ID] = data;
            return '<button class="btn btn-success" ng-click="edit(' + data.ID + ')">' +
                '   <i class="fa fa-edit"></i>' +
                '</button>&nbsp;' +
                '<button class="btn btn-danger" ng-click="deleteRow(' + data.ID + ')">' +
                '   <i class="fa fa-trash-o"></i>' +
                '</button>';
        }


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

        function edit(id) {
            $scope.EditUI(id);
        }

        $scope.destroy = function () {
            $scope.isTableShown = false;
        }


        $scope.refressGrid = function () {
            $scope.isTableShown = true;
            $scope.toggleOne = toggleOne;
            $scope.selected = {};
            $scope.array = [];

            $scope.location = {};
            $scope.edit = edit;
            $scope.deleteRow = deleteRow;
            $scope.dtInstance = {};
            $scope.dtColumns = [
                                 DTColumnBuilder.newColumn("Code").withOption('width', '10%'),
                                 DTColumnBuilder.newColumn("Name").withOption('width', '30%'),
                                 DTColumnBuilder.newColumn("PhoneNo").withOption('width', '10%'),
                                 DTColumnBuilder.newColumn("Email").withOption('width', '20%'),
                                 DTColumnBuilder.newColumn("ParentLocation").withOption('width', '20%'),
                                 DTColumnBuilder.newColumn(null).withTitle('Edit/Delete').notSortable().renderWith(actionsHtml).withOption('width', '10%')
            ]


            $scope.dtOptions = DTOptionsBuilder.newOptions().withOption('ajax', {
                url: "/Security/Location/LoadAllLocation",
                type: "POST"
            }).withPaginationType('full_numbers').withOption('responsive', true)
              .withDisplayLength(10)
              .withOption('createdRow', createdRow);


            function actionsHtml(data, type, full, meta) {
                $scope.location[data.ID] = data;
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
        }


        //Delete data save
        function deleteRow(id) {
            $scope.destroy();
            $http.get('/Security/Location/DeleteLocation/' + id).success(function (data) {
                if (data.success === true) {
                    toastr.success(data.message);
                    $scope.refressGrid();
                    $window.location.reload();
                } else {
                    toastr.error(data.message);
                    $scope.refressGrid();
                    $window.location.reload();
                }
            });
        };

        function createdRow(row, data, dataIndex) {
            // Recompiling so we can bind Angular directive to the DT
            $compile(angular.element(row).contents())($scope);
        }

        //DropDown load
        $scope.LoadDropdowns = function() {
            $scope.LoadLocations();
        }

        $scope.LoadLocations = function () {
            $http.get('/Security/Location/GetAllParentLocations').success(function (data) {
                if (data.success === true) {
                    $scope.LocationList = data.data;
                } else {
                    toastr.error(data.message);
                }
            });
        };

        $scope.filterValueForNumberOnly = function ($event) {
            if (isNaN(String.fromCharCode($event.keyCode))) {
                $event.preventDefault();
            }
        };


        //Create Location
        $scope.CreateLocation = function () {
            if ($scope.CreateLocationForm.$valid) {
                $http({
                    method: 'POST',
                    url: '/Security/Location/CreateLocationSave',
                    data: $scope.location
                }).success(function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        $scope.location = {};
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


        //Edit Location
        $scope.EditUI = function (value) {
            $location.path('/EditLocation/' + value);
        };

        //Edit data Load
        $scope.LoadDropdownsAndLocation = function () {
            $scope.LoadLocations();
            $scope.LoadLocationById();
        }

        //Edit data load sub function
        $scope.LoadLocationById = function () {
            $http.get('/Security/Location/LoadLocationById/' + $routeParams.id).success(function (data) {
                if (data.success === true) {
                    $scope.editLocation = data.data;
                } else {
                    toastr.error(data.message);
                }
            });
        }


        //Edit data save
        $scope.UpdateLocation = function () {
            if ($scope.UpdateLocationForm.$valid) {
                $http({
                    method: 'POST',
                    url: '/Security/Location/EditLocationSave',
                    data: $scope.editLocation
                }).success(function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        $location.path('/Locations');
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

