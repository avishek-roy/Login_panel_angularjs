using BOEService.Entites.BOE;
using BOEService.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BOEService.Models;
using BOEUtility.Helper;

namespace BOEService.Factories
{
    public class CompanyFactory : GenericFactory<BOEEntities, TBLB_COMPANY>
    {
        
    }

    public class LocationFactory : GenericFactory<BOEEntities, TBLB_LOCATION>
    {

    }

    public class userFactory : GenericFactory<BOEEntities, TBLA_USER_INFORMATION>
    {

    }

    public class userPasswordFactory : GenericFactory<BOEEntities, TBLA_PASSWORD >
    {

    }

    public class loginStatusFactory : GenericFactory<BOEEntities, TBLA_LOGIN_STATUS>
    {

    }

    public class questionFactory : GenericFactory<BOEEntities, TBLA_SECURITY_QUESTION>
    {

    }

    public class userGroupFactory : GenericFactory<BOEEntities, TBLA_USER_GROUP>
    {

    }

    public class userActionMappingFactory : GenericFactory<BOEEntities, TBLA_USER_ACTION_MAPPING>
    {

    }

    public class uiPageFactory : GenericFactory<BOEEntities, TBLA_UI_PAGE>
    {

    }

    public class applicationModuleFactory : GenericFactory<BOEEntities, TBLA_APPLICATION_MODULE>
    {

    }

    public class moduleFactory : GenericFactory<BOEEntities, TBLA_UI_MODULE>
    {

    }

    public class urlPageFactory : GenericFactory<BOEEntities, TBLA_UI_PAGE>
    {

    }

    public class SecurityFactorys : ISecurityFactory
    {

        public class Result
        {
            public string message { get; set; }
            public bool isSucess { get; set; }

        }

        private BOEEntities context;
        private IGenericFactory<TBLB_COMPANY> factory;
        private IGenericFactory<TBLB_LOCATION> locationFactory;
        private IGenericFactory<TBLA_USER_INFORMATION> userFactory;
        private IGenericFactory<TBLA_PASSWORD> userPasswordFactory;
        private IGenericFactory<TBLA_UI_PAGE> _uiPageFactory;
        private IGenericFactory<TBLA_URL> _urlFactory;
        private IGenericFactory<TBLA_APPLICATION_MODULE> _applicationModuleFactory;
        private IGenericFactory<TBLA_UI_MODULE> _moduleFactory;
        private IGenericFactory<TBLA_USER_ACTION_MAPPING> _userActionMappingFactory;
        private IGenericFactory<TBLA_LOGIN_STATUS> _loginStatusFactory;

        public SecurityFactorys()
        {
            this.context = new BOEEntities();
        }

        public SecurityFactorys(BOEEntities context)
        {
            this.context = context;
        }

