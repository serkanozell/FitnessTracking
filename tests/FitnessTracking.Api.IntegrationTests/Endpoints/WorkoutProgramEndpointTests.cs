using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace FitnessTracking.Api.IntegrationTests.Endpoints;

public class WorkoutProgramEndpointTests : IClassFixture<FitnessTrackingWebAppFactory>
{
    private readonly HttpClient _client;

    public WorkoutProgramEndpointTests(FitnessTrackingWebAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateWorkoutProgram_ShouldReturn201_WhenValid()
    {
        var request = new
        {
            Name = "Push Pull Legs",
            StartDate = new DateTime(2025, 1, 1),
            EndDate = new DateTime(2025, 3, 31)
        };

        var response = await _client.PostAsJsonAsync("/api/v1/workout-programs", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<IdResponse>();
        body!.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CreateWorkoutProgram_ShouldReturn400_WhenNameIsEmpty()
    {
        var request = new
        {
            Name = "",
            StartDate = new DateTime(2025, 1, 1),
            EndDate = new DateTime(2025, 3, 31)
        };

        var response = await _client.PostAsJsonAsync("/api/v1/workout-programs", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetWorkoutProgramById_ShouldReturn404_WhenNotExists()
    {
        var response = await _client.GetAsync($"/api/v1/workout-programs/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetWorkoutPrograms_ShouldReturn200()
    {
        var response = await _client.GetAsync("/api/v1/workout-programs");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateAndGetWorkoutProgram_ShouldReturnCreatedProgram()
    {
        var createRequest = new
        {
            Name = "Upper Lower",
            StartDate = new DateTime(2025, 4, 1),
            EndDate = new DateTime(2025, 6, 30)
        };
        var createResponse = await _client.PostAsJsonAsync("/api/v1/workout-programs", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<IdResponse>();

        var getResponse = await _client.GetAsync($"/api/v1/workout-programs/{created!.Id}");

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private record IdResponse(Guid Id);
}
