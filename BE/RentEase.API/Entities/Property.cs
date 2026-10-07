namespace RentEase.API.Entities;

public class Property
{
    public long Id { get; set; }

    public long OwnerId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string PropertyType { get; set; } = string.Empty;

    public int Bedrooms { get; set; }

    public int Bathrooms { get; set; }

    public decimal MonthlyRent { get; set; }

    public decimal SecurityDeposit { get; set; }

    public string Status { get; set; } = "AVAILABLE";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public User Owner { get; set; } = null!;
}