using MagicVilla_Web.ViewModels.Identity;

namespace MagicVilla_Web.Services 
{
    public interface IAccountServices
    {
        public Task<T> Register<T>(RegisterViewModel viewModel);
        public  Task<T> Login<T>(LoginViewModel viewModel);
        public  Task<T> ResendConfirmEmail<T>(string Email);
        public  Task<T> ForgotPassword<T>(ResetPasswordRequestViewModel viewModel);
        public  Task<T> ResetPassword<T>(ResetPasswordViewModel viewModel);
     public  Task<T> IsUserNameAvailable<T>(string username);
     public  Task<T> IsEmailAvailable<T>(string Email);

    }
}
