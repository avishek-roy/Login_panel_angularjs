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
    public class SurveyComnucationController : Controller
    {

        private IGenericFactory<TBLA_USER_INFORMATION> _userFactory;
        private IGenericFactory<TBLT_SURVEY_DEPOSIT> _depositeFactory;
        private IGenericFactory<TBLT_SURVEY_COMMUNICATION> _clientComnucationFactory;

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns>View</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <CreatedDate>1_Nov_2016</CreatedDate>
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
        /// Loads the person by comnucation.
        /// </summary>
        /// <returns>List</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <CreatedDate>1_Nov_2016</CreatedDate>
        public ActionResult LoadPersonByComnucation()
        {
            try
            {
                Dictionary<int, CheckSessionData> dictionary = CheckSessionData.GetSessionValues();
                long companyId = Convert.ToInt32(dictionary[1].Id == "" ? 0 : Convert.ToInt32(dictionary[1].Id));
                if (companyId != 0)
                {
                    Guid userId = Guid.Parse(dictionary[3].Id);
                    _clientComnucationFactory = new ComnucationFactory();

                    var tblSurveyComuncation = _clientComnucationFactory.FindBy(x => x.CreatedBy == userId).ToList();
                    List<ClientSurveyCommunicationVM> surveyCommunicationList = new List<ClientSurveyCommunicationVM>();
                    foreach (var item in tblSurveyComuncation)
                    {
                        ClientSurveyCommunicationVM surveyCommunication = new ClientSurveyCommunicationVM();
                        surveyCommunication.Id = item.ID;
                        surveyCommunication.DateShow = item.CommunicationDate.ToShortDateString();
                        surveyCommunication.Address = item.Address == null ? "" : item.Address;
                        surveyCommunication.ContactPerson = item.ContactPerson == null ? "" : item.ContactPerson;
                        surveyCommunication.ContactNo = item.ContactNo == null ? "" : item.ContactNo;
                        surveyCommunication.FeedBack = item.FeedBack == null ? "" : item.FeedBack;
                        surveyCommunication.FollowUpDateShow = item.FollowUpDate == null ? "" : item.FollowUpDate.GetValueOrDefault().ToShortDateString();
                        surveyCommunication.Type = item.ComnucationType;
                        surveyCommunicationList.Add(surveyCommunication);
                    }

                    var surveyHistory = surveyCommunicationList.Select(x => new
                    {
                        ID = x.Id,
                        x.DateShow,
                        x.Address,
                        x.ContactPerson,
                        x.ContactNo,
                        x.FeedBack,
                        x.FollowUpDateShow,
                        x.Type
                    }).ToList();

                    return Json(surveyHistory);

                }
                return Json(new { success = false, message = "LogOut" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                return Json(new { success = false, message = exception.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Creates the survey.
        /// </summary>
        /// <returns>View</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <CreatedDate>1_Nov_2016</CreatedDate>
        public ActionResult CreateSurvey() 
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
        /// <CreatedDate>1_Nov_2016</CreatedDate>
        public JsonResult GetTypes()
        {
            try
            {
                Dictionary<int, CheckSessionData> dictionary = CheckSessionData.GetSessionValues();
                long companyId = Convert.ToInt32(dictionary[1].Id == "" ? 0 : Convert.ToInt32(dictionary[1].Id));
                if (companyId != 0)
                {
                    var types = DropDown.ComuncationType();

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
        /// Clients the survey save.
        /// </summary>
        /// <param name="surveyCommunication">The survey communication.</param>
        /// <returns>Bool</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <CreatedDate>1_Nov_2016</CreatedDate>
        public ActionResult ClientSurveySave(ClientSurveyCommunicationVM surveyCommunication) 
        {
            try
            {
                Dictionary<int, CheckSessionData> dictionary = CheckSessionData.GetSessionValues();
                int companyId = Convert.ToInt32(dictionary[1].Id == "" ? 0 : Convert.ToInt32(dictionary[1].Id));
                int locationId = Convert.ToInt32(dictionary[2].Id == "" ? 0 : Convert.ToInt32(dictionary[2].Id));
                if (companyId != 0)
                {
                    ISecurityFactory _securityLogInFactory = new SecurityFactorys();
                    int userGroupID = Convert.ToInt32(dictionary[6].Id == "" ? 0 : Convert.ToInt32(dictionary[6].Id));
                    PagePermissionVM tblUserActionMapping = _securityLogInFactory.GetCRUDPermission(userGroupID, "ClientSurvey");
                    if (tblUserActionMapping.Edit)
                    {
                        Guid userId = Guid.Parse(dictionary[3].Id);
                        _clientComnucationFactory = new ComnucationFactory();
                        TBLT_SURVEY_COMMUNICATION isDuplicate = _clientComnucationFactory.FindBy(x => x.Code.ToLower() == surveyCommunication.Code.ToLower().Trim()).FirstOrDefault();
                        if (isDuplicate == null)
                        {
                            TBLT_SURVEY_COMMUNICATION tblSurveyComuncation = new TBLT_SURVEY_COMMUNICATION();
                            tblSurveyComuncation.Code = surveyCommunication.Code;
                            tblSurveyComuncation.Address = surveyCommunication.Address;
                            string comDateString = surveyCommunication.CommunicationDate == null ? "" : surveyCommunication.CommunicationDate;
                            string[] comDateStringEdit = comDateString.Split('-');
                            tblSurveyComuncation.CommunicationDate = Convert.ToDateTime(comDateStringEdit[1] + "/" + comDateStringEdit[0] + "/" + comDateStringEdit[2]);
                            //tblSurveyComuncation.CommunicationDate = surveyCommunication.CommunicationDate == null ? DateTime.Now : Convert.ToDateTime(surveyCommunication.CommunicationDate);
                            tblSurveyComuncation.ComnucationType = surveyCommunication.ComnucationType;
                            tblSurveyComuncation.ContactNo = surveyCommunication.ContactNo;
                            tblSurveyComuncation.ContactPerson = surveyCommunication.ContactPerson;
                            tblSurveyComuncation.CreatedBy = userId;
                            tblSurveyComuncation.CreatedDate = DateTime.Now;
                            tblSurveyComuncation.Email = surveyCommunication.Email;
                            tblSurveyComuncation.FeedBack = surveyCommunication.FeedBack;
                            string follDateString = surveyCommunication.FollowUpDate == null ? "" : surveyCommunication.FollowUpDate;
                            string[] followUpDateEdit = follDateString.Split('-');
                            tblSurveyComuncation.FollowUpDate = Convert.ToDateTime(followUpDateEdit[1] + "/" + followUpDateEdit[0] + "/" + followUpDateEdit[2]);
                            //tblSurveyComuncation.FollowUpDate = surveyCommunication.FollowUpDate == null ? DateTime.Now : Convert.ToDateTime(surveyCommunication.FollowUpDate);
                            tblSurveyComuncation.Reason = surveyCommunication.Reason;
                            //Its Will be swaped in feature 
                            tblSurveyComuncation.LocationID = companyId;
                            tblSurveyComuncation.CompanyID = locationId;
                            _clientComnucationFactory.Add(tblSurveyComuncation);
                            _clientComnucationFactory.Save();
                            return Json(new { success = true, message = "Saved Successfuly" }, JsonRequestBehavior.AllowGet);
                        }
                        return Json(new { success = false, message = "Your entared user name are duplicate please chose another name" }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { success = false, message = "You has no created permission" }, JsonRequestBehavior.AllowGet);
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
        /// Deletes the survey.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Bool</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <CreatedDate>1_Nov_2016</CreatedDate>
        public JsonResult DeleteSurvey(int id)
        {
            try
            {
                Dictionary<int, CheckSessionData> dictionary = CheckSessionData.GetSessionValues();
                long companyId = Convert.ToInt32(dictionary[1].Id == "" ? 0 : Convert.ToInt32(dictionary[1].Id));
                int userGroupID = Convert.ToInt32(dictionary[6].Id == "" ? 0 : Convert.ToInt32(dictionary[6].Id));
                if (companyId != 0)
                {
                    ISecurityFactory _securityLogInFactory = new SecurityFactorys();
                    PagePermissionVM tblUserActionMapping = _securityLogInFactory.GetCRUDPermission(userGroupID, "ClientSurvey");
                    if (tblUserActionMapping.Delete)
                    {
                        _clientComnucationFactory = new ComnucationFactory();
                        _clientComnucationFactory.Delete(x => x.ID == id);
                        _clientComnucationFactory.Save();
                        return Json(new { success = true, message = "Deleted Successfuly" }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { success = false, message = "You has no deleted permission" }, JsonRequestBehavior.AllowGet);
                }
                Session["logInSession"] = "false";
                return Json(new { success = false, message = "LogOut" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "You cant delet! Another page use this Location" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Edits the survay.
        /// </summary>
        /// <returns>View</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <CreatedDate>1_Nov_2016</CreatedDate>
        public ActionResult EditSurvay() 
        {
            Dictionary<int, CheckSessionData> dictionary = CheckSessionData.GetSessionValues();
            long companyId = Convert.ToInt32(dictionary[1].Id == "" ? 0 : Convert.ToInt32(dictionary[1].Id));
            int userGroupID = Convert.ToInt32(dictionary[6].Id == "" ? 0 : Convert.ToInt32(dictionary[6].Id));
            if (companyId != 0)
            {
                ISecurityFactory _securityLogInFactory = new SecurityFactorys();
                PagePermissionVM tblUserActionMapping = _securityLogInFactory.GetCRUDPermission(userGroupID, "ClientSurvey");
                if (tblUserActionMapping.Edit)
                {
                    return View();
                }
                return Json(new { success = false, message = "You has no Edit permission" }, JsonRequestBehavior.AllowGet);
            }
            Session["logInSession"] = "false";
            return Redirect("/#/");
        }

        /// <summary>
        /// Loads the survey data by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Object</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <CreatedDate>1_Nov_2016</CreatedDate>
        public JsonResult LoadSurveyDataById(int id)
        {
            try
            {
                Dictionary<int, CheckSessionData> dictionary = CheckSessionData.GetSessionValues();
                long companyId = Convert.ToInt32(dictionary[1].Id == "" ? 0 : Convert.ToInt32(dictionary[1].Id));
                if (companyId != 0)
                {
                    _clientComnucationFactory = new ComnucationFactory();
                    var editSurvey = _clientComnucationFactory.FindBy(x => x.ID == id).Select(a => new
                    {
                        a.ID,
                        a.Code,
                        a.Address,
                        a.ContactNo,
                        a.ContactPerson,
                        a.FeedBack,
                        a.ComnucationType,
                        a.Email,
                        a.Reason,
                        a.FollowUpDate,
                        a.CommunicationDate
                    }).FirstOrDefault();
                    ClientSurveyCommunicationVM _ClientSurveyCommunicationVM = new ClientSurveyCommunicationVM();
                    _ClientSurveyCommunicationVM.Id = editSurvey.ID;
                    _ClientSurveyCommunicationVM.Code = editSurvey.Code;
                    _ClientSurveyCommunicationVM.Address = editSurvey.Code;
                    _ClientSurveyCommunicationVM.ContactNo = editSurvey.ContactNo;
                    _ClientSurveyCommunicationVM.ContactPerson = editSurvey.ContactPerson;
                    _ClientSurveyCommunicationVM.FeedBack = editSurvey.FeedBack;
                    _ClientSurveyCommunicationVM.ComnucationType = editSurvey.ComnucationType;
                    _ClientSurveyCommunicationVM.Email = editSurvey.Email;
                    _ClientSurveyCommunicationVM.Reason = editSurvey.Reason;
                    string[] comDateString = editSurvey.CommunicationDate.ToShortDateString().Split('/');
                    string[] folDateString = editSurvey.FollowUpDate.GetValueOrDefault().ToShortDateString().Split('/');
                    _ClientSurveyCommunicationVM.CommunicationDate = comDateString[1] + "-" + comDateString[0] + "-" + comDateString[2];
                    _ClientSurveyCommunicationVM.FollowUpDate = folDateString[1] + "-" + folDateString[0] + "-" + folDateString[2];
                    return Json(new { success = true, data = _ClientSurveyCommunicationVM }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = false, message = "LogOut" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                return Json(new { success = false, message = exception.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Edits the survey save.
        /// </summary>
        /// <param name="survey">The survey.</param>
        /// <returns>Bool</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <CreatedDate>1_Nov_2016</CreatedDate>
        public JsonResult EditSurveySave(ClientSurveyCommunicationVM survey)
        {
            try
            {
                Dictionary<int, CheckSessionData> dictionary = CheckSessionData.GetSessionValues();
                int companyId = Convert.ToInt32(dictionary[1].Id);
                Guid userId = Guid.Parse(dictionary[3].Id);
                if (companyId != 0)
                {
                    ISecurityFactory _securityLogInFactory = new SecurityFactorys();
                    int userGroupID = Convert.ToInt32(dictionary[6].Id == "" ? 0 : Convert.ToInt32(dictionary[6].Id));
                    PagePermissionVM tblUserActionMapping = _securityLogInFactory.GetCRUDPermission(userGroupID, "ClientSurvey");
                    if (tblUserActionMapping.Edit)
                    {
                        _clientComnucationFactory = new ComnucationFactory();
                        TBLT_SURVEY_COMMUNICATION tblSurveyComuncation = _clientComnucationFactory.FindBy(x => x.ID == survey.Id).FirstOrDefault();
                        if (tblSurveyComuncation != null)
                        {
                            tblSurveyComuncation.ModifiedBy = userId;
                            tblSurveyComuncation.ModifiedDate = DateTime.Now;
                            tblSurveyComuncation.FeedBack = survey.FeedBack != null ? survey.FeedBack : tblSurveyComuncation.FeedBack;
                            tblSurveyComuncation.Code = survey.Code != null ? survey.Code : tblSurveyComuncation.Code;
                            tblSurveyComuncation.Address = survey.Address != null ? survey.Address : tblSurveyComuncation.Address;
                            tblSurveyComuncation.ContactNo = survey.ContactNo != null ? survey.ContactNo : tblSurveyComuncation.ContactNo;
                            tblSurveyComuncation.Email = survey.Email != null ? survey.Email : tblSurveyComuncation.Email;
                            tblSurveyComuncation.ComnucationType = survey.Text != null ? survey.Text : tblSurveyComuncation.ComnucationType;
                            tblSurveyComuncation.Reason = survey.Reason != null ? survey.Reason : tblSurveyComuncation.Reason;
                            string comDateString = survey.CommunicationDate == null ? "" : survey.CommunicationDate;
                            string[] comDateStringEdit = comDateString.Split('-');
                            tblSurveyComuncation.CommunicationDate = Convert.ToDateTime(comDateStringEdit[1] + "/" + comDateStringEdit[0] + "/" + comDateStringEdit[2]);
                            string follDateString = survey.FollowUpDate == null ? "" : survey.FollowUpDate;
                            string[] followUpDateEdit = follDateString.Split('-');
                            tblSurveyComuncation.FollowUpDate = Convert.ToDateTime(followUpDateEdit[1] + "/" + followUpDateEdit[0] + "/" + followUpDateEdit[2]);
                            //tblSurveyComuncation.FollowUpDate = survey.FollowUpDate != null ? Convert.ToDateTime(survey.CommunicationDate) : tblSurveyComuncation.FollowUpDate;
                            //tblSurveyComuncation.CommunicationDate = survey.CommunicationDate != null ? Convert.ToDateTime(survey.CommunicationDate) : tblSurveyComuncation.CommunicationDate;
                            _clientComnucationFactory.Edit(tblSurveyComuncation);
                        }
                        bool isDuplicate = _clientComnucationFactory.FindBy(x => x.Code.ToLower() == survey.Code.ToLower()).Where(x => x.ID != survey.Id)
                                          .Any(x => x.Code.ToLower() == survey.Code.ToLower());
                        if (!isDuplicate)
                        {
                            _clientComnucationFactory.Save();
                            return Json(new { success = true, message = "Data Updated successfully" }, JsonRequestBehavior.AllowGet);
                        }
                        return Json(new { success = false, message = "You'r entared code are duplicate" }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { success = false, message = "You has no Edit permission" }, JsonRequestBehavior.AllowGet);
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
        /// Surveys the status monitor.
        /// </summary>
        /// <returns>View</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <CreatedDate>2_Nov_2016</CreatedDate>
        public ActionResult SurveyStatusMonitor()
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
        /// Gets the traders.
        /// </summary>
        /// <returns>list</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <CreatedDate>2_Nov_2016</CreatedDate>
        public JsonResult GetTraders()
        {
            try
            {
                Dictionary<int, CheckSessionData> dictionary = CheckSessionData.GetSessionValues();
                long companyId = Convert.ToInt32(dictionary[1].Id == "" ? 0 : Convert.ToInt32(dictionary[1].Id));
                if (companyId != 0)
                {
                    _userFactory = new userFactory();
                    var traders = _userFactory.FindBy(x => x.CompanyID == companyId).Select(x=> new { 
                         x.UserName,
                         x.UserFullName
                    }).ToList();

                    return Json(new { success = true, data = traders }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = false, message = "LogOut" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                return Json(new { success = false, message = exception.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Gets the trader identifier for session.
        /// </summary>
        /// <param name="survayCom">The survay COM.</param>
        /// <returns>Json</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <CreatedDate>2_Nov_2016</CreatedDate>
        public ActionResult GetTraderIDForSession(ClientSurveyCommunicationVM survayCom)
        {
            try
            {
                Session["Trader"] = survayCom.Username;
                Session["FromDate"] = survayCom.FromDate.ToString();
                Session["ToDate"] = survayCom.ToDate.ToString();
                return Json(new { success = true, data = "OK" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                return Json(new { success = true, data = "Please select user, from date and to date properly" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Loads the survey history by identifier.
        /// </summary>
        /// <returns>List</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <CreatedDate>2_Nov_2016</CreatedDate>
        public ActionResult LoadSurveyHistoryById() 
        {
            try
            {
                Dictionary<int, CheckSessionData> dictionary = CheckSessionData.GetSessionValues();
                long companyId = Convert.ToInt32(dictionary[1].Id == "" ? 0 : Convert.ToInt32(dictionary[1].Id));
                if (companyId != 0)
                {
                    Guid userId = Guid.Parse(dictionary[3].Id);
                    _clientComnucationFactory = new ComnucationFactory();
                    _userFactory = new userFactory();
                    string userName = Session["Trader"].ToString();
                    Guid traderID = _userFactory.FindBy(x => x.UserName == userName).Select(x => x.ID).FirstOrDefault();

                    string fromDateString = Session["FromDate"].ToString();
                    string[] fromDateArray = fromDateString.Split('-');
                    DateTime fromDate = Convert.ToDateTime(fromDateArray[1] + "/" + fromDateArray[0] + "/" + fromDateArray[2]);
                    string toDateString = Session["ToDate"].ToString();
                    string[] toDateArray = toDateString.Split('-');
                    DateTime toDate = Convert.ToDateTime(toDateArray[1] + "/" + toDateArray[0] + "/" + toDateArray[2]).AddDays(1).AddSeconds(-1);
                    if (fromDate > toDate)
                    {
                    
                    }
                    //DateTime fromDate = Convert.ToDateTime(Session["FromDate"].ToString());
                    //DateTime toDate = Convert.ToDateTime(Session["ToDate"].ToString()).AddDays(1).AddSeconds(-1);
                    if (traderID == null && fromDate == null && toDate == null)
                    {
                        return Json(new { success = false, message = "Please select" }, JsonRequestBehavior.AllowGet);
                    }
                    var tblSurveyComuncation = _clientComnucationFactory.FindBy(x => x.CreatedBy == traderID && (x.CommunicationDate >= fromDate && x.CommunicationDate <= toDate)).ToList();
                    List<ClientSurveyCommunicationVM> surveyCommunicationList = new List<ClientSurveyCommunicationVM>();
                    foreach (var item in tblSurveyComuncation)
                    {
                        ClientSurveyCommunicationVM surveyCommunication = new ClientSurveyCommunicationVM();
                        surveyCommunication.Id = item.ID;
                        surveyCommunication.DateShow = item.CommunicationDate.ToShortDateString();
                        surveyCommunication.Address = item.Address;
                        surveyCommunication.ContactPerson = item.ContactPerson;
                        surveyCommunication.ContactNo = item.ContactNo;
                        surveyCommunication.FeedBack = item.FeedBack;
                        surveyCommunication.Email = item.Email;
                        surveyCommunication.FollowUpDateShow = item.FollowUpDate.GetValueOrDefault().ToShortDateString();
                        surveyCommunication.Type = item.ComnucationType;
                        surveyCommunicationList.Add(surveyCommunication);
                    }

                    var surveyHistory = surveyCommunicationList.Select(x => new
                    {
                        ID = x.Id,
                        x.DateShow,
                        x.Address,
                        x.ContactPerson,
                        x.ContactNo,
                        x.FeedBack,
                        x.FollowUpDateShow,
                        ComnucationType = x.Type
                    }).ToList();
                    return Json(surveyHistory);
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
                    string userName = Session["Trader"].ToString();
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


    }
}