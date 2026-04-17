using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nutrition.Application.Dtos;

namespace Nutrition.Application.Features.Foods.GetFoodById
{
    public sealed class GetFoodByIdEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/foods/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new GetFoodByIdQuery(id), ct);

                return result.IsSuccess
                    ? Results.Ok(result.Data)
                    : result.Error!.ToProblem("Food not found.");
            })
            .WithName("GetFoodById")
            .WithTags("Foods")
            .WithSummary("Gets a food by ID")
            .WithDescription("Returns a single food by its unique identifier")
            .Produces<FoodDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
}
