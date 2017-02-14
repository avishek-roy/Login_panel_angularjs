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

namespace BOE.Areas.Security.Controllers
{
    public class UserController : Controller
    {
        private IGenericFactory<TBLA_USER_INFORMATION> _userFactory;
        private IGenericFactory<TBLA_SECURITY_QUESTION> _questionFactory;
        private IGenericFactory<TBLA_PASSWORD> _passwordFactory;
        private IGenericFactory<TBLA_USER_GROUP> _userGroupFactory;
        private IGenericFactory<TBLB_LOCATION> _locationFactory;
        private IGenericFactory<TBLB_COMPANY> _companyFactory;

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns>View</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <Date>18-Sep-2016</Date>
        public ActionResult Index()
        {
            Dictionary<int, CheckSessionData> dictionary = CheckSessionData.GetSessionValues();
            long companyId = Convert.ToInt32(dictionary[1].Id == ""? 0:Convert.ToInt32(dictionary[1].Id));
            int userGroupID = Convert.ToInt32(dictionary[6].Id == "" ? 0 : Convert.ToInt32(dictionary[6].Id));
            if (companyId != 0)
            {
                ISecurityFactory _securityLogInFactory = new SecurityFactorys();
                PagePermissionVM tblUserActionMapping = _securityLogInFactory.GetCRUDPermission(userGroupID, "User");
                if (tblUserActionMapping.Select)
                {
                    return View();
                }
            }
            Session["logInSession"] = "false";
            return Redirect("/#/");
        }

