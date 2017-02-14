angular.module('app').controller('userController', ['$scope', '$http', '$compile', '$location', '$routeParams', '$route', '$templateCache', 'DTOptionsBuilder', 'DTColumnBuilder',
    function ($scope, $http, $compile, $location, $routeParams, $route,$templateCache, DTOptionsBuilder, DTColumnBuilder, RowSelect) {

        $scope.Reload = function () {
            var currentPageTemplate = $route.current.templateUrl;
            $templateCache.remove(currentPageTemplate);
            $route.reload();
        }

        //$scope.filterValueForNumberOnly = function ($event) {
        //    if (isNaN(String.fromCharCode($event.keyCode))) {
        //        $event.preventDefault();
        //    }
        //};

        //$scope.filterValueForNumberOnly = function ($event) {
        //    var key = window.$event ? $event.keyCode : $event.which;
        //    debugger;
        //    if (($event.charCode >= 48 && $event.charCode <= 57) || $event.charCode == 8 || $event.charCode == 46 || $event.keyCode == 8 || $event.keyCode == 46) {
        //        return true;
        //    }
        //    return false;
        //}

        $scope.isTableShown = true;
        $scope.user = {};
        $scope.edit = edit;
        $scope.deleteRow = deleteRow;
        $scope.dtInstance = {};
        $scope.toggleOne = toggleOne;
        $scope.selected = {};
        $scope.array = [];


        $scope.dtColumns = [
                             DTColumnBuilder.newColumn("UserFullName").withOption('width', '23%'),
                             DTColumnBuilder.newColumn("Address").withOption('width', '28%'),
                             DTColumnBuilder.newColumn("PhoneNo").withOption('width', '10%'),
                             DTColumnBuilder.newColumn("Location").withOption('width', '12%'),
                             DTColumnBuilder.newColumn("Group").withOption('width', '10%'),
                             DTColumnBuilder.newColumn(null).withTitle('Active').notSortable().renderWith(activeHtml).withOption('width', '5%'),
                             DTColumnBuilder.newColumn(null).withTitle('Edit/Delete').notSortable().renderWith(actionsHtml).withOption('width', '12%')
                             //DTColumnBuilder.newColumn(null).withTitle('').notSortable().renderWith(hideIDHtml).withOption('width', '0%')
        ]

        $scope.dtOptions = DTOptionsBuilder.newOptions().withOption('ajax', {
            url: "/Security/User/LoadAllUser",
            type: "POST"
        }).withPaginationType('full_numbers').withOption('responsive', true)
          .withDisplayLength(10)
          .withOption('createdRow', createdRow);

        function createdRow(row, data, dataIndex) {
            // Recompiling so we can bind Angular directive to the DT
            $compile(angular.element(row).contents())($scope);
        }

        //function hideIDHtml(data, type, full, meta) {
        //    $scope.user[data.User] = data;
        //    return '<input type="text" ng-show="0" class="ng-hide" value=\'' + data.UserName + '\'/>';
        //}

        function actionsHtml(data, type, full, meta) {
            $scope.user[data.User] = data;
            return '<button class="btn btn-success" ng-click="edit(\'' + data.ID + '\')">' +
                '   <i class="fa fa-edit"></i>' +
                '</button>&nbsp;' +
                '<button class="btn btn-danger" ng-click="deleteRow(\'' + data.ID + '\')">' +
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

        function edit(id)
        {
            $scope.EditUI(id);
        }

        function deleteRow(id) {
            $scope.destroy();
            $http.get('/Security/User/Delete/' + id).success(function (data) {
                if (data.success === true) {
                    toastr.success(data.message);
                    $scope.refressGrid();
                    $window.location.reload();
                } else {
                    toastr.error(data.message);
                    $window.location.reload();
                }
            });
        }

        function activeHtml(data, type, full, meta) {
            $scope.selected[full.id] = false;
            $scope.user[data.IsActive] = data;
            if (data.IsActive === true)
                return '<input type="checkbox" ng-click="toggleOne(\'' + data.ID + '\',' + data.IsActive + ')" checked="checked" value=' + data.IsActive + '/>';
            else
                return '<input type="checkbox" ng-click="toggleOne(\'' + data.ID + '\',' + data.IsActive + ')" value=' + data.IsActive + '/>';
        }


        function toggleOne( id, action ) {
            debugger;
            var status = false;
            if ($scope.array.length != 0) {
                for (var i = 0; i <= ($scope.array.length - 1) ; i++) {
                    if (id === $scope.array[i].ID) {
                        $scope.array[i].IsActive = $scope.array[i].IsActive == true ? false : true;
                        status = true;
                    }
                }
                if (!status) {
                    $scope.array.push({
                        ID: id,
                        IsActive: action == true ? false : true
                    });
                }
            } else {
                $scope.array.push({
                    ID: id,
                    IsActive: action == true ? false : true
                });
            }

            for (var i = 0; i <= ($scope.array.length - 1) ; i++) {
                if (id === $scope.array[i].ID) {
                    if ($scope.array[i].IsActive) {
                        $scope.Active(id);
                    }
                    else{
                        $scope.DeActive(id);
                    }
                }
            }
        }

        //Active User Group
        $scope.Active = function (value) {
            $http.get('/Security/User/ActiveUser/' + value).success(function (data) {
                if (data.success === true) {
                    toastr.success(data.message);
                    //$scope.loadAllUser();
                    $window.location.reload();
                } else {
                    toastr.error(data.message);
                    $window.location.reload();
                }
            });
        };

        //DeActive User
        $scope.DeActive = function (value) {
            $http.get('/Security/User/DeActiveUser/' + value).success(function (data) {
                if (data.success === true) {
                    toastr.success(data.message);
                    //$scope.loadAllUser();
                    $window.location.reload();
                } else {
                    toastr.error(data.message);
                    $window.location.reload();
                }
            });
        };

        $scope.destroy = function () {
            $scope.isTableShown = false;
        }

        $scope.refressGrid = function () {
            $scope.isTableShown = true;
            $scope.user = {};
            $scope.edit = edit;
            $scope.deleteRow = deleteRow;
            $scope.dtInstance = {};
            $scope.toggleOne = toggleOne;
            $scope.selected = {};
            $scope.array = [];


            $scope.dtColumns = [
                                 DTColumnBuilder.newColumn("UserFullName").withOption('width', '23%'),
                                 DTColumnBuilder.newColumn("Address").withOption('width', '28%'),
                                 DTColumnBuilder.newColumn("PhoneNo").withOption('width', '10%'),
                                 DTColumnBuilder.newColumn("Location").withOption('width', '12%'),
                                 DTColumnBuilder.newColumn("Group").withOption('width', '10%'),
                                 DTColumnBuilder.newColumn(null).withTitle('Active').notSortable().renderWith(activeHtml).withOption('width', '5%'),
                                 DTColumnBuilder.newColumn(null).withTitle('Edit/Delete').notSortable().renderWith(actionsHtml).withOption('width', '12%')
                                 //DTColumnBuilder.newColumn(null).withTitle('').notSortable().renderWith(hideIDHtml).withOption('width', '0%')
            ]

            $scope.dtOptions = DTOptionsBuilder.newOptions().withOption('ajax', {
                url: "/Security/User/LoadAllUser",
                type: "POST"
            }).withPaginationType('full_numbers').withOption('responsive', true)
              .withDisplayLength(10)
              .withOption('createdRow', createdRow);

            function createdRow(row, data, dataIndex) {
                // Recompiling so we can bind Angular directive to the DT
                $compile(angular.element(row).contents())($scope);
            }

            //function hideIDHtml(data, type, full, meta) {
            //    $scope.user[data.User] = data;
            //    return '<input type="text" ng-show="0" class="ng-hide" value=\'' + data.ID + '\'/>';
            //}

            function actionsHtml(data, type, full, meta) {
                $scope.user[data.User] = data;
                return '<button class="btn btn-success" ng-click="edit(\'' + data.ID + '\')">' +
                    '   <i class="fa fa-edit"></i>' +
                    '</button>&nbsp;' +
                    '<button class="btn btn-danger" ng-click="deleteRow(\'' + data.ID + '\')">' +
                    '   <i class="fa fa-trash-o"></i>' +
                    '</button>';
            }

            function edit(id) {
                debugger;
                $scope.EditUI(id);
            }

            function deleteRow(id) {
                $scope.destroy();
                $http.get('/Security/User/Delete/' + id).success(function (data) {
                    if (data.success === true) {
                        toastr.success(data.message);
                        $window.location.reload();
                    } else {
                        toastr.error(data.message);
                        $window.location.reload();
                    }
                });
            }

            function activeHtml(data, type, full, meta) {
                $scope.selected[full.id] = false;
                $scope.user[data.IsActive] = data;
                if (data.IsActive === true)
                    return '<input type="checkbox" ng-click="toggleOne(\'' + data.User + '\',' + data.IsActive + ')" checked="checked" value=' + data.IsActive + '/>';
                else
                    return '<input type="checkbox" ng-click="toggleOne(\'' + data.User + '\',' + data.IsActive + ')" value=' + data.IsActive + '/>';
            }


            function toggleOne(id, action) {
                debugger;
                var status = false;
                if ($scope.array.length != 0) {
                    for (var i = 0; i <= ($scope.array.length - 1) ; i++) {
                        if (id === $scope.array[i].ID) {
                            $scope.array[i].IsActive = $scope.array[i].IsActive == true ? false : true;
                            status = true;
                        }
                    }
                    if (!status) {
                        $scope.array.push({
                            ID: id,
                            IsActive: action == true ? false : true
                        });
                    }
                } else {
                    $scope.array.push({
                        ID: id,
                        IsActive: action == true ? false : true
                    });
                }

                for (var i = 0; i <= ($scope.array.length - 1) ; i++) {
                    if (id === $scope.array[i].ID) {
                        if ($scope.array[i].IsActive) {
                            $scope.Active(id);
                        }
                        else {
                            $scope.DeActive(id);
                        }
                    }
                }
            }

            //Active User Group
            $scope.Active = function (value) {
                $http.get('/Security/User/ActiveUser/' + value).success(function (data) {
                    if (data.success === true) {
                        toastr.success(data.message);
                        //$scope.loadAllUser();
                        $window.location.reload();
                    } else {
                        toastr.error(data.message);
                        $window.location.reload();
                    }
                });
            };

            //DeActive User
            $scope.DeActive = function (value) {
                $http.get('/Security/User/DeActiveUser/' + value).success(function (data) {
                    if (data.success === true) {
                        toastr.success(data.message);
                        //$scope.loadAllUser();
                        $window.location.reload();
                    } else {
                        toastr.error(data.message);
                        $window.location.reload();
                    }
                });
            };

        }

        //Dropdown's data Load in create
        $scope.LoadDropdowns = function () {
            $scope.LoadLocations();
            $scope.LoadUserGroup();
        }

        //load location dropdown
        $scope.LoadLocations = function () {
            $http.get('/Security/User/GetAllLocations').success(function (data) {
                if (data.success === true) {
                    $scope.LocationList = data.data;
                } else {
                    toastr.error(data.message);
                }
            });
        };

        //load location dropdown
        $scope.LoadUserGroup = function () {
            $http.get('/Security/User/GetAllUserGroups').success(function (data) {
                if (data.success === true) {
                    $scope.UserGroupList = data.data;
                } else {
                    toastr.error(data.message);
                }
            });
        };

        $scope.ShowTC = false;
        $scope.ShowBC = false;
        $scope.ShowEmp = true;

        $scope.CheckBoxChanged = function () {
            if ($scope.user.IsClient) {
                $scope.ShowTC = true;
                $scope.ShowBC = true;
                $scope.ShowEmp = false;
            } else {
                $scope.ShowTC = false;
                $scope.ShowBC = false;
                $scope.ShowEmp = true;
            }
        };

        $scope.CheckBoxChangedEdit = function () {
            if ($scope.editUser.IsClient) {
                $scope.ShowTC = true;
                $scope.ShowBC = true;
                $scope.ShowEmp = false;
            } else {
                $scope.ShowTC = false;
                $scope.ShowBC = false;
                $scope.ShowEmp = true;
            }
        };


        //Create User
        $scope.CreateUser = function () {
            debugger;
            var newPassword = $scope.user.Password;
            var confirmPassword = $scope.user.ConfirmPassword;
            var employeeCode = $scope.user.Code;
            var tradingCode = $scope.user.Trading;
            var boCode = $scope.user.BOCode;

            if (newPassword.length < 6 || confirmPassword < 6) {
                toastr.error("Your password will be minimum 6 character");
                return;
            }
            if ($scope.user.Password !== $scope.user.ConfirmPassword) {
                toastr.error("New and Confirm Password are not matched");
                return;
            }
            if ($scope.user.Checkbox) {
                if (tradingCode === "" && boCode === "") {
                    toastr.error("Your must be fill the Trading code and BO code");
                    $scope.user.Code = {};
                    return;
                }
            } else {
                if (employeeCode === "") {
                    toastr.error("Your must be fill the employee code");
                    $scope.user.Trading = {};
                    $scope.user.BOCode = {};
                    return;
                }
            }
            if ($scope.CreateUserForm.$valid) {
                $http({
                    method: 'POST',
                    url: '/Security/User/CreateUserSave',
                    data: $scope.user
                }).success(function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        $scope.user = {};
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

        //Edit User
        $scope.EditUI = function (value) {
            $location.path('/EditUser/' + value);
        };

        //Edit data Load
        $scope.LoadUserEditData = function () {
            //$scope.LoadUserGroups();
            $scope.LoadUserById();
            $scope.LoadLocations();
            $scope.LoadUserGroup();
            //$scope.LoadUserById();
        };



        //Edit data load sub function
        $scope.LoadUserById = function () {
            $http.get('/Security/User/LoadUserById/' + $routeParams.id)
                .success(function (data) {
                    if (data.success === true) {
                        $scope.editUser = data.data;
                    } else {
                        if (data.message === "LogOut") {
                            $location.path('/LogIn');
                        }
                        toastr.error(data.message);
                    }
                });
        };

        //Edit User
        $scope.EditUser = function () {
            var employeeCode = $scope.editUser.Code == "" ? null : $scope.editUser.Code;
            var tradingCode = $scope.editUser.Trading;
            var boCode = $scope.editUser.BOCode;
            if ($scope.editUser.Checkbox) {
                if (tradingCode === "" && boCode === "") {
                    toastr.error("Your must be fill the Trading code and BO code");
                    $scope.editUser.Code = {};
                    return;
                }
            } else {
                if (employeeCode === "") {
                    toastr.error("Your must be fill the employee code");
                    $scope.editUser.Trading = {};
                    $scope.editUser.BOCode = {};
                    return;
                }
            }
            //if ($scope.UpdateUserForm.$valid) {
                $http({
                    method: 'POST',
                    url: '/Security/User/UpdateUserForm',
                    data: $scope.editUser
                }).success(function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        $scope.editUser = {};
                        $location.path('/User');
                    } else {
                        if (data.message === "LogOut") {
                            $location.path('/LogIn');
                        }
                        toastr.error(data.message);
                    }
                }).error(function (XMLHttpRequest, textStatus, errorThrown) {
                    toastr.error(XMLHttpRequest + ": " + textStatus + ": " + errorThrown, 'Error!!!');
                });
            //}
        };



    }]);