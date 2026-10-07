using Microsoft.EntityFrameworkCore;
using RentEase.API.Data;
using RentEase.API.Entities;

namespace RentEase.API.Repositories;

public class PropertyRepository : IPropertyRepository
{
    private readonly AppDbContext _context;

    public PropertyRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Property>> GetAllAsync()
    {
        return await _context.Properties
            .Include(property => property.Owner)
            .ToListAsync();
    }

    public async Task<Property?> GetByIdAsync(long id)
    {
        return await _context.Properties
            .Include(property => property.Owner)
            .FirstOrDefaultAsync(property => property.Id == id);
    }

    public async Task<Property> CreateAsync(Property property)
    {
        _context.Properties.Add(property);
        await _context.SaveChangesAsync();

        return property;
    }

    public async Task<Property> UpdateAsync(Property property)
    {
        _context.Properties.Update(property);
        await _context.SaveChangesAsync();

        return property;
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var property = await _context.Properties
            .FirstOrDefaultAsync(property => property.Id == id);

        if (property == null)
        {
            return false;
        }

        _context.Properties.Remove(property);
        await _context.SaveChangesAsync();

        return true;
    }
}