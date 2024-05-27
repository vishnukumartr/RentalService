using Microsoft.EntityFrameworkCore;
using RentalService.Data;
using RentalService.Dtos;
using RentalService.Models;
using RentalService;

namespace RentalService.Services.PropertyService
{
    public class PropertyService : IPropertyService
    {
        private readonly RentalDbContext _context;
        private readonly ILogger<PropertyService> _logger;

        public const string Seller = "Seller";

        public PropertyService(RentalDbContext dbContext, ILogger<PropertyService> logger) 
        {
            _context = dbContext;
            _logger = logger;
        }    
        public async Task<Property> CreatePropertyAsync(CreatePropertyDto createPropertyDto)
        {
            try
            {
                var user = await _context.Users.FindAsync(createPropertyDto.UserId);
                if (user == null || user.UserRole != Seller)
                {
                    throw new InvalidOperationException("Invalid user or user is not a seller.");
                }

                var property = new Property
                {
                    Title = createPropertyDto.Title,
                    Description = createPropertyDto.Description,
                    Place = createPropertyDto.Place,
                    Area = createPropertyDto.Area,
                    Bedrooms = createPropertyDto.Bedrooms,
                    Bathrooms = createPropertyDto.Bathrooms,
                    NearbyHospitals = createPropertyDto.NearbyHospitals,
                    NearbyColleges = createPropertyDto.NearbyColleges,
                    UserId = createPropertyDto.UserId
                };

                _context.Properties.Add(property);
                await _context.SaveChangesAsync();

                return property;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the Property.");
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> DeletePropertyAsync(int id)
        {
            try
            {
                var property = await _context.Properties.FindAsync(id);
                if (property == null)
                {
                    return false;
                }

                _context.Properties.Remove(property);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the Property.");
                throw new Exception(ex.Message);
            }

        }

        public async Task<IEnumerable<Property>> GetPropertiesAsync()
        {
            try
            {
                return await _context.Properties.Include(p => p.User).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all the Properties.");
                throw new Exception(ex.Message);
            }
        }

        public async Task<Property> GetPropertyByIdAsync(int id)
        {
            try
            {
                var property = await _context.Properties.Include(p => p.User).FirstOrDefaultAsync(p => p.PropertyId == id);
                if (property == null)
                {
                    throw new KeyNotFoundException("Property not found.");
                }
                return property;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the Property.");
                throw new Exception(ex.Message);
            }

        }

        public async Task<Property> UpdatePropertyAsync(int id, CreatePropertyDto updatePropertyDto)
        {
            try
            {
                var property = await _context.Properties.FindAsync(id);
                if (property == null)
                {
                    throw new KeyNotFoundException("Property not found.");
                }

                var user = await _context.Users.FindAsync(updatePropertyDto.UserId);
                if (user == null || user.UserRole != Seller)
                {
                    throw new InvalidOperationException("Invalid user or user is not a seller.");
                }

                property.Title = updatePropertyDto.Title;
                property.Description = updatePropertyDto.Description;
                property.Place = updatePropertyDto.Place;
                property.Area = updatePropertyDto.Area;
                property.Bedrooms = updatePropertyDto.Bedrooms;
                property.Bathrooms = updatePropertyDto.Bathrooms;
                property.NearbyHospitals = updatePropertyDto.NearbyHospitals;
                property.NearbyColleges = updatePropertyDto.NearbyColleges;

                _context.Entry(property).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return property;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the Property.");
                throw new Exception(ex.Message);
            }
        }

    }
}
