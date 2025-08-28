using System.ComponentModel.DataAnnotations;

namespace MagicVilla_Web.Dto.VillaDto
{
    public class DtoVillaCreate
    {
        [Required]
        [MaxLength(30)]
        public string Name { get; set; }
        public string? Details { get; set; }
        public double Rate { get; set; }
        public int? Occupancy { get; set; }
        public string? Amenity { get; set; }
        public IFormFile? File { get; set; }

        public int? Sqft { get; set; }
    }
}
