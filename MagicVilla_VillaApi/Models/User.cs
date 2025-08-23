using Microsoft.AspNetCore.Identity;

namespace MagicVilla_VillaApi.Models
{
    public class User: IdentityUser
    {
        public string Name { get; set; }

    }
}
