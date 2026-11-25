using RentEase.API.DTOs.Properties;

namespace RentEase.API.Services.Interfaces;

public interface IPropertyService
{
    Task<List<PropertyResponse>> GetAllAsync();

    Task<PropertyResponse?> GetByIdAsync(long id);

    Task<PropertyResponse> CreateAsync(long userId, CreatePropertyRequest request);

    Task<PropertyResponse?> UpdateAsync(
        long id,
        long userId,
        UpdatePropertyRequest request
    );

    Task<bool> DeleteAsync(long id, long userId);
}