using Microsoft.AspNetCore.Mvc;
using RentEase.API.DTOs.Properties;
using RentEase.API.Services;

namespace RentEase.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PropertyController : ControllerBase
{
    private readonly IPropertyService _propertyService;

    public PropertyController(IPropertyService propertyService)
    {
        _propertyService = propertyService;
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
        var property = await _propertyService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = property.Id },
            property
        );
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        long id,
        UpdatePropertyRequest request)
    {
        var property = await _propertyService.UpdateAsync(id, request);

        if (property == null)
        {
            return NotFound();
        }

        return Ok(property);
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