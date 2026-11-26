using RentEase.API.Entities;
using RentEase.API.Enum;

namespace RentEase.API.Repositories.Interfaces;

public interface IOwnerApplicationRepository
{
    Task<OwnerApplication> CreateAsync(OwnerApplication application);

    Task<OwnerApplication?> GetLatestByUserIdAsync(long userId);

    Task<List<OwnerApplication>> GetPendingAsync();

    Task<OwnerApplication?> GetByIdAsync(long id);

    Task<OwnerApplication?> ReviewAsync(
        long id,
        OwnerApplicationStatus status,
        DateTime reviewedAt,
        string? adminRemarks);
}
