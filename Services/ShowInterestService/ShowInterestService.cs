using Microsoft.EntityFrameworkCore;
using RentalService.Data;
using RentalService.Dtos;
using RentalService.Models;

namespace RentalService.Services.InterestService
{
    public class ShowInterestService : IShowInterestService
    {
        private readonly RentalDbContext _context;
        private readonly ILogger<ShowInterestService> _logger;  

        public const string Buyer = "Buyer";

        public ShowInterestService(RentalDbContext dbContext, ILogger<ShowInterestService> logger) 
        {
            _context = dbContext;
            _logger = logger;
        } 
        
        public async Task<Interest> ShowInterestAsync(ShowInterestDto showInterestDto)
        {
            try
            {
                var property = await _context.Properties.FindAsync(showInterestDto.PropertyId);
                if (property == null)
                {
                    throw new KeyNotFoundException("Property not found.");
                }

                var buyer = await _context.Users.FindAsync(showInterestDto.BuyerId);

                if (buyer == null || buyer.UserRole != Buyer)
                {
                    throw new InvalidOperationException("Invalid user or user is not a buyer.");
                }

                var interest = new Interest
                {
                    PropertyId = showInterestDto.PropertyId,
                    BuyerId = showInterestDto.BuyerId,
                    InterestedAt = DateTime.UtcNow
                };

                _context.Interests.Add(interest);
                await _context.SaveChangesAsync();

                return interest;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while show interest an property.");
                throw new Exception("An error occurred while show interest an Property.");
            }
        }

        public async Task<bool> RemoveInterestAsync(int interestId)
        {
            try
            {
                var interest = await _context.Interests.FindAsync(interestId);
                if (interest == null)
                {
                    return false;
                }

                _context.Interests.Remove(interest);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while removing interest an property.");
                throw new Exception("An error occurred while removing interest an Property.");
            }
        }

    }
}