        /// <summary>
        /// Checks the log in.
        /// </summary>
        /// <param name="_Entity">The _ entity.</param>
        /// <returns>Bool</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <CreatedDate>20_Sep_2016</CreatedDate>
        public LogInStatus CheckLogIn(LogOnModel _Entity)
        {
            LogInStatus _LogInStatus = new LogInStatus();
            Dictionary<string, string> _list = new Dictionary<string, string>();
            Encription _encription = new Encription();
            try
            {
                _loginStatusFactory = new loginStatusFactory();
                userFactory = new userFactory();
                TBLA_USER_INFORMATION _tblUserInformation = userFactory.FindBy(x=>x.UserName == _Entity.UserName && x.IsActive == true).FirstOrDefault();
                if (_tblUserInformation != null)
                {
                    TBLA_LOGIN_STATUS logInStatus = _loginStatusFactory.FindBy(x => x.UserID == _tblUserInformation.ID).FirstOrDefault();
                    if (logInStatus != null)
                    {
                        //if (logInStatus.PresentLogInStatus == true)
                        //{
                        //    _LogInStatus.IsAllowed = false;
                        //    _LogInStatus.Message = "You are already loged in throw another device";
                        //}
                        //else 
                        if (logInStatus.ForcedLogOutStatus == true)
                        {
                            _LogInStatus.IsAllowed = false;
                            _LogInStatus.Message = "Some maintainence work are processing";
                        }
                        else {
                            userPasswordFactory = new userPasswordFactory();
                            factory = new CompanyFactory();
                            locationFactory = new LocationFactory();
                            TBLA_PASSWORD tblPassword = userPasswordFactory.FindBy(x => x.ID == _tblUserInformation.PasswordID).FirstOrDefault();
                            string u = _encription.Decrypt(tblPassword.NewPassword).Trim();
                            if (_encription.Decrypt(tblPassword.NewPassword).Trim() == (_Entity.Password.Trim()))
                            {

                                if (_tblUserInformation != null)
                                {
                                    //_list.Add("UserId", _tblUserInformation.ID.ToString());
                                    _list.Add("UserName", _tblUserInformation.UserName);
                                    _list.Add("Name", _tblUserInformation.UserFullName);

                                    TBLB_COMPANY _Company = factory.FindBy(x => x.ID == _tblUserInformation.CompanyID).FirstOrDefault();
                                    _list.Add("CompanyId", _Company.ID.ToString());
                                    _list.Add("Company", _Company.Name);


                                    TBLB_LOCATION _Location = locationFactory.FindBy(x => x.ID == _tblUserInformation.LocationID).FirstOrDefault();
                                    _list.Add("LocationId", _Location.ID.ToString());
                                    _list.Add("Location", _Location.Name);

                                    _LogInStatus.IsAllowed = true;
                                    _LogInStatus.Status = _list;
                                    _LogInStatus.Message = "Login Successfully";
                                }
                            }
                            else
                            {
                                _LogInStatus.IsAllowed = false;
                                _LogInStatus.Message = "Password or User Name not matching";
                            }
                        }
                    }
                    else 
                    { 
                        userPasswordFactory = new userPasswordFactory();
                        factory = new CompanyFactory();
                        locationFactory = new LocationFactory();
                        TBLA_PASSWORD tblPassword = userPasswordFactory.FindBy(x => x.ID == _tblUserInformation.PasswordID).FirstOrDefault();
                        string u = _encription.Decrypt(tblPassword.NewPassword).Trim();
                        if (_encription.Decrypt(tblPassword.NewPassword).Trim() == (_Entity.Password.Trim()))
                        {

                            if (_tblUserInformation != null)
                            {
                                //_list.Add("UserId", _tblUserInformation.ID.ToString());
                                _list.Add("UserName", _tblUserInformation.UserName);
                                _list.Add("Name", _tblUserInformation.UserFullName);

                                TBLB_COMPANY _Company = factory.FindBy(x => x.ID == _tblUserInformation.CompanyID).FirstOrDefault();
                                _list.Add("CompanyId", _Company.ID.ToString());
                                _list.Add("Company", _Company.Name);


                                TBLB_LOCATION _Location = locationFactory.FindBy(x => x.ID == _tblUserInformation.LocationID).FirstOrDefault();
                                _list.Add("LocationId", _Location.ID.ToString());
                                _list.Add("Location", _Location.Name);

                                _LogInStatus.IsAllowed = true;
                                _LogInStatus.Status = _list;
                                _LogInStatus.Message = "Login Successfully";
                            }
                        }
                        else
                        {
                            _LogInStatus.IsAllowed = false;
                            _LogInStatus.Message = "Password or User Name not matching";
                        }  
                    }
                }
                else
                {
                    _LogInStatus.IsAllowed = false;
                    _LogInStatus.Message = "User are not exist";
                }

                return _LogInStatus;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Pages the permissed list.
        /// </summary>
        /// <param name="userGroupID">The user group identifier.</param>
        /// <returns>List</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <CreatedDate>2_Oct_2016</CreatedDate>
        public dynamic PagePermissedList(int userGroupID) 
        {
            try
            {
                var application = (from APP in context.TBLA_APPLICATION_MODULE
                                   join MAP in context.TBLA_USER_ACTION_MAPPING.Where(x => (x.UserGroupID == userGroupID) && (x.IsCreate == true || x.IsDelete == true || x.IsEdit == true || x.IsSelect == true))
                                   on APP.ID equals MAP.ApplicationID
                                   select new
                                   {
                                       ApplicationID = MAP.ApplicationID,
                                       ModuleID = APP.ID,
                                       Name = APP.Name
                                   }).Distinct();

                var modules = (from MOD in context.TBLA_UI_MODULE
                               join MAP in context.TBLA_USER_ACTION_MAPPING.Where(x => (x.UserGroupID == userGroupID) && (x.IsCreate == true || x.IsDelete == true || x.IsEdit == true || x.IsSelect == true))
                               on MOD.ID equals MAP.UIModuleID
                               select new
                               {
                                   ApplicationID = MAP.ApplicationID,
                                   ModuleID = MOD.ID,
                                   Name = MOD.Name
                               }).Distinct();

                var urlWithPage = (from MAP in context.TBLA_USER_ACTION_MAPPING.Where(x => (x.UserGroupID == userGroupID) && (x.IsCreate == true || x.IsDelete == true || x.IsEdit == true || x.IsSelect == true))
                                   join UIP in context.TBLA_UI_PAGE on MAP.UIPageID equals UIP.ID
                                   join URL in context.TBLA_URL on UIP.UrlID equals URL.ID
                                   select new
                                   {
                                       ID = UIP.ID,
                                       Module = MAP.UIModuleID,
                                       UIPage = UIP.UIPageName,
                                       IsSelect = MAP.IsSelect,
                                       IsCreate = MAP.IsCreate,
                                       IsEdit = MAP.IsEdit,
                                       IsDelete = MAP.IsDelete,
                                       URL = URL.Url
                                   });

                var menuModule = from item in modules
                           select new
                           {
                               ModuleID = item.ModuleID,
                               ApplicationID = item.ApplicationID,
                               ModuleName = item.Name,
                               SubSubmodule = urlWithPage.Where(x => x.Module == item.ModuleID).ToList()
                           };

                var menu = from item in application
                                 select new
                                 {
                                     ApplicationID = item.ApplicationID,
                                     ModuleID = item.ModuleID,
                                     Name = item.Name,
                                     Submodule = menuModule.Where(x => x.ModuleID == item.ModuleID).ToList()
                                 };

                return menu;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets the crud permission.
        /// </summary>
        /// <param name="userGroupID">The user group identifier.</param>
        /// <param name="pageName">Name of the page.</param>
        /// <returns>Object</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <CreatedDtae>2_Oct_2016</CreatedDtae>
        public PagePermissionVM GetCRUDPermission(int userGroupID, string pageName) 
        {
            try
            {
                Dictionary<int, CheckSessionData> dictionary = CheckSessionData.GetSessionValues();
                Guid userId = Guid.Parse(dictionary[3].Id);
                bool forcedLogInStatus = context.TBLA_LOGIN_STATUS.Where(x => x.UserID == userId && x.ForcedLogOutStatus == true).FirstOrDefault() == null ? false:true;

                PagePermissionVM accountmapping = (from MAP in context.TBLA_USER_ACTION_MAPPING.Where(x => (x.UserGroupID == userGroupID) && (x.IsCreate == true || x.IsDelete == true || x.IsEdit == true || x.IsSelect == true))
                                                   join UIP in context.TBLA_UI_PAGE.Where(x=>x.Name.Trim().ToLower() == pageName.Trim().ToLower()) on MAP.UIPageID equals UIP.ID
                                                   select new PagePermissionVM()
                                                        {
                                                            ID = MAP.ID,
                                                            UserGroupId = MAP.UserGroupID,
                                                            Select = MAP.IsSelect,
                                                            Create = MAP.IsCreate,
                                                            Edit = MAP.IsEdit,
                                                            Delete = MAP.IsDelete,
                                                            ForcedLogOut = forcedLogInStatus
                                                        }).FirstOrDefault();
                return accountmapping;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



    }                                                                                                           
}
