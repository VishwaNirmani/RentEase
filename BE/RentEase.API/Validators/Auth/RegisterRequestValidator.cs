using FluentValidation;
using RentEase.API.DTO.Auth;

namespace RentEase.API.Validators.Auth;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name is required")
            .MaximumLength(100);
        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name is required")
            .MaximumLength(100);
        RuleFor(y => y.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Email is invalid")
            .MaximumLength(255);
        RuleFor(y => y.Password)
            .NotEmpty()
            .WithMessage("Password is required")
            .MinimumLength(8)
            .WithMessage("Password must contain 8 characters")
            .MaximumLength(128)
            /*.Matches("[A-Z]")
            .WithMessage("password must contain upper case letter")
            .Matches("[a-z]")
            .WithMessage("password must contain lower case letter")
            .Matches("[0-9]")
            .WithMessage("password must contain number")
            .Matches("[^a-zA-Z0-9]")
            .WithMessage("password must contain special character")*/;


        RuleFor(y => y.PhoneNumber)
            .NotEmpty()
            .WithMessage("Phone number is required")
            .Matches(@"^(?:\+94|0)7\d{8}$").WithMessage("Phone number is invalid");
    }
}