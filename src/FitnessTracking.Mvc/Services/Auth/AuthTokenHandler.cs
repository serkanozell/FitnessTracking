using System.Net.Http.Headers;

namespace FitnessTracking.Mvc.Services.Auth;

public sealed class AuthTokenHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
    internal const string JwtClaimType = "api_token";

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = httpContextAccessor.HttpContext?.User
            .FindFirst(JwtClaimType)?.Value;

        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
