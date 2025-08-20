namespace MagicVilla_VillaApi.Models
{
    public class Villa
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public double Rate { get; set; }
        public int Occupancy { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
        public string Amenity { get; set; } = string.Empty;
        public int Sqft { get; set; }
    }
}
