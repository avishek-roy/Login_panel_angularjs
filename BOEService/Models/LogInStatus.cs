using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOEService.Models
{
    public class LogInStatus
    {
        public bool IsAllowed { get; set; }
        public string Message { get; set; }

        public Dictionary<string, string> Status = new Dictionary<string, string>();
    }

    public class LogOnModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public long CompanyId { get; set; }
        public long LocationId { get; set; }
    }
}
