using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using RentEase.API.DTOs.Properties;
using RentEase.API.Services;

namespace RentEase.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PropertyController : ControllerBase
{
    private readonly IPropertyService _propertyService;
    private readonly IValidator<CreatePropertyRequest> _createPropertyValidator;
    private readonly IValidator<UpdatePropertyRequest> _updatePropertyRequestValidator;


    public PropertyController(IPropertyService propertyService, IValidator<CreatePropertyRequest> createPropertyValidator,IValidator<UpdatePropertyRequest> updatePropertyRequestValidator)
    {
        _propertyService = propertyService;
        _createPropertyValidator = createPropertyValidator;
        _updatePropertyRequestValidator = updatePropertyRequestValidator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var properties = await _propertyService.GetAllAsync();

        return Ok(properties);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        var property = await _propertyService.GetByIdAsync(id);

        if (property == null)
        {
            return NotFound();
        }

        return Ok(property);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreatePropertyRequest request)
    {
        var validationResult =
            await _createPropertyValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(error => error.PropertyName)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(error => error.ErrorMessage).ToArray()
                );

            return BadRequest(new { errors });
        }

        var property = await _propertyService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = property.Id },
            property
        );
    }
    
   
    [HttpPut("{id:long}")]
    public async Task<IActionResult> UpdateProperty(
        long id,
        [FromBody] UpdatePropertyRequest request)
    {
        // 1. Validate the request
        var validationResult =
            await _updatePropertyRequestValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(error => error.PropertyName)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(error => error.ErrorMessage).ToArray()
                );

            return BadRequest(new
            {
                message = "Validation failed.",
                errors
            });
        }

        // 2. Update the property
        var updatedProperty =
            await _propertyService.UpdateAsync(id, request);

        // 3. Check whether the property exists
        if (updatedProperty == null)
        {
            return NotFound(new
            {
                message = $"Property with ID {id} was not found."
            });
        }

        // 4. Return the updated property
        return Ok(updatedProperty);
    }



    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        var deleted = await _propertyService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}