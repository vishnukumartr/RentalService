using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentalService.Data;
using RentalService.Dtos;
using RentalService.Models;
using RentalService.Services.PropertyService;

namespace RentalService.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PropertyController : ControllerBase
    {
        private readonly IPropertyService _propertyService;
        public PropertyController(IPropertyService propertyService)
        {
            _propertyService = propertyService;
        }

        [HttpPost("AddProperty")]
        public async Task<ActionResult<Property>> PostProperty(CreatePropertyDto createPropertyDto)
        {
            try
            {
                var property = await _propertyService.CreatePropertyAsync(createPropertyDto);
                return CreatedAtAction(nameof(GetProperty), new { id = property.PropertyId }, property);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Property>> GetProperty(int id)
        {
            try
            {
                var property = await _propertyService.GetPropertyByIdAsync(id);
                return Ok(property);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("GetAllProperties")]
        public async Task<ActionResult<IEnumerable<Property>>> GetProperties()
        {
            var properties = await _propertyService.GetPropertiesAsync();
            return Ok(properties);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProperty(int id, CreatePropertyDto updatePropertyDto)
        {
            try
            {
                var property = await _propertyService.UpdatePropertyAsync(id, updatePropertyDto);
                return Ok(property);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
 
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProperty(int id)
        {
            var success = await _propertyService.DeletePropertyAsync(id);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }

    }
}
