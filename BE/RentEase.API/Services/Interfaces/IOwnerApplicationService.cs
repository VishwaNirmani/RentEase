using RentEase.API.DTO.OwnerApplications;

namespace RentEase.API.Services.Interfaces;

public interface IOwnerApplicationService
{
    Task<OwnerApplicationResponse> SubmitAsync(
        long userId,
        CreateOwnerApplicationRequest request);

    Task<OwnerApplicationResponse?> GetLatestForUserAsync(long userId);

    Task<List<OwnerApplicationResponse>> GetPendingAsync();

    Task<OwnerApplicationResponse?> GetByIdAsync(long id);

    Task<OwnerApplicationResponse> ApproveAsync(
        long applicationId,
        long adminId,
        string? adminRemarks);

    Task<OwnerApplicationResponse> RejectAsync(
        long applicationId,
        long adminId,
        string? adminRemarks);
}
