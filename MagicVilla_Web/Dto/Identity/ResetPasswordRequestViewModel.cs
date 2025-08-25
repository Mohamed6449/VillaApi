using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MagicVilla_Web.ViewModels.Identity
{
    public class ResetPasswordRequestViewModel
    {
        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
    }
}
