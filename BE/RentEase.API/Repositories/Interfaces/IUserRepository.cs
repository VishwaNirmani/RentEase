using RentEase.API.Entities;

namespace RentEase.API.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);

    Task<User?> GetByIdAsync(long id);

    Task<User> CreateAsync(User user);
}