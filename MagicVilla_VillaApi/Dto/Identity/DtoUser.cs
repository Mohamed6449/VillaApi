using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MagicVilla_VillaApi.Dto.Identity
{
    public class DtoUser
    {
        public string Token {  get; set; }

        public string RefreshToken { get; set; }

    }
}
