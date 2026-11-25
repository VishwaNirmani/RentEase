
using Microsoft.EntityFrameworkCore;
using RentEase.API.Data;
using RentEase.API.Entities;
using RentEase.API.Enum;
using RentEase.API.Repositories.Interfaces;

namespace RentEase.API.Repositories.Implementations;

public class BookingRepository : IBookingRepository
{
    private readonly AppDbContext _context;

    public BookingRepository(AppDbContext context)
    {
        _context = context;
    }

    // Create a new booking
    public async Task<Booking> CreateAsync(Booking booking)
    {
        await _context.Bookings.AddAsync(booking);
        await _context.SaveChangesAsync();

        return booking;
    }

    // Get one booking with its tenant and property
    public async Task<Booking?> GetByIdAsync(long id)
    {
        return await _context.Bookings
            .Include(b => b.Tenant)
            .Include(b => b.Property)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    // Get all bookings created by a particular tenant
    public async Task<List<Booking>> GetByTenantIdAsync(long tenantId)
    {
        return await _context.Bookings
            .Include(b => b.Property)
            .Include(b => b.Tenant)
            .Where(b => b.TenantId == tenantId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    // Get all bookings for a particular property
    public async Task<List<Booking>> GetByPropertyIdAsync(long propertyId)
    {
        return await _context.Bookings
            .Include(b => b.Property)
            .Include(b => b.Tenant)
            .Where(b => b.PropertyId == propertyId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    // Check whether an approved booking overlaps the requested dates
    public async Task<bool> HasOverlappingApprovedBookingAsync(
        long propertyId,
        DateTime startDate,
        DateTime endDate,
        long? excludeBookingId = null)
    {
        return await _context.Bookings.AnyAsync(b =>
            b.PropertyId == propertyId &&
            b.Status == BookingStatus.Approved &&
            b.StartDate < endDate &&
            b.EndDate > startDate &&
            (excludeBookingId == null || b.Id != excludeBookingId.Value));
    }

    // Update an existing booking
    public async Task<Booking> UpdateAsync(Booking booking)
    {
        _context.Bookings.Update(booking);
        await _context.SaveChangesAsync();

        return booking;
    }
}
