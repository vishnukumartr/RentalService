using RentalService.Models;
using System.ComponentModel.DataAnnotations;

namespace RentalService.Dtos
{
    public class UserRegisterDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserEmail { get; set; }
        public string Password { get; set; } 
        public string PhoneNumber { get; set; }
        public string UserRole { get; set; } // Seller or Buyer

    }
}
