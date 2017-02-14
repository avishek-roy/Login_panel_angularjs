using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using BOE.Models;
using BOEService.Entites.BOE;
using BOEService.Factories;
using BOEService.Interfaces;
using BOEService.Models;
using BOEUtility.Helper;

namespace BOE.Controllers
{
    public class AccountController : Controller
    {

        ISecurityFactory _securityFactory;
        private IGenericFactory<TBLA_USER_INFORMATION> _userFactory;
        private IGenericFactory<TBLA_LOGIN_STATUS> _loginStatusFactory;

        /// <summary>
        /// Logins this instance.
        /// </summary>
        /// <returns>View</returns>
        /// <CreatdBy>Avishek</CreatdBy>
        /// <CreatdDate>8_Sep_2016</CreatdDate>
        public ActionResult Login()
        {
            return View();
        }


        /// <summary>
        /// Logins the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>Bool</returns>
        /// <exception cref="System.ArgumentException">Value cannot be null or empty.;userName</exception>
        /// <CreatdBy>Avishek</CreatdBy>
        /// <CreatdDate>8_Sep_2016</CreatdDate>
        [HttpPost]
        //public ActionResult Login(LoginViewModel model, string returnUrl)
        public JsonResult Login(LoginViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    bool GetLan = false;

                    string visitorIPAddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                    if (String.IsNullOrEmpty(visitorIPAddress))
                        visitorIPAddress = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    if (string.IsNullOrEmpty(visitorIPAddress))
                        visitorIPAddress = System.Web.HttpContext.Current.Request.UserHostAddress;

                    if (string.IsNullOrEmpty(visitorIPAddress) || visitorIPAddress.Trim() == "::1")
                    {
                        GetLan = true;
                        visitorIPAddress = string.Empty;
                    }

                    if (GetLan && string.IsNullOrEmpty(visitorIPAddress))
                    {
                        //This is for Local(LAN) Connected ID Address
                        string stringHostName = Dns.GetHostName();
                        //Get Ip Host Entry
                        IPHostEntry ipHostEntries = Dns.GetHostEntry(stringHostName);
                        //Get Ip Address From The Ip Host Entry Address List
                        IPAddress[] arrIpAddress = ipHostEntries.AddressList;

                        try
                        {
                            visitorIPAddress = arrIpAddress[arrIpAddress.Length - 2].ToString();
                        }
                        catch
                        {
                            try
                            {
                                visitorIPAddress = arrIpAddress[0].ToString();
                            }
                            catch
                            {
                                try
                                {
                                    arrIpAddress = Dns.GetHostAddresses(stringHostName);
                                    visitorIPAddress = arrIpAddress[0].ToString();
                                }
                                catch
                                {
                                    visitorIPAddress = "127.0.0.1";
                                }
                            }
                        }

                    }


                    ////////////////////////////////////
                    _securityFactory = new SecurityFactorys();
                    _userFactory = new userFactory();
                    var logInStatus = _securityFactory.CheckLogIn(new LogOnModel { UserName = model.UserName.ToLower().Trim(), Password = model.Password });

                    if (logInStatus.IsAllowed)
                    {
                        var aSecurityUser = _userFactory.FindBy(x => x.UserName.Contains(model.UserName)).FirstOrDefault();

                        if (aSecurityUser != null)
                        {
                            System.Web.HttpContext.Current.Session["LoginCompany"] = aSecurityUser.CompanyID;
                            System.Web.HttpContext.Current.Session["LoginLocation"] = aSecurityUser.LocationID;
                            System.Web.HttpContext.Current.Session["LoginUserID"] = aSecurityUser.ID;
                            System.Web.HttpContext.Current.Session["LoginUserName"] = aSecurityUser.UserName;
                            System.Web.HttpContext.Current.Session["LoginUserFullName"] = aSecurityUser.UserFullName;
                            System.Web.HttpContext.Current.Session["UserGroupID"] = aSecurityUser.UserGroupID;
                            System.Web.HttpContext.Current.Session["IPAddress"] = visitorIPAddress;
                            //System.Web.HttpContext.Current.Session["PCName"] = Dns.GetHostEntry(Request.ServerVariables["REMOTE_ADDR"]).HostName;
                            string[] computer_name = System.Net.Dns.GetHostEntry(Request.ServerVariables["remote_addr"]).HostName.Split(new Char[] { '.' });
                            String ecname = System.Environment.MachineName;
                            System.Web.HttpContext.Current.Session["PCName"] = computer_name[0].ToString();

                            if (!String.IsNullOrEmpty(model.UserName))
                            {
                                if (!aSecurityUser.UserName.Equals(model.UserName, StringComparison.Ordinal))
                                {
                                    return Json(new { success = false, message = "Incorrect User Name or Password." }, JsonRequestBehavior.AllowGet);
                                }
                            }
                            else
                            {
                                System.Web.HttpContext.Current.Session["LoginUser"] = 0;
                            }

                            if (!logInStatus.IsAllowed)
                            {
                                return Json(new { success = false, message = logInStatus.Message }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                //if (String.IsNullOrEmpty(model.UserName)) throw new ArgumentException("Value cannot be null or empty.", "userName");
                                //FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                                TBLA_LOGIN_STATUS tblLogInStatus = new TBLA_LOGIN_STATUS();
                                _loginStatusFactory = new loginStatusFactory();
                                tblLogInStatus.UserID = aSecurityUser.ID;
                                tblLogInStatus.PresentLogInStatus = true;
                                tblLogInStatus.LogInTime = DateTime.Now;
                                tblLogInStatus.LogOutTime = DateTime.Now;
                                tblLogInStatus.LocationID = aSecurityUser.LocationID;
                                tblLogInStatus.CompanyID = aSecurityUser.CompanyID;
                                tblLogInStatus.ForcedLogOutStatus = false;
                                _loginStatusFactory.Add(tblLogInStatus);
                                _loginStatusFactory.Save();

                                Session["logInSession"] = "true";
                                //ViewBag.Error = "";
                                //return Redirect("/#/");
                                return Json(new { success = false, message = "Success" }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        return Json(new { success = false, message = "The user name or password provided is incorrect." }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { success = false, message = logInStatus.Message }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, message = "The user name or password provided is incorrect." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                Route();
            }
           return Json(new { success = false, message = "The user name or password provided is incorrect." }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Logs the off.
        /// </summary>
        /// <returns>bool</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <CreatedBy>3_Oct_2016</CreatedBy>
        public ActionResult LogOff()
        {
            try
            {
                Dictionary<int, CheckSessionData> dictionary = CheckSessionData.GetSessionValues();
                if (dictionary[3].Id.ToString() != null || dictionary[3].Id.ToString() != "")
                {
                    Guid userId = Guid.Parse(dictionary[3].Id);
                    _loginStatusFactory = new loginStatusFactory();

                    TBLA_LOGIN_STATUS loginStatus = _loginStatusFactory.FindBy(x => x.UserID == userId).FirstOrDefault();
                    loginStatus.PresentLogInStatus = false;
                    loginStatus.LogOutTime = DateTime.Now;
                    loginStatus.ForcedLogOutStatus = false;
                    _loginStatusFactory.Edit(loginStatus);
                    _loginStatusFactory.Save();

                    System.Web.HttpContext.Current.Session["LoginCompany"] = 0;
                    System.Web.HttpContext.Current.Session["LoginLocation"] = 0;
                    System.Web.HttpContext.Current.Session["LoginUserID"] = 0;
                    System.Web.HttpContext.Current.Session["LoginUserName"] = 0;

                    Session["logInSession"] = null;

                    return Redirect("/#/");
                }
                return Redirect("/#/");
            }
            catch (Exception)
            {
               return Redirect("/#/");
            }
        }

        public ActionResult Route() 
        {
            return Redirect("/#/");
        } 

    }
}