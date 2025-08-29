
using AutoMapper;
using CQRS_test.ViewModels.Identity;
using MagicVilla_VillaApi.Data;
using MagicVilla_VillaApi.Dto.ApiResponses;
using MagicVilla_VillaApi.Dto.Identity;
using MagicVilla_VillaApi.Models;
using MagicVilla_VillaApi.Services.InterFaces;
using MagicVilla_VillaApi.SharedRepo;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;


namespace MagicVilla_VillaApi.Services.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly IMemoryCache _cache;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<AccountService> _logger;
        private readonly ApplicationDbContext _Context;
        private readonly IGenericRepo<RefreshToken> _sharedRepo;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AccountService(IMemoryCache cache, UserManager<User> userManager
            , ApplicationDbContext context, ILogger<AccountService> logger, IMapper mapper
            , IConfiguration configuration, IGenericRepo<RefreshToken> sharedRepo)
        {
            _mapper = mapper;
            _logger = logger;
            _Context = context;
            _cache = cache;
            _userManager = userManager;
            _configuration = configuration;
            _sharedRepo = sharedRepo;
        }


        public async Task<(ApiResponse response, User user)> Register(RegisterViewModel register)
        {
            var response = new ApiResponse();
            register.Email = register.Email.Trim().ToLower();
            var Exist = "";
            if (await _userManager.FindByEmailAsync(register.Email) != null) Exist += " (Email)";
            if (await _userManager.FindByNameAsync(register.UserName) != null) Exist += " (UserName)";
            if (Exist != "")
            {
                response.Errors.Add($"{Exist} already exists");
                response.Success = false;
                response.statusCode = HttpStatusCode.Conflict;
                return (response, null);
            }


            using var transaction = await _Context.Database.BeginTransactionAsync();
            try
            {
                var newUser = _mapper.Map<User>(register);
                var result = await _userManager.CreateAsync(newUser, register.Password);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        response.Errors.Add(error.Description);
                    }
                    await transaction.RollbackAsync();
                    response.Success = false;
                    response.statusCode = HttpStatusCode.NotAcceptable;
                    return (response, null);
                }

                await transaction.CommitAsync();
                response.Success = true;
                response.statusCode = HttpStatusCode.Created;
                return (response, newUser);

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                response.Errors.Add($"Error: {ex.Message}");
                response.Success = false;
                response.statusCode = HttpStatusCode.InternalServerError;
                return (response, null);
            }
        }

        public async Task<ApiResponse> Login(LoginViewModel model)
        {
            var response = new ApiResponse();
            model.Email = model.Email.Trim().ToLower();
            var isEmailValid = new EmailAddressAttribute().IsValid(model.Email);
            User user = null;

            if (isEmailValid) user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) user = await _userManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                _logger.LogWarning("Login failed. User with email/username {Email} not found", model.Email);
                response.Errors.Add($"User with email / username '{model.Email}' not found");
                response.Success = false;
                response.statusCode = HttpStatusCode.NotFound;
                return response;
            }

            if (await _userManager.IsLockedOutAsync(user))
            {
                _logger.LogWarning("User {UserName} is locked out", user.UserName);
                response.Errors.Add($"User {user.UserName} is temporarily locked");
                response.Success = false;
                response.statusCode = HttpStatusCode.BadRequest;
                return response;
            }

            var checkPassword = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!checkPassword)
            {
                _logger.LogWarning("Incorrect password", user.Email);
                await _userManager.AccessFailedAsync(user);
                response.Errors.Add($"Invalid password");
                response.Success = false;
                response.statusCode = HttpStatusCode.BadRequest;
                return response;
            }

            if (!user.EmailConfirmed)
            {
                var Canresend = CanUserResend(user.Id, "Email");
                _logger.LogWarning("Email not confirmed for user {Email}", user.Email);
                response.Errors.Add($"Email not confirmed'{model.Email}'");
                response.Success = false;
                response.result = new DtoConfirmEmail()
                {
                    Email = user.Email,
                    Id = user.Id,
                    CanResend = Canresend.canResend,
                    TimeRemain = (Canresend.remainingTime != null) ? (TimeSpan)Canresend.remainingTime : new TimeSpan(0)
                };
                response.statusCode = HttpStatusCode.Redirect;
                return response;
            }
            _logger.LogInformation("User {Email} logged in successfully", user.Email);
            _cache.Remove($"resend_{user.Id}password");
            var tokenId = Guid.NewGuid().ToString();
            var jwt = await GetJWT(user.Id, tokenId);
            var refreshToken = await CreateRefreshToken(user.Id, tokenId);
            var dtoUser = new DtoUser()
            {
                RefreshToken = refreshToken,
                Token = jwt

            };
            response.result = dtoUser;
            response.Success = true;
            response.statusCode = HttpStatusCode.OK;
            return response;
        }

        public async Task<ApiResponse> IsUserNameAvailable(string username)
        {
            var response = new ApiResponse();
            if (string.IsNullOrWhiteSpace(username))
            {
                response.Success = false;
                response.statusCode = HttpStatusCode.BadRequest;
                response.Errors.Add("Username is required");
                return response;
            }

            var user = await _userManager.FindByNameAsync(username);
            if (user != null)
            {
                response.Success = true;
                response.statusCode = HttpStatusCode.OK;
                return response;
            }
            response.statusCode = HttpStatusCode.Conflict;
            response.Errors.Add("Username is Exist");
            response.Success = false;

            return response;
        }

        public async Task<ApiResponse> IsEmailAvailable(string Email)
        {
            var response = new ApiResponse();

            if (string.IsNullOrWhiteSpace(Email))
            {
                response.Success = false;
                response.statusCode = HttpStatusCode.BadRequest;
                response.Errors.Add("Email is required");
                return response;
            }
            Email = Email.Trim().ToLower();

            var user = await _userManager.FindByEmailAsync(Email);
            if (user == null)
            {
                response.Success = true;
                response.statusCode = HttpStatusCode.OK;
                return response;
            }
            response.statusCode = HttpStatusCode.Conflict;
            response.Errors.Add("Email is Exist");
            response.Success = false;

            return response;
        }

        public async Task<ApiResponse> ConfirmEmail(string userId, string token)
        {
            var response = new ApiResponse();
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                response.Success = false;
                response.statusCode = HttpStatusCode.BadRequest;
                response.Errors.Add("Invalid confirmation link");
                return response;
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found during email confirmation.", userId);
                response.Success = false;
                response.statusCode = HttpStatusCode.BadRequest;
                response.Errors.Add($"IUser with ID {userId} not found");
                return response;
            }
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                _logger.LogInformation("Email confirmed successfully for user {UserId}.", userId);
                _cache.Remove($"resend_{userId}");
                response.Success = true;
                return response;
            }
            response.Success = false;
            response.statusCode = HttpStatusCode.BadRequest;
            response.Errors.Add("Email confirmation failed");
            return response;
        }

        public async Task<(ApiResponse response, User user)> ResendConfirmation(string email)
        {
            var response = new ApiResponse();
            if (string.IsNullOrWhiteSpace(email))
            {
                {
                    response.Success = false;
                    response.statusCode = HttpStatusCode.BadRequest;
                    response.Errors.Add("Email is required");
                    return (response, null);
                }
            }

            email = email.Trim().ToLower();
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                response.Success = false;
                response.statusCode = HttpStatusCode.NotFound;
                response.Errors.Add("User not found");
                return (response, null);
            }

            if (user.EmailConfirmed)
            {

                response.Success = false;
                response.statusCode = HttpStatusCode.BadRequest;
                response.Errors.Add("Email is already confirmed");
                return (response, null);
            }

            // تحقق من المدة المسموح بها قبل إعادة الإرسال
            var resendCheck = CanUserResend(user.Id, "Email");
            if (!resendCheck.canResend)
            {

                response.Success = true;
                response.statusCode = HttpStatusCode.NotAcceptable;
                response.Errors.Add("Can Not resent until time out");
                response.result = resendCheck.remainingTime?.ToString(@"hh\:mm\:ss");
                return (response, null);

            }
            response.Success = true;
            response.statusCode = HttpStatusCode.OK;
            RecordResend(user.Id, "Email");
            return (response, user);


        }

        public async Task<(ApiResponse response, User user)> ForgotPassword(ResetPasswordRequestViewModel model)
        {
            var response = new ApiResponse();

            model.Email = model.Email.Trim().ToLower();

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                response.Success = false;
                response.Errors.Add("Email not exist");
                response.statusCode = HttpStatusCode.NotFound;
                return (response, null);
            }

            if (!user.EmailConfirmed)
            {
                _logger.LogWarning("Email not confirmed for user {Email}", user.Email);
                response.Errors.Add($"Email not confirmed'{model.Email}'");
                response.Success = false;
                response.statusCode = HttpStatusCode.BadRequest;
                return (response, null);
            }

            // تحقق من مدة إعادة الإرسال
            var resendResult = CanUserResend(user.Id, "password");
            if (!resendResult.canResend)
            {

                response.Success = false;
                response.statusCode = HttpStatusCode.NotAcceptable;
                response.Errors.Add("Can Not resent until time out");
                response.result = resendResult.remainingTime?.ToString(@"hh\:mm\:ss");
                return (response, null);
            }
            _logger.LogInformation("Reset password email sent to {Email}", model.Email);

            response.Success = true;
            response.statusCode = HttpStatusCode.OK;
            RecordResend(user.Id, "password");
            return (response, user);


        }

        public async Task<ApiResponse> ResetPassword([FromBody] ResetPasswordViewModel model)
        {
            var response = new ApiResponse();

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found during email confirmation.", model.UserId);
                response.Success = false;
                response.statusCode = HttpStatusCode.BadRequest;
                response.Errors.Add($"IUser with ID {model.UserId} not found");
                return response;
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded)
            {
                _cache.Remove($"resend_{user.Id}password");
                _logger.LogInformation("Password reset successfully for user {UserId}", user.Id);
                response.Success = true;
                return response;
            }

            _logger.LogWarning("Password reset failed for user {UserId}. Errors: {Errors}", user.Id, result.Errors);
            response.Success = false;
            response.statusCode = HttpStatusCode.BadRequest;
            response.Errors.Add("Password reset failed");
            return response;

        }
        public (bool canResend, TimeSpan? remainingTime) CanUserResend(string userId, string type)
        {
            string key = $"resend_{userId}{type}";

            if (_cache.TryGetValue(key, out (int count, DateTime lastSent) data))
            {
                var now = DateTime.UtcNow;
                var nextAvailable = data.count == 1 ? data.lastSent.AddMinutes(5) : data.lastSent.AddHours(24);

                if (now < nextAvailable)
                    return (false, nextAvailable - now);
            }

            return (true, null);
        }

        public void RecordResend(string userId, string type)
        {
            string key = $"resend_{userId}{type}";
            if (_cache.TryGetValue(key, out (int count, DateTime lastSent) data))
            {
                _cache.Set(key, (data.count + 1, DateTime.UtcNow));
            }
            else
            {
                _cache.Set(key, (1, DateTime.UtcNow));
            }
        }


        public async Task<string> GetJWT(string userId,string TokenId)
        {
            var user=await _Context.User.FirstOrDefaultAsync(W => W.Id == userId);
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));
            var Id = new Claim(JwtRegisteredClaimNames.Jti, TokenId);
            var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.UserName),
                    Id,
                };
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.ToString()));
            }

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)


            );
            var JwtSecurity = new JwtSecurityTokenHandler().WriteToken(token);
            return (JwtSecurity);

        }
        public async Task<ApiResponse> GetNewTokenFromRefreshToken(DtoUser dtoUser)
        {
            var response = new ApiResponse();
            var DbRefreshToken = _Context.RefreshTokens.FirstOrDefault(A => A.Refresh_Token == dtoUser.RefreshToken);
            if (DbRefreshToken == null)
            {
                response.Errors.Add("Invalid Token");
                response.Success = false;
                response.statusCode = HttpStatusCode.BadRequest;
                return response;

            }
                var DataAccessToken = GetAccessTekenData(dtoUser.Token);
            if (!DataAccessToken.Success || DbRefreshToken.UserId != DataAccessToken.UserId
                || DbRefreshToken.JwtTokenId != DataAccessToken.TokenId )
            {
                DbRefreshToken.IsValid = false;
               await _Context.SaveChangesAsync();
                response.Errors.Add( "Invalid Token");
                response.Success = false;
                response.statusCode = HttpStatusCode.BadRequest;
                return response;
            }
            if (!DbRefreshToken.IsValid)
            {
                var Chain = await _Context.RefreshTokens.
                    Where(W => W.JwtTokenId == DbRefreshToken.JwtTokenId && W.UserId == DbRefreshToken.UserId).
                    ExecuteUpdateAsync(E => E.SetProperty(s => s.IsValid, false));

                response.Errors.Add("Invalid Token");
                response.Success = false;
                response.statusCode = HttpStatusCode.BadRequest;
                return response;
            }
            if( DbRefreshToken.Expiration < DateTime.UtcNow)
            {
                DbRefreshToken.IsValid = false;
                await _Context.SaveChangesAsync();
                response.Errors.Add("Invalid Token");
                response.Success = false;
                response.statusCode = HttpStatusCode.BadRequest;
                return response;

            }

           var newRefreshToken=  await CreateRefreshToken(DataAccessToken.UserId,DataAccessToken.TokenId);
            DbRefreshToken.IsValid = false;
            await _Context.SaveChangesAsync();
            var newToken =await GetJWT(DataAccessToken.UserId, DataAccessToken.TokenId);
            response.Success = true;
            response.statusCode = HttpStatusCode.OK;
            response.result = new DtoUser()
            {
                RefreshToken = newRefreshToken,
                Token = newToken
            };
            return response;

        }

        public async Task<string> CreateRefreshToken(string userId, string TokenId)
        {
            var refreshToken = new RefreshToken()
            {
                IsValid = true,
                JwtTokenId = TokenId,
                UserId = userId,
                Expiration = DateTime.UtcNow.AddDays(10)
                ,Refresh_Token=Guid.NewGuid().ToString()+ "_" + Guid.NewGuid().ToString()
            };
           await _sharedRepo.AddAsync(refreshToken);
            return refreshToken.Refresh_Token;
        }
        public (bool Success,string UserId,string TokenId) GetAccessTekenData(string  jwt)
        {
            try
            {

                var readToken = new JwtSecurityTokenHandler().ReadJwtToken(jwt);
                var userId = readToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
                var tokenId = readToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti).Value;
                return (true, userId, tokenId);
            }
            
            catch (Exception ex)
            {
                return (false, null, null);

            }

        }


    }
}
