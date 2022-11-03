using AutoMapper;
using Domain.Models;
using Shared.Dtos;

namespace Application.Mapping;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserItemDto>().ForMember(u => u.FullName, opt => opt.MapFrom(u => u.Firstname + " " + u.Lastname));
        CreateMap<User, UserViewDto>();
        CreateMap<UserUpdateDto, User>();
    }
}