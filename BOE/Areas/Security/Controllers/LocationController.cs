using BOEService.Entites.BOE;
using BOEService.Factories;
using BOEService.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BOEUtility.Helper;
using BOEService.Models;

namespace BOE.Areas.Security.Controllers
{
    public class LocationController : Controller
    {

        private IGenericFactory<TBLB_LOCATION> _locationFactory;

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns>View</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <Date>08-Aug-2016</Date>
        public ActionResult Index()
        {
            Dictionary<int, CheckSessionData> dictionary = CheckSessionData.GetSessionValues();
            long companyId = Convert.ToInt32(dictionary[1].Id == ""? 0:Convert.ToInt32(dictionary[1].Id));
            int userGroupID = Convert.ToInt32(dictionary[6].Id == "" ? 0 : Convert.ToInt32(dictionary[6].Id));
            if (companyId != 0)
            {
                ISecurityFactory _securityLogInFactory = new SecurityFactorys();
                PagePermissionVM tblUserActionMapping = _securityLogInFactory.GetCRUDPermission(userGroupID, "Location");
                if (tblUserActionMapping.Select)
                {
                    return View();
                }
            }
            Session["logInSession"] = "false";
            return Redirect("/#/");
        }

        /// <summary>
        /// Loads all location.
        /// </summary>
        /// <returns>Json</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <Date>08-Aug-2016</Date>
        public ActionResult LoadAllLocation()
        {
            try
            {
                Dictionary<int, CheckSessionData> dictionary = CheckSessionData.GetSessionValues();
                long companyId = Convert.ToInt32(dictionary[1].Id == ""? 0:Convert.ToInt32(dictionary[1].Id));
                if (companyId != 0)
                {
                    _locationFactory = new LocationFactory();
                    var locations = _locationFactory.FindBy(x=>x.CompanyID == companyId).Select(x => new
                    {
                        x.ID,
                        x.Name,
                        x.Code,
                        x.Address,
                        x.PhoneNo,
                        x.Email,
                        ParentLocation = x.TBLB_LOCATION2.Name.ToString()??""
                    }).ToList();
                    return Json(locations);
                }
                return Json(new { success = false, message = "LogOut" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                return Json(new { success = false, message = exception.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Creates the location.
        /// </summary>
        /// <returns>View</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <Date>08-Aug-2016</Date>
        public ActionResult CreateLocation()
        {
            Dictionary<int, CheckSessionData> dictionary = CheckSessionData.GetSessionValues();
            long companyId = Convert.ToInt32(dictionary[1].Id == ""? 0:Convert.ToInt32(dictionary[1].Id));
            int userGroupID = Convert.ToInt32(dictionary[6].Id == "" ? 0 : Convert.ToInt32(dictionary[6].Id));
            if (companyId != 0)
            {
                ISecurityFactory _securityLogInFactory = new SecurityFactorys();
                PagePermissionVM tblUserActionMapping = _securityLogInFactory.GetCRUDPermission(userGroupID, "Location");
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
        /// <returns>Json</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <Date>08-Aug-2016</Date>
        public JsonResult GetAllParentLocations()
        {
            try
            {
                Dictionary<int, CheckSessionData> dictionary = CheckSessionData.GetSessionValues();
                long companyId = Convert.ToInt32(dictionary[1].Id == ""? 0:Convert.ToInt32(dictionary[1].Id));
                if (companyId != 0)
                {
                    _locationFactory = new LocationFactory();
                    var locations = _locationFactory.FindBy(x=>x.CompanyID == companyId).Select(x => new
                    {
                        ParentLocationId = x.ID,
                        Name = x.Name
                    }).ToList();
                    return Json(new { success = true, data = locations.OrderBy(x => x.Name) }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = false, message = "LogOut"}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                return Json(new { success = false, message = exception.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Creates the location save.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <returns>Json/Bool</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <Date>08-Aug-2016</Date>
        /// <ModificationDate>24-Nov_2016</ModificationDate>
        public JsonResult CreateLocationSave(TBLB_LOCATION location)
        {
            try
            {
                Dictionary<int, CheckSessionData> dictionary = CheckSessionData.GetSessionValues();
                int companyId = Convert.ToInt32(dictionary[1].Id);
                Guid userId = Guid.Parse(dictionary[3].Id);
                if (companyId != 0)
                {
                    _locationFactory = new LocationFactory();
                    location.CreatedDate = DateTime.Now;
                    location.CreatedBy = userId;
                    location.CompanyID = companyId;
                    location.MobileNo = location.MobileNo;
                    location.IsActive = true;
                    _locationFactory.Add(location);
                    bool isDuplicate = _locationFactory.FindBy(x => x.Code.ToLower() == location.Code.ToLower() || x.Name.ToLower() == location.Name.ToLower())
                                       .Any(x => x.Name.ToLower() == location.Name.ToLower() || x.Code.ToLower() == location.Code.ToLower());
                    if (!isDuplicate)
                    {
                        _locationFactory.Save();
                        return Json(new { success = true, message = "Data Updated successfully" }, JsonRequestBehavior.AllowGet);
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
        /// Edits the location.
        /// </summary>
        /// <returns>View</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <Date>08-Aug-2016</Date>
        public ActionResult EditLocation()
        {
            Dictionary<int, CheckSessionData> dictionary = CheckSessionData.GetSessionValues();
            long companyId = Convert.ToInt32(dictionary[1].Id == ""? 0:Convert.ToInt32(dictionary[1].Id));
            int userGroupID = Convert.ToInt32(dictionary[6].Id == "" ? 0 : Convert.ToInt32(dictionary[6].Id));
            if (companyId != 0)
            {
                ISecurityFactory _securityLogInFactory = new SecurityFactorys();
                PagePermissionVM tblUserActionMapping = _securityLogInFactory.GetCRUDPermission(userGroupID, "Location");
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
        /// <returns>Json</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <Date>08-Aug-2016</Date>
        public JsonResult LoadLocationById(int id)
        {
            try
            {  
                Dictionary<int, CheckSessionData> dictionary = CheckSessionData.GetSessionValues();
                long companyId = Convert.ToInt32(dictionary[1].Id == ""? 0:Convert.ToInt32(dictionary[1].Id));
                if (companyId != 0)
                {
                    _locationFactory = new LocationFactory();
                    var editLocation = _locationFactory.FindBy(x => x.ID == id).Select(a => new
                    {
                        a.ID,
                        a.Name,
                        a.Code,
                        a.Address,
                        a.PhoneNo,
                        a.Email,
                        a.MobileNo,
                        a.FaxNo,
                        a.ParentLocationId
                    }).FirstOrDefault();
                    return Json(new { success = true, data = editLocation }, JsonRequestBehavior.AllowGet);
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
        /// <param name="parentLocationId">The parent location identifier.</param>
        /// <returns>Json</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <Date>08-Aug-2016</Date>
        public JsonResult LoadLocationByLocationId(int parentLocationId)
        {
            try
            {
                _locationFactory = new LocationFactory();
                var parentLocation = _locationFactory.FindBy(x => x.ID == parentLocationId)
                    .Select(a => new
                    {
                        ParentLocationId = a.ID,
                        a.Name
                    }
                    ).ToList();

                return Json(new { success = true, data = parentLocation }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                return Json(new { success = false, message = exception.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Edits the location save.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <returns>Bool</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <Date>08-Aug-2016</Date>
        public JsonResult EditLocationSave(TBLB_LOCATION location)
        {
            try
            {
                Dictionary<int, CheckSessionData> dictionary = CheckSessionData.GetSessionValues();
                int companyId = Convert.ToInt32(dictionary[1].Id);
                Guid userId = Guid.Parse(dictionary[3].Id);
                if (companyId != 0)
                {
                    _locationFactory = new LocationFactory();
                    TBLB_LOCATION tbllocation = _locationFactory.FindBy(x => x.ID == location.ID).FirstOrDefault();
                    if (tbllocation != null)
                    {
                        tbllocation.ModifiedBy = userId;
                        tbllocation.ModifiedDate = DateTime.Now;
                        tbllocation.Name = location.Name;
                        tbllocation.Code = location.Code;
                        tbllocation.Address = location.Address;
                        tbllocation.PhoneNo = location.PhoneNo;
                        tbllocation.MobileNo = location.MobileNo;
                        tbllocation.FaxNo = location.FaxNo;
                        tbllocation.Email = location.Email;
                        tbllocation.ParentLocationId = location.ParentLocationId;
                        _locationFactory.Edit(tbllocation);
                    }
                    bool isDuplicate = _locationFactory.FindBy(x => x.Code.ToLower() == location.Code.ToLower()).Where(x => x.ID != location.ID)
                                      .Any(x => x.Name.ToLower() == location.Name.ToLower() || x.Code.ToLower() == location.Code.ToLower());
                    if (!isDuplicate)
                    {
                        _locationFactory.Save();
                        return Json(new { success = true, message = "Data Updated successfully" }, JsonRequestBehavior.AllowGet);
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
        /// Deletes the location.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>bool</returns>
        /// <CreatedBy>Avishek</CreatedBy>
        /// <Createddate>9_Oct_2016</Createddate>
        public JsonResult DeleteLocation(int id) 
        {
            try
            {	
                Dictionary<int, CheckSessionData> dictionary = CheckSessionData.GetSessionValues();
                long companyId = Convert.ToInt32(dictionary[1].Id == ""? 0:Convert.ToInt32(dictionary[1].Id));
                int userGroupID = Convert.ToInt32(dictionary[6].Id == "" ? 0 : Convert.ToInt32(dictionary[6].Id));
                if (companyId != 0)
                {
                    ISecurityFactory _securityLogInFactory = new SecurityFactorys();
                    PagePermissionVM tblUserActionMapping = _securityLogInFactory.GetCRUDPermission(userGroupID, "Location");
                    if (tblUserActionMapping.Delete)
                    {
                        _locationFactory = new LocationFactory();
                        _locationFactory.Delete(x => x.ID == id);
                        _locationFactory.Save();
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
    }
}