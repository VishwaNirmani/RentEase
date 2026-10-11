
using AutoMapper;
using RentEase.API.DTOs.Properties;
using RentEase.API.Entities;
using RentEase.API.Repositories;

namespace RentEase.API.Services;

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
        CreatePropertyRequest request)
    {
        var property = _mapper.Map<Property>(request);

        var createdProperty =
            await _propertyRepository.CreateAsync(property);

        return _mapper.Map<PropertyResponse>(createdProperty);
    }

    public async Task<PropertyResponse?> UpdateAsync(
        long id,
        UpdatePropertyRequest request)
    {
        var property = await _propertyRepository.GetByIdAsync(id);

        if (property == null)
        {
            return null;
        }

        _mapper.Map(request, property);

        var updatedProperty =
            await _propertyRepository.UpdateAsync(property);

        return _mapper.Map<PropertyResponse>(updatedProperty);
    }

    public async Task<bool> DeleteAsync(long id)
    {
        return await _propertyRepository.DeleteAsync(id);
    }
}
