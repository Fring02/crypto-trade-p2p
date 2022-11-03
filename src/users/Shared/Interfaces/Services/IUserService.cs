using Domain.Models;
using Shared.Dtos;
using Shared.Interfaces.Services.Base;

namespace Shared.Interfaces.Services;

public interface IUserService : ICrudService<User, Guid, UserUpdateDto>
{
}