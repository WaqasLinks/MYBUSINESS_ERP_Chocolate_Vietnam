using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MYBUSINESS.Models
{
  
  public class ResetPassword2ViewModel
  {
    //[Required]
    //[EmailAddress]
    //[Display(Name = "Email")]
    //public string Email { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; }

    //[DataType(DataType.Password)]
    //[Display(Name = "Confirm password")]
    //[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    //public string ConfirmPassword { get; set; }

    public string Code { get; set; }
  }
  
}
