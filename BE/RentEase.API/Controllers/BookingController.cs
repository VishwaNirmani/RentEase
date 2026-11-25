
using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentEase.API.DTO.Bookings;
using RentEase.API.Services.Interfaces;

namespace RentEase.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;
    private readonly IValidator<CreateBookingRequest> _validator;

    public BookingController(
        IBookingService bookingService,
        IValidator<CreateBookingRequest> validator)
    {
        _bookingService = bookingService;
        _validator = validator;
    }

    // Get the authenticated user's ID from JWT
    private bool TryGetUserId(out long userId)
    {
        var userIdClaim = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        return long.TryParse(userIdClaim, out userId);
    }

    // 1. Create a booking request
    [HttpPost]
    [Authorize(Roles = "Tenant")]
    public async Task<IActionResult> Create(
        CreateBookingRequest request)
    {
        if (!TryGetUserId(out var tenantId))
        {
            return Unauthorized();
        }

        var validationResult = await _validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(new
            {
                errors = validationResult.Errors.Select(e => e.ErrorMessage)
            });
        }

        try
        {
            var booking = await _bookingService.CreateAsync(
                tenantId,
                request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = booking.Id },
                booking);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    // 2. Get a booking by ID
    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        try
        {
            var booking = await _bookingService.GetByIdForUserAsync(
                id,
                userId);

            if (booking == null)
            {
                return NotFound();
            }

            return Ok(booking);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    // 3. Get the current tenant's bookings
    [HttpGet("my-bookings")]
    [Authorize(Roles = "Tenant")]
    public async Task<IActionResult> GetMyBookings()
    {
        if (!TryGetUserId(out var tenantId))
        {
            return Unauthorized();
        }

        var bookings = await _bookingService.GetByTenantIdAsync(
            tenantId);

        return Ok(bookings);
    }

    // 4. Get bookings for one of the owner's properties
    [HttpGet("property/{propertyId:long}")]
    [Authorize(Roles = "Owner")]
    public async Task<IActionResult> GetPropertyBookings(
        long propertyId)
    {
        if (!TryGetUserId(out var ownerId))
        {
            return Unauthorized();
        }

        try
        {
            var bookings =
                await _bookingService.GetByPropertyForOwnerAsync(
                    propertyId,
                    ownerId);

            return Ok(bookings);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    // 5. Approve a booking
    [HttpPost("{id:long}/approve")]
    [Authorize(Roles = "Owner")]
    public async Task<IActionResult> Approve(long id)
    {
        if (!TryGetUserId(out var ownerId))
        {
            return Unauthorized();
        }

        try
        {
            var booking = await _bookingService.ApproveAsync(
                id,
                ownerId);

            return Ok(booking);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    // 6. Reject a booking
    [HttpPost("{id:long}/reject")]
    [Authorize(Roles = "Owner")]
    public async Task<IActionResult> Reject(long id)
    {
        if (!TryGetUserId(out var ownerId))
        {
            return Unauthorized();
        }

        try
        {
            var booking = await _bookingService.RejectAsync(
                id,
                ownerId);

            return Ok(booking);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    // 7. Cancel a booking
    [HttpPost("{id:long}/cancel")]
    [Authorize(Roles = "Tenant")]
    public async Task<IActionResult> Cancel(long id)
    {
        if (!TryGetUserId(out var tenantId))
        {
            return Unauthorized();
        }

        try
        {
            var booking = await _bookingService.CancelAsync(
                id,
                tenantId);

            return Ok(booking);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    // 8. Mark a booking as completed
    [HttpPost("{id:long}/complete")]
    [Authorize(Roles = "Owner")]
    public async Task<IActionResult> Complete(long id)
    {
        if (!TryGetUserId(out var ownerId))
        {
            return Unauthorized();
        }

        try
        {
            var booking = await _bookingService.CompleteAsync(
                id,
                ownerId);

            return Ok(booking);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
}
