using RentEase.API.Enum;

namespace RentEase.API.DTO.Bookings;

public class BookingResponse
{
    public long Id { get; set; }

    public long PropertyId { get; set; }

    public string PropertyTitle { get; set; } = string.Empty;

    public long TenantId { get; set; }

    public string TenantName { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public decimal AgreedMonthlyRent { get; set; }

    public decimal AgreedSecurityDeposit { get; set; }

    public BookingStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }
}