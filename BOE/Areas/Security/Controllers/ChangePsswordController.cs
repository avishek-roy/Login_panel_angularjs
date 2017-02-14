using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BOE.Areas.Security.Models;
using BOEService.Entites.BOE;
using BOEService.Factories;
using BOEService.Interfaces;
using BOEService.Models;
using BOEUtility.Helper;

namespace BOE.Areas.Security.Controllers
{
    public class ChangePsswordController : Controller
    {
        ISecurityFactory _securityLogInFactory;
        private IGenericFactory<TBLA_SECURITY_QUESTION> _questionFactory;
        private IGenericFactory<TBLA_PASSWORD> _passwordFactory;
        private IGenericFactory<TBLA_USER_INFORMATION> _userFactory;
        private IGenericFactory<TBLA_USER_GROUP> _userGroup;

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns>View</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <Date>08-Aug-2016</Date>
        public ActionResult ChangePassword()
        {
            Dictionary<int, CheckSessionData> dictionary = CheckSessionData.GetSessionValues();
            long companyId = Convert.ToInt32(dictionary[1].Id == ""? 0:Convert.ToInt32(dictionary[1].Id));
            int userGroupID = Convert.ToInt32(dictionary[6].Id == "" ? 0 : Convert.ToInt32(dictionary[6].Id));
            if (companyId != 0)
            {
                ISecurityFactory _securityLogInFactory = new SecurityFactorys();
                PagePermissionVM tblUserActionMapping = _securityLogInFactory.GetCRUDPermission(userGroupID, "ChangePassword");
                if (tblUserActionMapping.Select)
                {
                    return View();
                }
            }
            Session["logInSession"] = "false";
            return Redirect("/#/");
        }

