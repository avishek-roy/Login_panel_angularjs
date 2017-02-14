using BOE.Areas.Security.Models;
using BOEService.Entites.BOE;
using BOEService.Factories;
using BOEService.Interfaces;
using BOEService.Models;
using BOEUtility.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Expressions;

namespace BOE.Areas.Security.Controllers
{
    public class UserGroupController : Controller
    {
        private IGenericFactory<TBLB_LOCATION> _locationFactory;
        private IGenericFactory<TBLA_USER_GROUP> _userGroupFactory;
        private IGenericFactory<TBLA_USER_INFORMATION> _userFactory;
        private IGenericFactory<TBLB_COMPANY> _companyFactory;
        private IGenericFactory<TBLA_USER_ACTION_MAPPING> _userActionMappingFactory;
        private IGenericFactory<TBLA_UI_PAGE> _uiPageFactory;

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns>View</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <Date>19-Aug-2016</Date>
        public ActionResult Index()
        {
            try
            {
                Dictionary<int, CheckSessionData> dictionary = CheckSessionData.GetSessionValues();
                long companyId = Convert.ToInt32(dictionary[1].Id == "" ? 0 : Convert.ToInt32(dictionary[1].Id));
                int userGroupID = Convert.ToInt32(dictionary[6].Id == "" ? 0 : Convert.ToInt32(dictionary[6].Id));
                if (companyId != 0)
                {
                    ISecurityFactory _securityLogInFactory = new SecurityFactorys();
                    PagePermissionVM tblUserActionMapping = _securityLogInFactory.GetCRUDPermission(userGroupID, "UserGroup");
                    if (tblUserActionMapping.Select == true)
                    {
                        return View();
                    }
                }
                Session["logInSession"] = "false";
                return Redirect("/#/");
            }
            catch (Exception)
            {
                Session["logInSession"] = "false";
                return Redirect("/#/");
            }
        }

