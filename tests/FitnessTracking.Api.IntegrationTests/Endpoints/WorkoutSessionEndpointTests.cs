using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace FitnessTracking.Api.IntegrationTests.Endpoints;

public class WorkoutSessionEndpointTests : IClassFixture<FitnessTrackingWebAppFactory>
{
    private readonly HttpClient _client;

    public WorkoutSessionEndpointTests(FitnessTrackingWebAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateWorkoutSession_ShouldReturn201_WhenValid()
    {
        var request = new
        {
            WorkoutProgramId = Guid.NewGuid(),
            Date = new DateTime(2025, 6, 15)
        };

        var response = await _client.PostAsJsonAsync("/api/workout-sessions", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<IdResponse>();
        body!.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CreateWorkoutSession_ShouldReturn400_WhenProgramIdIsEmpty()
    {
        var request = new
        {
            WorkoutProgramId = Guid.Empty,
            Date = new DateTime(2025, 6, 15)
        };

        var response = await _client.PostAsJsonAsync("/api/workout-sessions", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetWorkoutSessions_ShouldReturn200()
    {
        var response = await _client.GetAsync("/api/workout-sessions");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetWorkoutSessionById_ShouldReturn404_WhenNotExists()
    {
        var response = await _client.GetAsync($"/api/workout-sessions/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private record IdResponse(Guid Id);
}