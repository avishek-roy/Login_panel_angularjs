//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BOEService.Entites.BOE
{
    using System;
    using System.Collections.Generic;
    
    public partial class TBLA_USER_ACTION_MAPPING
    {
        public int ID { get; set; }
        public int UserGroupID { get; set; }
        public int ApplicationID { get; set; }
        public int UIModuleID { get; set; }
        public int UIPageID { get; set; }
        public bool IsSelect { get; set; }
        public bool IsCreate { get; set; }
        public bool IsEdit { get; set; }
        public bool IsDelete { get; set; }
        public Nullable<System.Guid> UrlID { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public System.Guid CreatedBy { get; set; }
        public int LocationID { get; set; }
        public int CompanyID { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<System.Guid> ModifiedBy { get; set; }
    
        public virtual TBLA_APPLICATION_MODULE TBLA_APPLICATION_MODULE { get; set; }
        public virtual TBLA_UI_MODULE TBLA_UI_MODULE { get; set; }
        public virtual TBLA_UI_PAGE TBLA_UI_PAGE { get; set; }
        public virtual TBLA_URL TBLA_URL { get; set; }
        public virtual TBLB_COMPANY TBLB_COMPANY { get; set; }
        public virtual TBLB_LOCATION TBLB_LOCATION { get; set; }
        public virtual TBLA_USER_GROUP TBLA_USER_GROUP { get; set; }
        public virtual TBLA_USER_INFORMATION TBLA_USER_INFORMATION { get; set; }
        public virtual TBLA_USER_INFORMATION TBLA_USER_INFORMATION1 { get; set; }
    }
}
