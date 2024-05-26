using System.ComponentModel.DataAnnotations;

namespace RentalService.Dtos
{
    public class ShowInterestDto
    {
        [Required]
        public int PropertyId { get; set; }

        [Required]
        public int BuyerId { get; set; }
    }
}
