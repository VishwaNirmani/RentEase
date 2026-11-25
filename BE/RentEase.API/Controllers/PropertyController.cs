using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentEase.API.DTOs.Properties;
using RentEase.API.Services;
using RentEase.API.Services.Interfaces;
using System.Security.Claims;

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
    [Authorize]
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

    [Authorize(Roles = "Owner")]
    [HttpPost]
    public async Task<IActionResult> Create(CreatePropertyRequest request)
    {
        // Get the logged-in user's ID from JWT claims
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!long.TryParse(userIdClaim, out var userId))
            return Unauthorized();

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

        var property = await _propertyService.CreateAsync(userId, request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = property.Id },
            property
        );
    }
   
    [Authorize(Roles = "Owner")]
    [HttpPut("{id:long}")]
    public async Task<IActionResult> UpdateProperty(
        long id,
        [FromBody] UpdatePropertyRequest request)
    {
        // Get logged-in user's ID from JWT
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!long.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        // Validate request
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

        try
        {
            var updatedProperty =
                await _propertyService.UpdateAsync(id, userId, request);

            if (updatedProperty == null)
            {
                return NotFound();
            }

            return Ok(updatedProperty);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { message = ex.Message });
        }
    }


    [Authorize(Roles = "Owner")]
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        // Get logged-in user's ID from JWT
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!long.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        try
        {
            var deleted = await _propertyService.DeleteAsync(id, userId);

            if (!deleted)
            {
                return NotFound(new
                {
                    message = $"Property with ID {id} was not found."
                });
            }

            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { message = ex.Message });
        }
    }
}