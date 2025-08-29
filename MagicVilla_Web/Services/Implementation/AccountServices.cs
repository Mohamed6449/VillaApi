using ClassLibrary1;
using MagicVilla_Web.Dto;
using MagicVilla_Web.Dto.Identity;
using MagicVilla_Web.ViewModels.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MagicVilla_Web.Services.Implementation
{
    public class AccountServices: BaseService, IAccountServices
    {

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ITokenProvider _tokenProvider;
        private readonly ApiRequest _Request;

        public AccountServices(ITokenProvider tokenProvider, IHttpClientFactory httpClientFactory, IConfiguration configuration , IHttpContextAccessor contextAccessor) :base (httpClientFactory) 
        {
            _tokenProvider = tokenProvider;
            _contextAccessor= contextAccessor;
            _httpClientFactory = httpClientFactory;
            _Request = new ApiRequest()
            {
                url = configuration.GetValue<string>("ApiUrl:applicationUrl") + $"/Api/{SD.CurrentVersion}/Account/"
            };
        }

        public  async Task<T> Register<T>(RegisterViewModel viewModel)
        {
            _Request.url += "Register";
            _Request.model = viewModel;
            _Request.apiType = SD.ApiType.Post;
            return await SendAsync<T>(_Request);
        }

        public  async Task<T> LoginRequest<T>(LoginViewModel viewModel)
        {
            _Request.url += "Login";
            _Request.model = viewModel;
            _Request.apiType = SD.ApiType.Post;
            return await SendAsync<T>(_Request);
        }

        public async Task  Login(object DbTokens)
        {
            var Tokens = JsonConvert.DeserializeObject<DtoUser>(Convert.ToString(DbTokens));

            var claims = GetDatafromJwt(Tokens.Token);
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, claims.UserName));
            foreach (var item in claims.Roles)
            {
                identity.AddClaim(new Claim(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Role, item));
            }
            var principal = new ClaimsPrincipal(identity);
            await _contextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            _tokenProvider.SetTokens(Tokens);
            
        }

        public void Logout()
        {
            _tokenProvider.ClearToken();
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


        public (string UserName, List<string> Roles) GetDatafromJwt(string Jwt)
        {
            var ReadJwt = new JwtSecurityTokenHandler();
            var Token = ReadJwt.ReadJwtToken(Jwt);
            var roles = Token.Claims.Where(C => C.Type == ClaimTypes.Role).Select(s => s.Value).ToList();
            var userName = Token.Claims.FirstOrDefault(C => C.Type == ClaimTypes.Name).Value;
            return (userName, roles);
        }


    }
}
