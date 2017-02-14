using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BOE.Areas.Security.Models
{
    public class UserVM
    {
        public System.Guid ID { get; set; }
        public string UserFullName { get; set; }
        public string UserName { get; set; }
        public string Address { get; set; }
        public string EMail { get; set; }
        public string PhoneNo { get; set; }
        public System.Guid PasswordID { get; set; }
        public System.Guid SecurityQuestionID { get; set; }
        public bool IsEMailVerified { get; set; }
        public bool IsPhoneNoVerified { get; set; }
        public bool IsActive { get; set; }
        public int LocationID { get; set; }
        public int CompanyID { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public System.Guid CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<System.Guid> ModifiedBy { get; set; }
        public int UserGroupID { get; set; }
        public string SecurityQuestion { get; set; }
        public string SecurityQueAns { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Code { get; set; }
        public string BOCode { get; set; }
        public string TradingCode { get; set; }
        public bool IsClient { get; set; }
    }
}