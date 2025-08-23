using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CQRS_test.ViewModels.Identity
{
    public class LoginViewModel
    {

        [Required(ErrorMessage = "Email is required")]

        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        public bool RemberMe { get; set; } = false;

        public string? ReturnUrl { get; set; }


    }
}
