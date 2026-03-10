using System.Net;
using System.Text.Json;
using BuildingBlocks.Domain.Exceptions;
using FitnessTracking.Api.ExceptionHandling;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace FitnessTracking.Api.IntegrationTests;

public class GlobalExceptionHandlerTests
{
    private readonly GlobalExceptionHandler _sut;

    public GlobalExceptionHandlerTests()
    {
        var logger = Substitute.For<ILogger<GlobalExceptionHandler>>();
        _sut = new GlobalExceptionHandler(logger);
    }

    private static DefaultHttpContext CreateHttpContext()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        return context;
    }

    private static async Task<ProblemDetails> ReadProblemDetails(HttpResponse response)
    {
        response.Body.Seek(0, SeekOrigin.Begin);
        return (await JsonSerializer.DeserializeAsync<ProblemDetails>(response.Body,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }))!;
    }

    [Fact]
    public async Task TryHandleAsync_ShouldReturn400_ForValidationException()
    {
        var context = CreateHttpContext();
        var failures = new List<ValidationFailure>
        {
            new("Name", "Name is required"),
            new("Name", "Name must be at most 150 characters")
        };
        var exception = new ValidationException(failures);

        var handled = await _sut.TryHandleAsync(context, exception, CancellationToken.None);

        handled.Should().BeTrue();
        context.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var json = await new StreamReader(context.Response.Body).ReadToEndAsync();
        json.Should().Contain("Validation Error");
        json.Should().Contain("Name");
    }

    [Fact]
    public async Task TryHandleAsync_ShouldReturn409_ForDbUpdateConcurrencyException()
    {
        var context = CreateHttpContext();
        var exception = new DbUpdateConcurrencyException("Concurrency conflict");

        var handled = await _sut.TryHandleAsync(context, exception, CancellationToken.None);

        handled.Should().BeTrue();
        context.Response.StatusCode.Should().Be(StatusCodes.Status409Conflict);

        var problem = await ReadProblemDetails(context.Response);
        problem.Title.Should().Be("Concurrency Conflict");
    }

    [Fact]
    public async Task TryHandleAsync_ShouldReturn409_ForBusinessRuleViolationException()
    {
        var context = CreateHttpContext();
        var exception = new BusinessRuleViolationException("Cannot delete active program");

        var handled = await _sut.TryHandleAsync(context, exception, CancellationToken.None);

        handled.Should().BeTrue();
        context.Response.StatusCode.Should().Be(StatusCodes.Status409Conflict);

        var problem = await ReadProblemDetails(context.Response);
        problem.Title.Should().Be("Business Rule Violation");
        problem.Detail.Should().Be("Cannot delete active program");
    }

    [Fact]
    public async Task TryHandleAsync_ShouldReturn404_ForDomainNotFoundException()
    {
        var context = CreateHttpContext();
        var exception = new DomainNotFoundException("Exercise", Guid.NewGuid());

        var handled = await _sut.TryHandleAsync(context, exception, CancellationToken.None);

        handled.Should().BeTrue();
        context.Response.StatusCode.Should().Be(StatusCodes.Status404NotFound);

        var problem = await ReadProblemDetails(context.Response);
        problem.Title.Should().Be("Entity Not Found");
    }

    [Fact]
    public async Task TryHandleAsync_ShouldReturn500_ForUnhandledException()
    {
        var context = CreateHttpContext();
        var exception = new InvalidOperationException("Something unexpected happened");

        var handled = await _sut.TryHandleAsync(context, exception, CancellationToken.None);

        handled.Should().BeTrue();
        context.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);

        var problem = await ReadProblemDetails(context.Response);
        problem.Title.Should().Be("Internal Server Error");
        problem.Detail.Should().Be("An unexpected error occurred.");
    }

    [Fact]
    public async Task TryHandleAsync_ShouldAlwaysReturnTrue()
    {
        var context = CreateHttpContext();

        var result = await _sut.TryHandleAsync(context, new Exception("test"), CancellationToken.None);

        result.Should().BeTrue();
    }
}
