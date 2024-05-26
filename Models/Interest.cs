namespace RentalService.Models
{
    public class Interest
    {
        public int InterestId { get; set; }

        public int PropertyId { get; set; }
        public Property Property { get; set; }

        public int BuyerId { get; set; }
        public User Buyer { get; set; }

        public DateTime InterestedAt { get; set; } = DateTime.Now;
    }

}
