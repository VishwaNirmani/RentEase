using Microsoft.EntityFrameworkCore;
using Npgsql;
using RentEase.API.Data;
using RentEase.API.Entities;
using RentEase.API.Enum;
using RentEase.API.Repositories.Interfaces;

namespace RentEase.API.Repositories.Implementations;

public class OwnerApplicationRepository : IOwnerApplicationRepository
{
    private readonly AppDbContext _context;

    public OwnerApplicationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<OwnerApplication> CreateAsync(
        OwnerApplication application)
    {
        await _context.OwnerApplications.AddAsync(application);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException exception)
            when (exception.InnerException is PostgresException postgresException &&
                  postgresException.SqlState == PostgresErrorCodes.UniqueViolation &&
                  postgresException.ConstraintName == "IX_OwnerApplications_UserId")
        {
            throw new InvalidOperationException(
                "A pending owner application already exists.",
                exception);
        }

        return application;
    }

    public async Task<OwnerApplication?> GetLatestByUserIdAsync(long userId)
    {
        return await _context.OwnerApplications
            .AsNoTracking()
            .Include(application => application.User)
            .Where(application => application.UserId == userId)
            .OrderByDescending(application => application.SubmittedAt)
            .ThenByDescending(application => application.Id)
            .FirstOrDefaultAsync();
    }

    public async Task<List<OwnerApplication>> GetPendingAsync()
    {
        return await _context.OwnerApplications
            .AsNoTracking()
            .Include(application => application.User)
            .Where(application => application.Status == OwnerApplicationStatus.Pending)
            .OrderBy(application => application.SubmittedAt)
            .ToListAsync();
    }

    public async Task<OwnerApplication?> GetByIdAsync(long id)
    {
        return await _context.OwnerApplications
            .AsNoTracking()
            .Include(application => application.User)
            .FirstOrDefaultAsync(application => application.Id == id);
    }

    public async Task<OwnerApplication?> ReviewAsync(
        long id,
        OwnerApplicationStatus status,
        DateTime reviewedAt,
        string? adminRemarks)
    {
        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        var userId = await _context.OwnerApplications
            .Where(application => application.Id == id)
            .Select(application => (long?)application.UserId)
            .SingleOrDefaultAsync();

        if (userId is null)
        {
            return null;
        }

        var updatedApplications = await _context.OwnerApplications
            .Where(application =>
                application.Id == id &&
                application.Status == OwnerApplicationStatus.Pending)
            .ExecuteUpdateAsync(updates => updates
                .SetProperty(application => application.Status, status)
                .SetProperty(application => application.ReviewedAt, reviewedAt)
                .SetProperty(application => application.AdminRemarks, adminRemarks));

        if (updatedApplications == 0)
        {
            throw new InvalidOperationException(
                "Only pending applications can be reviewed.");
        }

        if (status == OwnerApplicationStatus.Approved)
        {
            var updatedUsers = await _context.Users
                .Where(user =>
                    user.Id == userId &&
                    user.Role == UserRole.Tenant)
                .ExecuteUpdateAsync(updates => updates
                    .SetProperty(user => user.Role, UserRole.Owner));

            if (updatedUsers == 0)
            {
                throw new InvalidOperationException(
                    "Only tenant accounts can be approved as owners.");
            }
        }

        await transaction.CommitAsync();

        return await GetByIdAsync(id);
    }
}
