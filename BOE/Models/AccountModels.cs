using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BOE.Models
{
    public class AccountModels
    {
        public class ChangePasswordModel
        {
            //[Required]
            [Display(Name = "User Name")]
            public string UserName { get; set; }

            //[Required]
            [DataType(DataType.Password)]
            [Display(Name = "Old password")]
            public string OldPassword { get; set; }

            //[Required]
            //[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "New password")]
            public string NewPassword { get; set; }

            //[Required]
            [DataType(DataType.Password)]
            [Display(Name = "Confirm new password")]
            //[Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            public bool IsSecurityQuestionUpdate { get; set; }
            public long SecretQuestionId { get; set; }

            [Display(Name = "Secret Question")]
            public string SecretQuestion { get; set; }

            [Display(Name = "Secret Question Answer")]
            public string SecretQuestionAnswer { get; set; }

            public long CompanyId { get; set; }
            public long LocationId { get; set; }

            public string CompanyName { get; set; }
            public string LocationName { get; set; }
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

        public class RegisterModel
        {
            [Required]
            [Display(Name = "User name")]
            public string UserName { get; set; }

            [Required]
            [DataType(DataType.EmailAddress)]
            [Display(Name = "Email address")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            //[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            public long CompanyId { get; set; }
            public long LocationId { get; set; }
        }
    }
}