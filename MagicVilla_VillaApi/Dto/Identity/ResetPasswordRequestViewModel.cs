using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CQRS_test.ViewModels.Identity
{
    public class ResetPasswordRequestViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        [Remote("IsEmailNotAvailable", "Account", ErrorMessage = "Email Not Exist")]
        public string Email { get; set; }
    }
}
