using ClassLibrary1;
using MagicVilla_Web.Dto;
using MagicVilla_Web.ViewModels.Identity;
using Microsoft.Extensions.Configuration;

namespace MagicVilla_Web.Services.Implementation
{
    public class AccountServices: BaseService, IAccountServices
    {

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApiRequest _Request;

        public AccountServices(IHttpClientFactory httpClientFactory, IConfiguration configuration) :base (httpClientFactory) 
        {

            _httpClientFactory = httpClientFactory;
            _Request = new ApiRequest()
            {
                url = configuration.GetValue<string>("ApiUrl:applicationUrl") + "/Api/Account/"
            };
        }

        public  async Task<T> Register<T>(RegisterViewModel viewModel)
        {
            _Request.url += "Register";
            _Request.model = viewModel;
            _Request.apiType = SD.ApiType.Post;
            return await SendAsync<T>(_Request);
        }

        public  async Task<T> Login<T>(LoginViewModel viewModel)
        {
            _Request.url += "Login";
            _Request.model = viewModel;
            _Request.apiType = SD.ApiType.Post;
            return await SendAsync<T>(_Request);
        }
        public  async Task<T> ResendConfirmEmail<T>(string Email)
        {
            _Request.url += $"ResendConfirmation?email={Email}";
            _Request.apiType = SD.ApiType.Post;
            return await SendAsync<T>(_Request);
        }
        public  async Task<T> ForgotPassword<T>(ResetPasswordRequestViewModel viewModel)
        { 

            _Request.url += "ForgotPassword";
            _Request.model = viewModel;
            _Request.apiType = SD.ApiType.Get;
            return await SendAsync<T>(_Request);
        }
        public  async Task<T> ResetPassword<T>(ResetPasswordViewModel viewModel)
        {
            _Request.url += "ResetPassword";
            _Request.model = viewModel;
            _Request.apiType = SD.ApiType.Post;
            return await SendAsync<T>(_Request);
        }
     public  async Task<T> IsUserNameAvailable<T>(string username)
        {
            _Request.url += $"IsUserNameAvailable?username={username}";
            _Request.apiType = SD.ApiType.Get;
            return await SendAsync<T>(_Request);
        }


     public  async Task<T> IsEmailAvailable<T>(string Email)
        {
            _Request.url += $"IsEmailAvailable?Email={Email}";
            _Request.apiType = SD.ApiType.Get;
            return await SendAsync<T>(_Request);
        }




    }
}
