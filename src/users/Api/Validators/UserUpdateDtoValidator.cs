using FluentValidation;
using Shared.Dtos;

namespace Api.Validators;

public class UserUpdateDtoValidator : AbstractValidator<UserUpdateDto>
{
    public UserUpdateDtoValidator()
    {
        RuleFor(u => u.Email).EmailAddress().WithMessage("Email address is invalid")
            .When(u => !string.IsNullOrWhiteSpace(u.Email));
        RuleFor(u => u.Password).MinimumLength(8).WithMessage("Minimum length for password is 8 characters")
            .When(u => !string.IsNullOrWhiteSpace(u.Password));
    }
}