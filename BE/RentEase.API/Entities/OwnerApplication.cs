using RentEase.API.Enum;

namespace RentEase.API.Entities;

public class OwnerApplication
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public string Reason { get; set; } = string.Empty;

    public OwnerApplicationStatus Status { get; set; } = OwnerApplicationStatus.Pending;

    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ReviewedAt { get; set; }

    public string? AdminRemarks { get; set; }

    public User User { get; set; } = null!;
}
