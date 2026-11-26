using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentEase.API.DTO.OwnerApplications;
using RentEase.API.Services.Interfaces;

namespace RentEase.API.Controllers;

[ApiController]
[Route("api/owner-applications")]
[Authorize]
public class OwnerApplicationsController : ControllerBase
{
    private readonly IOwnerApplicationService _applicationService;
    private readonly IValidator<CreateOwnerApplicationRequest> _createValidator;
    private readonly IValidator<ReviewOwnerApplicationRequest> _reviewValidator;

    public OwnerApplicationsController(
        IOwnerApplicationService applicationService,
        IValidator<CreateOwnerApplicationRequest> createValidator,
        IValidator<ReviewOwnerApplicationRequest> reviewValidator)
    {
        _applicationService = applicationService;
        _createValidator = createValidator;
        _reviewValidator = reviewValidator;
    }

    [HttpPost]
    [Authorize(Roles = "Tenant")]
    public async Task<IActionResult> Submit(
        CreateOwnerApplicationRequest request)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        var validationResult = await _createValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(new
            {
                errors = validationResult.Errors.Select(error => error.ErrorMessage)
            });
        }

        try
        {
            var application = await _applicationService.SubmitAsync(
                userId,
                request);

            return CreatedAtAction(
                nameof(GetMyApplication),
                application);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(
                StatusCodes.Status403Forbidden,
                new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpGet("my-application")]
    [Authorize(Roles = "Tenant")]
    public async Task<IActionResult> GetMyApplication()
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        var application =
            await _applicationService.GetLatestForUserAsync(userId);

        return application is null
            ? NotFound(new { message = "No owner application was found." })
            : Ok(application);
    }

    [HttpGet("admin/pending")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetPendingApplications()
    {
        var applications = await _applicationService.GetPendingAsync();

        return Ok(applications);
    }

    [HttpGet("admin/{id:long}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetApplication(long id)
    {
        var application = await _applicationService.GetByIdAsync(id);

        return application is null
            ? NotFound(new { message = "Owner application not found." })
            : Ok(application);
    }

    [HttpPost("admin/{id:long}/approve")]
    [Authorize(Roles = "Admin")]
    public Task<IActionResult> Approve(
        long id,
        ReviewOwnerApplicationRequest request)
    {
        return ReviewAsync(id, request, approve: true);
    }

    [HttpPost("admin/{id:long}/reject")]
    [Authorize(Roles = "Admin")]
    public Task<IActionResult> Reject(
        long id,
        ReviewOwnerApplicationRequest request)
    {
        return ReviewAsync(id, request, approve: false);
    }

    private async Task<IActionResult> ReviewAsync(
        long id,
        ReviewOwnerApplicationRequest request,
        bool approve)
    {
        if (!TryGetUserId(out var adminId))
        {
            return Unauthorized();
        }

        var validationResult = await _reviewValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(new
            {
                errors = validationResult.Errors.Select(error => error.ErrorMessage)
            });
        }

        try
        {
            var application = approve
                ? await _applicationService.ApproveAsync(
                    id,
                    adminId,
                    request.AdminRemarks)
                : await _applicationService.RejectAsync(
                    id,
                    adminId,
                    request.AdminRemarks);

            return Ok(application);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(
                StatusCodes.Status403Forbidden,
                new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    private bool TryGetUserId(out long userId)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        return long.TryParse(userIdClaim, out userId);
    }
}
