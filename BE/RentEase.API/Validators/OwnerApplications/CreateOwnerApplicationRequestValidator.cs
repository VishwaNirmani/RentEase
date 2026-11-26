using FluentValidation;
using RentEase.API.DTO.OwnerApplications;

namespace RentEase.API.Validators.OwnerApplications;

public class CreateOwnerApplicationRequestValidator
    : AbstractValidator<CreateOwnerApplicationRequest>
{
    public CreateOwnerApplicationRequestValidator()
    {
        RuleFor(request => request.Reason)
            .NotEmpty()
            .Must(reason => !string.IsNullOrWhiteSpace(reason))
            .WithMessage("A reason for applying is required.")
            .MaximumLength(2000);
    }
}
