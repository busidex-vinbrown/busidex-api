using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Busidex4.Models
{

    public class ChangePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginModel
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

        public string CardOwnerToken { get; set; }
        public string FrontFileId { get; set; }
    }

    public class RegisterModel
    {

        private const string MatchEmailPattern =
            @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
     + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
     + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
     + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";

        [Required]
        [DataType(DataType.Text)]
        [StringLength(256, MinimumLength = 5, ErrorMessage = "{0} must be between {2} and {1} characters long")]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [StringLength(256, MinimumLength = 4, ErrorMessage = "{0} must be between {2} and {1} characters long")]
        [RegularExpression(MatchEmailPattern, ErrorMessage = "Please enter a valid email address")]
        [Display(Name = "Email address")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Confirm Email address")]
        [Compare("Email", ErrorMessage = "The Email address and confirmation Email address do not match")]
        public string ConfirmEmail { get; set; }

        [Required]
        [StringLength(128, MinimumLength = 8, ErrorMessage = "{0} must be between {2} and {1} characters long")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [IsTrue]
        public bool Agree { get; set; }

        [Required]
        public int AccountTypeId { get; set; }

        public string CardOwnerToken { get; set; }
        public bool IsMyCard { get; set; }

        public string StepHash { get; set; }
        public Guid? FrontFileId { get; set; }
        public string FrontFileType { get; set; }
        public string HumanQuestion { get; set; }
        public string HumanAnswer { get; set; }
        public bool IsValid { get; set; }
    }

    public class IsTrueAttribute : ValidationAttribute
    {
        #region Overrides of ValidationAttribute

        /// <summary>
        /// Determines whether the specified value of the object is valid. 
        /// </summary>
        /// <returns>
        /// true if the specified value is valid; otherwise, false. 
        /// </returns>
        /// <param name="value">The value of the specified validation object on which the <see cref="T:System.ComponentModel.DataAnnotations.ValidationAttribute"/> is declared.
        ///                 </param>
        public override bool IsValid(object value)
        {
            if (value == null) return false;
            if (value.GetType() != typeof(bool)) throw new InvalidOperationException("can only be used on boolean properties.");

            return (bool)value;
        }

        #endregion
    }
}
