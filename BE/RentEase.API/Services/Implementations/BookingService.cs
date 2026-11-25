
using AutoMapper;
using RentEase.API.DTO.Bookings;
using RentEase.API.Entities;
using RentEase.API.Enum;
using RentEase.API.Repositories.Interfaces;
using RentEase.API.Services.Interfaces;

namespace RentEase.API.Services.Implementations;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IMapper _mapper;

    public BookingService(
        IBookingRepository bookingRepository,
        IPropertyRepository propertyRepository,
        IMapper mapper)
    {
        _bookingRepository = bookingRepository;
        _propertyRepository = propertyRepository;
        _mapper = mapper;
    }

    // 1. Create a booking request
    public async Task<BookingResponse> CreateAsync(
        long tenantId,
        CreateBookingRequest request)
    {
        var property = await _propertyRepository.GetByIdAsync(
            request.PropertyId);

        if (property == null)
        {
            throw new KeyNotFoundException("Property not found.");
        }

        // A tenant cannot book their own property
        if (property.OwnerId == tenantId)
        {
            throw new InvalidOperationException(
                "You cannot book your own property.");
        }

        // Only available properties can receive booking requests
        if (property.Status != "AVAILABLE")
        {
            throw new InvalidOperationException(
                "This property is not available for booking.");
        }

        // Check whether an approved booking already exists
        var hasConflict =
            await _bookingRepository.HasOverlappingApprovedBookingAsync(
                property.Id,
                request.StartDate.Date,
                request.EndDate.Date);

        if (hasConflict)
        {
            throw new InvalidOperationException(
                "This property is already booked for the selected dates.");
        }

        // Create the booking
        var booking = new Booking
        {
            TenantId = tenantId,
            PropertyId = property.Id,
            StartDate = request.StartDate.Date,
            EndDate = request.EndDate.Date,

            // These values come from the database, not the tenant
            AgreedMonthlyRent = property.MonthlyRent,
            AgreedSecurityDeposit = property.SecurityDeposit,

            Status = BookingStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await _bookingRepository.CreateAsync(booking);

        // Reload to get the related Property and Tenant
        var savedBooking = await _bookingRepository.GetByIdAsync(
            booking.Id);

        return _mapper.Map<BookingResponse>(savedBooking);
    }

    // 2. Get one booking for its tenant or property owner
    public async Task<BookingResponse?> GetByIdForUserAsync(
        long id,
        long userId)
    {
        var booking = await _bookingRepository.GetByIdAsync(id);

        if (booking == null)
        {
            return null;
        }

        // Only the tenant or the property's owner can view it
        if (booking.TenantId != userId &&
            booking.Property.OwnerId != userId)
        {
            throw new UnauthorizedAccessException(
                "You are not allowed to view this booking.");
        }

        return _mapper.Map<BookingResponse>(booking);
    }

    // 3. Get all bookings belonging to a tenant
    public async Task<List<BookingResponse>> GetByTenantIdAsync(
        long tenantId)
    {
        var bookings = await _bookingRepository.GetByTenantIdAsync(
            tenantId);

        return _mapper.Map<List<BookingResponse>>(bookings);
    }

    // 4. Get bookings for a property (owner only)
    public async Task<List<BookingResponse>> GetByPropertyForOwnerAsync(
        long propertyId,
        long ownerId)
    {
        var property = await _propertyRepository.GetByIdAsync(propertyId);

        if (property == null)
        {
            throw new KeyNotFoundException("Property not found.");
        }

        if (property.OwnerId != ownerId)
        {
            throw new UnauthorizedAccessException(
                "You are not allowed to view bookings for this property.");
        }

        var bookings = await _bookingRepository.GetByPropertyIdAsync(
            propertyId);

        return _mapper.Map<List<BookingResponse>>(bookings);
    }

    // 5. Approve a booking (property owner only)
    public async Task<BookingResponse> ApproveAsync(
        long bookingId,
        long ownerId)
    {
        var booking = await _bookingRepository.GetByIdAsync(bookingId);

        if (booking == null)
        {
            throw new KeyNotFoundException("Booking not found.");
        }

        // Verify property ownership
        if (booking.Property.OwnerId != ownerId)
        {
            throw new UnauthorizedAccessException(
                "You are not allowed to approve this booking.");
        }

        // Only Pending bookings can be approved
        if (booking.Status != BookingStatus.Pending)
        {
            throw new InvalidOperationException(
                "Only pending bookings can be approved.");
        }

        // Check availability again before approval
        var hasConflict =
            await _bookingRepository.HasOverlappingApprovedBookingAsync(
                booking.PropertyId,
                booking.StartDate,
                booking.EndDate,
                booking.Id);

        if (hasConflict)
        {
            throw new InvalidOperationException(
                "Another booking has already been approved for these dates.");
        }

        booking.Status = BookingStatus.Approved;
        booking.UpdatedAt = DateTime.UtcNow;

        await _bookingRepository.UpdateAsync(booking);

        return _mapper.Map<BookingResponse>(booking);
    }

    // 6. Reject a booking (property owner only)
    public async Task<BookingResponse> RejectAsync(
        long bookingId,
        long ownerId)
    {
        var booking = await _bookingRepository.GetByIdAsync(bookingId);

        if (booking == null)
        {
            throw new KeyNotFoundException("Booking not found.");
        }

        if (booking.Property.OwnerId != ownerId)
        {
            throw new UnauthorizedAccessException(
                "You are not allowed to reject this booking.");
        }

        if (booking.Status != BookingStatus.Pending)
        {
            throw new InvalidOperationException(
                "Only pending bookings can be rejected.");
        }

        booking.Status = BookingStatus.Rejected;
        booking.UpdatedAt = DateTime.UtcNow;

        await _bookingRepository.UpdateAsync(booking);

        return _mapper.Map<BookingResponse>(booking);
    }

    // 7. Cancel a booking (tenant only)
    public async Task<BookingResponse> CancelAsync(
        long bookingId,
        long tenantId)
    {
        var booking = await _bookingRepository.GetByIdAsync(bookingId);

        if (booking == null)
        {
            throw new KeyNotFoundException("Booking not found.");
        }

        if (booking.TenantId != tenantId)
        {
            throw new UnauthorizedAccessException(
                "You are not allowed to cancel this booking.");
        }

        if (booking.Status != BookingStatus.Pending &&
            booking.Status != BookingStatus.Approved)
        {
            throw new InvalidOperationException(
                "Only pending or approved bookings can be cancelled.");
        }

        booking.Status = BookingStatus.Cancelled;
        booking.UpdatedAt = DateTime.UtcNow;

        await _bookingRepository.UpdateAsync(booking);

        return _mapper.Map<BookingResponse>(booking);
    }

    // 8. Mark a booking as completed (property owner only)
    public async Task<BookingResponse> CompleteAsync(
        long bookingId,
        long ownerId)
    {
        var booking = await _bookingRepository.GetByIdAsync(bookingId);

        if (booking == null)
        {
            throw new KeyNotFoundException("Booking not found.");
        }

        if (booking.Property.OwnerId != ownerId)
        {
            throw new UnauthorizedAccessException(
                "You are not allowed to complete this booking.");
        }

        if (booking.Status != BookingStatus.Approved)
        {
            throw new InvalidOperationException(
                "Only approved bookings can be completed.");
        }

        if (booking.EndDate.Date > DateTime.UtcNow.Date)
        {
            throw new InvalidOperationException(
                "A booking cannot be completed before its end date.");
        }

        booking.Status = BookingStatus.Completed;
        booking.UpdatedAt = DateTime.UtcNow;

        await _bookingRepository.UpdateAsync(booking);

        return _mapper.Map<BookingResponse>(booking);
    }
}
