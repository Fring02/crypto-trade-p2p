using Domain.Models;
using Domain.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using Shared.Interfaces.Services;

namespace Application.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly ILogger<EmailService> _logger;
    public EmailService(IOptions<EmailSettings> settings, ILogger<EmailService> logger)
    {
        _logger = logger;
        _settings = settings.Value;
    }

    public async Task SendEmailAsync(EmailMessage message, CancellationToken token = default)
    {
        var mimeMessage = new MimeMessage();
        mimeMessage.From.Add(MailboxAddress.Parse(_settings.Username));
        mimeMessage.To.Add(MailboxAddress.Parse(message.To));
        mimeMessage.Subject = message.Subject;
        mimeMessage.Body = new TextPart(TextFormat.Html) { Text = message.Body };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(host: _settings.Host, port: _settings.Port, options: SecureSocketOptions.StartTls, cancellationToken: token);
        await smtp.AuthenticateAsync(_settings.Username, _settings.Password, token);
        var response = await smtp.SendAsync(mimeMessage, token);
        _logger.LogInformation("Email sent: {Response}", response);
        await smtp.DisconnectAsync(true, token);
    }
}