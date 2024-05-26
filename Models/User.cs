using System.ComponentModel.DataAnnotations;

namespace RentalService.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public byte[] PasswordHash { get; set; } = new byte[0];
        public byte[] PasswordSalt { get; set; } = new byte[0];

        [Required]
        [StringLength(100)]
        public string UserEmail { get; set; }


        [StringLength(50)]
        public string PhoneNumber { get; set; }
        public string UserRole { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName {get; set; }

        [StringLength(50)]
        public string LastName { get; set; }
        public string FullName { get; set; }
        public bool IsSeller { get; set; }

        public ICollection<Property> Properties { get; set; }
        public ICollection<Interest> Interests { get; set; }

        public DateTime Created { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime TokenExpires { get; set; }
        public DateTime TokenCreated { get; set; }
    }
}
