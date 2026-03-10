using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FitnessTracking.Api.IntegrationTests.Endpoints;

public class ExerciseEndpointTests : IClassFixture<FitnessTrackingWebAppFactory>
{
    private readonly HttpClient _client;
    private readonly FitnessTrackingWebAppFactory _factory;

    public ExerciseEndpointTests(FitnessTrackingWebAppFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateExercise_ShouldReturn201_WhenValid()
    {
        var request = new
        {
            Name = "Bench Press",
            PrimaryMuscleGroup = "Chest",
            SecondaryMuscleGroup = "Triceps",
            Description = "Flat bench press"
        };

        var response = await _client.PostAsJsonAsync("/api/exercises", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<CreateExerciseResponse>();
        body!.Id.Should().NotBeEmpty();
        response.Headers.Location!.ToString().Should().Contain($"/api/exercises/{body.Id}");
    }

    [Fact]
    public async Task CreateExercise_ShouldReturn400_WhenNameIsEmpty()
    {
        var request = new
        {
            Name = "",
            PrimaryMuscleGroup = "Chest",
            Description = "test"
        };

        var response = await _client.PostAsJsonAsync("/api/exercises", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetAllExercises_ShouldReturn200()
    {
        // Temporarily disable exception handler to see raw exception
        var response = await _client.GetAsync("/api/exercises");
        var body = await response.Content.ReadAsStringAsync();

        // Check logs via factory server for actual exception
        if (response.StatusCode == HttpStatusCode.InternalServerError)
        {
            // Try through minimal pipeline to get actual exception
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<Exercises.Infrastructure.Persistence.ExercisesDbContext>();
            var canConnect = await db.Database.CanConnectAsync();
            var count = await db.Exercises.CountAsync();
            Assert.Fail($"DB connected: {canConnect}, Count: {count}, Body: {body}");
        }

        response.StatusCode.Should().Be(HttpStatusCode.OK, because: body);
    }

    [Fact]
    public async Task GetAllExercises_ShouldSupportPagination()
    {
        var response = await _client.GetAsync("/api/exercises?pageNumber=1&pageSize=5");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetExerciseById_ShouldReturn404_WhenNotExists()
    {
        var response = await _client.GetAsync($"/api/exercises/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateAndGetExercise_ShouldReturnCreatedExercise()
    {
        // Create
        var createRequest = new
        {
            Name = "Squat",
            PrimaryMuscleGroup = "Quadriceps",
            SecondaryMuscleGroup = "Glutes",
            Description = "Barbell squat"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/exercises", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<CreateExerciseResponse>();

        // Get
        var getResponse = await _client.GetAsync($"/api/exercises/{created!.Id}");

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await getResponse.Content.ReadFromJsonAsync<ExerciseResponse>();
        body!.Name.Should().Be("Squat");
        body.PrimaryMuscleGroup.Should().Be("Quadriceps");
    }

    [Fact]
    public async Task DeleteExercise_ShouldReturn204_WhenExists()
    {
        // Create
        var request = new
        {
            Name = "ToDelete",
            PrimaryMuscleGroup = "Chest",
            Description = "will be deleted"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/exercises", request);
        var created = await createResponse.Content.ReadFromJsonAsync<CreateExerciseResponse>();

        // Delete
        var deleteResponse = await _client.DeleteAsync($"/api/exercises/{created!.Id}");

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteExercise_ShouldReturn404_WhenNotExists()
    {
        var response = await _client.DeleteAsync($"/api/exercises/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private record CreateExerciseResponse(Guid Id);
    private record ExerciseResponse(Guid Id, string Name, string PrimaryMuscleGroup, string? SecondaryMuscleGroup, string Description);
}
