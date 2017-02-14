using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BOE.Areas.Security.Models
{
    public class ChangePasswordVM
    {
        public string Answre { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
    }
}