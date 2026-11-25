namespace RentEase.API.DTO.Bookings;

public class CreateBookingRequest
{
    public long PropertyId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }
}