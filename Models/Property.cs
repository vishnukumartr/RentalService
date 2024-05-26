using System.ComponentModel.DataAnnotations;

namespace RentalService.Models
{
    public class Property
    {
        public int PropertyId { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        [StringLength(100)]
        public string Place { get; set; }

        public int Area { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }

        [StringLength(255)]
        public string NearbyHospitals { get; set; }

        [StringLength(255)]
        public string NearbyColleges { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public ICollection<Interest> Interests { get; set; }
    }
}
