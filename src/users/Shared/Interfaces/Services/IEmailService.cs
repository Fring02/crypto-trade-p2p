using Domain.Models;

namespace Shared.Interfaces.Services;

public interface IEmailService
{
    Task SendEmailAsync(EmailMessage message, CancellationToken token = default);
}