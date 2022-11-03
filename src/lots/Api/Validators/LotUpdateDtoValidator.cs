using Domain.Enums;
using FluentValidation;
using Shared.Dtos;

namespace Api.Validators;

public class LotUpdateDtoValidator : AbstractValidator<LotUpdateDto>
{
    public LotUpdateDtoValidator()
    {
        When(dto => dto.MinLimit != null && dto.MaxLimit != null, () =>
        {
            RuleFor(dto => dto.MinLimit).GreaterThan(0).WithMessage("Minimum limit must be greater than 0")
                .LessThan(dto => dto.MaxLimit).WithMessage("Minimum limit must be less than maximum limit")
                .LessThan(dto => dto.Supply).WithMessage("Minimum limit must not exceed supply");
            RuleFor(dto => dto.MaxLimit).LessThanOrEqualTo(1).WithMessage("Maximum limit must be less than or equal to 1")
                .GreaterThan(dto => dto.MinLimit).WithMessage("Maximum limit must be greater than minimum limit")
                .LessThan(dto => dto.Supply).WithMessage("Maximum limit must not exceed supply");;
        });
        RuleFor(dto => dto.LotType).IsEnumName(typeof(LotType)).WithMessage("Lot type must be either Sell or Buy")
            .When(dto => !string.IsNullOrWhiteSpace(dto.LotType));
        
        RuleFor(dto => dto.FiatType).IsEnumName(typeof(FiatCurrency)).WithMessage("Fiat currency must be either KZT, RUB or USD")
            .When(dto => !string.IsNullOrWhiteSpace(dto.FiatType));
        
        RuleFor(dto => dto.CryptoType).IsEnumName(typeof(CryptoCurrency)).WithMessage("Crypto currency must be ETH or ERC20")
            .When(dto => !string.IsNullOrWhiteSpace(dto.CryptoType));
        
        RuleFor(dto => dto.Supply).LessThanOrEqualTo(1).GreaterThan(0).WithMessage("Supply must vary between 0 exclusively and 1")
            .When(d => d.Supply != null);
        
        RuleFor(dto => dto.Price).GreaterThan(0).WithMessage("Price must be greater than 0")
            .When(dto => dto.Price != null);
    }
}