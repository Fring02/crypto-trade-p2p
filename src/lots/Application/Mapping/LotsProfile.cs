using AutoMapper;
using Domain.Enums;
using Domain.Models;
using Shared.Dtos;

namespace Application.Mapping;
public class LotsProfile : Profile
{
    public LotsProfile()
    {
        CreateMap<LotCreateDto, Lot>()
            .ForMember(lot => lot.Type, opt => opt.MapFrom(dto => Enum.Parse<LotType>(dto.LotType)))
            .ForMember(lot => lot.FiatType, opt => opt.MapFrom(dto => Enum.Parse<FiatCurrency>(dto.FiatType)))
            .ForMember(lot => lot.CryptoType, opt => opt.MapFrom(dto => Enum.Parse<CryptoCurrency>(dto.CryptoType)));
        CreateMap<Lot, LotCreateDto>()
            .ForMember(lot => lot.LotType, opt => opt.MapFrom(dto => Enum.GetName(dto.Type)))
            .ForMember(lot => lot.FiatType, opt => opt.MapFrom(dto => Enum.GetName(dto.FiatType)))
            .ForMember(lot => lot.CryptoType, opt => opt.MapFrom(dto => Enum.GetName(dto.CryptoType)));
        CreateMap<Lot, LotItemDto>()
            .ForMember(dto => dto.LotType, opt => opt.MapFrom(lot => Enum.GetName(lot.Type)))
            .ForMember(dto => dto.FiatType, opt => opt.MapFrom(lot => Enum.GetName(lot.FiatType)))
            .ForMember(dto => dto.CryptoType, opt => opt.MapFrom(lot => Enum.GetName(lot.CryptoType)))
            .ForMember(dto => dto.CreatedAt, opt => opt.MapFrom(lot => lot.CreatedAt.ToString("MMMM dd, yyyy")));
        CreateMap<Lot, LotViewDto>()
            .ForMember(dto => dto.LotType, opt => opt.MapFrom(lot => Enum.GetName(lot.Type)))
            .ForMember(dto => dto.FiatType, opt => opt.MapFrom(lot => Enum.GetName(lot.FiatType)))
            .ForMember(dto => dto.CryptoType, opt => opt.MapFrom(lot => Enum.GetName(lot.CryptoType)))
            .ForMember(dto => dto.CreatedAt, opt => opt.MapFrom(lot => lot.CreatedAt.ToString("MMMM dd, yyyy")));
        CreateMap<Lot, LotOwnerDto>();
    }
}