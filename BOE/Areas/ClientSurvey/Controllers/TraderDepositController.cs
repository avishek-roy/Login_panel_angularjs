using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BOE.Areas.ClientSurvey.Models;
using BOEService.Entites.BOE;
using BOEService.Factories;
using BOEService.Interfaces;
using BOEService.Models;
using BOEUtility;
using BOEUtility.Helper;

namespace BOE.Areas.ClientSurvey.Controllers
{
    public class TraderDepositController : Controller
    {

        private IGenericFactory<BOEService.Entites.BOE.TBLT_SURVEY_DEPOSIT> _depositeFactory;
        private IGenericFactory<TBLA_USER_INFORMATION> _userFactory;
        private IGenericFactory<BOEService.Entites.BOE.TBLT_SURVEY_COMMUNICATION> _clientComnucationFactory;

        public ActionResult Index()
        {
            Dictionary<int, CheckSessionData> dictionary = CheckSessionData.GetSessionValues();
            long companyId = Convert.ToInt32(dictionary[1].Id == "" ? 0 : Convert.ToInt32(dictionary[1].Id));
            if (companyId != 0)
            {
                return View();
            }
            return Redirect("/#/");
        }

        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Bool</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <CreatedDate>30_Nov_2016</CreatedDate>
        public JsonResult Delete(int id)
        {
            try
            {
                Dictionary<int, CheckSessionData> dictionary = CheckSessionData.GetSessionValues();
                long companyId = Convert.ToInt32(dictionary[1].Id == "" ? 0 : Convert.ToInt32(dictionary[1].Id));
                if (companyId != 0)
                {
                    int userGroupID = Convert.ToInt32(dictionary[6].Id == "" ? 0 : Convert.ToInt32(dictionary[6].Id));
                    ISecurityFactory _securityLogInFactory = new SecurityFactorys();
                    PagePermissionVM tblUserActionMapping = _securityLogInFactory.GetCRUDPermission(userGroupID, "ClientDeposite");
                    if (tblUserActionMapping.Delete)
                    {
                        if (tblUserActionMapping.ForcedLogOut)
                        {
                            return Json(new { success = false, message = "Some maintainence work are processing" }, JsonRequestBehavior.AllowGet);
                        }
                        _depositeFactory = new DepositeFactory();
                        _depositeFactory.Delete(x => x.ID == id);
                        _depositeFactory.Save();
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

        /// <summary>
        /// Creates the survey deposite.
        /// </summary>
        /// <returns>View</returns>
        /// <CreateddBy>Avishek</CreateddBy>
        /// <EreatedDate>3_Nov_2015</EreatedDate>
        public ActionResult CreateSurveyDeposit()
        {
            Dictionary<int, CheckSessionData> dictionary = CheckSessionData.GetSessionValues();
            long companyId = Convert.ToInt32(dictionary[1].Id == "" ? 0 : Convert.ToInt32(dictionary[1].Id));
            if (companyId != 0)
            {
                return View();
            }
            return Redirect("/#/");
        }

        /// <summary>
        /// Gets the types.
        /// </summary>
        /// <returns>List</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <CreatedDate>30_Nov_2016</CreatedDate>
        public JsonResult GetTypes()
        {
            try
            {
                Dictionary<int, CheckSessionData> dictionary = CheckSessionData.GetSessionValues();
                long companyId = Convert.ToInt32(dictionary[1].Id == "" ? 0 : Convert.ToInt32(dictionary[1].Id));
                if (companyId != 0)
                {
                    var types = DropDown.ClientType();

                    return Json(new { success = true, data = types.ToList() }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = false, message = "LogOut" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                return Json(new { success = false, message = exception.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Loads all deposite.
        /// </summary>
        /// <returns>List</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <CreatedDate>30_Nov_2016</CreatedDate>
        public ActionResult LoadAllDeposit()
        {
            try
            {
                Dictionary<int, CheckSessionData> dictionary = CheckSessionData.GetSessionValues();
                long companyId = Convert.ToInt32(dictionary[1].Id == "" ? 0 : Convert.ToInt32(dictionary[1].Id));
                if (companyId != 0)
                {
                    Guid userId = Guid.Parse(dictionary[3].Id);
                    _depositeFactory = new BOEService.Factories.DepositeFactory();
                    //DateTime fromDate = Convert.ToDateTime(Session["FromDate"]);
                    //DateTime toDate = Convert.ToDateTime(Session["ToDate"]); 
                    List<TBLT_SURVEY_DEPOSIT> depositeList = _depositeFactory.FindBy(x => x.CreatedBy == userId).ToList();
                    List<ClientSurveyVM> ClientSurveyVMList = new List<ClientSurveyVM>();

                    foreach (var item in depositeList)
                    {
                        ClientSurveyVM ClientSurveyVM = new ClientSurveyVM();
                        ClientSurveyVM.Date = item.DepositeDate.ToShortDateString();
                        ClientSurveyVM.DepositeNo = item.DepositNo;
                        ClientSurveyVM.Amount = item.Amount;
                        ClientSurveyVM.Type = item.Type;
                        ClientSurveyVM.ID = item.ID;
                        ClientSurveyVMList.Add(ClientSurveyVM);
                    }

                    var surveyDeposite = ClientSurveyVMList.Select(x => new
                    {
                        x.ID,
                        x.DepositeNo,
                        x.Amount,
                        x.Date,
                        x.Type
                    }).ToList();

                    return Json(surveyDeposite);
                }
                return Json(new { success = false, message = "LogOut" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                return Json(new { success = false, message = exception.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Gets the trader identifier forossition.
        /// </summary>
        /// <param name="survayPosition">The survay position.</param>
        /// <returns>GetSessionData</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <CreatedDate>30_Nov_2016</CreatedDate>
        //public ActionResult GetTraderIDForossition(ClientSurveyVM survayPosition)
        //{
        //    try
        //    {
        //        Session["FromDate"] = survayPosition.FromDate.ToString();
        //        Session["ToDate"] = survayPosition.ToDate.ToString();
        //        return Json(new { success = true, data = "OK" }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception exception)
        //    {
        //        return Json(new { success = false, message = exception.Message }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        /// <summary>
        /// Gets the total.
        /// </summary>
        /// <returns>Decimal</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <CreatedDate>30_Nov_2016</CreatedDate>
        //public JsonResult GetTotal() 
        //{
        //    try
        //    {
        //        Dictionary<int, CheckSessionData> dictionary = CheckSessionData.GetSessionValues();
        //        long companyId = Convert.ToInt32(dictionary[1].Id == "" ? 0 : Convert.ToInt32(dictionary[1].Id));
        //        if (companyId != 0)
        //        {
        //            Guid userId = Guid.Parse(dictionary[3].Id);
        //            DateTime fromDate = Convert.ToDateTime(Session["FromDate"]);
        //            DateTime toDate = Convert.ToDateTime(Session["ToDate"]); 
        //            _depositeFactory = new BOEService.Factories.DepositeFactory();
        //            string surveyDeposite = _depositeFactory.FindBy(x => x.CreatedBy == userId && x.DepositeDate >= fromDate && x.DepositeDate <= toDate).ToList().Sum(x => x.Amount).ToString();
        //            return Json(new { success = true, data = surveyDeposite }, JsonRequestBehavior.AllowGet);
        //        }
        //        return Json(new { success = false, message = "LogOut" }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception exception)
        //    {
        //        return Json(new { success = false, message = exception.Message }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        /// <summary>
        /// Clients the survey save.
        /// </summary>
        /// <param name="clientDeposite">The client deposite.</param>
        /// <returns>Bool</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <CreatedDate>30_Nov_2016</CreatedDate>
        public JsonResult ClientSurveySave(ClientSurveyVM clientDeposite)
        {
            try
            {
                Dictionary<int, CheckSessionData> dictionary = CheckSessionData.GetSessionValues();
                int companyId = Convert.ToInt32(dictionary[1].Id == "" ? 0 : Convert.ToInt32(dictionary[1].Id));
                int locationId = Convert.ToInt32(dictionary[2].Id == "" ? 0 : Convert.ToInt32(dictionary[2].Id));
                if (companyId != 0)
                {
                    int userGroupID = Convert.ToInt32(dictionary[6].Id == "" ? 0 : Convert.ToInt32(dictionary[6].Id));
                    ISecurityFactory _securityLogInFactory = new SecurityFactorys();
                    PagePermissionVM tblUserActionMapping = _securityLogInFactory.GetCRUDPermission(userGroupID, "ClientDeposite");
                    if (tblUserActionMapping.Create)
                    {
                        if (tblUserActionMapping.ForcedLogOut)
                        {
                            return Json(new { success = false, message = "Some maintainence work are processing" }, JsonRequestBehavior.AllowGet);
                        }
                        Guid userId = Guid.Parse(dictionary[3].Id);
                        _depositeFactory = new BOEService.Factories.DepositeFactory();
                        TBLT_SURVEY_DEPOSIT _tblSurveyDeposit = new TBLT_SURVEY_DEPOSIT();
                        _tblSurveyDeposit.DepositNo = clientDeposite.DepositeNo;
                        _tblSurveyDeposit.Amount = clientDeposite.Amount;

                        string depositeDateString = clientDeposite.DepositeDate == null ? "" : clientDeposite.DepositeDate;
                        string[] depositeDateEdit = depositeDateString.Split('-');
                        _tblSurveyDeposit.DepositeDate = Convert.ToDateTime(depositeDateEdit[1] + "/" + depositeDateEdit[0] + "/" + depositeDateEdit[2]);
                        //_tblSurveyDeposit.DepositeDate = clientDeposite.DepositeDate;
                        _tblSurveyDeposit.Type = clientDeposite.ClientType;
                        _tblSurveyDeposit.CreatedDate = DateTime.Now;
                        _tblSurveyDeposit.CreatedBy = userId;
                        _tblSurveyDeposit.LocationID = locationId;
                        _tblSurveyDeposit.CompanyID = companyId;
                        _depositeFactory.Add(_tblSurveyDeposit);
                        _depositeFactory.Save();
                        return Json(new { success = true, message = "Saved Sucesifully" }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { success = false, message = "You has no Create permission" }, JsonRequestBehavior.AllowGet);
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
        /// Edits the client survey.
        /// </summary>
        /// <returns>view</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <CreatedDate>3_Nov_2016</CreatedDate>
        public ActionResult EditClientSurvey()
        {
            Dictionary<int, CheckSessionData> dictionary = CheckSessionData.GetSessionValues();
            long companyId = Convert.ToInt32(dictionary[1].Id == "" ? 0 : Convert.ToInt32(dictionary[1].Id));
            int userGroupID = Convert.ToInt32(dictionary[6].Id == "" ? 0 : Convert.ToInt32(dictionary[6].Id));
            if (companyId != 0)
            {
                ISecurityFactory _securityLogInFactory = new SecurityFactorys();
                PagePermissionVM tblUserActionMapping = _securityLogInFactory.GetCRUDPermission(userGroupID, "ClientDeposite");
                if (tblUserActionMapping.Edit)
                {
                    return View();
                }
            }
            Session["logInSession"] = "false";
            return Redirect("/#/");
        }

        /// <summary>
        /// Loads the location by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Object</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <CreatedDate>3_Nov_2016</CreatedDate>
        public JsonResult LoadLocationById(int id)
        {
            try
            {
                Dictionary<int, CheckSessionData> dictionary = CheckSessionData.GetSessionValues();
                long companyId = Convert.ToInt32(dictionary[1].Id == "" ? 0 : Convert.ToInt32(dictionary[1].Id));
                if (companyId != 0)
                {
                    _depositeFactory = new DepositeFactory();
                    var deposite = _depositeFactory.FindBy(x => x.ID == id).Select(a => new
                    {
                        a.ID,
                        DepositeNo = a.DepositNo,
                        a.Amount,
                        ClientType = a.Type,
                        a.DepositeDate
                    }).FirstOrDefault();
                    ClientSurveyVM _ClientSurveyVM = new ClientSurveyVM();
                    string[] dateString = deposite.DepositeDate.ToShortDateString().Split('/');
                    _ClientSurveyVM.DepositeDateEdit = dateString[1] + "-" + dateString[0]+"-"+dateString[2];
                    _ClientSurveyVM.ID = deposite.ID;
                    _ClientSurveyVM.DepositeNo = deposite.DepositeNo;
                    _ClientSurveyVM.ClientType = deposite.ClientType;
                    _ClientSurveyVM.Amount = deposite.Amount;

                    return Json(new { success = true, data = _ClientSurveyVM }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = false, message = "LogOut" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                return Json(new { success = false, message = exception.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Edits the deposite save.
        /// </summary>
        /// <param name="clientDeposite">The client deposite.</param>
        /// <returns>Bool</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <CreatedDate>3_Nov_2016</CreatedDate>
        public JsonResult EditDepositeSave(ClientSurveyVM clientDeposite)
        {
            try
            {
                Dictionary<int, CheckSessionData> dictionary = CheckSessionData.GetSessionValues();
                int companyId = Convert.ToInt32(dictionary[1].Id);
                Guid userId = Guid.Parse(dictionary[3].Id);
                if (companyId != 0)
                {
                    int userGroupID = Convert.ToInt32(dictionary[6].Id == "" ? 0 : Convert.ToInt32(dictionary[6].Id));
                    ISecurityFactory _securityLogInFactory = new SecurityFactorys();
                    PagePermissionVM tblUserActionMapping = _securityLogInFactory.GetCRUDPermission(userGroupID, "ClientDeposite");
                    if (tblUserActionMapping.Edit)
                    {
                        if (tblUserActionMapping.ForcedLogOut)
                        {
                            return Json(new { success = false, message = "Some maintainence work are processing" }, JsonRequestBehavior.AllowGet);
                        }
                        _depositeFactory = new DepositeFactory();
                        TBLT_SURVEY_DEPOSIT tblSurveyDeposit = _depositeFactory.FindBy(x => x.ID == clientDeposite.ID).FirstOrDefault();
                        if (tblSurveyDeposit != null)
                        {
                            tblSurveyDeposit.ModifiedBy = userId;
                            tblSurveyDeposit.ModifiedDate = DateTime.Now;
                            tblSurveyDeposit.DepositNo = clientDeposite.DepositeNo;
                            tblSurveyDeposit.Amount = Convert.ToDecimal(clientDeposite.Amount);

                            string[] dateString = clientDeposite.DepositeDateEdit.Split('-');

                            tblSurveyDeposit.DepositeDate = Convert.ToDateTime(dateString[1] + "/" + dateString[0] + "/" + dateString[2]);
                            tblSurveyDeposit.Type = clientDeposite.ClientType;
                            _depositeFactory.Edit(tblSurveyDeposit);
                            _depositeFactory.Save();
                            return Json(new { success = true, message = "Data Updated successfully" }, JsonRequestBehavior.AllowGet);
                        }
                        return Json(new { success = false, message = "You'r entared code are duplicate" }, JsonRequestBehavior.AllowGet);
                    }
                }
                Session["logInSession"] = "false";
                return Json(new { success = false, message = "LogOut" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                return Json(new { success = false, message = exception.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        #region
        public ActionResult TraderPositionByDeposit()
        {
            Dictionary<int, CheckSessionData> dictionary = CheckSessionData.GetSessionValues();
            long companyId = Convert.ToInt32(dictionary[1].Id == "" ? 0 : Convert.ToInt32(dictionary[1].Id));
            if (companyId != 0)
            {
                return View();
            }
            return Redirect("/#/");
        }

        public JsonResult GetTrader()
        {
            try
            {
                Dictionary<int, CheckSessionData> dictionary = CheckSessionData.GetSessionValues();
                long companyId = Convert.ToInt32(dictionary[1].Id == "" ? 0 : Convert.ToInt32(dictionary[1].Id));
                if (companyId != 0)
                {
                    _userFactory = new userFactory();
                    var users = _userFactory.GetAll().Select(x => new
                    {
                        x.UserName,
                        x.UserFullName
                    }).ToList();

                    return Json(new { success = true, data = users.OrderBy(x => x.UserFullName) }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = false, message = "LogOut" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                return Json(new { success = false, message = exception.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetTraderIDForossition(ClientSurveyVM survayPosition)
        {
            try
            {
                Session["UserName"] = survayPosition.UserName.ToString();
                Session["FromDate"] = survayPosition.FromDate.ToString();
                Session["ToDate"] = survayPosition.ToDate.ToString();

                return Json(new { success = true, data = "OK" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                return Json(new { success = false, message = exception.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult LoadTrader()
        {
            try
            {
                Dictionary<int, CheckSessionData> dictionary = CheckSessionData.GetSessionValues();
                long companyId = Convert.ToInt32(dictionary[1].Id == "" ? 0 : Convert.ToInt32(dictionary[1].Id));
                if (companyId != 0)
                {
                    Guid userId = Guid.Parse(dictionary[3].Id);
                    _depositeFactory = new BOEService.Factories.DepositeFactory();
                    string user = Session["UserName"].ToString();
                    string fromDateString = Session["FromDate"].ToString();
                    string[] fromDateArray = fromDateString.Split('-');
                    DateTime fromDate = Convert.ToDateTime(fromDateArray[1] + "/" + fromDateArray[0] + "/" + fromDateArray[2]);
                    string toDateString = Session["ToDate"].ToString();
                    string[] toDateArray = toDateString.Split('-');
                    DateTime toDate = Convert.ToDateTime(toDateArray[1] + "/" + toDateArray[0] + "/" + toDateArray[2]).AddDays(1).AddSeconds(-1);

                    _userFactory = new userFactory();
                    Guid traderID = _userFactory.FindBy(x => x.UserName == user).Select(x => x.ID).FirstOrDefault();
                    List<TBLT_SURVEY_DEPOSIT> depositeList = _depositeFactory.FindBy(x => x.CreatedBy == traderID && x.DepositeDate >= fromDate && x.DepositeDate <= toDate).ToList();
                    List<ClientSurveyVM> ClientSurveyVMList = new List<ClientSurveyVM>();

                    foreach (var item in depositeList)
                    {
                        ClientSurveyVM ClientSurveyVM = new ClientSurveyVM();
                        ClientSurveyVM.Date = item.DepositeDate.ToShortDateString();
                        ClientSurveyVM.DepositeNo = item.DepositNo;
                        ClientSurveyVM.Amount = item.Amount;
                        ClientSurveyVM.Type = item.Type;
                        ClientSurveyVM.ID = item.ID;
                        ClientSurveyVMList.Add(ClientSurveyVM);
                    }

                    var surveyDeposite = ClientSurveyVMList.Select(x => new
                    {
                        x.ID,
                        x.DepositeNo,
                        x.Amount,
                        x.Date,
                        x.Type
                    }).ToList();

                    return Json(surveyDeposite);
                    //return Json(new { success = true, data = surveyDeposite }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = false, message = "LogOut" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                return Json(new { success = false, message = exception.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult LoadTraderInfo()
        {
            try
            {
                Dictionary<int, CheckSessionData> dictionary = CheckSessionData.GetSessionValues();
                long companyId = Convert.ToInt32(dictionary[1].Id == "" ? 0 : Convert.ToInt32(dictionary[1].Id));
                if (companyId != 0)
                {
                    _userFactory = new userFactory();
                    string userName = Session["UserName"].ToString();
                    Guid traderID = _userFactory.FindBy(x => x.UserName == userName).Select(x => x.ID).FirstOrDefault();
                    var traderInfo = _userFactory.FindBy(x => x.ID == traderID).Select(a => new
                    {
                        a.ID,
                        a.UserFullName,
                        a.TBLB_LOCATION1.Name
                    }).FirstOrDefault();
                    return Json(new { success = true, data = traderInfo }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = false, message = "LogOut" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                return Json(new { success = false, message = exception.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
    }
}