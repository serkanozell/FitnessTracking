using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using System.Net.Sockets;

namespace BuildingBlocks.Infrastructure.Email
{
    internal sealed class SmtpHealthCheck(IOptions<EmailOptions> options) : IHealthCheck
    {
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                using var client = new TcpClient();
                await client.ConnectAsync(options.Value.Host, options.Value.Port, cancellationToken);
                return HealthCheckResult.Healthy($"SMTP server {options.Value.Host}:{options.Value.Port} is reachable.");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy($"SMTP server {options.Value.Host}:{options.Value.Port} is unreachable.", ex);
            }
        }
    }
}