using AutoMapper;
using Domain.Models;
using Shared.Dtos;

namespace Application.Mapping;

public class RequisiteProfile : Profile
{
    public RequisiteProfile()
    {
        CreateMap<RequisiteCreateDto, RequisiteDetails>();
        CreateMap<RequisiteUpdateDto, RequisiteDetails>();
        CreateMap<RequisiteDetails, RequisiteViewDto>()
            .ForMember(dto => dto.UserName, opts => opts.MapFrom(d => d.User.Firstname + " " + d.User.Lastname))
            .ForMember(dto => dto.UserEmail, opts => opts.MapFrom(d => d.User.Email));
        CreateMap<RequisiteDetails, RequisiteItemDto>();
        CreateMap<RequisiteDetails, RequisiteUserDto>();
    }
}