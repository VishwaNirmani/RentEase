using FluentValidation;
using RentEase.API.DTOs;
using RentEase.API.DTOs.Properties;

public class CreatePropertyRequestValidator
    : AbstractValidator<CreatePropertyRequest>
{
    public CreatePropertyRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Property title is required.")
            .MinimumLength(3)
            .WithMessage("Property title must contain at least 3 characters.")
            .MaximumLength(150)
            .WithMessage("Property title cannot exceed 150 characters.");

        RuleFor(x => x.MonthlyRent)
            .GreaterThan(0)
            .WithMessage("Monthly rent must be greater than zero.");

        RuleFor(x => x.Bedrooms)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Bedrooms cannot be negative.");
        RuleFor(x => x.SecurityDeposit)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Security cannot be negative");
        RuleFor(x => x.Address)
            .NotEmpty()
            .WithMessage("Address is required.")
            .MaximumLength(250)
            .WithMessage("Address cannot exceed 250 characters.");
        RuleFor(x => x.City)
            .NotEmpty()
            .WithMessage("City is required.")
            .MaximumLength(100)
            .WithMessage("City cannot exceed 100 characters.");
        RuleFor(x=>x.Bathrooms)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Bathrooms cannot be negative.");
        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Property description is required.")
            .MaximumLength(2000)
            .WithMessage("Description cannot exceed 2000 characters.");
        RuleFor(x => x.PropertyType)
            .IsInEnum()
            .WithMessage("Please select a valid property type.");
        

    }
}