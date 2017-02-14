
angular.module('app').controller('userGroupController', ['$scope', '$http', '$compile', '$location', '$routeParams', '$route', '$templateCache', 'DTOptionsBuilder', 'DTColumnBuilder',
    function ($scope, $http, $compile, $location, $routeParams, $route, $templateCache, DTOptionsBuilder, DTColumnBuilder, RowSelect) {

        $scope.Reload = function () {
            var currentPageTemplate = $route.current.templateUrl;
            $templateCache.remove(currentPageTemplate);
            $route.reload();
        }

        $scope.isTableShown = true;
        $scope.userGroup = {};
        $scope.edit = edit;
        $scope.deleteRow = deleteRow;
        $scope.dtInstance = {};
        $scope.dtColumns = [
                             DTColumnBuilder.newColumn("UserGroup").withOption('width', '70%'),
                             DTColumnBuilder.newColumn(null).withTitle('Is Admin').notSortable().renderWith(isAdminHtml).withOption('width', '10%'),
                             DTColumnBuilder.newColumn(null).withTitle('Edit/Delete').notSortable().renderWith(actionsHtml).withOption('width', '15%')
                           ]

        $scope.dtOptions = DTOptionsBuilder.newOptions().withOption('ajax', {
            url: "/Security/UserGroup/LoadAllUserGroup",
            type: "POST"
        }).withPaginationType('full_numbers').withOption('responsive', true)
       .withDisplayLength(10)
       .withOption('createdRow', createdRow);

        function isAdminHtml(data, type, full, meta) {
            $scope.userGroup[data.IsAdmin] = data;
            if (data.IsAdmin)
                return '<input type="checkbox" checked="checked" disabled readonly value=' + data.IsAdmin + '/>';
            else
                return '<input type="checkbox" disabled readonly value=' + data.IsAdmin + '/>';
        }

        function actionsHtml(data, type, full, meta) {
            $scope.userGroup[data.ID] = data;
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


        $scope.sortOrder = true;
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

        $scope.getSortClass = function (column, status) {
            if ($scope.sortColumn == column)
                return $scope.sortOrder ? 'glyphicon glyphicon-sort-by-attributes' : 'glyphicon glyphicon-sort-by-attributes-alt'
            return 'glyphicon glyphicon-sort';
        }
        //Footer sorting end


        function edit(id) {
            $scope.EditUI(id);
        }

        $scope.dtInstance = {};

        $scope.destroy = function () {
            $scope.isTableShown = false;
        }

        function deleteRow(id) {
            $scope.DeleteUI(id);
            // Then reload the data so that DT is refreshed
            //$scope.dtInstance.reloadData();
        }

        //Delete User Group
        $scope.DeleteUI = function (value) {
            $scope.destroy();
            $http.get('/Security/UserGroup/DeleteUserGroup/' + value)
                .success(function (data) {
                    if (data.success === true) {
                        toastr.success(data.message);
                        //$scope.LoadAllUserGroup();
                        $scope.refressGrid();
                    } else {
                        toastr.error(data.message);
                    }
                });
        };

        function createdRow(row, data, dataIndex) {
            // Recompiling so we can bind Angular directive to the DT
            $compile(angular.element(row).contents())($scope);
        }

        //DropDown load
        $scope.LoadDropdowns = function () {
            $scope.LoadLocations();
        }


        $scope.refressGrid = function () {
            $scope.isTableShown = true;
            $scope.userGroup = {};
            $scope.edit = edit;
            $scope.deleteRow = deleteRow;
            $scope.dtInstance = {};
            $scope.dtColumns = [
                                 DTColumnBuilder.newColumn("UserGroup").withOption('width', '70%'),
                                 DTColumnBuilder.newColumn(null).withTitle('Is Admin').notSortable().renderWith(isAdminHtml).withOption('width', '10%'),
                                 DTColumnBuilder.newColumn(null).withTitle('Edit/Delete').notSortable().renderWith(actionsHtml).withOption('width', '15%')
            ]

            $scope.dtOptions = DTOptionsBuilder.newOptions().withOption('ajax', {
                url: "/Security/UserGroup/LoadAllUserGroup",
                type: "POST"
            }).withPaginationType('full_numbers').withOption('responsive', true)
           .withDisplayLength(10)
           .withOption('createdRow', createdRow);

            function isAdminHtml(data, type, full, meta) {
                $scope.userGroup[data.IsAdmin] = data;
                if (data.IsAdmin)
                    return '<input type="checkbox" checked="checked" disabled readonly value=' + data.IsAdmin + '/>';
                else
                    return '<input type="checkbox" disabled readonly value=' + data.IsAdmin + '/>';
            }

            function actionsHtml(data, type, full, meta) {
                $scope.userGroup[data.ID] = data;
                return '<button class="btn btn-success" ng-click="edit(' + data.ID + ')">' +
                    '   <i class="fa fa-edit"></i>' +
                    '</button>&nbsp;' +
                    '<button class="btn btn-danger" ng-click="deleteRow(' + data.ID + ')">' +
                    '   <i class="fa fa-trash-o"></i>' +
                    '</button>';
            }
        }

       

        //CreateUserGroup
        $scope.CreateUserGroup = function () {
            //if ($scope.CreateUserGroupForm.$valid) {
                $http({
                    method: 'POST',
                    url: '/Security/UserGroup/CreateUserGroupSave',
                    data: {
                            userGroup: $scope.userGroup,
                            userMappingVM: $scope.pageMapping
                         }
                    })
                    .success(function(data) {
                        if (data.success) {
                            toastr.success(data.message);
                            $scope.userGroup = {};
                            $scope.pageMapping = [];
                            $scope.userGroupCreate = {};
                            $scope.selected = {};
                        } else {
                            if (data.message === "LogOut") {
                                $location.path('/LogIn');
                            }
                            toastr.error(data.message);
                        }
                    })
                    .error(function(XMLHttpRequest, textStatus, errorThrown) {
                        toastr.error(XMLHttpRequest + ": " + textStatus + ": " + errorThrown, 'Error!!!');
                    });
            //}
        };

        //Edit Location
        $scope.EditUI = function(value) {
            $location.path('/EditUserGroup/' + value);
        };

        //Edit dropdown's data Load
        $scope.LoadDropdownsAndUserGroup = function() {
            //$scope.LoadCompanys();
            //$scope.LoadLocations();
            $scope.LoadUserGroupById();
        }


        //Edit data load sub function
        $scope.LoadUserGroupById = function() {
            $http.get('/Security/UserGroup/LoadUserGroupById/' + $routeParams.id)
                .success(function(data) {
                    if (data.success === true) {
                        $scope.editUserGroup = data.data;
                        //$scope.loadMappingTableForEdit($routeParams.id);
                        //$scope.LoadCompanyDropDownById($scope.editUserGroup.CompanyID);
                        //$scope.LoadLocationDropDownById($scope.editUserGroup.LocationID);
                    } else {
                        toastr.error(data.message);
                    }
                });
        }


        //Update UserGroup
        $scope.UpdateUserGroup = function () {
            if ($scope.UpdateUserGroupForm.$valid) {
                $http({
                        method: 'POST',
                        url: '/Security/UserGroup/EditUserGroupSave',
                        data: {
                            userGroup: $scope.editUserGroup,
                            userMappingVM: $scope.pageMappingForEdit
                        }
                        //data: $scope.editUserGroup
                    })
                    .success(function(data) {
                        if (data.success) {
                            toastr.success(data.message);
                            $location.path('/UaerGroup');
                        } else {
                            if (data.message === "LogOut") {
                                $location.path('/LogIn');
                            }
                            toastr.error(data.message);
                        }
                    })
                    .error(function(XMLHttpRequest, textStatus, errorThrown) {
                        toastr.error(XMLHttpRequest + ": " + textStatus + ": " + errorThrown, 'Error!!!');
                    });
            }
        };

        
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        $scope.userGroupCreate = {};
        $scope.selectAll = false;
        $scope.toggleOne = toggleOne;
        $scope.selected = {};

        $scope.dtColumnsCreate =
            [
                DTColumnBuilder.newColumn("Application").withTitle('Application'),
                DTColumnBuilder.newColumn("Module").withTitle('Module'),
                DTColumnBuilder.newColumn("UIPage").withTitle('UI Page'),
                DTColumnBuilder.newColumn(null).withTitle('Select').notSortable().renderWith(selectHtml),
                DTColumnBuilder.newColumn(null).withTitle('Create').notSortable().renderWith(createHtml),
                DTColumnBuilder.newColumn(null).withTitle('Edit').notSortable().renderWith(editHtml),
                DTColumnBuilder.newColumn(null).withTitle('Delete').notSortable().renderWith(deleteHtml)
                //DTColumnBuilder.newColumn(null).withTitle('').notSortable().renderWith(hideIDHtml).withOption('width', '0%'),
                //DTColumnBuilder.newColumn(null).withTitle('').notSortable().renderWith(applicationIDHtml).withOption('width', '0%'),
                //DTColumnBuilder.newColumn(null).withTitle('').notSortable().renderWith(moduleIDHtml).withOption('width', '0%')
            ];


        $scope.dtOptionsCreate = DTOptionsBuilder.newOptions().withOption('ajax', {
            url: "/Security/UserGroup/LoadPages",
            type: "POST"
        }).withPaginationType('full_numbers')
          .withOption('responsive', true)
          .withDisplayLength(10)
          .withOption('createdRow', createdRow);


        function hideIDHtml(data, type, full, meta) {
            $scope.userGroup[data.ID] = data;
            return '<input type="text" ng-show="0" class="ng-hide" value=' + data.ID + '/>';
        }

        function applicationIDHtml(data, type, full, meta) {
            $scope.userGroup[data.ApplicationID] = data;
            return '<input type="text" ng-show="0" class="ng-hide" value=' + data.ApplicationID + '/>';
        }

        function moduleIDHtml(data, type, full, meta) {
            $scope.userGroup[data.ModuleID] = data;
            return '<input type="text" ng-show="0" class="ng-hide" value=' + data.ModuleID + '/>';
        }

        function selectHtml(data, type, full, meta) {
            $scope.selected[full.id] = false;
            var tag = 1;
            return '<input type="checkbox" ng-model="selected[' + data.ID + "9000" + tag + ']" ng-click="toggleOne(selected ' + ',' + data.ApplicationID + ',' + data.ModuleID + ',' + tag + ')" />';
        }

        function createHtml(data, type, full, meta) {
            $scope.selected[full.id] = false;
            var tag = 3;
            return '<input type="checkbox" ng-model="selected[' + data.ID + "9000" + tag + ']" ng-click="toggleOne(selected ' + ',' + data.ApplicationID + ',' + data.ModuleID + ',' + tag + ')" />';
        }

        function editHtml(data, type, full, meta) {
            $scope.selected[full.id] = false;
            var tag = 2;
            return '<input type="checkbox" ng-model="selected[' + data.ID + "9000" + tag + ']" ng-click="toggleOne(selected ' + ',' + data.ApplicationID + ',' + data.ModuleID + ',' + tag + ')" />';
        }

        function deleteHtml(data, type, full, meta) {
            var tag = 4;
            return '<input type="checkbox" ng-model="selected[' + data.ID +"9000"+tag+ ']" ng-click="toggleOne(selected ' + ',' + data.ApplicationID + ',' + data.ModuleID + ',' + tag + ')" />';
        }

        $scope.pageMapping = [];

        function toggleOne(selectedItems, application, moduleID, tag)
        {
            $scope.pageMapping = [];
            debugger;
            for (var id in selectedItems)
            {
                if (selectedItems.hasOwnProperty(id) && id != "undefined")
                {
                    var pageID = [];
                    pageID = id.split('9000');
                    $scope.pageMapping.push({
                        UserGroupId: 0,
                        ID: pageID[0],
                        ApplicationID: application,
                        ModuleID: moduleID,
                        Select: pageID[1] === "1" ? selectedItems[id] : null,
                        Edit: pageID[1] === "2" ? selectedItems[id] : null,
                        Create: pageID[1] === "3" ? selectedItems[id] : null,
                        Delete: pageID[1] === "4" ? selectedItems[id] : null
                    });
                }
            }
        }

        $scope.toggleOneEdit = toggleOneEdit;
        $scope.selectedEdit = {};
        $scope.pageMappingForEdit = [];
        $scope.userGroupEdit = {};
        $scope.dtColumnsEdit =
            [
                DTColumnBuilder.newColumn("Application").withTitle('Application'),
                DTColumnBuilder.newColumn("Module").withTitle('Module'),
                DTColumnBuilder.newColumn("UIPage").withTitle('UI Page'),
                DTColumnBuilder.newColumn(null).withTitle('Select').notSortable().renderWith(selectEditHtml),
                DTColumnBuilder.newColumn(null).withTitle('Create').notSortable().renderWith(createEditHtml),
                DTColumnBuilder.newColumn(null).withTitle('Edit').notSortable().renderWith(editEditHtml),
                DTColumnBuilder.newColumn(null).withTitle('Delete').notSortable().renderWith(deleteEditHtml)
                //DTColumnBuilder.newColumn(null).withTitle('').notSortable().renderWith(hideEditIDHtml).withOption('width', '0%'),
                //DTColumnBuilder.newColumn(null).withTitle('').notSortable().renderWith(applicationEditIDHtml).withOption('width', '0%'),
                //DTColumnBuilder.newColumn(null).withTitle('').notSortable().renderWith(moduleEditIDHtml).withOption('width', '0%')
            ];

        $scope.dtOptionsEdit = DTOptionsBuilder.newOptions().withOption('ajax', {
            url: "/Security/UserGroup/LoadMappingDataForEdit",
            type: "POST"
        }).withPaginationType('full_numbers')
          .withOption('responsive', true)
          .withDisplayLength(10)
          .withOption('createdRow', createdRow);

        //$scope.dtOptionsEdit = DTOptionsBuilder.newOptions()
        //    .withOption('ajax', {
        //        url: "/Security/UserGroup/LoadMappingDataForEdit",
        //        type: "POST"
        //    })
        //    //.withOption($scope.loadMappingTableForEdit)
        //    .withPaginationType('full_numbers')
        //    .withOption('responsive', true)
        //    .withDisplayLength(10)
        //    .withOption('createdRow', createdRow);

        function hideEditIDHtml(data, type, full, meta) {
            $scope.userGroup[data.ID] = data;
            return '<input type="text" ng-show="0" class="ng-hide" value=' + data.ID + '/>';
        }

        function applicationEditIDHtml(data, type, full, meta) {
            $scope.userGroup[data.ApplicationID] = data;
            return '<input type="text" ng-show="0" class="ng-hide" value=' + data.ApplicationID + '/>';
        }

        function moduleEditIDHtml(data, type, full, meta) {
            $scope.userGroup[data.ModuleID] = data;
            return '<input type="text" ng-show="0" class="ng-hide" value=' + data.ModuleID + '/>';
        }

        function selectEditHtml(data, type, full, meta) {
            debugger;
            $scope.selectedEdit[full.id] = false;
            var tag = 1;
            $scope.userGroupEdit[data.Select] = data;
            if (data.Select === true)
                return '<input type="checkbox" ng-click="toggleOneEdit(' + data.ID + tag + ',' + data.ID + ',' + tag + ',' + data.Select + ')" checked="checked" value=' + data.Select + '/>';
            else
                return '<input type="checkbox" ng-click="toggleOneEdit(' + data.ID + tag + ',' + data.ID + ',' + tag + ',' + data.Select + ')" value=' + data.Select + '/>';

        }
        
        function createEditHtml(data, type, full, meta) {
            debugger;
            $scope.selected[full.id] = false;
            var tag = 3;
            $scope.userGroupEdit[data.Create] = data;
            if (data.Create === true)
                return '<input type="checkbox" ng-click="toggleOneEdit(' + data.ID + tag + ',' + data.ID + ',' + tag + ',' + data.Create + ')" checked="checked" value=' + data.Create + '/>';
            else
                return '<input type="checkbox" ng-click="toggleOneEdit(' + data.ID + tag + ',' + data.ID + ',' + tag + ',' + data.Create + ')" value=' + data.Create + '/>';
        }

        function editEditHtml(data, type, full, meta) {
            debugger;
            $scope.selected[full.id] = false;
            var tag = 2;
            $scope.userGroupEdit[data.Edit] = data;
            if (data.Edit === true)
                return '<input type="checkbox" ng-click="toggleOneEdit(' + data.ID + tag + ',' + data.ID + ',' + tag + ',' + data.Edit + ')" checked="checked" value=' + data.Edit + '/>';
            else
                return '<input type="checkbox" ng-click="toggleOneEdit(' + data.ID + tag + ',' + data.ID + ',' + tag + ',' + data.Edit + ')" value=' + data.Edit + '/>';
        }

        function deleteEditHtml(data, type, full, meta) {
            debugger;
            var tag = 4;
            $scope.userGroupEdit[data.Delete] = data;
            if (data.Delete === true)
                return '<input type="checkbox" ng-click="toggleOneEdit(' + data.ID + tag + ',' + data.ID + ',' + tag + ',' + data.Delete + ')" checked="checked" value=' + data.Delete + '/>';
            else
                return '<input type="checkbox" ng-click="toggleOneEdit(' + data.ID + tag + ',' + data.ID + ',' + tag + ',' + data.Delete + ')" value=' + data.Delete + '/>';
        }

        function toggleOneEdit( rowID, id, tag, action)
        {
            debugger;
            var status = false;
            if ($scope.pageMappingForEdit.length != 0) {
                for (var i = 0; i <= ($scope.pageMappingForEdit.length - 1); i++)
                {
                    if (rowID === $scope.pageMappingForEdit[i].Application)
                    {
                        $scope.pageMappingForEdit[i].Select = tag === 1 ?  ($scope.pageMappingForEdit[i].Select == true ? false : true) : null;
                        $scope.pageMappingForEdit[i].Edit = tag === 2 ? ($scope.pageMappingForEdit[i].Select == true ? false : true) : null;
                        $scope.pageMappingForEdit[i].Create = tag === 3 ? ($scope.pageMappingForEdit[i].Select == true ? false : true) : null;
                        $scope.pageMappingForEdit[i].Delete = tag === 4 ? ($scope.pageMappingForEdit[i].Select == true ? false : true) : null;
                        status = true;
                    }
                }
                if (!status)
                {
                    $scope.pageMappingForEdit.push({
                        Application: rowID,
                        ID: id,
                        Select: tag === 1 ? (action == true ? false : true) : null,
                        Edit: tag === 2 ? (action == true ? false : true) : null,
                        Create: tag === 3 ? (action == true ? false : true) : null,
                        Delete: tag === 4 ? (action == true ? false : true) : null,
                    });
                }
            } else {
                $scope.pageMappingForEdit.push({
                    Application: rowID,
                    ID: id,
                    Select: tag === 1 ? (action == true ? false : true) : null,
                    Edit: tag === 2 ? (action == true ? false : true) : null,
                    Create: tag === 3 ? (action == true ? false : true) : null,
                    Delete: tag === 4 ? (action == true ? false : true) : null,
                });
            }
        }

}]);


