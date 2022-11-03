using Domain.Enums;
using FluentValidation;
using Shared.Dtos;

namespace Api.Validators;

public class LotCreateDtoValidator : AbstractValidator<LotCreateDto>
{
    public LotCreateDtoValidator()
    {
        RuleFor(dto => dto.OwnerEmail).EmailAddress().WithMessage("Wrong email address format");
        RuleFor(dto => dto.OwnerWallet).NotEmpty();
        RuleFor(dto => dto.MinLimit).GreaterThan(0).WithMessage("Minimum limit must be greater than 0");
        RuleFor(dto => dto.MaxLimit).LessThanOrEqualTo(1).WithMessage("Maximum limit must be less than or equal to 1");
        RuleFor(dto => dto.LotType).IsEnumName(typeof(LotType)).WithMessage("Lot type must be either Sell or Buy");
        RuleFor(dto => dto.FiatType).IsEnumName(typeof(FiatCurrency)).WithMessage("Fiat currency must be either KZT, RUB or USD");
        RuleFor(dto => dto.CryptoType).IsEnumName(typeof(CryptoCurrency)).WithMessage("Crypto currency must be ETH or ERC20");
        RuleFor(dto => dto.Supply).LessThanOrEqualTo(1).GreaterThan(0).WithMessage("Supply must vary between 0 exclusively and 1");
        RuleFor(dto => dto.Price).GreaterThan(0).WithMessage("Price must be greater than 0");
    }
}