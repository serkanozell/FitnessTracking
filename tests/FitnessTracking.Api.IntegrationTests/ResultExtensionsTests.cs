using BuildingBlocks.Application.Results;
using BuildingBlocks.Web;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace FitnessTracking.Api.IntegrationTests;

public class ResultExtensionsTests
{
    [Fact]
    public void ToProblem_ShouldReturn403_ForForbiddenError()
    {
        var error = Error.Forbidden();

        var result = error.ToProblem("Test");

        var problem = ExtractStatusCode(result);
        problem.Should().Be(StatusCodes.Status403Forbidden);
    }

    [Fact]
    public void ToProblem_ShouldReturn404_ForNotFoundError()
    {
        var error = Error.NotFound("Entity", Guid.NewGuid());

        var result = error.ToProblem("Test");

        var problem = ExtractStatusCode(result);
        problem.Should().Be(StatusCodes.Status404NotFound);
    }

    [Fact]
    public void ToProblem_ShouldReturn400_ForValidationError()
    {
        var error = Error.Validation("Invalid input");

        var result = error.ToProblem("Test");

        var problem = ExtractStatusCode(result);
        problem.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public void ToProblem_ShouldReturn409_ForConflictError()
    {
        var error = Error.Conflict("Conflict occurred");

        var result = error.ToProblem("Test");

        var problem = ExtractStatusCode(result);
        problem.Should().Be(StatusCodes.Status409Conflict);
    }

    [Fact]
    public void ToProblem_ShouldReturn400_ForUnknownErrorCode()
    {
        var error = new Error("Custom.Error", "Something went wrong");

        var result = error.ToProblem("Test");

        var problem = ExtractStatusCode(result);
        problem.Should().Be(StatusCodes.Status400BadRequest);
    }

    private static int? ExtractStatusCode(IResult result)
    {
        // Microsoft.AspNetCore.Http.HttpResults.ProblemHttpResult
        var statusCodeProp = result.GetType().GetProperty("StatusCode");
        return statusCodeProp?.GetValue(result) as int?;
    }
}
