using FluentValidation;
using RentEase.API.DTO.Bookings;

namespace RentEase.API.Validators.Bookings;

public class CreateBookingRequestValidator
    : AbstractValidator<CreateBookingRequest>
{
    public CreateBookingRequestValidator()
    {
        RuleFor(x => x.PropertyId)
            .GreaterThan(0)
            .WithMessage("Please select a valid property.");

        RuleFor(x => x.StartDate)
            .NotEmpty()
            .WithMessage("Start date is required.");

        RuleFor(x => x.EndDate)
            .NotEmpty()
            .WithMessage("End date is required.");

        RuleFor(x => x.EndDate)
            .Must((request, endDate) =>
                endDate.Date > request.StartDate.Date)
            .WithMessage("End date must be after the start date.");

        RuleFor(x => x.StartDate)
            .Must(startDate => startDate.Date >= DateTime.UtcNow.Date)
            .WithMessage("Start date cannot be in the past.");
    }
}