namespace RentEase.API.DTO.OwnerApplications;

public class OwnerApplicationResponse
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public string ApplicantFirstName { get; set; } = string.Empty;

    public string ApplicantLastName { get; set; } = string.Empty;

    public string ApplicantEmail { get; set; } = string.Empty;

    public string Reason { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public DateTime SubmittedAt { get; set; }

    public DateTime? ReviewedAt { get; set; }

    public string? AdminRemarks { get; set; }
}
