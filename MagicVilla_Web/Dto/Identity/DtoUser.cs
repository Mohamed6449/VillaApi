using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MagicVilla_Web.Dto.Identity
{
    public class DtoUser
    {
        public string Id { get; set; }


        public string UserName { get; set; }

        public string[] Roles { get; set; }

        public string Teken { get; set; }

        public string RefreshToken { get; set; }
        public DateTime expirationDateToken { get; set; }

    }
}
