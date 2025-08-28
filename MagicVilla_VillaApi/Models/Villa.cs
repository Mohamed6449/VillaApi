namespace MagicVilla_VillaApi.Models
{
    public class Villa
    {
        public int Id { get; set; }
        public string Name { get; set; } 
        public string Details { get; set; } 
        public string ImageUrl { get; set; } 
        public double Rate { get; set; }
        public int Occupancy { get; set; } = 0;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
        public string Amenity { get; set; }
        public int Sqft { get; set; } = 0;
        public string? UrlImg {get;set;}
        public string? LocalUrlImg {get;set;}
        public virtual ICollection< VillaNumber> VillaNumbers { get; set; }
    }
}
