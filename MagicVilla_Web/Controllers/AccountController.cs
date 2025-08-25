

using ClassLibrary1;
using MagicVilla_Web.Dto.ApiResponses;
using MagicVilla_Web.Dto.Identity;
using MagicVilla_Web.Services;
using MagicVilla_Web.ViewModels.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;
using System.Net;
using System.Security.Claims;

namespace MagicVilla_VillaApi.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountServices _accountService;

        public AccountController(IAccountServices accountService)
        {
            _accountService = accountService;

        }
        [HttpGet]

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Login(LoginViewModel model)
        {
         var result= await _accountService.Login<ApiResponse>(model);
            if (result.Success)
            {
                var userData = JsonConvert.DeserializeObject<DtoUser>(Convert.ToString(result.result));
                
                
                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                identity.AddClaim(new Claim(ClaimTypes.Name, userData.UserName));
                foreach(var item in userData.Roles)
                {
                identity.AddClaim(new Claim(CookieAuthenticationDefaults.AuthenticationScheme,ClaimTypes.Role, item));
                }
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,principal);

                HttpContext.Session.SetString(SD.KeySessionJWT,userData.Teken);
               return RedirectToAction("Index", "Home");
            }
            if(result.statusCode== HttpStatusCode.Redirect)
            {
                var dtoConfirmEmail = JsonConvert.DeserializeObject< DtoConfirmEmail>(Convert.ToString(result.result));
               return RedirectToAction("EmailNotConfirmed", dtoConfirmEmail);
            }
            foreach (var item in result.Errors)
            {
                ModelState.AddModelError("", item);
            }
            return View(model);
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Register(RegisterViewModel model)
        {
         var result= await _accountService.Register<ApiResponse>(model);
            if (result.Success)
            {
               return RedirectToAction("Index", "Home");
            }
            foreach (var item in result.Errors)
            {
                ModelState.AddModelError("", item);
            }
            return View(model);
        }
    
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            HttpContext.Session.SetString(SD.KeySessionJWT, "");
            return RedirectToAction("Index","Home");
        }



        public IActionResult AccessDenied(string returnUrl)
        {
            return View();
        }

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> IsUserNameAvailable(string username)
        {
          var response= await _accountService.IsUserNameAvailable<ApiResponse>(username);
            return (!response.Success) ? Json(true) : Json("UserName is Exist");
        }

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> IsEmailAvailable(string Email)
        {
            var response = await _accountService.IsEmailAvailable<ApiResponse>(Email);
            
            return (response.Success) ? Json(true) : Json("Email is Exist");
        }





        public async Task<IActionResult> EmailNotConfirmed(DtoConfirmEmail model)
        {
            var time = model.TimeRemain;
            if (time != null)
                ViewBag.RemainingTime = $"{time.Hours}:{time.Minutes}:{time.Seconds}";

            ViewBag.CanResend = model.CanResend;
            return View("EmailNotConfirmed",model.Email);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendConfirmation(string email)
        {

            var response = await _accountService.ResendConfirmEmail<ApiResponse>(email);
            if (!response.Success)
                {
                    TempData["Failed"] = response.Errors.FirstOrDefault();
                }
            else
                {

                   TempData["Success"] = "Success resend confirm to email";
                }
            return  RedirectToAction(nameof(Login));
        }


       
        [HttpGet("ForgotPassword")]
        public IActionResult ForgotPassword()
        {
            return View();
        }



        [HttpPost("ForgotPassword")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ResetPasswordRequestViewModel model)
        {
         var response= await  _accountService.ForgotPassword<ApiResponse>(model);
            if (response.Success)
            {
               TempData["Success"] = "Success send reset password to your email";
                 return  RedirectToAction(nameof(Login));
            }
            if(response.statusCode== HttpStatusCode.NotAcceptable)
            {
                 ViewBag.CanResend = false;
                ViewBag.RemainingTime =Convert.ToString( response.result);
                return View("CanRestPass");
            }

            TempData["Failed"] = response.Errors.FirstOrDefault();
            return  RedirectToAction(nameof(Login));
        }


        [HttpGet]
        public IActionResult ResetPassword(string userId, string token)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
            {
                return BadRequest("Invalid reset password link.");
            }

            var model = new ResetPasswordViewModel
            {
                UserId = userId,
                Token = token
            };

            return View(model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
           var response =await _accountService.ResetPassword<ApiResponse>(model);
            if (response.Success)
            {
                TempData["Success"] = "Success reset password";
                return RedirectToAction(nameof(Login));
            }

            ModelState.AddModelError("",response.Errors.FirstOrDefault());
            return View(model);
        }

    }
}

