using CQRS_test.ViewModels.Identity;
using MagicVilla_VillaApi.Dto.ApiResponses;
using MagicVilla_VillaApi.Dto.Identity;
using MagicVilla_VillaApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaApi.Services.InterFaces
{
    public interface IAccountService
    {
        public (bool canResend, TimeSpan? remainingTime) CanUserResend(string userId, string type);
        public void RecordResend(string userId, string type);

        public Task<ApiResponse> GetNewTokenFromRefreshToken(DtoUser dtoUser);
        public Task<(ApiResponse response, User user)> Register(RegisterViewModel register);
        public  Task<ApiResponse> Login(LoginViewModel model);

        public  Task<ApiResponse> IsUserNameAvailable(string username);
        public  Task<string> GetJWT(string userId, string TokenId);

        public  Task<ApiResponse> IsEmailAvailable(string Email);
        public Task<ApiResponse> ConfirmEmail(string userId, string token);

        public Task<(ApiResponse response, User user)> ResendConfirmation(string email);
        public Task<(ApiResponse response, User user)> ForgotPassword(ResetPasswordRequestViewModel model);
        public  Task<ApiResponse> ResetPassword([FromBody] ResetPasswordViewModel model);

        public  Task<string> CreateRefreshToken(string userId, string TokenId);
        public (bool Success, string UserId, string TokenId) GetAccessTekenData(string jwt);


    }
}
