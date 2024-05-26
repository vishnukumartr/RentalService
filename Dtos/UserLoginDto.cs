using RentalService.Models;

namespace RentalService.Dtos
{
    public class UserLoginDto
    {
        public string UserEmail { get; set; }

        public string Password { get; set; } 
    }
}
