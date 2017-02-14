using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BOE.Models
{
    public class MenuItemModels
    {
      public int MenuID {get;set;}
      public string ApplicationID { get; set; }
      public string ModuleID { get; set; }
      public int PageID { get; set; }
      public string ModuleName {get;set;}
      public string URL {get;set;}
      public string IconClass { get; set; }
    }
}