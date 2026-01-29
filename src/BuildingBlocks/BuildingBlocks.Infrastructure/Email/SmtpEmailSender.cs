using BuildingBlocks.Application.Abstractions;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

public sealed class SmtpEmailSender : IEmailSender
{
    private readonly EmailOptions _options;

    public SmtpEmailSender(IOptions<EmailOptions> options)
    {
        _options = options.Value;
    }

    public async Task SendAsync(string to, string subject, string body, bool isHtml, CancellationToken cancellationToken = default)
    {
        var message = new MailMessage(_options.From,
                                      to,
                                      subject,
                                      body)
        {
            IsBodyHtml = isHtml
        };

        using var client = new SmtpClient(_options.Host, _options.Port)
        {
            Credentials = new NetworkCredential(
                _options.Username,
                _options.Password),
            EnableSsl = _options.EnableSsl
        };

        await client.SendMailAsync(message, cancellationToken);
    }
}