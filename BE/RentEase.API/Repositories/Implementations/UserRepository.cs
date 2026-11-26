using Microsoft.EntityFrameworkCore;
using RentEase.API.Data;
using RentEase.API.Entities;
using RentEase.API.Repositories.Interfaces;

namespace RentEase.API.Repositories.Implementations;

public class UserRepository : IUserRepository

{
    private readonly AppDbContext _context;
    public UserRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(user => user.Email == email);
    }

    public async Task<User?> GetByIdAsync(long id)
    {
        return await _context.Users
            .FirstOrDefaultAsync(user => user.Id == id);
    }

    public async Task<User> CreateAsync(User user)
    {
        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        return user;
    }
}