using ClassLibrary1;
using MagicVilla_Web.Dto.Identity;

namespace MagicVilla_Web.Services.Implementation
{
    public class TokenProvider:ITokenProvider
    {
        private readonly IHttpContextAccessor _contextAccessor;
        public TokenProvider(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public DtoUser GetToken()
        {
            try
            {

               bool hasToken= _contextAccessor.HttpContext.Request.Cookies.TryGetValue(SD.AccessToken,out string token);
                _contextAccessor.HttpContext.Request.Cookies.TryGetValue(SD.RefreshToken, out string refreshToken);
                DtoUser Dto = new ()
               {
                   RefreshToken = refreshToken,
                   Token = token
               };
                return hasToken? Dto:null;
            }
            catch
       
            {
                return null;
            }
        }
   
        public void ClearToken()
        {
            _contextAccessor.HttpContext?.Response.Cookies.Delete(SD.AccessToken);
            _contextAccessor.HttpContext?.Response.Cookies.Delete(SD.RefreshToken);
        }

        public void SetTokens(DtoUser Tokens)
        {
            var CookiesOptions=new CookieOptions { Expires = DateTime.UtcNow.AddDays(40) };
            _contextAccessor.HttpContext.Response.Cookies.Append(SD.AccessToken, Tokens.Token, CookiesOptions);
            _contextAccessor.HttpContext.Response.Cookies.Append(SD.RefreshToken, Tokens.RefreshToken, CookiesOptions);
        }
    
    }
}
