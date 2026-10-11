using System.ComponentModel.DataAnnotations;

namespace RentEase.API.DTOs.Properties;
using RentEase.API.Enum;


public class CreatePropertyRequest
{
    public long OwnerId { get; set; }


    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public required PropertyType PropertyType { get; set; }

    public int Bedrooms { get; set; }

    public int Bathrooms { get; set; }

    public decimal MonthlyRent { get; set; }

    public decimal SecurityDeposit { get; set; }
}