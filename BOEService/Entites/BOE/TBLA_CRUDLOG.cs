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
    
    public partial class TBLA_CRUDLOG
    {
        public int ID { get; set; }
        public System.Guid UserID { get; set; }
        public string IPAddress { get; set; }
        public string MacAddress { get; set; }
        public string DeviceAddress { get; set; }
        public string DeviceName { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string TableName { get; set; }
        public Nullable<int> PageID { get; set; }
        public string Action { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public int LocationID { get; set; }
        public int CompanyID { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    
        public virtual TBLB_COMPANY TBLB_COMPANY { get; set; }
        public virtual TBLB_LOCATION TBLB_LOCATION { get; set; }
        public virtual TBLA_UI_PAGE TBLA_UI_PAGE { get; set; }
        public virtual TBLA_USER_INFORMATION TBLA_USER_INFORMATION { get; set; }
    }
}