        /// <summary>
        /// Selfs the password change.
        /// </summary>
        /// <param name="changePassword">The change password.</param>
        /// <returns>Json</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <Date>08-Aug-2016</Date>
        public ActionResult SelfPasswordChange(ChangePasswordVM changePassword) 
        {
          try
           {
                LogInStatus _LogInStatus = new LogInStatus();
                _securityLogInFactory = new SecurityFactorys();
                Dictionary<int, CheckSessionData> dictionary = CheckSessionData.GetSessionValues();
                int companyId = Convert.ToInt32(dictionary[1].Id == "" ? 0 : Convert.ToInt32(dictionary[1].Id));
                int userGroupID = Convert.ToInt32(dictionary[6].Id == "" ? 0 : Convert.ToInt32(dictionary[6].Id));

                if (companyId != 0)
                {
                    Guid userId = Guid.Parse(dictionary[3].Id);
                    string userName = dictionary[4].Id;
                    int locationId = Convert.ToInt32(dictionary[2].Id == "" ? 0 : Convert.ToInt32(dictionary[2].Id));
                    _LogInStatus = _securityLogInFactory.CheckLogIn(new LogOnModel { UserName = userName, Password = changePassword.OldPassword });
                    if (_LogInStatus.Message == "Login Successfully")
                    {
                        TBLA_USER_INFORMATION _tblUserInformation = new TBLA_USER_INFORMATION();
                        _userFactory = new userFactory();
                        _tblUserInformation = _userFactory.FindBy(x => x.ID == userId).FirstOrDefault();
                        //_questionFactory = new questionFactory();
                        //TBLA_SECURITY_QUESTION tblSecurityQuestion = _questionFactory.FindBy(x => x.SecutiryAnswer.ToLower().Trim() == changePassword.Answre.ToLower().Trim()).FirstOrDefault();
                        //if (tblSecurityQuestion != null)
                        //{
                            _passwordFactory = new userPasswordFactory();
                            Encription encription = new Encription();
                            TBLA_PASSWORD tblPassword = _passwordFactory.GetAll().FirstOrDefault(x => x.ID == _tblUserInformation.PasswordID);
                            if (tblPassword != null)
                            {
                                tblPassword.OldPassword = tblPassword.NewPassword;
                                tblPassword.NewPassword = encription.Encrypt(changePassword.NewPassword.Trim());
                                tblPassword.IsSelfChanged = true;
                                tblPassword.LocationID = locationId;
                                tblPassword.CompanyID = companyId;
                                tblPassword.ModifiedDate = DateTime.Now;
                                tblPassword.ModifiedBy = userId;
                                _passwordFactory.Edit(tblPassword);
                            }
                            _passwordFactory.Save();
                            return Json(new { success = true, message = "Changed Password Sucessfully" }, JsonRequestBehavior.AllowGet);
                        //}
                        //return Json(new { success = false, message = "Your Password or User Name or Answer are nor correct" }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { success = false, message = "Password not Changed try again" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = false, message = "LogOut" }, JsonRequestBehavior.AllowGet);
             }
            catch (Exception exception)
            {
                return Json(new { success = false, message = exception.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Passwords the change by admin.
        /// </summary>
        /// <returns>View</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <Date>08-Aug-2016</Date>
        public ActionResult PasswordChangeByAdmin()
        {
            Dictionary<int, CheckSessionData> dictionary = CheckSessionData.GetSessionValues();
            int companyId = Convert.ToInt32(dictionary[1].Id);
            int userGroupID = Convert.ToInt32(dictionary[6].Id == "" ? 0 : Convert.ToInt32(dictionary[6].Id));
            if (companyId != 0)
            {
                ISecurityFactory _securityLogInFactory = new SecurityFactorys();
                PagePermissionVM tblUserActionMapping = _securityLogInFactory.GetCRUDPermission(userGroupID, "ChangePasswordByAdmin");
                if (tblUserActionMapping.Select)
                {
                    return View();
                }
            }
            return Redirect("/Login");
        }


        /// <summary>
        /// Loads all full name.
        /// </summary>
        /// <returns>JsonList</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <Date>08-Aug-2016</Date>
        public JsonResult LoadAllFullName()
        {
            try
            {
                _userFactory = new userFactory();
                var userFullName = _userFactory.FindBy(x => x.IsActive == true)
                    .Select(a => new
                    {
                        a.UserFullName
                    }).ToList();

                return Json(new { success = true, data = userFullName }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                return Json(new { success = false, errorMessage = exception.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Loads the name of all user.
        /// </summary>
        /// <returns>JsonList</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <Date>08-Aug-2016</Date>
        public JsonResult LoadAllUserName()
        {
            try
            {
                _userFactory = new userFactory();
                var userName = _userFactory.FindBy(x => x.IsActive == true)
                    .Select(a => new
                    {
                        a.UserName
                    }).ToList();

                return Json(new { success = true, data = userName }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                return Json(new { success = false, errorMessage = exception.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// Passwords the change by admin save.
        /// </summary>
        /// <param name="changePassword">The change password.</param>
        /// <returns>Json/Bool</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <Date>08-Aug-2016</Date>
        public JsonResult PasswordChangeByAdminSave(ChangePasswordVM changePassword)
        {
            try
            {
                Dictionary<int, CheckSessionData> dictionary = CheckSessionData.GetSessionValues();
                int companyId = Convert.ToInt32(dictionary[1].Id);
                int locationId = Convert.ToInt32(dictionary[2].Id);
                Guid userId = Guid.Parse(dictionary[3].Id);
                string userName = dictionary[4].Id;

                if (companyId != 0)
                {
                   _userFactory = new userFactory();
                   _passwordFactory = new userPasswordFactory();
                   _userGroup = new userGroupFactory();
                   Encription encription = new Encription();
                   TBLA_PASSWORD tblPassword = new TBLA_PASSWORD();
                   TBLA_USER_INFORMATION _tblUserInformation = new TBLA_USER_INFORMATION();
                   TBLA_USER_GROUP _tblUserGroup = new TBLA_USER_GROUP();

                   if (changePassword.FullName != "" || changePassword.UserName != "")
                   {
                       if (changePassword.UserName == "" || changePassword.UserName == null)
                       {
                           int countUser = _userFactory.FindBy(x => x.UserFullName == changePassword.FullName).Count();
                           if (countUser > 1)
                           {
                               return Json(new { success = false, message = "Found more than one user for selected User Full Name. So, please selete User Name " }, JsonRequestBehavior.AllowGet);
                           }
                           _tblUserInformation = _userFactory.FindBy(x => x.UserFullName == changePassword.FullName).FirstOrDefault();
                       }
                       else {
                           _tblUserInformation = _userFactory.FindBy(x => x.UserName == changePassword.UserName).FirstOrDefault();
                       }

                       _tblUserGroup = _userGroup.FindBy(x => x.ID == _tblUserInformation.UserGroupID).FirstOrDefault();
                       if (_tblUserGroup != null)
                       {
                           if(_tblUserGroup.IsAdmin == false)
                           {
                               return Json(new { success = false, message = "You are not a Admin" }, JsonRequestBehavior.AllowGet);
                           }
                           tblPassword = _passwordFactory.GetAll().FirstOrDefault(x => x.ID == _tblUserInformation.PasswordID);
                           if (tblPassword != null)
                           {
                               tblPassword.OldPassword = tblPassword.NewPassword;
                               tblPassword.NewPassword = encription.Encrypt(changePassword.NewPassword.Trim());
                               tblPassword.IsSelfChanged = false;
                               tblPassword.LocationID = locationId;
                               tblPassword.CompanyID = companyId;
                               tblPassword.ModifiedDate = DateTime.Now;
                               tblPassword.ModifiedBy = userId;
                               _passwordFactory.Edit(tblPassword);
                           }
                           _passwordFactory.Save();
                           return Json(new { success = true, message = "Changed Password Sucessfully" }, JsonRequestBehavior.AllowGet);
                       }
                       return Json(new { success = false, message = "User cant found" }, JsonRequestBehavior.AllowGet);
                   }
                   return Json(new { success = false, message = "Password not Changed try again" }, JsonRequestBehavior.AllowGet);
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