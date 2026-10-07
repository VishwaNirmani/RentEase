using RentEase.API.Entities;

namespace RentEase.API.Repositories;

public interface IPropertyRepository
{
    Task<List<Property>> GetAllAsync();

    Task<Property?> GetByIdAsync(long id);

    Task<Property> CreateAsync(Property property);

    Task<Property> UpdateAsync(Property property);

    Task<bool> DeleteAsync(long id);
}