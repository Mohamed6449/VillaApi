using AutoMapper;
using CQRS_test.ViewModels.Identity;
using MagicVilla_VillaApi.Dto.ApiResponses;
using MagicVilla_VillaApi.Dto.Identity;
using MagicVilla_VillaApi.Models;
using MagicVilla_VillaApi.Services.InterFaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace MagicVilla_VillaApi.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersionNeutral]//not belong to any versions

    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<User> _userManager;

        public AccountController(UserManager<User> userManager,
                                IEmailSender emailSender, IAccountService accountService)
        {

            _userManager = userManager;


            _emailSender = emailSender;
            _accountService = accountService;

        }

        [HttpPost("Login")]
        public async Task<ActionResult<ApiResponse>> Login([FromBody] LoginViewModel model)
        {

            return Ok(await _accountService.Login(model));
        }



        [HttpPost("Register")]
        public async Task<ActionResult> Register([FromBody] RegisterViewModel register)
        {
            var result = await _accountService.Register(register);
            if (result.response.Success)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(result.user);
                var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = result.user.Id, token = token }, Request.Scheme);
                await _emailSender.SendEmailAsync(result.user.Email, "Confirm your email", confirmationLink, 1);
            }
            return Ok(result.response);

        }


        [HttpGet("IsUserNameAvailable")]
        public async Task<ActionResult<ApiResponse>> IsUserNameAvailable([FromQuery] string username)
        {

            return Ok(await _accountService.IsUserNameAvailable(username));

        }

        [HttpGet("IsEmailAvailable")]
        public async Task<ActionResult<ApiResponse>> IsEmailAvailable([FromQuery] string Email)
        {
            return Ok(await _accountService.IsEmailAvailable(Email));
        }

        [HttpPost("ConfirmEmail")]
        public async Task<ActionResult<ApiResponse>> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
        {
            var response = await _accountService.ConfirmEmail(userId, token);
            if (response.Success)
            {
                return Redirect("https://localhost:7001/Account/login");
            }
            return Ok(response);
        }


        [HttpPost("ResendConfirmation")]
        public async Task<ActionResult<ApiResponse>> ResendConfirmation([FromQuery] string email)
        {
            var result = await _accountService.ResendConfirmation(email);
            if (result.response.Success)
            {

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(result.user);
                var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = result.user.Id, token = token }, Request.Scheme);
                await _emailSender.SendEmailAsync(result.user.Email, "Confirm your email", confirmationLink, 1);
            }
            return Ok(result.response);
        }



        [HttpGet("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ResetPasswordRequestViewModel model)
        {
            var result = await _accountService.ForgotPassword(model);
            if (result.response.Success)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(result.user);
                //var resetLink = Url.Action("ResetPassword", "Account", new { userId = result.user.Id, token = token },
                //    Request.Scheme
                //);
                var resetLink = $"https://localhost:7001/Account/ResetPassword?userId={result.user.Id}&token={token}";
                await _emailSender.SendEmailAsync(model.Email, "Reset Password", resetLink, 2);
                _accountService.RecordResend(result.user.Id, "password");
            }
            return Ok(result.response);



        }


        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordViewModel model)
        {
            var response = await _accountService.ResetPassword(model);
            if (response.Success)
            {
                return Redirect("https://frontend-app.com/email-confirmed");
            }
            return Ok(response);

        }

        [HttpPost("Register")]
        public async Task<ActionResult> RefreshTokenDto([FromBody] DtoUser register)
        {
            var result = await _accountService.Register();
            if (result.response.Success)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(result.user);
                var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = result.user.Id, token = token }, Request.Scheme);
                await _emailSender.SendEmailAsync(result.user.Email, "Confirm your email", confirmationLink, 1);
            }
            return Ok(result.response);

        }


    }
}
