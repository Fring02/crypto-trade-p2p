using AuthService.Configuration;
using AuthService.Interfaces.Repositories;
using AuthService.Models;
using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;

namespace AuthService.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ILogger<UserRepository> _logger;
    private readonly string _connectionString;
    public UserRepository(IOptions<ConnectionStrings> options, ILogger<UserRepository> logger)
    {
        _logger = logger;
        _connectionString = options.Value.DefaultConnection;
    }
    public async Task<User> CreateAsync(User user, CancellationToken token = default)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(token);
        await using var transaction = await connection.BeginTransactionAsync(token);
        _logger.LogInformation("Beginning transaction in database...");
        try
        {
            var parameters = new DynamicParameters(new
            {
                email = user.Email, firstName = user.Firstname, lastName = user.Lastname,
                hash = user.Hash, salt = user.Salt, refreshToken = user.RefreshToken, refreshExpires = user.RefreshExpires, role = user.Role
            });
            var id = await connection.QuerySingleAsync<Guid>(
                @"INSERT INTO public.""Users""(""Id"", ""Firstname"", ""Lastname"", ""Email"", ""Hash"", ""Salt"",
                             ""RefreshToken"", ""RefreshExpires"", ""Role"")
                  VALUES (gen_random_uuid(), @firstName, @lastName, @email, @hash, @salt, @refreshToken, @refreshExpires, @role)
                  RETURNING ""Id"";", parameters, transaction);
            await transaction.CommitAsync(token);
            user.Id = id;
            _logger.LogInformation("Inserted new user with id {UserId}", user.Id);
            return user;
        }
        catch (Exception e)
        {
            _logger.LogError("Error while inserting new user: {Message}", e.Message);
            await transaction.RollbackAsync(token); 
            throw;
        }
    }
    public async Task<bool> UpdateAsync(Guid id, User user, CancellationToken token = default)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(token);
        await using var transaction = await connection.BeginTransactionAsync(token);
        _logger.LogInformation("Beginning transaction in SQL Server database...");
        try
        {
            var result = await connection.ExecuteAsync(@"UPDATE public.""Users"" SET ""RefreshToken"" = @refreshToken, ""RefreshExpires"" = @refreshExpires where ""Id"" = @id",
                new DynamicParameters(new {refreshToken = user.RefreshToken, refreshExpires = user.RefreshExpires, id}), transaction);
            await transaction.CommitAsync(token);
            _logger.LogInformation("Updated user's refresh token and expiry date with id {UserId}", user.Id);
            return result > 0;
        }
        catch (Exception e)
        {
            _logger.LogError("Error while updating user: {Message}", e.Message);
            await transaction.RollbackAsync(token); throw;
        }
    }
    public async Task<bool> ExistsAsync(string email, CancellationToken token = default)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(token);
        return await connection.ExecuteScalarAsync<bool>(@"SELECT COUNT(1) FROM public.""Users"" WHERE ""Email"" = @email", new DynamicParameters(new {email}));
    }
    public async Task<User?> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(token);
        return await connection.QuerySingleOrDefaultAsync<User>(@"SELECT ""Id"", ""Email"", ""Hash"", ""Salt"", ""RefreshToken"", ""RefreshExpires""
                                                                    FROM public.""Users"" WHERE ""Id"" = @id LIMIT 1", new DynamicParameters(new {id}));
    }
    public async Task<User?> GetByEmailAsync(string email, CancellationToken token = default)
    {        
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(token);
        return await connection.QuerySingleOrDefaultAsync<User>(@"SELECT ""Id"", ""Email"", ""Hash"", ""Salt"", ""RefreshToken"", ""RefreshExpires"", ""Role""
                                                                    FROM public.""Users"" WHERE ""Email"" = @email LIMIT 1", new DynamicParameters(new {email}));
    }
}