        /// <summary>
        /// Loads all user group.
        /// </summary>
        /// <returns>List</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <Date>19-Aug-2016</Date>
        public ActionResult LoadAllUserGroup()
        {
            try
            {
                Dictionary<int, CheckSessionData> dictionary = CheckSessionData.GetSessionValues();
                long companyId = Convert.ToInt32(dictionary[1].Id == ""? 0:Convert.ToInt32(dictionary[1].Id));
                if (companyId != 0)
                {
                    _userGroupFactory = new userGroupFactory();
                    //var userGroup = _userGroupFactory.FindBy(x => x.CompanyID == companyId).Select(x => new
                    var userGroup = _userGroupFactory.GetAll().Select(x => new
                    {
                        x.ID,
                        UserGroup = x.Name,
                        Company = x.TBLB_COMPANY.Name,
                        Location = x.TBLB_LOCATION.Name,
                        IsAdmin = x.IsAdmin
                    }).ToList();
                    return Json(userGroup.OrderBy(x=>x.UserGroup).ToList());
                }
                return Json(new { success = false, message = "LogOut" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                return Json(new { success = false, message = exception.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// Creates the user group.
        /// </summary>
        /// <returns>View</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <Date>19-Aug-2016</Date>
        public ActionResult CreateUserGroup() 
        {
            Dictionary<int, CheckSessionData> dictionary = CheckSessionData.GetSessionValues();
            long companyId = Convert.ToInt32(dictionary[1].Id == ""? 0:Convert.ToInt32(dictionary[1].Id));
            int userGroupID = Convert.ToInt32(dictionary[6].Id == "" ? 0 : Convert.ToInt32(dictionary[6].Id));
            if (companyId != 0)
            {
                ISecurityFactory _securityLogInFactory = new SecurityFactorys();
                PagePermissionVM tblUserActionMapping = _securityLogInFactory.GetCRUDPermission(userGroupID, "UserGroup");
                if (tblUserActionMapping.Create) 
                {
                    return View();
                }
            }
            Session["logInSession"] = "false";
            return Redirect("/#/");
        }


        /// <summary>
        /// Creates the user group save.
        /// </summary>
        /// <param name="userGroup">The user group.</param>
        /// <returns>Bool</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <Date>19-Aug-2016</Date>
        /// <ModificationDate>26-Sep-2016</ModificationDate>
        public JsonResult CreateUserGroupSave(TBLA_USER_GROUP userGroup, List<UserMappingVM> userMappingVM = null) 
        {
            try
            {
                Dictionary<int, CheckSessionData> dictionary = CheckSessionData.GetSessionValues();
                int companyId = Convert.ToInt32(dictionary[1].Id == "" ? 0 : Convert.ToInt32(dictionary[1].Id));
                int locationId = Convert.ToInt32(dictionary[2].Id == "" ? 0 : Convert.ToInt32(dictionary[2].Id));
                Guid userId = Guid.Parse(dictionary[3].Id);
                if (companyId != 0)
                {
                    _userGroupFactory = new userGroupFactory();
                    userGroup.CreatedDate = DateTime.Now;
                    userGroup.CreatedBy = userId;
                    userGroup.CompanyID = companyId;
                    userGroup.LocationID = locationId;
                    _userGroupFactory.Add(userGroup);
                    bool isDuplicate = _userGroupFactory.FindBy(x => x.Name.ToLower() == userGroup.Name.ToLower()).Any(x => x.Name.ToLower() == userGroup.Name.ToLower());
                    if (!isDuplicate)
                    {
                        _userGroupFactory.Save();
                        _uiPageFactory = new uiPageFactory();
                        _userActionMappingFactory = new userActionMappingFactory();
                        List<TBLA_UI_PAGE> uiPageList = _uiPageFactory.FindBy(x => x.CompanyID == companyId).ToList();
                        foreach (var item in uiPageList) 
                        {
                            TBLA_USER_ACTION_MAPPING userMappings = new TBLA_USER_ACTION_MAPPING();
                            userMappings.UserGroupID = Convert.ToInt32(userGroup.ID);
                            userMappings.ApplicationID = item.ApplicationID;
                            userMappings.UIModuleID = item.ModuleID;
                            userMappings.UIPageID = item.ID;
                            userMappings.IsSelect = false;
                            userMappings.IsCreate = false;
                            userMappings.IsEdit = false;
                            userMappings.IsDelete = false;
                            userMappings.CreatedDate = DateTime.Now;
                            userMappings.CreatedBy = userId;
                            userMappings.LocationID = locationId; 
                            userMappings.CompanyID = companyId;
                            _userActionMappingFactory.Add(userMappings);
                            _userActionMappingFactory.Save();
                        }

                        if (userMappingVM != null) 
                        {
                            int userGroupID = Convert.ToInt32(userGroup.ID);
                            List<TBLA_USER_ACTION_MAPPING> userMappingList = _userActionMappingFactory.FindBy(x => x.UserGroupID == userGroupID).ToList();

                            IEnumerable<UserMappingVM> userMappingVMList = userMappingVM.Distinct().ToList();

                            foreach (var item in userMappingVMList)
                            {
                                TBLA_USER_ACTION_MAPPING userMapping = new TBLA_USER_ACTION_MAPPING();
                                userMapping = userMappingList.Where(x => x.UIPageID == item.ID).FirstOrDefault(); 
                                userMapping.IsSelect = (item.Select == "True" || item.Select == "False") ? (item.Select == "True" ? true : false) : userMapping.IsSelect;
                                userMapping.IsCreate = (item.Create == "True" || item.Create == "False") ? (item.Create == "True" ? true : false) : userMapping.IsCreate;
                                userMapping.IsEdit = (item.Edit == "True" || item.Edit == "False") ? (item.Edit == "True" ? true : false) : userMapping.IsEdit;
                                userMapping.IsDelete = (item.Delete == "True" || item.Delete == "False") ? (item.Delete == "True" ? true : false) : userMapping.IsDelete;
                                _userActionMappingFactory.Edit(userMapping);
                                _userActionMappingFactory.Save();
                            }
                        }
                       
                        return Json(new { success = true, message = "Data Saved successfully" }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { success = false, message = "You'r entared code are duplicate" }, JsonRequestBehavior.AllowGet);
                }
                Session["logInSession"] = "false";
                return Json(new { success = false, message = "LogOut" }, JsonRequestBehavior.AllowGet);
            }
            
            catch (Exception exception)
            {
                return Json(new { success = false, message = exception.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Deletes the user group.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Bool</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <Date>19-Aug-2016</Date>
        public JsonResult DeleteUserGroup(int id)
        {
            try
            {
                Dictionary<int, CheckSessionData> dictionary = CheckSessionData.GetSessionValues();
                long companyId = Convert.ToInt32(dictionary[1].Id == ""? 0:Convert.ToInt32(dictionary[1].Id));
                int userGroupID = Convert.ToInt32(dictionary[6].Id == "" ? 0 : Convert.ToInt32(dictionary[6].Id));
                if (companyId != 0)
                {
                   ISecurityFactory _securityLogInFactory = new SecurityFactorys();
                   PagePermissionVM tblUserActionMapping = _securityLogInFactory.GetCRUDPermission(userGroupID, "UserGroup");
                   if (tblUserActionMapping.Delete)
                   {
                       _userFactory = new userFactory();
                       _userGroupFactory = new userGroupFactory();
                       int countUser = _userFactory.FindBy(x => x.UserGroupID == id).Count();
                       if (countUser == 0)
                       {
                           _userActionMappingFactory = new userActionMappingFactory();
                           _userActionMappingFactory.Delete(x => x.UserGroupID == id);
                           _userActionMappingFactory.Save();
                           _userGroupFactory.Delete(x => x.ID == id);
                           _userGroupFactory.Save();
                           return Json(new { success = true, message = "Sucessifuly deleted the User Group" }, JsonRequestBehavior.AllowGet);
                       }
                       return Json(new { success = false, message = "You cant delete this another one use this User Group" }, JsonRequestBehavior.AllowGet);
                   }
                   return Json(new { success = false, message = "You are not permitted for this action" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = false, message = "LogOut" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
               return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// Edits the user group.
        /// </summary>
        /// <returns>View</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <Date>19-Aug-2016</Date>
        public ActionResult EditUserGroup()
        {
            Dictionary<int, CheckSessionData> dictionary = CheckSessionData.GetSessionValues();
            long companyId = Convert.ToInt32(dictionary[1].Id == ""? 0:Convert.ToInt32(dictionary[1].Id));
            int userGroupID = Convert.ToInt32(dictionary[6].Id == "" ? 0 : Convert.ToInt32(dictionary[6].Id));
            if (companyId != 0)
            {
                ISecurityFactory _securityLogInFactory = new SecurityFactorys();
                PagePermissionVM tblUserActionMapping = _securityLogInFactory.GetCRUDPermission(userGroupID, "UserGroup");
                if (tblUserActionMapping.Edit)
                {
                    return View();
                }
            }
            Session["logInSession"] = "false";
            return Redirect("/#/");
        }

        /// <summary>
        /// Gets all locations.
        /// </summary>
        /// <returns>List</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <Date>19-Aug-2016</Date>
        /// <ModifiedDate>21-Aug-2016<ModifiedDate>
        /// <ModifiedAction>CommentOut<ModifiedAction>
        //public JsonResult GetAllLocations()
        //{
        //    try
        //    {
        //        Dictionary<int, CheckSessionData> dictionary = CheckSessionData.GetSessionValues();
        //        long companyId = Convert.ToInt32(dictionary[1].Id == ""? 0:Convert.ToInt32(dictionary[1].Id));
        //        if (companyId != 0)
        //        {
        //            _locationFactory = new LocationFactory();
        //            var locations = _locationFactory.FindBy(x => x.CompanyID == companyId).Select(x => new
        //            {
        //                x.ID,
        //                x.Name
        //            }).ToList();
        //            return Json(new { success = true, data = locations.OrderBy(x => x.Name) }, JsonRequestBehavior.AllowGet);
        //        }
        //        return Json(new { success = false, message = "LogOut"}, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception exception)
        //    {
        //        return Json(new { success = false, message = exception.Message }, JsonRequestBehavior.AllowGet);
        //    }
        //}


        /// <summary>
        /// Gets all companys.
        /// </summary>
        /// <returns>List</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <Date>19-Aug-2016</Date>
        /// <ModifiedDate>21-Aug-2016<ModifiedDate>
        /// <ModifiedAction>CommentOut<ModifiedAction>
        //public JsonResult GetAllCompanys()
        //{
        //    try
        //    {
        //        _companyFactory = new CompanyFactory();
        //        var locations = _companyFactory.GetAll().Select(x => new
        //        {
        //            x.ID,
        //            x.Name
        //        }).ToList();
        //        return Json(new { success = true, data = locations.OrderBy(x => x.Name) }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception exception)
        //    {
        //        return Json(new { success = false, message = exception.Message }, JsonRequestBehavior.AllowGet);
        //    }
        //}


        /// <summary>
        /// Loads the user group by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Object</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <Date>19-Aug-2016</Date>
        public JsonResult LoadUserGroupById(int id)
        {
            try
            {
                Dictionary<int, CheckSessionData> dictionary = CheckSessionData.GetSessionValues();
                long companyId = Convert.ToInt32(dictionary[1].Id == ""? 0:Convert.ToInt32(dictionary[1].Id));
                if (companyId != 0)
                {
                    _userGroupFactory = new userGroupFactory();
                    var userGroup = _userGroupFactory.FindBy(x => x.ID == id).Select(a => new
                    {
                        a.ID,
                        a.Name,
                        a.CompanyID,
                        a.LocationID,
                        a.IsAdmin
                    }).FirstOrDefault();
                    Session["UserGroupID"] = null;
                    Session["UserGroupID"] = id;
                    return Json(new { success = true, data = userGroup }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = false, message = "LogOut" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                return Json(new { success = false, message = exception.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// Loads the location by location identifier.
        /// </summary>
        /// <param name="locationId">The location identifier.</param>
        /// <returns>Object</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <Date>19-Aug-2016</Date>
        /// <ModifiedDate>21-Aug-2016<ModifiedDate>
        /// <ModifiedAction>CommentOut<ModifiedAction>
        //public JsonResult LoadLocationByLocationId(int locationId)
        //{
        //    try
        //    {
        //        _locationFactory = new LocationFactory();
        //        var location = _locationFactory.FindBy(x => x.ID == locationId)
        //            .Select(a => new
        //            {
        //                LocationId = a.ID,
        //                a.Name
        //            }).ToList();

        //        return Json(new { success = true, data = location }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception exception)
        //    {
        //        return Json(new { success = false, message = exception.Message }, JsonRequestBehavior.AllowGet);
        //    }
        //}


        /// <summary>
        /// Loads the company by company identifier.
        /// </summary>
        /// <param name="companyId">The company identifier.</param>
        /// <returns>Object</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <Date>19-Aug-2016</Date>
        /// <ModifiedDate>21-Aug-2016<ModifiedDate>
        /// <ModifiedAction>CommentOut<ModifiedAction>
        //public JsonResult LoadCompanyByCompanyId(int companyId)
        //{
        //    try
        //    {
        //        _companyFactory = new CompanyFactory();
        //        var company = _companyFactory.FindBy(x => x.ID == companyId)
        //            .Select(a => new
        //            {
        //                CompanyId = a.ID,
        //                a.Name
        //            }).ToList();

        //        return Json(new { success = true, data = company }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception exception)
        //    {
        //        return Json(new { success = false, message = exception.Message }, JsonRequestBehavior.AllowGet);
        //    }
        //}


        /// <summary>
        /// Edits the user group save.
        /// </summary>
        /// <param name="userGroup">The user group.</param>
        /// <returns>Bool</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <Date>19-Aug-2016</Date>
        public JsonResult EditUserGroupSave(TBLA_USER_GROUP userGroup, List<UserMappingVM> userMappingVM = null)
        {
            try
            {
                Dictionary<int, CheckSessionData> dictionary = CheckSessionData.GetSessionValues();
                int companyId = Convert.ToInt32(dictionary[1].Id);
                int locationId = Convert.ToInt32(dictionary[2].Id == "" ? 0 : Convert.ToInt32(dictionary[2].Id));
                Guid userId = Guid.Parse(dictionary[3].Id);
                if (companyId != 0)
                {
                    _userGroupFactory = new userGroupFactory();
                    TBLA_USER_GROUP tblUserGroup = _userGroupFactory.FindBy(x => x.ID == userGroup.ID).FirstOrDefault();
                    if (tblUserGroup != null)
                    {
                        tblUserGroup.ModifiedBy = userId;
                        tblUserGroup.ModifiedDate = DateTime.Now;
                        tblUserGroup.Name = userGroup.Name;
                        tblUserGroup.CompanyID = userGroup.CompanyID;
                        tblUserGroup.LocationID = userGroup.LocationID;
                        tblUserGroup.IsAdmin = userGroup.IsAdmin;
                        _userGroupFactory.Edit(tblUserGroup);
                    }
                    bool isDuplicate = _userGroupFactory.FindBy(x => x.Name.ToLower() == userGroup.Name.ToLower()).Where(x => x.ID != userGroup.ID)
                                      .Any(x => x.Name.ToLower() == userGroup.Name.ToLower());
                    if (!isDuplicate)
                    {
                        _userGroupFactory.Save();
                        if (userMappingVM != null) 
                        {
                            _userActionMappingFactory = new userActionMappingFactory();
                            int userGroupID = Convert.ToInt32(userGroup.ID);
                            List<TBLA_USER_ACTION_MAPPING> userMappingList = _userActionMappingFactory.FindBy(x => x.UserGroupID == userGroupID).ToList();
                            foreach (var item in userMappingVM)
                            {
                                TBLA_USER_ACTION_MAPPING userMapping = userMappingList.Where(x => x.ID == item.ID).FirstOrDefault(); ;
                                userMapping.IsSelect = item.Select == null ? userMapping.IsSelect : item.Select == "True" ? true : false;
                                userMapping.IsCreate = item.Create == null ? userMapping.IsCreate : item.Create == "True" ? true : false;
                                userMapping.IsEdit = item.Edit == null ? userMapping.IsEdit : item.Edit == "True" ? true : false;
                                userMapping.IsDelete = item.Delete == null ? userMapping.IsDelete : item.Delete == "True" ? true : false; 
                                userMapping.ModifiedDate = DateTime.Now;
                                userMapping.ModifiedBy = userId;
                                userMapping.LocationID = locationId;
                                userMapping.CompanyID = companyId;
                                _userActionMappingFactory.Edit(userMapping);
                                _userActionMappingFactory.Save();
                            }
                            return Json(new { success = true, message = "Data Updated successfully" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    return Json(new { success = false, message = "You'r entared code are duplicate" }, JsonRequestBehavior.AllowGet);
                }
                Session["logInSession"] = "false";
                return Json(new { success = false, message = "LogOut" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                return Json(new { success = false, message = exception.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// Loads the pages.
        /// </summary>
        /// <returns>list</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <CreatedDate>26-Sep-2016</CreatedDate>
        public ActionResult LoadPages()
        {
            try
            {
                Dictionary<int, CheckSessionData> dictionary = CheckSessionData.GetSessionValues();
                long companyId = Convert.ToInt32(dictionary[1].Id == "" ? 0 : Convert.ToInt32(dictionary[1].Id));
                if (companyId != 0)
                {
                    _uiPageFactory = new uiPageFactory();
                    var userPage = _uiPageFactory.FindBy(x => x.CompanyID == companyId).Select(x => new
                    {
                        x.ID,
                        ApplicationID = x.TBLA_APPLICATION_MODULE.ID,
                        Application = x.TBLA_APPLICATION_MODULE.UIName,
                        ModuleID = x.TBLA_UI_MODULE.ID,
                        Module = x.TBLA_UI_MODULE.UIName,
                        UIPage = x.UIPageName,
                        Select = false,
                        Edit = false,
                        Create = false,
                        Delete = false
                    }).ToList();
                    return Json(userPage.OrderBy(x => x.Application));
                }
                return Json(new { success = false, message = "LogOut" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                return Json(new { success = false, message = exception.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        //public JsonResult LoadMappingDataForEdit(int id)
        public ActionResult LoadMappingDataForEdit()
        {
            try
            {
                int id = Convert.ToInt32(Session["UserGroupID"]);
                Dictionary<int, CheckSessionData> dictionary = CheckSessionData.GetSessionValues();
                long companyId = Convert.ToInt32(dictionary[1].Id == "" ? 0 : Convert.ToInt32(dictionary[1].Id));
                if (companyId != 0)
                {
                     _userActionMappingFactory = new userActionMappingFactory();
                     var userPagemapping = _userActionMappingFactory.FindBy(x => x.CompanyID == companyId && x.UserGroupID == id).Select(x => new
                    {
                        x.ID,
                        ApplicationID = x.TBLA_APPLICATION_MODULE.ID,
                        Application = x.TBLA_APPLICATION_MODULE.Name,
                        ModuleID = x.TBLA_UI_MODULE.ID,
                        Module = x.TBLA_UI_MODULE.Name,
                        UIPageID = x.TBLA_UI_PAGE.ID,
                        UIPage = x.TBLA_UI_PAGE.Name,
                        x.UserGroupID,
                        Select = x.IsSelect,
                        Edit = x.IsEdit,
                        Create = x.IsCreate,
                        Delete = x.IsDelete
                    }).ToList();
                     return Json(userPagemapping.OrderBy(x => x.Application));
                }
                return Json(new { success = false, message = "LogOut" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                return Json(new { success = false, message = exception.Message }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}