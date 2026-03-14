using BuildingBlocks.Application.Abstractions;
using Users.Domain.Events;

namespace Users.Application.Features.Users.EventHandlers
{
    internal sealed class UserRegisteredEventHandler(IEmailSender _emailSender) : IDomainEventHandler<UserRegisteredEvent>
    {
        public async Task Handle(UserRegisteredEvent notification, CancellationToken cancellationToken)
        {
            var subject = "FitnessTracking - Hoş Geldiniz!";
            var body = $"""
                <h2>FitnessTracking'e Hoş Geldiniz!</h2>
                <p>Hesabınız başarıyla oluşturuldu.</p>
                <p>Fitness yolculuğunuza hemen başlayabilirsiniz.</p>
                """;

            await _emailSender.SendAsync(
                notification.Email,
                subject,
                body,
                isHtml: true,
                cancellationToken);
        }
    }
}