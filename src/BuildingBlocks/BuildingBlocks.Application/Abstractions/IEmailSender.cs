namespace BuildingBlocks.Application.Abstractions
{
    public interface IEmailSender
    {
        Task SendAsync(string to,
                       string subject,
                       string body,
                       bool isHtml,
                       CancellationToken cancellationToken = default);
    }
}