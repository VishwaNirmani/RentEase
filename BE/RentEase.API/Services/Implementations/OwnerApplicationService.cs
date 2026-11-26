using RentEase.API.DTO.OwnerApplications;
using RentEase.API.Entities;
using RentEase.API.Enum;
using RentEase.API.Repositories.Interfaces;
using RentEase.API.Services.Interfaces;

namespace RentEase.API.Services.Implementations;

public class OwnerApplicationService : IOwnerApplicationService
{
    private readonly IOwnerApplicationRepository _applicationRepository;
    private readonly IUserRepository _userRepository;

    public OwnerApplicationService(
        IOwnerApplicationRepository applicationRepository,
        IUserRepository userRepository)
    {
        _applicationRepository = applicationRepository;
        _userRepository = userRepository;
    }

    public async Task<OwnerApplicationResponse> SubmitAsync(
        long userId,
        CreateOwnerApplicationRequest request)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user is null || !user.IsActive)
        {
            throw new UnauthorizedAccessException(
                "The authenticated user is no longer active.");
        }

        if (user.Role != UserRole.Tenant)
        {
            throw new UnauthorizedAccessException(
                "Only tenant accounts can apply to become owners.");
        }

        var existingApplication =
            await _applicationRepository.GetLatestByUserIdAsync(userId);

        if (existingApplication?.Status == OwnerApplicationStatus.Pending)
        {
            throw new InvalidOperationException(
                "A pending owner application already exists.");
        }

        var application = new OwnerApplication
        {
            UserId = userId,
            Reason = request.Reason.Trim(),
            Status = OwnerApplicationStatus.Pending,
            SubmittedAt = DateTime.UtcNow
        };

        var createdApplication =
            await _applicationRepository.CreateAsync(application);

        var savedApplication =
            await _applicationRepository.GetByIdAsync(createdApplication.Id);

        return Map(savedApplication ?? createdApplication);
    }

    public async Task<OwnerApplicationResponse?> GetLatestForUserAsync(
        long userId)
    {
        var application =
            await _applicationRepository.GetLatestByUserIdAsync(userId);

        return application is null ? null : Map(application);
    }

    public async Task<List<OwnerApplicationResponse>> GetPendingAsync()
    {
        var applications = await _applicationRepository.GetPendingAsync();

        return applications.Select(Map).ToList();
    }

    public async Task<OwnerApplicationResponse?> GetByIdAsync(long id)
    {
        var application = await _applicationRepository.GetByIdAsync(id);

        return application is null ? null : Map(application);
    }

    public Task<OwnerApplicationResponse> ApproveAsync(
        long applicationId,
        long adminId,
        string? adminRemarks)
    {
        return ReviewAsync(
            applicationId,
            adminId,
            OwnerApplicationStatus.Approved,
            adminRemarks);
    }

    public Task<OwnerApplicationResponse> RejectAsync(
        long applicationId,
        long adminId,
        string? adminRemarks)
    {
        return ReviewAsync(
            applicationId,
            adminId,
            OwnerApplicationStatus.Rejected,
            adminRemarks);
    }

    private async Task<OwnerApplicationResponse> ReviewAsync(
        long applicationId,
        long adminId,
        OwnerApplicationStatus status,
        string? adminRemarks)
    {
        var application =
            await _applicationRepository.GetByIdAsync(applicationId);

        if (application is null)
        {
            throw new KeyNotFoundException("Owner application not found.");
        }

        if (application.UserId == adminId)
        {
            throw new UnauthorizedAccessException(
                "You cannot review your own owner application.");
        }

        var reviewedApplication = await _applicationRepository.ReviewAsync(
            applicationId,
            status,
            DateTime.UtcNow,
            string.IsNullOrWhiteSpace(adminRemarks)
                ? null
                : adminRemarks.Trim());

        if (reviewedApplication is null)
        {
            throw new KeyNotFoundException("Owner application not found.");
        }

        return Map(reviewedApplication);
    }

    private static OwnerApplicationResponse Map(OwnerApplication application)
    {
        return new OwnerApplicationResponse
        {
            Id = application.Id,
            UserId = application.UserId,
            ApplicantFirstName = application.User.FirstName,
            ApplicantLastName = application.User.LastName,
            ApplicantEmail = application.User.Email,
            Reason = application.Reason,
            Status = application.Status.ToString(),
            SubmittedAt = application.SubmittedAt,
            ReviewedAt = application.ReviewedAt,
            AdminRemarks = application.AdminRemarks
        };
    }
}
