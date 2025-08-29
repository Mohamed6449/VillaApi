using MagicVilla_Web.Dto.Identity;

namespace MagicVilla_Web.Services
{
   public interface ITokenProvider
    {

        public void SetTokens(DtoUser Tokens);
        public void ClearToken();
        public DtoUser GetToken();
    }
}
