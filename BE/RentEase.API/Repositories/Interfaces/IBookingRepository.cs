using RentEase.API.Entities;

namespace RentEase.API.Repositories.Interfaces;

public interface IBookingRepository
{
    Task<Booking> CreateAsync(Booking booking);

    Task<Booking?> GetByIdAsync(long id);

    Task<List<Booking>> GetByTenantIdAsync(long tenantId);

    Task<List<Booking>> GetByPropertyIdAsync(long propertyId);

    Task<bool> HasOverlappingApprovedBookingAsync(
        long propertyId,
        DateTime startDate,
        DateTime endDate,
        long? excludeBookingId = null);

    Task<Booking> UpdateAsync(Booking booking);
}