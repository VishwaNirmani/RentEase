using RentEase.API.DTOs.Properties;

namespace RentEase.API.Services;

public interface IPropertyService
{
    Task<List<PropertyResponse>> GetAllAsync();

    Task<PropertyResponse?> GetByIdAsync(long id);

    Task<PropertyResponse> CreateAsync(CreatePropertyRequest request);

    Task<PropertyResponse?> UpdateAsync(
        long id,
        UpdatePropertyRequest request
    );

    Task<bool> DeleteAsync(long id);
}