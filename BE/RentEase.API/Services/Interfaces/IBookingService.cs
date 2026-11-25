using RentEase.API.DTO.Bookings;

namespace RentEase.API.Services.Interfaces;

public interface IBookingService
{
    Task<BookingResponse> CreateAsync(
        long tenantId,
        CreateBookingRequest request);

    Task<BookingResponse?> GetByIdForUserAsync(
        long id,
        long userId);

    Task<List<BookingResponse>> GetByTenantIdAsync(
        long tenantId);

    Task<List<BookingResponse>> GetByPropertyForOwnerAsync(
        long propertyId,
        long ownerId);

    Task<BookingResponse> ApproveAsync(
        long bookingId,
        long ownerId);

    Task<BookingResponse> RejectAsync(
        long bookingId,
        long ownerId);

    Task<BookingResponse> CancelAsync(
        long bookingId,
        long tenantId);

    Task<BookingResponse> CompleteAsync(
        long bookingId,
        long ownerId);
}