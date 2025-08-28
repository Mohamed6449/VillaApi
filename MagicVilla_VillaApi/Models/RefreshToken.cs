using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MagicVilla_VillaApi.Models
{
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; }
        public string Refresh_Token { get; set; }

        public string JwtTokenId { get; set; }

        public bool IsValid { get; set; }

        public DateTime Expiration {  get; set; }

        [ForeignKey("user")]
        public string UserId { get; set; }
        public virtual User User { get; set; }


    }
}
