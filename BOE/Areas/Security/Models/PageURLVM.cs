using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BOE.Areas.Security.Models
{
    public class PageURLVM
    {
        public int ID  {get;set;}
        public int ModuleID  {get;set;}
        public string UIPage { get; set; }
        public string URL { get; set; }
    }
}