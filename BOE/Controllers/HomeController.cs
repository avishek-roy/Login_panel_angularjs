using BOE.Areas.Security.Models;
using BOE.Models;
using BOEService.Entites.BOE;
using BOEService.Factories;
using BOEService.Interfaces;
using BOEUtility.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace BOE.Controllers
{
    public class HomeController : Controller
    {
        private IGenericFactory<TBLB_COMPANY> _factory;
        //private IGenericFactory<tblURL> _urlFactory;
        //private IGenericFactory<tblUIApplicationModule> _applicationFactory;
        //private IGenericFactory<tblUIPage> _uiPageFactory;
        //private IGenericFactory<tblUIModule> _uiModuleFactory;
        private IGenericFactory<TBLA_USER_ACTION_MAPPING> _uiMappingFactory;

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult HomeMaster()
        {
            return View();
        }
        public JsonResult GetSiteMenu() 
        {
            Dictionary<int, CheckSessionData> dictionary = CheckSessionData.GetSessionValues();
            int userGroupID = Convert.ToInt32(dictionary[6].Id == "" ? 0 : Convert.ToInt32(dictionary[6].Id));
            _uiMappingFactory = new userActionMappingFactory();
            ISecurityFactory _securityLogInFactory = new SecurityFactorys();
            //_uiPageFactory = new uiPageFactory();
            //_uiModuleFactory = new moduleFactory();
            //_urlFactory = new UrlFactory();

            //var url = _urlFactory.GetAll();
            //var module = _uiModuleFactory.GetAll().ToList();
            //var page = _uiPageFactory.GetAll();
           
            //List<PageURLVM> _pageURLVMList = new List<PageURLVM>();
            //foreach (var item in page)
            //{
            //    PageURLVM pageURLVM = new PageURLVM();
            //    pageURLVM.ID = item.ID;
            //    pageURLVM.ModuleID = item.ModuleID;
            //    pageURLVM.UIPage = item.Name;
            //    pageURLVM.URL = url.Where(x => x.UIPageID == item.ID).Select(x => x.Url).FirstOrDefault();
            //    _pageURLVMList.Add(pageURLVM);
            //}


            //var menu = from item in module select new
            //               {
            //                   ID = item.ID,
            //                   Name = item.Name,
            //                   Submodule = _pageURLVMList.Where(x => x.ModuleID == item.ID).ToList()
            //               };

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            try
            {
                string path = path = HttpContext.Server.MapPath("~/App_Data/Menu.xml"); ;

                DataSet ds = new DataSet();
                ds.ReadXml(path);
                //XDocument xmlDoc = XDocument.Load(path);
                //var list = xmlDoc.Root.Select(element => element.Value)
                //                           .ToList();
                List<MenuItemModels> menuList = new List<MenuItemModels>();
                foreach (DataTable dt in ds.Tables)
                {
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            //for (int j = 0; j <= dt.Rows[i].ItemArray.Length; j++)
                            //{
                                MenuItemModels menuItem = new MenuItemModels();
                                menuItem.MenuID = Convert.ToInt32(dt.Rows[i].ItemArray[0]);
                                menuItem.ApplicationID = dt.Rows[i].ItemArray[1].ToString();
                                menuItem.ModuleID = dt.Rows[i].ItemArray[2].ToString();
                                menuItem.PageID = Convert.ToInt32(dt.Rows[i].ItemArray[3]);
                                menuItem.ModuleName = dt.Rows[i].ItemArray[4].ToString();
                                menuItem.URL = dt.Rows[i].ItemArray[5].ToString();
                                menuItem.IconClass = dt.Rows[i].ItemArray[6].ToString();
                                menuList.Add(menuItem);
                           // }
                        }
                    }
                }

                List<TBLA_USER_ACTION_MAPPING> mapping = _uiMappingFactory.FindBy(x => (x.UserGroupID == userGroupID) && (x.IsCreate == true || x.IsDelete == true || x.IsEdit == true || x.IsSelect == true)).ToList();

                var menu = from XML in menuList
                           from MAP in mapping
                           where XML.PageID == MAP.UIPageID
                           select new {  XML.ApplicationID, 
                                         XML.ModuleID, 
                                         XML.ModuleName, 
                                         XML.URL, 
                                         XML.IconClass 
                           };

                //var menu = (from XML in menuList
                //            join MAP in mapping
                //            on new { XML.ApplicationID, XML.ModuleID, XML.PageID } equals new { MAP.ApplicationID, MAP.UIModuleID, MAP.UIPageID }
                //            select new { XML.ApplicationID });
                return Json(new { data = menu }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                Session["XmlMessage"] = "You're file format is not OK";
            }














////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            //var menu = _securityLogInFactory.PagePermissedList(userGroupID);

            return Json(new { data = "" }, JsonRequestBehavior.AllowGet);
        }


    }
}