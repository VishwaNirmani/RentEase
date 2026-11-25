using RentEase.API.Enum;

namespace RentEase.API.Entities;

public class Booking
{
    public long Id { get; set; }

    public long TenantId { get; set; }

    public long PropertyId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public decimal AgreedMonthlyRent { get; set; }

    public decimal AgreedSecurityDeposit { get; set; }

    public BookingStatus Status { get; set; } = BookingStatus.Pending;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
    
    public User Tenant { get; set; } = null!;

    public Property Property { get; set; } = null!;
}