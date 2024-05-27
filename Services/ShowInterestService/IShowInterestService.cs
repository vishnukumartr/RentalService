using RentalService.Dtos;
using RentalService.Models;

namespace RentalService.Services.InterestService
{
    public interface IShowInterestService
    {
        Task<Interest> ShowInterestAsync(ShowInterestDto showInterestDto);
        Task<bool> RemoveInterestAsync(int interestId);
    }
}
