using System.Text.RegularExpressions;
using FluentValidation;
using Shared.Dtos;

namespace Api.Validators;

public class RequisiteUpdateDtoValidator : AbstractValidator<RequisiteUpdateDto>
{
    public RequisiteUpdateDtoValidator()
    {
        RuleFor(r => r.CreditCardNumber).CreditCard().WithMessage("Credit card is invalid")
            .When(r => !string.IsNullOrWhiteSpace(r.CreditCardNumber));
        RuleFor(r => r.PhoneNumber).Matches(new Regex(@"^\+?77([0124567][0-8]\d{7})$")).WithMessage("Phone number is invalid")
            .When(r => !string.IsNullOrWhiteSpace(r.PhoneNumber));
        RuleFor(r => r.BankName).MaximumLength(20).WithMessage("Bank name's length must not exceed 20 characters")
            .When(r => !string.IsNullOrWhiteSpace(r.BankName));
    }
}