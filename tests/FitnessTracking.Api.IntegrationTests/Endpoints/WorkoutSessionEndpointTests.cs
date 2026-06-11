using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using WorkoutPrograms.Domain.Entity;
using WorkoutPrograms.Infrastructure.Persistence;
using Xunit;

namespace FitnessTracking.Api.IntegrationTests.Endpoints;

public class WorkoutSessionEndpointTests : IClassFixture<FitnessTrackingWebAppFactory>
{
    private readonly FitnessTrackingWebAppFactory _factory;
    private readonly HttpClient _client;

    public WorkoutSessionEndpointTests(FitnessTrackingWebAppFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateWorkoutSession_ShouldReturn201_WhenValid()
    {
        // A session requires an existing program and a split that belongs to it
        // (cross-module existence + split-membership checks in the handler). Seed the
        // prerequisite aggregate directly into the shared in-memory database so this
        // WorkoutSession test does not depend on the WorkoutProgram module's endpoints.
        var (programId, splitId) = await SeedProgramWithSplitAsync();

        var request = new
        {
            WorkoutProgramId = programId,
            WorkoutProgramSplitId = splitId,
            Date = new DateTime(2025, 6, 15)
        };

        var response = await _client.PostAsJsonAsync("/api/v1/workout-sessions", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<IdResponse>();
        body!.Id.Should().NotBeEmpty();
    }

    private async Task<(Guid ProgramId, Guid SplitId)> SeedProgramWithSplitAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<WorkoutProgramsDbContext>();

        var program = WorkoutProgram.Create(
            FitnessTrackingWebAppFactory.TestUserId,
            "Test Program",
            description: null,
            new DateTime(2025, 1, 1),
            new DateTime(2025, 12, 31));

        var split = program.AddSplit("Push Day", 1);

        db.WorkoutPrograms.Add(program);
        await db.SaveChangesAsync();

        return (program.Id, split.Id);
    }

    [Fact]
    public async Task CreateWorkoutSession_ShouldReturn400_WhenProgramIdIsEmpty()
    {
        var request = new
        {
            WorkoutProgramId = Guid.Empty,
            Date = new DateTime(2025, 6, 15)
        };

        var response = await _client.PostAsJsonAsync("/api/v1/workout-sessions", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetWorkoutSessions_ShouldReturn200()
    {
        var response = await _client.GetAsync("/api/v1/workout-sessions");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetWorkoutSessionById_ShouldReturn404_WhenNotExists()
    {
        var response = await _client.GetAsync($"/api/v1/workout-sessions/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetWorkoutSessionDetailView_ShouldReturn404_WhenNotExists()
    {
        var response = await _client.GetAsync($"/api/v1/workout-sessions/{Guid.NewGuid()}/detail-view");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private record IdResponse(Guid Id);
}