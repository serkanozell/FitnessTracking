using BuildingBlocks.Web;
using Dashboard.Application.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Dashboard.Application.Features.Analytics.GetPersonalRecords
{
    public sealed class GetPersonalRecordsEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/dashboard/analytics/personal-records", async (int? top, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new GetPersonalRecordsQuery(top ?? 10), ct);

                return result.IsSuccess
                    ? Results.Ok(result.Data)
                    : result.Error!.ToProblem("Failed to load personal records.");
            })
            .WithName("GetPersonalRecords")
            .WithTags("Dashboard")
            .WithSummary("Gets top personal records (estimated 1RM) for the current user")
            .WithDescription("Returns the top N exercises by estimated 1RM (Epley formula)")
            .Produces<IReadOnlyList<PersonalRecordDto>>(StatusCodes.Status200OK);
        }
    }
}
