using System.Security.Cryptography;
using System.Text;
using Application.Services.Base;
using AutoMapper;
using Domain.Models;
using Data.Contexts;
using Domain.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shared.Dtos;
using Shared.Interfaces.Services;

namespace Application.Services;

public class UserService : CrudBaseService<User, Guid, UserUpdateDto>, IUserService
{
    private readonly IEmailService _emailService;
    private readonly EmailSettings _settings;
    public UserService(ApplicationContext context, IMapper mapper, IEmailService emailService, IOptions<EmailSettings> options) : base(context, mapper)
    {
        _emailService = emailService;
        _settings = options.Value;
    }
    public override async Task<bool> ExistsAsync(User entity, CancellationToken token = default)
        => await Context.Users.AnyAsync(u => u.Email == entity.Email, token);

    public override async Task UpdateAsync(UserUpdateDto entity, CancellationToken token = default)
    {
        if (!string.IsNullOrEmpty(entity.Email))
        {
            var email = await Context.Users.Where(u => u.Id == entity.Id).Select(u => u.Email).FirstOrDefaultAsync(token);
            if (string.IsNullOrEmpty(email)) throw new ArgumentException($"User with id {entity.Id} is not found");
            await _emailService.SendEmailAsync(new()
            {
                Subject = "Email update alert", To = email, 
                Body = $@"<h1>Please, proceed to this <a href=""{_settings.Link}"">link</a> to update your email.</h1>"
            }, token);
        }
        if (!string.IsNullOrEmpty(entity.Password))
        {
            var user = await Context.Users.FindAsync(new object?[] { entity.Id }, cancellationToken: token);
            if (user is null) throw new ArgumentException($"User with id {entity.Id} is not found");
            using var hmac = new HMACSHA256();
            user.Salt = hmac.Key;
            user.Hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(entity.Password));
            Context.Users.Update(user);
        }
        await base.UpdateAsync(entity, token);
    }
}