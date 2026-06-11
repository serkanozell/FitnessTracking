using System.Net;
using FitnessTracking.Api.Configuration;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FitnessTracking.Api.IntegrationTests;

public class RateLimitingTests : IClassFixture<FitnessTrackingWebAppFactory>
{
    private readonly FitnessTrackingWebAppFactory _factory;

    public RateLimitingTests(FitnessTrackingWebAppFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task DashboardPolicy_ShouldReturn429_AfterPermitLimitExceeded()
    {
        const int permitLimit = 2;

        // Tighten only the dashboard policy via PostConfigure. Because the limiter resolves rules
        // per request from IOptionsMonitor, this override deterministically takes effect.
        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.PostConfigure<RateLimitingOptions>(options =>
                {
                    options.Dashboard.PermitLimit = permitLimit;
                    options.Dashboard.WindowInSeconds = 60;
                    options.Dashboard.QueueLimit = 0;
                });
            });
        }).CreateClient();

        var statuses = new List<HttpStatusCode>();
        for (var i = 0; i < permitLimit + 3; i++)
        {
            var response = await client.GetAsync("/api/v1/dashboard");
            statuses.Add(response.StatusCode);
        }

        // Requests within the permit window are not throttled; excess requests get 429.
        statuses.Take(permitLimit).Should().NotContain(HttpStatusCode.TooManyRequests);
        statuses.Should().Contain(HttpStatusCode.TooManyRequests);
    }

    [Fact]
    public async Task DashboardPolicy_ShouldNotThrottle_WithinDefaultLimits()
    {
        // The shared factory uses very high limits, so a normal burst must never be throttled.
        var client = _factory.CreateClient();

        var statuses = new List<HttpStatusCode>();
        for (var i = 0; i < 10; i++)
        {
            var response = await client.GetAsync("/api/v1/dashboard");
            statuses.Add(response.StatusCode);
        }

        statuses.Should().NotContain(HttpStatusCode.TooManyRequests);
    }
}
