using Domain.Enums;
using FluentValidation;
using Shared.Dtos;

namespace Api.Validators;

public class LotFilterDtoValidator : AbstractValidator<LotFilterDto>
{
    public LotFilterDtoValidator()
    {
        RuleFor(l => l.LotType).IsEnumName(typeof(LotType))
                .WithMessage(@"Lot type must be either ""Buy"" or ""Sell""").When(l => !string.IsNullOrWhiteSpace(l.LotType));
        RuleFor(l => l.CryptoType).IsEnumName(typeof(CryptoCurrency))
                .WithMessage(@"Cryptocurrency type must be either ""ETH"" or ""ERC20""").When(l => !string.IsNullOrWhiteSpace(l.CryptoType));
        RuleFor(l => l.FiatType).IsEnumName(typeof(FiatCurrency))
                .WithMessage(@"Fiat type must be either ""KZT"" or ""RUB"" or ""USD""").When(l => !string.IsNullOrWhiteSpace(l.FiatType));
        RuleFor(l => l.MinLimit).GreaterThan(0).LessThanOrEqualTo(l => l.MaxLimit).When(l => l.MaxLimit != null);
        RuleFor(l => l.MaxLimit).GreaterThan(0).GreaterThanOrEqualTo(l => l.MinLimit).When(l => l.MinLimit != null);
        RuleFor(l => l.MinSupply).GreaterThan(0).LessThanOrEqualTo(l => l.MaxSupply).When(l => l.MaxSupply != null);
        RuleFor(l => l.MaxSupply).GreaterThan(0).GreaterThanOrEqualTo(l => l.MinSupply).When(l => l.MinSupply != null);
        RuleFor(l => l.MinPrice).GreaterThan(0).LessThanOrEqualTo(l => l.MaxPrice).When(l => l.MaxPrice != null);
        RuleFor(l => l.MaxPrice).GreaterThan(0).GreaterThanOrEqualTo(l => l.MinPrice).When(l => l.MinPrice != null);
    }
}