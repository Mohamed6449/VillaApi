using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MagicVilla_Web.Dto.Identity
{
    public class DtoUser
    {
        public string Id {  get; set; }
        public string Name { get; set; }

        public string? Address { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }
        public string[] Roles { get; set; }

        public string Teken {  get; set; }

        public DateTime expirationDateToken { get; set; }


    }
}
