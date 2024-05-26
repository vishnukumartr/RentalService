using RentalService.Dtos;
using RentalService.Models;

namespace RentalService.Services.PropertyService
{
    public interface IPropertyService
    {
        Task<Property> CreatePropertyAsync(CreatePropertyDto createPropertyDto);
        Task<IEnumerable<Property>> GetPropertiesAsync();
        Task<Property> GetPropertyByIdAsync(int id);
        Task<Property> UpdatePropertyAsync(int id, CreatePropertyDto updatePropertyDto);
        Task<bool> DeletePropertyAsync(int id);
    }
}
