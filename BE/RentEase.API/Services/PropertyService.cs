using RentEase.API.DTOs.Properties;
using RentEase.API.Entities;
using RentEase.API.Repositories;

namespace RentEase.API.Services;

public class PropertyService : IPropertyService
{
    private readonly IPropertyRepository _propertyRepository;

    public PropertyService(IPropertyRepository propertyRepository)
    {
        _propertyRepository = propertyRepository;
    }

    public async Task<List<PropertyResponse>> GetAllAsync()
    {
        var properties = await _propertyRepository.GetAllAsync();

        return properties.Select(property => new PropertyResponse
        {
            Id = property.Id,
            OwnerId = property.OwnerId,
            Title = property.Title,
            Description = property.Description,
            Address = property.Address,
            City = property.City,
            PropertyType = property.PropertyType,
            Bedrooms = property.Bedrooms,
            Bathrooms = property.Bathrooms,
            MonthlyRent = property.MonthlyRent,
            SecurityDeposit = property.SecurityDeposit,
            Status = property.Status,
            CreatedAt = property.CreatedAt
        }).ToList();
    }

    public async Task<PropertyResponse?> GetByIdAsync(long id)
    {
        var property = await _propertyRepository.GetByIdAsync(id);

        if (property == null)
        {
            return null;
        }

        return new PropertyResponse
        {
            Id = property.Id,
            OwnerId = property.OwnerId,
            Title = property.Title,
            Description = property.Description,
            Address = property.Address,
            City = property.City,
            PropertyType = property.PropertyType,
            Bedrooms = property.Bedrooms,
            Bathrooms = property.Bathrooms,
            MonthlyRent = property.MonthlyRent,
            SecurityDeposit = property.SecurityDeposit,
            Status = property.Status,
            CreatedAt = property.CreatedAt
        };
    }

    public async Task<PropertyResponse> CreateAsync(CreatePropertyRequest request)
    {
        var property = new Property
        {
            OwnerId = request.OwnerId,
            Title = request.Title,
            Description = request.Description,
            Address = request.Address,
            City = request.City,
            PropertyType = request.PropertyType,
            Bedrooms = request.Bedrooms,
            Bathrooms = request.Bathrooms,
            MonthlyRent = request.MonthlyRent,
            SecurityDeposit = request.SecurityDeposit,
            Status = "AVAILABLE"
        };

        var createdProperty = await _propertyRepository.CreateAsync(property);

        return new PropertyResponse
        {
            Id = createdProperty.Id,
            OwnerId = createdProperty.OwnerId,
            Title = createdProperty.Title,
            Description = createdProperty.Description,
            Address = createdProperty.Address,
            City = createdProperty.City,
            PropertyType = createdProperty.PropertyType,
            Bedrooms = createdProperty.Bedrooms,
            Bathrooms = createdProperty.Bathrooms,
            MonthlyRent = createdProperty.MonthlyRent,
            SecurityDeposit = createdProperty.SecurityDeposit,
            Status = createdProperty.Status,
            CreatedAt = createdProperty.CreatedAt
        };
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

        property.Title = request.Title;
        property.Description = request.Description;
        property.Address = request.Address;
        property.City = request.City;
        property.PropertyType = request.PropertyType;
        property.Bedrooms = request.Bedrooms;
        property.Bathrooms = request.Bathrooms;
        property.MonthlyRent = request.MonthlyRent;
        property.SecurityDeposit = request.SecurityDeposit;
        property.Status = request.Status;

        var updatedProperty = await _propertyRepository.UpdateAsync(property);

        return new PropertyResponse
        {
            Id = updatedProperty.Id,
            OwnerId = updatedProperty.OwnerId,
            Title = updatedProperty.Title,
            Description = updatedProperty.Description,
            Address = updatedProperty.Address,
            City = updatedProperty.City,
            PropertyType = updatedProperty.PropertyType,
            Bedrooms = updatedProperty.Bedrooms,
            Bathrooms = updatedProperty.Bathrooms,
            MonthlyRent = updatedProperty.MonthlyRent,
            SecurityDeposit = updatedProperty.SecurityDeposit,
            Status = updatedProperty.Status,
            CreatedAt = updatedProperty.CreatedAt
        };
    }

    public async Task<bool> DeleteAsync(long id)
    {
        return await _propertyRepository.DeleteAsync(id);
    }
}