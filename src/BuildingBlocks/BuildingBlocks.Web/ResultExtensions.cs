using BuildingBlocks.Application.Results;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Web;

public static class ResultExtensions
{
    public static IResult ToProblem(this Error error, string title)
    {
        var statusCode = error.Code switch
        {
            "Error.Forbidden" => StatusCodes.Status403Forbidden,
            "Error.Validation" => StatusCodes.Status400BadRequest,
            "Error.Conflict" => StatusCodes.Status409Conflict,
            _ when error.Code.EndsWith(".NotFound") => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status400BadRequest
        };

        return Results.Problem(title: title, detail: error.Message, statusCode: statusCode);
    }
}