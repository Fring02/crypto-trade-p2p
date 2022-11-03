using AuthService.Models;
namespace AuthService.Interfaces.Repositories;
public interface IUserRepository
{
    Task<User> CreateAsync(User user, CancellationToken token = default);
    Task<bool> UpdateAsync(Guid id, User user, CancellationToken token = default);
    Task<bool> ExistsAsync(string email, CancellationToken token = default);
    Task<User?> GetByIdAsync(Guid id, CancellationToken token = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken token = default);
}