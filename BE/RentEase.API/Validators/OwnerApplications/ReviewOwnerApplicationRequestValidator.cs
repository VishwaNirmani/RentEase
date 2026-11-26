using FluentValidation;
using RentEase.API.DTO.OwnerApplications;

namespace RentEase.API.Validators.OwnerApplications;

public class ReviewOwnerApplicationRequestValidator
    : AbstractValidator<ReviewOwnerApplicationRequest>
{
    public ReviewOwnerApplicationRequestValidator()
    {
        RuleFor(request => request.AdminRemarks)
            .MaximumLength(2000);
    }
}
