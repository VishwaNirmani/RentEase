namespace RentEase.API.DTOs.Properties;
using RentEase.API.Enum;

public class PropertyResponse
{
    public long Id { get; set; }

    public long OwnerId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public PropertyType PropertyType { get; set; } 

    public int Bedrooms { get; set; }

    public int Bathrooms { get; set; }

    public decimal MonthlyRent { get; set; }

    public decimal SecurityDeposit { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}