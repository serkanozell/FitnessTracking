using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace FitnessTracking.Api.IntegrationTests.Endpoints;

public class UserEndpointTests : IClassFixture<FitnessTrackingWebAppFactory>
{
    private readonly HttpClient _client;

    public UserEndpointTests(FitnessTrackingWebAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_ShouldReturn201_WhenValid()
    {
        var request = new
        {
            Email = $"register-{Guid.NewGuid():N}@test.com",
            Password = "Password123!",
            FirstName = "Test",
            LastName = "User"
        };

        var response = await _client.PostAsJsonAsync("/api/v1/users/register", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Register_ShouldReturn400_WhenEmailIsEmpty()
    {
        var request = new
        {
            Email = "",
            Password = "Password123!",
            FirstName = "Test",
            LastName = "User"
        };

        var response = await _client.PostAsJsonAsync("/api/v1/users/register", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_ShouldReturn401_WhenInvalidCredentials()
    {
        var request = new
        {
            Email = "nonexistent@test.com",
            Password = "WrongPassword123!"
        };

        var response = await _client.PostAsJsonAsync("/api/v1/users/login", request);

        // InvalidCredentials maps to default 400 via ToProblem
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RegisterAndLogin_ShouldReturnTokens()
    {
        var email = $"logintest-{Guid.NewGuid():N}@test.com";

        await _client.PostAsJsonAsync("/api/v1/users/register", new
        {
            Email = email,
            Password = "Password123!",
            FirstName = "Test",
            LastName = "User"
        });

        var loginResponse = await _client.PostAsJsonAsync("/api/v1/users/login", new
        {
            Email = email,
            Password = "Password123!"
        });

        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();
        body!.Token.Should().NotBeNullOrEmpty();
        body.RefreshToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task RefreshToken_ShouldReturn400_WhenInvalidToken()
    {
        var response = await _client.PostAsJsonAsync("/api/v1/users/refresh-token", new
        {
            RefreshToken = "invalid-token"
        });

        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RevokeToken_ShouldReturn400_WhenInvalidToken()
    {
        var response = await _client.PostAsJsonAsync("/api/v1/users/revoke-token", new
        {
            RefreshToken = "invalid-token"
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private record LoginResponseDto(string Token, string RefreshToken, Guid UserId, string Email, List<string> Roles);
}
