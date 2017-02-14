using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BOE.Areas.Security.Models
{
    public class UserMappingVM
    {
        public int UserGroupId { get; set; }
        public int ID { get; set; }
        public string Select { get; set; }
        public string Edit { get; set; }
        public string Create { get; set; }
        public string Delete { get; set; }
        public int ApplicationID { get; set; }
        public string Application { get; set; }
        public int ModuleID { get; set; }
        public string Module { get; set; }
        public int UIPageID { get; set; }
    }
}