        /// <summary>
        /// Loads all user.
        /// </summary>
        /// <returns>jsonList</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <Date>18-Sep-2016</Date>
        public ActionResult LoadAllUser()
        {
            try
            {
                Dictionary<int, CheckSessionData> dictionary = CheckSessionData.GetSessionValues();
                long companyId = Convert.ToInt32(dictionary[1].Id == ""? 0:Convert.ToInt32(dictionary[1].Id));
                if (companyId != 0)
                {
                    _userFactory = new userFactory();
                    var users = _userFactory.FindBy(x => x.CompanyID == companyId).Select(x => new
                    {
                        x.ID,
                        x.UserFullName,
                        x.Address,
                        x.PhoneNo,
                        Location = x.TBLB_LOCATION1.Name,
                        Group = x.TBLA_USER_GROUP2.Name,
                        //IsAdmin  = x.tblUserGroup.Name,
                        User = x.UserName,
                        x.IsActive
                    }).ToList();
                    return Json( users );
                }
                return Json(new { success = false, message = "LogOut" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                return Json(new { success = false, message = exception.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Deletes the user.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Bool</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <Date>18-Sep-2016</Date>
        public JsonResult DeActiveUser(Guid id)
        {
            try
            {
                Dictionary<int, CheckSessionData> dictionary = CheckSessionData.GetSessionValues();
                long companyId = Convert.ToInt32(dictionary[1].Id == "" ? 0 : Convert.ToInt32(dictionary[1].Id));
                int userGroupID = Convert.ToInt32(dictionary[6].Id == "" ? 0 : Convert.ToInt32(dictionary[6].Id));
                if (companyId != 0)
                { 
                    ISecurityFactory _securityLogInFactory = new SecurityFactorys();
                    PagePermissionVM tblUserActionMapping = _securityLogInFactory.GetCRUDPermission(userGroupID, "User");
                    if (tblUserActionMapping.Edit)
                    {
                        _userGroupFactory = new userGroupFactory();
                        _userFactory = new userFactory();
                        Guid userId = Guid.Parse(dictionary[3].Id);
                        TBLA_USER_INFORMATION user = _userFactory.FindBy(x => x.ID == userId).FirstOrDefault();
                        TBLA_USER_GROUP userGroup = _userGroupFactory.FindBy(x => x.ID == user.UserGroupID).FirstOrDefault();
                        if (userGroup != null && userGroup.IsAdmin)
                        {
                            _userFactory = new userFactory();
                            TBLA_USER_INFORMATION tblUserInformation = _userFactory.FindBy(x => x.ID == id).FirstOrDefault();
                            if (tblUserInformation != null)
                            {
                                tblUserInformation.IsActive = false;
                                _userFactory.Edit(tblUserInformation);
                            }
                            _userFactory.Save();
                            return Json(new { success = true, message = "Sucessifuly de-activeted the User Group" }, JsonRequestBehavior.AllowGet);
                        }
                        return Json(new { success = false, message = "You are not Admin User" }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { success = false, message = "You has no permission for edit" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = false, message = "LogOut" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult ActiveUser(Guid id)
        {
            try
            {
                Dictionary<int, CheckSessionData> dictionary = CheckSessionData.GetSessionValues();
                long companyId = Convert.ToInt32(dictionary[1].Id == "" ? 0 : Convert.ToInt32(dictionary[1].Id));
                int userGroupID = Convert.ToInt32(dictionary[6].Id == "" ? 0 : Convert.ToInt32(dictionary[6].Id));
                if (companyId != 0)
                {
                    ISecurityFactory _securityLogInFactory = new SecurityFactorys();
                    PagePermissionVM tblUserActionMapping = _securityLogInFactory.GetCRUDPermission(userGroupID, "User");
                    if (tblUserActionMapping.Edit)
                    {
                        _userGroupFactory = new userGroupFactory();
                        _userFactory = new userFactory();
                        Guid userId = Guid.Parse(dictionary[3].Id);
                        TBLA_USER_INFORMATION user = _userFactory.FindBy(x => x.ID == userId).FirstOrDefault();
                        TBLA_USER_GROUP userGroup = _userGroupFactory.FindBy(x => x.ID == user.UserGroupID).FirstOrDefault();
                        if (userGroup != null && userGroup.IsAdmin)
                        {
                            _userFactory = new userFactory();
                            TBLA_USER_INFORMATION tblUserInformation = _userFactory.FindBy(x => x.ID == id).FirstOrDefault();
                            if (tblUserInformation != null)
                            {
                                tblUserInformation.IsActive = true;
                                _userFactory.Edit(tblUserInformation);
                            }
                            _userFactory.Save();
                            return Json(new { success = true, message = "Sucessifuly activeted the User Group" }, JsonRequestBehavior.AllowGet);
                        }
                        return Json(new { success = false, message = "You are not Admin User" }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { success = false, message = "You has no permission for edit" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = false, message = "LogOut" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Creates the user.
        /// </summary>
        /// <returns>View</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <Date>18-Sep-2016</Date>
        public ActionResult CreateUser() 
        {
            Dictionary<int, CheckSessionData> dictionary = CheckSessionData.GetSessionValues();
            long companyId = Convert.ToInt32(dictionary[1].Id == "" ? 0 : Convert.ToInt32(dictionary[1].Id));
            int userGroupID = Convert.ToInt32(dictionary[6].Id == "" ? 0 : Convert.ToInt32(dictionary[6].Id));
            if (companyId != 0)
            {
                ISecurityFactory _securityLogInFactory = new SecurityFactorys();
                PagePermissionVM tblUserActionMapping = _securityLogInFactory.GetCRUDPermission(userGroupID, "User");
                if (tblUserActionMapping.Create)
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
        /// <Date>18-Sep-2016</Date>
        public JsonResult GetAllLocations()
        {
            try
            {
                Dictionary<int, CheckSessionData> dictionary = CheckSessionData.GetSessionValues();
                long companyId = Convert.ToInt32(dictionary[1].Id == "" ? 0 : Convert.ToInt32(dictionary[1].Id));
                if (companyId != 0)
                {
                    _locationFactory = new LocationFactory();
                    var locations = _locationFactory.FindBy(x=>x.CompanyID == companyId).Select(x => new
                    {
                        x.ID,
                        x.Name
                    }).ToList();
                    return Json(new { success = true, data = locations.OrderBy(x => x.Name) }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = false, message = "LogOut" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                return Json(new { success = false, message = exception.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        //public JsonResult GetAllCompanys()
        //{
        //    try
        //    {
        //        Dictionary<int, CheckSessionData> dictionary = CheckSessionData.GetSessionValues();
        //        long companyId = Convert.ToInt32(dictionary[1].Id == "" ? 0 : Convert.ToInt32(dictionary[1].Id));
        //        if (companyId != 0)
        //        {
        //            _companyFactory = new CompanyFactory();
        //            var companys = _companyFactory.GetAll().Select(x => new
        //            {
        //                x.ID,
        //                x.Name
        //            }).ToList();
        //            return Json(new { success = true, data = companys.OrderBy(x => x.Name) }, JsonRequestBehavior.AllowGet);
        //        }
        //        return Json(new { success = false, message = "LogOut" }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception exception)
        //    {
        //        return Json(new { success = false, message = exception.Message }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        /// <summary>
        /// Gets all user groups.
        /// </summary>
        /// <returns>List</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <Date>18-Sep-2016</Date>
        public JsonResult GetAllUserGroups()
        {
            try
            {
                _userGroupFactory = new userGroupFactory();
                var userGroups = _userGroupFactory.GetAll().Select(x => new
                {
                    x.ID,
                    x.Name
                }).ToList();
                return Json(new { success = true, data = userGroups.OrderBy(x => x.Name) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                return Json(new { success = false, message = exception.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Creates the user save.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>Bool</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <Date>18-Sep-2016</Date>
        public JsonResult CreateUserSave(UserVM user) 
        {
            try
            {
                Dictionary<int, CheckSessionData> dictionary = CheckSessionData.GetSessionValues();
                int companyId = Convert.ToInt32(dictionary[1].Id == "" ? 0 : Convert.ToInt32(dictionary[1].Id));
                int locationId = Convert.ToInt32(dictionary[2].Id == "" ? 0 : Convert.ToInt32(dictionary[2].Id));
                if (companyId != 0)
                {
                    Guid userId = Guid.Parse(dictionary[3].Id);
                    _userFactory = new userFactory();
                    TBLA_USER_INFORMATION isDuplicate = _userFactory.FindBy(x => x.UserName == user.UserName.ToLower().Trim()).FirstOrDefault();
                    if (isDuplicate == null)
                    {
                        return CreateUser(user, userId, companyId);
                    }
                    return Json(new { success = false, message = "Your entared user name are duplicate please chose another name" }, JsonRequestBehavior.AllowGet);
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
        /// Jsons the result.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="companyId">The company identifier.</param>
        /// <returns>Bool</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <Date>18-Sep-2016</Date>
        private JsonResult CreateUser(UserVM user, Guid userId, int companyId)
        {
            _questionFactory = new questionFactory();
            var question = new TBLA_SECURITY_QUESTION();
            question.ID = Guid.NewGuid();
            question.SecurityQuestion = user.SecurityQuestion;
            question.SecutiryAnswer = user.SecurityQueAns;
            question.CreatedBy = userId;
            question.CreatedDate = DateTime.Now;
            question.CompanyID = companyId;
            question.LocationID = user.LocationID;
            _questionFactory.Add(question);
            _questionFactory.Save();

            _passwordFactory = new userPasswordFactory();
            var password = new TBLA_PASSWORD();
            var encription = new Encription();
            password.ID = Guid.NewGuid();
            password.NewPassword = encription.Encrypt(user.Password);
            password.OldPassword = "";
            password.IsSelfChanged = false;
            password.CreatedBy = userId;
            password.CreatedDate = DateTime.Now;
            password.LocationID = user.LocationID;
            password.CompanyID = companyId;
            _passwordFactory.Add(password);
            _passwordFactory.Save();

            var userInformation = new TBLA_USER_INFORMATION();
            userInformation.ID = Guid.NewGuid();
            userInformation.UserFullName = user.UserFullName;
            userInformation.UserName = user.UserName.ToLower().Trim();
            userInformation.Address = user.Address;
            userInformation.Email = user.EMail;
            userInformation.PhoneNo = user.PhoneNo;
            userInformation.SecurityQuestionID = question.ID;
            userInformation.PasswordID = password.ID;
            userInformation.IsEMailVerified = false;
            userInformation.IsPhoneNoVerified = false;
            userInformation.IsActive = true;
            userInformation.CreatedBy = userId;
            userInformation.CreatedDate = DateTime.Now;
            userInformation.LocationID = user.LocationID;
            userInformation.UserGroupID = user.UserGroupID;
            userInformation.CompanyID = companyId;
            userInformation.IsClient = user.IsClient;
            if (user.IsClient)
            {
                userInformation.TradingCode = user.TradingCode;
                userInformation.BOCode = user.BOCode;
            }
            else
            {
                userInformation.EmployeeCode = user.Code;
            }
            _userFactory.Add(userInformation);
            _userFactory.Save();
            return Json(new {success = true, message = "Data Updated successfully"}, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Edits the user.
        /// </summary>
        /// <returns>View</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <Date>18-Sep-2016</Date>
        public ActionResult EditUser() 
        {
            Dictionary<int, CheckSessionData> dictionary = CheckSessionData.GetSessionValues();
            long companyId = Convert.ToInt32(dictionary[1].Id == "" ? 0 : Convert.ToInt32(dictionary[1].Id));
            int userGroupID = Convert.ToInt32(dictionary[6].Id == "" ? 0 : Convert.ToInt32(dictionary[6].Id));
            if (companyId != 0)
            {
                ISecurityFactory _securityLogInFactory = new SecurityFactorys();
                PagePermissionVM tblUserActionMapping = _securityLogInFactory.GetCRUDPermission(userGroupID, "User");
                if (tblUserActionMapping.Edit)
                {
                    return View();
                }
            }
            Session["logInSession"] = "false";
            return Redirect("/#/");
        }

        /// <summary>
        /// Loads the user by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>List</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <Date>18-Sep-2016</Date>
        public JsonResult LoadUserById(Guid id) 
        {
            try
            {
                Dictionary<int, CheckSessionData> dictionary = CheckSessionData.GetSessionValues();
                long companyId = Convert.ToInt32(dictionary[1].Id == "" ? 0 : Convert.ToInt32(dictionary[1].Id));
                if (companyId != 0)
                {
                    _userGroupFactory = new userGroupFactory();
                    _userFactory = new userFactory();
                    Guid userId = Guid.Parse(dictionary[3].Id);
                    TBLA_USER_INFORMATION user = _userFactory.FindBy(x => x.ID == userId).FirstOrDefault();
                    TBLA_USER_GROUP userGroup = _userGroupFactory.FindBy(x => x.ID == user.UserGroupID).FirstOrDefault();
                    _questionFactory = new questionFactory();
                    TBLA_SECURITY_QUESTION securityQuestion = _questionFactory.FindBy(x => x.ID == user.SecurityQuestionID).FirstOrDefault();
                    if (userGroup != null && (userGroup.IsAdmin || userId == id))
                    {
                        _userFactory = new userFactory();
                        var editUser = _userFactory.FindBy(x => x.ID == id).Select(a => new
                        {
                            a.ID,
                            a.UserFullName,
                            a.UserName,
                            a.Address,
                            a.PhoneNo,
                            a.Email,
                            a.LocationID,
                            a.UserGroupID,
                            BOCode = a.BOCode??"",
                            Code = a.EmployeeCode??"",
                            TradingCode = a.TradingCode ?? "",
                            a.IsClient
                            //securityQuestion.SecurityQuestion
                            //SecurityQueAns = securityQuestion.SecutiryAnswer
                        }).FirstOrDefault();
                        return Json(new { success = true, data = editUser }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { success = false, message = "You are not a Admin or same user. So, you cant see another information" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = false, message = "LogOut" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                return Json(new { success = false, message = exception.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Updates the user form.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>19-Sep-2015</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <CreatedDate>25-Sep-2016</CreatedDate>
        public JsonResult UpdateUserForm(UserVM user)
        {
            try
            {
                Dictionary<int, CheckSessionData> dictionary = CheckSessionData.GetSessionValues();
                int companyId = Convert.ToInt32(dictionary[1].Id == "" ? 0 : Convert.ToInt32(dictionary[1].Id));
                int locationId = Convert.ToInt32(dictionary[2].Id == "" ? 0 : Convert.ToInt32(dictionary[2].Id));
                if (companyId != 0)
                {
                    Guid userId = Guid.Parse(dictionary[3].Id);
                    _userFactory = new userFactory();
                    TBLA_USER_INFORMATION aUserInformation = _userFactory.FindBy(x => x.UserName == user.UserName.ToLower().Trim()).Where(x=>x.ID != user.ID).FirstOrDefault();
                    if (aUserInformation == null)
                    {
                        TBLA_USER_INFORMATION userInformation = _userFactory.FindBy(x => x.ID == user.ID).FirstOrDefault();
                        if (userInformation != null)
                        {
                            userInformation.LocationID = user.LocationID;
                            userInformation.ModifiedDate = DateTime.Now;
                            userInformation.ModifiedBy = userId;
                            userInformation.Email = user.EMail;
                            userInformation.PhoneNo = user.PhoneNo;
                            userInformation.BOCode = user.BOCode;
                            userInformation.TradingCode = user.TradingCode;
                            userInformation.UserGroupID = user.UserGroupID;
                            userInformation.EmployeeCode = user.Code;
                            userInformation.UserFullName = user.UserFullName;
                            userInformation.UserName = user.UserName;
                            userInformation.IsClient = user.IsClient;
                            _userFactory.Edit(userInformation);
                        }
                        _userFactory.Save();

                        _questionFactory = new questionFactory();
                        TBLA_SECURITY_QUESTION securityQuestion =_questionFactory.FindBy(x => x.ID == user.SecurityQuestionID).FirstOrDefault();
                        if (securityQuestion != null && (securityQuestion.SecurityQuestion == user.SecurityQuestion || securityQuestion.SecutiryAnswer == user.SecurityQueAns))
                        {
                            securityQuestion.SecurityQuestion = user.SecurityQuestion;
                            securityQuestion.SecutiryAnswer = user.SecurityQueAns;
                            securityQuestion.LocationID = locationId;
                            securityQuestion.ModifiedDate = DateTime.Now;
                            securityQuestion.ModifiedBy = userId;
                            _questionFactory.Edit(securityQuestion);
                            _questionFactory.Save();
                        }
                        return Json(new { success = true, message = "Update Successfully" }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { success = false, message = "Your entared user name are duplicate please chose another name" }, JsonRequestBehavior.AllowGet);
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
        /// Loads the user groups.
        /// </summary>
        /// <returns>list</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <CreatedDate>25-Sep-2016</CreatedDate>
        public JsonResult LoadUserGroups()
        {
            try
            {
                Dictionary<int, CheckSessionData> dictionary = CheckSessionData.GetSessionValues();
                long companyId = Convert.ToInt32(dictionary[1].Id == "" ? 0 : Convert.ToInt32(dictionary[1].Id));
                if (companyId != 0)
                {
                    _userGroupFactory = new userGroupFactory();
                    var userGroups = _userGroupFactory.GetAll().Select(x => new
                    {
                        id = x.Name,
                        Group = x.Name
                    }).ToList();
                    return Json(new { success = true, data = userGroups.OrderBy(x => x.Group) }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = false, message = "LogOut" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                return Json(new { success = false, message = exception.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Users the role change.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="userRole">The user role.</param>
        /// <returns>Bool</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <CreatedDate>25-Sep-2016</CreatedDate>
        public JsonResult UserRoleChange(Guid id, string userRole )
        {
            try
            {
                Dictionary<int, CheckSessionData> dictionary = CheckSessionData.GetSessionValues();
                long companyId = Convert.ToInt32(dictionary[1].Id == "" ? 0 : Convert.ToInt32(dictionary[1].Id));
                if (companyId != 0)
                {
                    _userGroupFactory = new userGroupFactory();
                    _userFactory = new userFactory();
                    Guid userId = Guid.Parse(dictionary[3].Id);
                    TBLA_USER_INFORMATION user = _userFactory.FindBy(x => x.ID == userId).FirstOrDefault();
                    TBLA_USER_GROUP userGroup = _userGroupFactory.FindBy(x => x.ID == user.UserGroupID).FirstOrDefault();
                    if (userGroup != null && userGroup.IsAdmin)
                    {
                        TBLA_USER_GROUP role = _userGroupFactory.FindBy(x => x.Name == userRole).FirstOrDefault();
                        _userFactory = new userFactory();
                        TBLA_USER_INFORMATION tblUserInformation = _userFactory.FindBy(x => x.ID == id).FirstOrDefault();
                        if (tblUserInformation != null)
                        {
                            tblUserInformation.UserGroupID = role.ID;
                            _userFactory.Edit(tblUserInformation);
                        }
                        _userFactory.Save();
                        return Json(new { success = true, message = "Sucessifuly changed the user role" }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { success = false, message = "You are not Admin User" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = false, message = "LogOut" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>bool</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <DateofCreate>27-Oct-2016</DateofCreate>
        public JsonResult Delete(Guid id)
        {
            try
            {
                Dictionary<int, CheckSessionData> dictionary = CheckSessionData.GetSessionValues();
                long companyId = Convert.ToInt32(dictionary[1].Id == "" ? 0 : Convert.ToInt32(dictionary[1].Id));
                if (companyId != 0)
                {
                    int userGroupID = Convert.ToInt32(dictionary[6].Id == "" ? 0 : Convert.ToInt32(dictionary[6].Id));
                    ISecurityFactory _securityLogInFactory = new SecurityFactorys();
                    PagePermissionVM tblUserActionMapping = _securityLogInFactory.GetCRUDPermission(userGroupID, "User");
                    if (tblUserActionMapping.Delete)
                    {
                        _userFactory = new userFactory();
                        _userFactory.Delete(x => x.ID == id);
                        _userFactory.Save();
                        return Json(new { success = true, message = "Deleted Successfuly" }, JsonRequestBehavior.AllowGet); 
                    }
                    return Json(new { success = false, message = "You has no delete permission" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = false, message = "LogOut" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Another page use this User data" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}