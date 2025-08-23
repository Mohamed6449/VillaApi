using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CQRS_test.ViewModels.Identity
{
    public class RegisterViewModel
    {

        [Required(ErrorMessage = "Name Is Required")]
        public string Name { get; set; }

        public string? Address { get; set; }

        [Required(ErrorMessage = "UserName is required")]
        [Remote("IsUserNameAvailable", "Account", ErrorMessage = "UserNameIsExist")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        [Remote("IsEmailAvailable", "Account", ErrorMessage = "EmailIsExist")]

        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "ConfirmPassword is required")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password),ErrorMessage ="The password and Confirmation password not match")]
        public string ConfirmPassword { get; set;}
    
    }
}
