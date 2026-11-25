using AutoMapper;
using RentEase.API.DTOs.Properties;
using RentEase.API.Entities;
using RentEase.API.Repositories.Interfaces;
using RentEase.API.Services.Interfaces;

namespace RentEase.API.Services.Implementations;

public class PropertyService : IPropertyService
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly IMapper _mapper;

    public PropertyService(
        IPropertyRepository propertyRepository,
        IMapper mapper)
    {
        _propertyRepository = propertyRepository;
        _mapper = mapper;
    }

    public async Task<List<PropertyResponse>> GetAllAsync()
    {
        var properties = await _propertyRepository.GetAllAsync();

        return _mapper.Map<List<PropertyResponse>>(properties);
    }
        

    public async Task<PropertyResponse?> GetByIdAsync(long id)
    {
        var property = await _propertyRepository.GetByIdAsync(id);

        if (property == null)
        {
            return null;
        }

        return _mapper.Map<PropertyResponse>(property);
    }

    
    public async Task<PropertyResponse> CreateAsync(
        long userId,
        CreatePropertyRequest request)
    {
        var property = _mapper.Map<Property>(request);

        // Assign the authenticated user's ID
        property.OwnerId = userId;

        var createdProperty =
            await _propertyRepository.CreateAsync(property);

        return _mapper.Map<PropertyResponse>(createdProperty);
    }

    public async Task<PropertyResponse?> UpdateAsync(
        long id,
        long userId,
        UpdatePropertyRequest request)
    {
        var property = await _propertyRepository.GetByIdAsync(id);

        if (property == null)
        {
            return null;
        }

        // Check whether this property belongs to the logged-in user
        if (property.OwnerId != userId)
        {
            throw new UnauthorizedAccessException(
                "You are not allowed to update this property.");
        }

        _mapper.Map(request, property);

        var updatedProperty =
            await _propertyRepository.UpdateAsync(property);

        return _mapper.Map<PropertyResponse>(updatedProperty);
    }

    public async Task<bool> DeleteAsync(long id, long userId)
    {
        var property = await _propertyRepository.GetByIdAsync(id);

        if (property == null)
        {
            return false;
        }

        // Check property ownership
        if (property.OwnerId != userId)
        {
            throw new UnauthorizedAccessException(
                "You are not allowed to delete this property.");
        }

        return await _propertyRepository.DeleteAsync(id);
    }
}
