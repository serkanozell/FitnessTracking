using BuildingBlocks.Domain.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTracking.Api.ExceptionHandling
{
    public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var problemDetails = exception switch
            {
                ValidationException validationException => CreateValidationProblem(validationException),
                BusinessRuleViolationException => CreateProblem(StatusCodes.Status409Conflict,
                                                                "Business Rule Violation",
                                                                exception.Message),
                DomainNotFoundException => CreateProblem(StatusCodes.Status404NotFound,
                                                         "Entity Not Found",
                                                         exception.Message),
                DomainException => CreateProblem(StatusCodes.Status422UnprocessableEntity,
                                                 "Domain Error",
                                                 exception.Message),
                _ => CreateProblem(StatusCodes.Status500InternalServerError,
                                   "Internal Server Error",
                                   "An unexpected error occurred.")
            };

            if (problemDetails.Status >= 500)
                logger.LogError(exception, "Unhandled exception occurred: {Message}", exception.Message);
            else
                logger.LogWarning(exception, "Handled domain exception: {Message}", exception.Message);

            httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }

        private static ProblemDetails CreateProblem(int statusCode, string title, string detail) => new()
        {
            Status = statusCode,
            Title = title,
            Detail = detail
        };

        private static ProblemDetails CreateValidationProblem(ValidationException exception)
        {
            var errors = exception.Errors
                                         .GroupBy(e => e.PropertyName)
                                         .ToDictionary(g => g.Key,
                                                       g => g.Select(e => e.ErrorMessage).ToArray());

            return new ValidationProblemDetails(errors)
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Validation Error",
                Detail = "One or more validation errors occurred."
            };
        }
    }
}
